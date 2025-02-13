﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Card.CardActions;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Utils;
using NueGames.NueDeck.ThirdParty.NueTooltip.Core;
using NueGames.NueDeck.ThirdParty.NueTooltip.CursorSystem;
using NueGames.NueDeck.ThirdParty.NueTooltip.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardBase : MonoBehaviour, I2DTooltipTarget, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Base References")]
        [SerializeField] protected Transform descriptionRoot;
        [SerializeField] protected Image cardImage;
        [SerializeField] protected Image passiveImage;
        [SerializeField] protected Image grayedImage;
        [SerializeField] protected TextMeshProUGUI nameTextField;
        [SerializeField] protected TextMeshProUGUI descTextField;
        [SerializeField] protected TextMeshProUGUI manaTextField;
        [SerializeField] protected Image _instantCostImage;
        [SerializeField] protected Image _turnCostCostImage;
        [SerializeField] protected Image _fatigueCostImage;
        [SerializeField] protected List<RarityRoot> rarityRootList;
        [SerializeField] protected List<TypeRoot> _typeRootList;
        [SerializeField] protected RarityColors _rarityColors;
        [SerializeField] public TextMeshProUGUI weightTextField;


        #region Cache
        public CardData CardData { get; private set; }
        public bool IsInactive { get; protected set; }
        protected Transform CachedTransform { get; set; }
        protected WaitForEndOfFrame CachedWaitFrame { get; set; }
        public bool IsPlayable { get; protected set; } = true;

        public List<RarityRoot> RarityRootList => rarityRootList;
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        private PersistentGameplayData persistentData => GameManager.PersistentGameplayData;
        protected CombatManager CombatManager => CombatManager.Instance;
        protected CollectionManager CollectionManager => CollectionManager.Instance;
        protected StatusStats FocusStat => CombatManager.CurrentMainAlly.CharacterStats.StatusDict[StatusType.Focus];


        public bool IsExhausted { get; private set; }

        public int FinalManaCost => CardData.FocusedManaCost;

        public bool Channel => CardData.Channel;

        #endregion

        #region Setup
        protected virtual void Awake()
        {
            CachedTransform = transform;
            CachedWaitFrame = new WaitForEndOfFrame();
        }

        public virtual void SetCard(CardData targetProfile, bool isPlayable = true)
        {
            CardData = targetProfile;
            IsPlayable = isPlayable;
            nameTextField.text = CardData.CardName;
            CardData.UpdateDescription();
            descTextField.text = CardData.MyDescription;

            manaTextField.text = CardData.ManaCost.ToString();
            cardImage.sprite = CardData.CardSprite;
            if (CardData.Type == CardType.Incantation)
            {
                manaTextField.transform.parent.gameObject.SetActive(false);
            }
            foreach (var rarityRoot in RarityRootList)
                rarityRoot.gameObject.SetActive(rarityRoot.Rarity == CardData.Rarity);

            foreach (var typeRoot in _typeRootList)
                typeRoot.gameObject.SetActive(typeRoot.Type == CardData.Type);

            if (CardData.EvolveToCard && persistentData.EvoultionCardsPlayed.Contains(CardData.Id))
                SetCard(Instantiate(CardData.EvolveToCard));

            _rarityColors.SetColors(CardData.Rarity);

            weightTextField.text = $"(W:{CollectionManager.WeightOf(CardData)})";
        }

        #endregion

        #region Card Methods
        public virtual void Use(CharacterBase self, CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies)
        {
            if (!IsPlayable) return;

            StartCoroutine(CardUseRoutine(self, targetCharacter, allEnemies, allAllies));
        }

        private IEnumerator CardUseRoutine(CharacterBase self, CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies)
        {
            var actionsBlackBoard = new CardBlackboard(CardData);

            SpendMana(FinalManaCost);

            AudioManager.Instance.PlayOneShot(AudioActionType.CardPlayed);


            var actionDataListCopy = new List<CardActionData>();

            //Prepare Actions to execute
            foreach (var actionData in CardData.CardActionDataList)
            {
                //actionDataListCopy.AddRange(Enumerable.Repeat(actionData, actionData.RepeatAmount));
                actionDataListCopy.Add(actionData);
            }

            if (!Channel)
            {
                foreach (var actionData in actionDataListCopy)
                {
                    var action = CardActionProcessor.GetAction(actionData.CardActionType);

                    int amountToRepeat = actionData.RepeatAmount; //to break the reference and avoid extending the loop
                    for (int i = 0; i < amountToRepeat; i++)
                    {
                        yield return new WaitForSeconds(actionData.ActionDelay);
                        var targetList = DetermineTargets(targetCharacter, allEnemies, allAllies, actionData);
                        foreach (var target in targetList)
                            action.DoAction(new CardActionParameters(
                                actionData.GetModifiedValue(CardData),
                                actionData.ActionAreaValue,
                                target, self, CardData, this,
                                actionData.ActionAudioType == AudioActionType.CardDefault ? CardData.AudioType : actionData.ActionAudioType
                                )
                            , actionsBlackBoard);
                    }


                }

                if (actionsBlackBoard.ResetPower)
                    self.CharacterStats.ClearStatus(StatusType.Power);
            }
            else
            {
                CombatManager.ChannelCard.CardData.CardActionDataList.AddRange(CardData.CardActionDataList);
            }
          
            if (CardData.Type == CardType.Spell)
            {
                self.CharacterStats.ClearStatus(StatusType.Focus);
            }

            if (CardData.EvolveToCard)
            {
                persistentData.EvoultionCardsPlayed.Add(CardData.Id);
                var copiesInHand = new List<CardBase>(CollectionManager.HandController.hand.Where(c => c.CardData.Id == CardData.Id));
                foreach (var card in copiesInHand)
                {
                    card.SetCard(card.CardData);
                }
            }

            if (persistentData.HandellIsActive)
            {
                CollectionManager.DrawCards(1);
            }

            ChargeHandell(1);
            CollectionManager.OnCardPlayed(this);

            SpendTurn(CardData.TurnCost);
        }

        public static List<CharacterBase> DetermineTargets(CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies,
            CardActionData playerAction)
        {
            List<CharacterBase> targetList = new List<CharacterBase>();
            List<EnemyBase> _allEnemies = new List<EnemyBase>(allEnemies);

            switch (playerAction.ActionTargetType)
            {
                case ActionTargetType.Enemy:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.Ally:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.AllEnemies:
                    foreach (var enemyBase in _allEnemies)
                        targetList.Add(enemyBase);
                    break;
                case ActionTargetType.AllAllies:
                    foreach (var allyBase in allAllies)
                        targetList.Add(allyBase);
                    break;
                case ActionTargetType.RandomEnemy:
                    for (int i = 0; i < playerAction.ActionAreaValue && _allEnemies.Count > 0; i++)
                        targetList.Add(_allEnemies.RandomItemRemove());
                    break;

                case ActionTargetType.LowestHPEnemy:
                    targetList.AddRange(_allEnemies
                                           .OrderBy(e => e.CharacterStats.CurrentHealth)
                                           .ThenBy(e => e.transform.position.x)
                                           .Take(playerAction.ActionAreaValue)
                                           .ToList()
                                           );

                    break;

                case ActionTargetType.ClosestEnemies:
                    targetList.AddRange(_allEnemies
                                    .OrderBy(e => e.transform.position.x)
                                    .Take(playerAction.ActionAreaValue)
                                    .ToList()
                                    );

                    break;

                case ActionTargetType.EnemyAndLineBehind:
                    targetList.Add(targetCharacter);
                    targetList.AddRange(_allEnemies
                                      .Where(e => e.transform.position.y == targetCharacter.transform.position.y)
                                      .Where(e => e.transform.position.x > targetCharacter.transform.position.x)
                                    );
                    break;

                case ActionTargetType.EnemyAndAllBehind:
                    targetList.Add(targetCharacter);
                    targetList.AddRange(_allEnemies
                                      .Where(e => e.transform.position.x > targetCharacter.transform.position.x)
                                    );
                    break;

                case ActionTargetType.LineBehindEnemy:
                    targetList.AddRange(_allEnemies
                                      .Where(e => e.transform.position.y == targetCharacter.transform.position.y)
                                      .Where(e => e.transform.position.x > targetCharacter.transform.position.x)
                                    );
                    break;
                case ActionTargetType.AllBehindEnemy:
                    targetList.AddRange(_allEnemies
                                      .Where(e => e.transform.position.x > targetCharacter.transform.position.x)
                                    );
                    break;

                //case ActionTargetType.ClosestAndConsecutives:
                //    targetList.Add(targetCharacter);
                //    targetList.AddRange(_allEnemies);
                //    targetList.Sort((a, b) => Vector3.Distance(targetCharacter.transform.position, a.transform.position).CompareTo(
                //                            Vector3.Distance(targetCharacter.transform.position, b.transform.position)));
                //    break;

                case ActionTargetType.RandomAlly:
                    if (allAllies.Count > 0)
                        targetList.Add(allAllies.RandomItem());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return targetList;
        }

        public virtual void Discard()
        {
            if (IsExhausted) return;
            if (!IsPlayable) return;
            CollectionManager.OnCardDiscarded(this);
            StartCoroutine(DiscardRoutine());
        }

        public virtual void Exhaust(bool destroy = true)
        {
            if (IsExhausted) return;
            if (!IsPlayable) return;
            IsExhausted = true;
            CollectionManager.OnCardExhausted(this);
            StartCoroutine(ExhaustRoutine(destroy));
        }

        protected virtual void SpendMana(int value)
        {
            if (!IsPlayable) return;

            if (CardData.Type == CardType.Spell)
            {
                FocusStat.StatusValue = 0;
            }

            CombatManager.IncreaseMana(-value);
        }

        protected virtual void SpendTurn(int value)
        {
            if (!IsPlayable) return;

            persistentData.TurnDebt += value;

            if (persistentData.TurnDebt > 0)
            {
                CombatManager.EndTurn();

            }
        }

        protected virtual void ChargeHandell(int sumValue)
        {
            if (!IsPlayable) return;

            persistentData.HandellCount += sumValue;
        }

        public virtual void SetInactiveMaterialState(bool isInactive)
        {
            if (!IsPlayable) return;
            if (isInactive == this.IsInactive) return;

            IsInactive = isInactive;
            passiveImage.gameObject.SetActive(isInactive);
        }
        public virtual void SetGrayedMaterialState(bool state)
        {
            if (!IsPlayable) return;

            grayedImage.gameObject.SetActive(state);
        }


        public virtual void UpdateCardText()
        {
            CardData.UpdateDescription();


            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;

            _instantCostImage.gameObject.SetActive(CardData.TurnCost == 0);
            _turnCostCostImage.gameObject.SetActive(CardData.TurnCost > 0);
            _fatigueCostImage.gameObject.SetActive(CardData.TurnCost > 1);


            if (CardData.Type == CardType.Incantation)
            {
                manaTextField.transform.parent.gameObject.SetActive(false);
                return;
            }

            int manaValue = FinalManaCost;

            if (manaValue > CardData.ManaCost)
            {
                manaTextField.fontStyle = FontStyles.Underline;
                manaTextField.color = Color.red;
            }
            else if (manaValue < CardData.ManaCost)
            {
                manaTextField.color = Color.green;
                manaTextField.fontStyle = FontStyles.Underline;

            }
            else
            {
                manaTextField.fontStyle = FontStyles.Normal;
                manaTextField.color = Color.white;
            }

            manaTextField.text = manaValue.ToString();

        }

        #endregion

        #region Routines
        protected virtual IEnumerator DiscardRoutine(bool destroy = true)
        {
            var timer = 0f;
            transform.SetParent(CollectionManager.HandController.discardTransform);

            var startPos = CachedTransform.localPosition;
            var endPos = Vector3.zero;

            var startScale = CachedTransform.localScale;
            var endScale = Vector3.zero;

            var startRot = CachedTransform.localRotation;
            var endRot = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);

            while (true)
            {
                timer += Time.deltaTime * 5;

                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot, endRot, timer);
                CachedTransform.localScale = Vector3.Lerp(startScale, endScale, timer);

                if (timer >= 1f) break;

                yield return CachedWaitFrame;
            }

            if (destroy)
                Destroy(gameObject);

        }

        static int cardsBeingExhausted = 0;
        static Vector3[] exhaustPositions = new Vector3[]
        {
            Vector3.zero,
            Vector3.right * 1.5f,
            Vector3.left * 1.5f,
            Vector3.right * 3,
            Vector3.left * 3,
            Vector3.down,
            Vector3.down + Vector3.right,
            Vector3.down + Vector3.left,
            Vector3.down + Vector3.right * 2,
            Vector3.down + Vector3.left * 2,
        };

        protected virtual IEnumerator ExhaustRoutine(bool destroy = true)
        {
            var timer = 0f;
            transform.SetParent(CollectionManager.HandController.exhaustTransform);

            var startPos = CachedTransform.localPosition;
            var endPos = Vector3.zero + exhaustPositions[cardsBeingExhausted];
            cardsBeingExhausted++;

            var startScale = CachedTransform.localScale;
            var endScale = Vector3.zero;

            var startRot = CachedTransform.localRotation;
            var endRot = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);

            while (true)
            {
                timer += Time.deltaTime * 3;
                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos - Vector3.down * 0.4f, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, timer);

                if (timer >= 1f) break;

                yield return CachedWaitFrame;
            }

            startPos = CachedTransform.localPosition;
            startRot = CachedTransform.localRotation;
            timer = 0;

            while (true)
            {
                timer += Time.deltaTime;
                //CachedTransform.localPosition = startPos;
                //CachedTransform.localRotation = startRot;

                if (timer >= 0.8f) break;

                yield return CachedWaitFrame;
            }

            timer = 0;

            while (true)
            {
                timer += Time.deltaTime * 3;

                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot, endRot, timer);
                CachedTransform.localScale = Vector3.Lerp(startScale, endScale, timer);

                if (timer >= 1f) break;

                yield return CachedWaitFrame;
            }

            if (destroy)
                Destroy(gameObject);

            cardsBeingExhausted--;

        }

        #endregion

        #region Pointer Events
        protected Coroutine tooltipCR = null;
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipCR == null)
                tooltipCR = StartCoroutine(ShowTooltipInfo());

            AudioManager.Instance.PlayOneShot(AudioActionType.CardHovered);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            HideTooltipInfo(TooltipManager.Instance);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //HideTooltipInfo(TooltipManager.Instance);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //ShowTooltipInfo();
        }
        #endregion

        #region Tooltip
        protected virtual IEnumerator ShowTooltipInfo()
        {
            if (!descriptionRoot) yield break;

            var transformActions = CardData.CardActionDataList.Where(a => a.CardActionType == CardActionType.TransformAllCards);

            if (CardData.KeywordsList.Count <= 0 && transformActions.Count() <= 0) yield break;

            yield return new WaitForSeconds(0.25f);

            var tooltipManager = TooltipManager.Instance;
            foreach (var cardDataSpecialKeyword in CardData.KeywordsList)
            {
                var specialKeyword = tooltipManager.SpecialKeywordData.SpecialKeywordBaseList.Find(x => x.SpecialKeyword == cardDataSpecialKeyword);
                if (specialKeyword != null)
                    ShowTooltipInfo(tooltipManager, specialKeyword.GetContent(), specialKeyword.GetHeader(), descriptionRoot, CursorType.Default, CollectionManager ? CollectionManager.HandController.cam : Camera.main);
            }

            foreach (var transformAction in transformActions)
            {
                string toEraseID = transformAction.ActionAreaValue.ToString();
                string toCreateID = transformAction.ActionValue.ToString();

                CardData cardToEraseData = GameManager.GameplayData.AllCardsList.CardList.Find(c => c.Id == toEraseID);
                CardData cardToCreateData = GameManager.GameplayData.AllCardsList.CardList.Find(c => c.Id == toCreateID);

                ShowTooltipInfo(tooltipManager, cardToEraseData.GetDescriptionForTooltip(), cardToEraseData.CardName, descriptionRoot, CursorType.Default, CollectionManager ? CollectionManager.HandController.cam : Camera.main);
                ShowTooltipInfo(tooltipManager, cardToCreateData.GetDescriptionForTooltip(), cardToCreateData.CardName, descriptionRoot, CursorType.Default, CollectionManager ? CollectionManager.HandController.cam : Camera.main);

            }
        }
        public virtual void ShowTooltipInfo(TooltipManager tooltipManager, string content, string header = "", Transform tooltipStaticTransform = null, CursorType targetCursor = CursorType.Default, Camera cam = null, float delayShow = 0)
        {
            tooltipManager.ShowTooltip(content, header, tooltipStaticTransform, targetCursor, cam, delayShow);
        }

        public virtual void HideTooltipInfo(TooltipManager tooltipManager)
        {
            if (tooltipCR != null)
                StopCoroutine(tooltipCR);
            tooltipCR = null;
            tooltipManager.HideTooltip();
        }
        #endregion

    }
}