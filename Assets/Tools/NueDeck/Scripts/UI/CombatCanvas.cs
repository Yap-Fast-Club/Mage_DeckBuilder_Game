using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.ThirdParty.NueTooltip.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace NueGames.NueDeck.Scripts.UI
{
    public class CombatCanvas : CanvasBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _paidConsumeHandellButton;
        [SerializeField] private Button _freeConsumeHandellButton;
        [SerializeField] private Image _freeHandellProgressBar;
 
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI drawPileTextField;
        [SerializeField] private TextMeshProUGUI discardPileTextField;
        [SerializeField] private TextMeshProUGUI exhaustPileTextField;
        [SerializeField] private TextMeshProUGUI manaTextTextField;
        [SerializeField] private Image _manaHighlight;
        
        [Header("Panels")]
        [SerializeField] private GameObject nextCombatPanel;
        [SerializeField] private GameObject combatWinPanel;
        [SerializeField] private GameObject combatLosePanel;

        [Header("Other")]
        [SerializeField] TooltipTrigger2D _channeledCardsTooltip;
        [SerializeField] TextMeshProUGUI _channeledCardsAmount;

        public TooltipTrigger2D ChanneledCards => _channeledCardsTooltip;
        public TextMeshProUGUI DrawPileTextField => drawPileTextField;
        public TextMeshProUGUI DiscardPileTextField => discardPileTextField;
        public TextMeshProUGUI ManaTextTextField => manaTextTextField;
        public GameObject CombatWinPanel => combatWinPanel;
        public GameObject NextCombatPanel => nextCombatPanel;
        public GameObject CombatLosePanel => combatLosePanel;

        public TextMeshProUGUI ExhaustPileTextField => exhaustPileTextField;




        #region Setup
        private void Awake()
        {
            CombatWinPanel.SetActive(false);
            NextCombatPanel.SetActive(false);
            CombatLosePanel.SetActive(false);
        }

        public void Bind()
        {
            CollectionManager.CardPlayed += OnCardPlayed;
        }

        

        public void Unbind()
        {
            CollectionManager.CardPlayed -= OnCardPlayed;
        }

        private void Start()
        {
            _paidConsumeHandellButton.onClick.AddListener(ConsumeHandell);
            _freeConsumeHandellButton.onClick.AddListener(ConsumeHandell);
            _paidConsumeHandellButton.gameObject.SetActive(true);
            _freeConsumeHandellButton.gameObject.SetActive(false);
            _freeHandellProgressBar.fillAmount = GameManager.PersistentGameplayData.HandellCount / GameManager.PersistentGameplayData.HandellThreshold;
        }
        private void OnDisable()
        {
            Unbind();
        }

        public void EnableHandell(bool state) => _freeConsumeHandellButton.interactable = _paidConsumeHandellButton.interactable = state;

        Dictionary<string, int> _playedChannelCards = new Dictionary<string, int>();
        private void OnCardPlayed(CardBase playedCard)
        {
            //channel
            if (playedCard.Channel && playedCard.CardData.TurnCost == 0)
            {
                _channeledCardsTooltip.gameObject.SetActive(true);


                if (_playedChannelCards.ContainsKey(playedCard.CardData.CardName))
                    _playedChannelCards[playedCard.CardData.CardName]++;
                else
                    _playedChannelCards.Add(playedCard.CardData.CardName, 1);

                _channeledCardsTooltip.ModifyContent(string.Join(Environment.NewLine, _playedChannelCards));
                _channeledCardsAmount.text = $"x{_playedChannelCards.Sum(x => x.Value).ToString()}";
            }

            if (playedCard.CardData.TurnCost >= 1)
            {
                _channeledCardsTooltip.gameObject.SetActive(false);
                _playedChannelCards.Clear();
            }

            //mana
            if (playedCard.FinalManaCost > 0)
            {
                StartCoroutine(DimMana());
            }

            else if(playedCard.CardData.CardActionDataList.Any(a => a.CardActionType == CardActionType.EarnMana))
            {
                StartCoroutine(GlowMana());
            }

            _freeHandellProgressBar.fillAmount = (float)GameManager.PersistentGameplayData.HandellCount / GameManager.PersistentGameplayData.HandellThreshold;

            if (GameManager.PersistentGameplayData.HandellIsActive)
                ShowFreeHandell(true);
        }

        private IEnumerator DimMana()
        {
            float elapsedTime = 0f;
            float inTime = 0.3f;
            float outTime = 0.15f;

            Color initialColor = _manaHighlight.color;
            Color targetColor = new Color(initialColor.r / 2, initialColor.g / 2, initialColor.b / 2);

            while (elapsedTime < inTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / inTime);

                _manaHighlight.color = Color.Lerp(initialColor, targetColor, t);

                yield return null;
            }
            elapsedTime = 0f;
            while (elapsedTime < outTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / outTime);

                _manaHighlight.color = Color.Lerp(targetColor, initialColor, t);

                yield return null;
            }

            _manaHighlight.color = initialColor;
        }

        private IEnumerator GlowMana()
        {
            float elapsedTime = 0f;
            float inTime = 0.25f;
            float outTime = 0.25f;

            Color initialColor = _manaHighlight.color;
            Color targetColor = new Color(initialColor.r, initialColor.g + 50, initialColor.b);

            while (elapsedTime < inTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / inTime);

                _manaHighlight.color = Color.Lerp(initialColor, targetColor, t);

                yield return null;
            }
            elapsedTime = 0f;
            while (elapsedTime < outTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / outTime);

                _manaHighlight.color = Color.Lerp(targetColor, initialColor, t);

                yield return null;
            }

            _manaHighlight.color = initialColor;
        }

        private void ConsumeHandell()
        {
            CollectionManager.Instance.DiscardHand();
            CollectionManager.Instance.DrawCards(GameManager.PersistentGameplayData.DrawCount);

            if (!GameManager.PersistentGameplayData.HandellIsActive)
                EndTurn();

            GameManager.PersistentGameplayData.HandellCount = 0;
            _freeHandellProgressBar.fillAmount = 0; 
            ShowFreeHandell(false);
        }


        private void ShowFreeHandell(bool show)
        {
            _paidConsumeHandellButton.gameObject.SetActive(!show);
            _freeConsumeHandellButton.gameObject.SetActive(show);

            if (show)
                AudioManager.Instance.PlayOneShot(AudioActionType.HandellActivation);
        }

        #endregion

        #region Public Methods
        public void SetPileTexts()
        {
            DrawPileTextField.text = $"{CollectionManager.DrawPile.Count.ToString()}";
            DiscardPileTextField.text = $"{CollectionManager.DiscardPile.Count.ToString()}";
            ExhaustPileTextField.text =  $"{CollectionManager.ExhaustPile.Count.ToString()}";
            ManaTextTextField.text = $"{GameManager.PersistentGameplayData.CurrentMana.ToString()}/{GameManager.PersistentGameplayData.MaxMana}";
        }

        public override void ResetCanvas()
        {
            base.ResetCanvas();
            CombatWinPanel.SetActive(false);
            CombatLosePanel.SetActive(false);
            NextCombatPanel.SetActive(false);
            ShowFreeHandell(false);
            _freeHandellProgressBar.fillAmount = 0;
        }

        public void EndTurn()
        {
            if (CombatManager.CurrentCombatStateType == CombatStateType.AllyTurn)
                CombatManager.EndTurn();
        }
        #endregion
    }
}