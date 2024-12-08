using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.UI
{
    public class InformationCanvas : CanvasBase
    {
        [Header("Settings")]
        [SerializeField] private GameObject randomizedDeckObject;
        [SerializeField] private TextMeshProUGUI roomTextField;
        [SerializeField] private TextMeshProUGUI goldTextField;
        [SerializeField] private Image _soulsOnImg;
        [SerializeField] private LilShake _soulsShake;
        [SerializeField] private TextMeshProUGUI _soulsTextField;
        [SerializeField] private TextMeshProUGUI nameTextField;
        [SerializeField] private Image healthProgressBar;
        [SerializeField] private TextMeshProUGUI healthTextField;
        [SerializeField] private Button _copyCurrentSeedButton;
        [SerializeField] private TextMeshProUGUI _currentSeedText;
        [SerializeField] private List<RectTransform> _soulContainers = new List<RectTransform>();
        [SerializeField] private List<RectTransform> _soulOnIcons = new List<RectTransform>();
        [SerializeField] private RectTransform _soulOffRoot = new RectTransform();
        [SerializeField] private Color _soulsDimColor;
        [SerializeField] private Color _soulsOnColor;
        [SerializeField] private Color _soulsGlowColor;

        public GameObject RandomizedDeckObject => randomizedDeckObject;
        public TextMeshProUGUI RoomTextField => roomTextField;
        public TextMeshProUGUI GoldTextField => goldTextField;
        public TextMeshProUGUI SoulsTextField => _soulsTextField;
        public TextMeshProUGUI NameTextField => nameTextField;
        public TextMeshProUGUI HealthTextField => healthTextField;
        public Image HealthProgressBar => healthProgressBar;

        private PersistentGameplayData _persistentGameplayData => GameManager.PersistentGameplayData;
        private Queue<Coroutine> _gainSoulsCRs = new Queue<Coroutine>();

        #region Setup
        private void Awake()
        {
            ResetCanvas();
            _copyCurrentSeedButton.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = _currentSeedText.text;
                FxManager.Instance.SpawnFloatingText(_currentSeedText.rectTransform, "Copied to ClipBoard!", duration: 3);
            });
        }
        #endregion

        #region Public Methods
        public void SetRoomText(int roomNumber, bool useStage = false, int stageNumber = -1) =>
            RoomTextField.text = useStage ? $"Encounter {roomNumber}" : $"Room {roomNumber}";

        public void SetRoomText(string text) => RoomTextField.text = text;

        public void SetGoldText(int value) => GoldTextField.text = $"{value}";

        public void InstantUpdateSoulsGUI()
        {
            SoulsTextField.text = $"{_persistentGameplayData.CurrentSouls}";
            _soulsOnImg.color = new Color(_soulsOnColor.r, _soulsOnColor.g, _soulsOnColor.b,_persistentGameplayData.CurrentSouls / (float)_persistentGameplayData.MaxSouls);
            _soulsShake.Amount = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);
            _soulsShake.Speed = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);


            for (int i = 0; i < _soulOnIcons.Count; i++)
            {
                RectTransform soulIcon = _soulOnIcons[i];
                if (i < _persistentGameplayData.CurrentSouls)
                {
                    RectTransform soulContainer = _soulContainers[Mathf.Min(i, _soulContainers.Count - 1)];
                    soulIcon.position = soulContainer.position;
                }
                else
                {
                    soulIcon.position = _soulOffRoot.position;
                }

            }
        }

        public void UpdateSoulsGUI(EnemyBase deadEnemy)
        {
            SoulsTextField.text = $"{_persistentGameplayData.CurrentSouls}";
            _soulsOnImg.color = new Color(_soulsOnColor.r, _soulsOnColor.g, _soulsOnColor.b, _persistentGameplayData.CurrentSouls / (float)_persistentGameplayData.MaxSouls);
            _soulsShake.Amount = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);
            _soulsShake.Speed = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);


            int soulAmout = deadEnemy.CharacterStats.CurrentSouls;

            for (int i = _persistentGameplayData.CurrentSouls - soulAmout; i < _persistentGameplayData.CurrentSouls; i++)
            {
                int soulIndex = Mathf.Min(i, _soulOnIcons.Count - 1);
                soulIndex = Mathf.Max(0, soulIndex);

                var enemyScreenPos = Camera.main.WorldToScreenPoint(deadEnemy.transform.position);

                deadEnemy.EnemyCanvas.IntentImage.rectTransform.SetParent(_soulOffRoot.parent, true);
                _soulOnIcons[soulIndex].position = enemyScreenPos;


                _gainSoulsCRs.Enqueue(StartCoroutine(GainSoulCR(soulIndex)));
            }

        }

        private Coroutine GlowSoulsCR = null;
        public void AnimateSoulsGUI()
        {
            if (GlowSoulsCR == null)
                GlowSoulsCR = StartCoroutine(GlowSouls());
        }


        private IEnumerator GlowSouls()
        {
            float elapsedTime = 0f;
            float inTime = 0.4f;
            float outTime = 0.25f;

            Color initialColor = _soulsOnColor;
            Color targetColor = _soulsGlowColor;

            while (elapsedTime < inTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / inTime);

                _soulsOnImg.color = Color.Lerp(initialColor, targetColor, t);

                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
            yield return new WaitWhile(() => UIManager.RewardCanvas.isActiveAndEnabled);

            initialColor = _soulsGlowColor;
            targetColor = _soulsDimColor;

            elapsedTime = 0f;
            while (elapsedTime < outTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / outTime);

                _soulsOnImg.color = Color.Lerp(initialColor, targetColor, t);

                yield return null;
            }

            _soulsOnImg.color = _soulsDimColor;
            _soulsShake.Amount = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);
            _soulsShake.Speed = Mathf.Min(_persistentGameplayData.CurrentSouls, _persistentGameplayData.MaxSouls);
            GlowSoulsCR = null;

        }

        public void SetNameText(string name) => NameTextField.text = $"{name}";

        public void SetHealthGUI(int currentHealth, int maxHealth)
        {
            HealthTextField.text = $"{currentHealth}/{maxHealth}";
            HealthProgressBar.fillAmount =  (float)currentHealth/ (float)maxHealth;
        }

        public override void ResetCanvas()
        {
            //RandomizedDeckObject.SetActive(_persistentGameplayData.IsRandomHand);
            SetHealthGUI(_persistentGameplayData.AllyList[0].AllyCharacterData.MaxHealth, _persistentGameplayData.AllyList[0].AllyCharacterData.MaxHealth);
            SetNameText(GameManager.GameplayData.DefaultName);


            string encounterName = GameManager.EncounterData.GetCurrentLevelName(
                        _persistentGameplayData.CurrentStageId,
                        _persistentGameplayData.CurrentEncounterId,
                        _persistentGameplayData.IsFinalEncounter
            );
            SetRoomText(encounterName);

            UIManager.InformationCanvas.SetGoldText(_persistentGameplayData.CurrentGold);
            UIManager.InformationCanvas.InstantUpdateSoulsGUI();

            _currentSeedText.text = $"{GameManager.PersistentGameplayData.CurrentSeed}";
        }
        #endregion


        protected virtual IEnumerator GainSoulCR(int soulIndex)
        {

            RectTransform soulIcon = _soulOnIcons[soulIndex];
            RectTransform soulContainer = _soulContainers[Mathf.Min(soulIndex, _soulContainers.Count - 1)];

            var startPos = soulIcon.position;
            var endPos = soulContainer.position;
            var startScale = soulContainer.localScale * 0.1f;
            var endScale = soulContainer.localScale * 1;

            var timer = 0f;

            while (true)
            {
                timer += Time.deltaTime * 2.5f;
                soulIcon.position = Vector3.Lerp(startPos + Vector3.down * 10, startPos, timer);
                soulIcon.localScale = Vector3.Lerp(startScale, endScale, timer);

                if (timer >= 1f) break;

                yield return new WaitForEndOfFrame();
            }

            startPos = soulIcon.position;
            timer = 0;

            while (true)
            {
                timer += Time.deltaTime * 4f;
                soulIcon.position = Vector3.Lerp(startPos, endPos, timer);

                if (timer >= 1f) break;

                yield return new WaitForEndOfFrame();
            }


            _gainSoulsCRs.Dequeue();
        }
    }
}