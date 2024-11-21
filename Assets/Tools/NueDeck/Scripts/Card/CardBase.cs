﻿using System;
using System.Collections;
using System.Collections.Generic;
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
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardBase : MonoBehaviour,I2DTooltipTarget, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Base References")]
        [SerializeField] protected Transform descriptionRoot;
        [SerializeField] protected Image cardImage;
        [SerializeField] protected Image passiveImage;
        [SerializeField] protected Image grayedImage;
        [SerializeField] protected TextMeshProUGUI nameTextField;
        [SerializeField] protected TextMeshProUGUI descTextField;
        [SerializeField] protected TextMeshProUGUI manaTextField;
        [SerializeField] protected List<RarityRoot> rarityRootList;
        

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
        
        public bool IsExhausted { get; private set; }

        #endregion
        
        #region Setup
        protected virtual void Awake()
        {
            CachedTransform = transform;
            CachedWaitFrame = new WaitForEndOfFrame();
        }

        public virtual void SetCard(CardData targetProfile,bool isPlayable = true)
        {
            CardData = targetProfile;
            IsPlayable = isPlayable;
            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;
            manaTextField.text = CardData.ManaCost.ToString();
            cardImage.sprite = CardData.CardSprite;
            if (CardData.Type == CardType.Incantation)
            {
                manaTextField.transform.parent.gameObject.SetActive(false);
            }
            foreach (var rarityRoot in RarityRootList)
                rarityRoot.gameObject.SetActive(rarityRoot.Rarity == CardData.Rarity);
        }
        
        #endregion
        
        #region Card Methods
        public virtual void Use(CharacterBase self,CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies)
        {
            if (!IsPlayable) return;
         
            StartCoroutine(CardUseRoutine(self, targetCharacter, allEnemies, allAllies));
        }

        private IEnumerator CardUseRoutine(CharacterBase self,CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies)
        {
            SpendMana(CardData.ManaCost);
            AudioManager.Instance.PlayOneShot(AudioActionType.CardPlayed);

            bool resetEnchantment = false;

            foreach (var actionData in CardData.CardActionDataList)
            {
                yield return new WaitForSeconds(actionData.ActionDelay);
                var targetList = DetermineTargets(targetCharacter, allEnemies, allAllies, actionData);

                var action = CardActionProcessor.GetAction(actionData.CardActionType);
                foreach (var target in targetList)
                    action.DoAction(new CardActionParameters(actionData.ActionValue,target,self,CardData,this));

                if (action is AttackAction)
                    resetEnchantment = true;
            }

            if (resetEnchantment)
                self.CharacterStats.StatusDict[StatusType.Power].StatusValue = 0;


            if (persistentData.HandellIsActive)
            {
                CollectionManager.DrawCards(1);
            }

            ChargeHandell(1);
            CollectionManager.OnCardPlayed(this);

            SpendTurn(CardData.TurnCost);
        }

        private static List<CharacterBase> DetermineTargets(CharacterBase targetCharacter, List<EnemyBase> allEnemies, List<AllyBase> allAllies,
            CardActionData playerAction)
        {
            List<CharacterBase> targetList = new List<CharacterBase>();
            switch (playerAction.ActionTargetType)
            {
                case ActionTargetType.Enemy:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.Ally:
                    targetList.Add(targetCharacter);
                    break;
                case ActionTargetType.AllEnemies:
                    foreach (var enemyBase in allEnemies)
                        targetList.Add(enemyBase);
                    break;
                case ActionTargetType.AllAllies:
                    foreach (var allyBase in allAllies)
                        targetList.Add(allyBase);
                    break;
                case ActionTargetType.RandomEnemy:
                    if (allEnemies.Count>0)
                        targetList.Add(allEnemies.RandomItem());
                    
                    break;
                case ActionTargetType.RandomAlly:
                    if (allAllies.Count>0)
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
            persistentData.CurrentMana -= value;
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
            manaTextField.text = CardData.ManaCost.ToString();

            if (CardData.Type == CardType.Incantation)
                manaTextField.transform.parent.gameObject.SetActive(false);

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
                timer += Time.deltaTime*5;

                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot,endRot,timer);
                CachedTransform.localScale = Vector3.Lerp(startScale, endScale, timer);
                
                if (timer>=1f)  break;
                
                yield return CachedWaitFrame;
            }

            if (destroy)
                Destroy(gameObject);
           
        }
        
        protected virtual IEnumerator ExhaustRoutine(bool destroy = true)
        {
            var timer = 0f;
            transform.SetParent(CollectionManager.HandController.exhaustTransform);

            var startPos = CachedTransform.localPosition;
            var endPos = Vector3.zero;

            var startScale = CachedTransform.localScale;
            var endScale = Vector3.zero;

            var startRot = CachedTransform.localRotation;
            var endRot = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);

            while (true)
            {
                timer += Time.deltaTime * 2;
                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos - Vector3.down * 0.4f, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot,Quaternion.identity,timer);

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
                timer += Time.deltaTime * 2;

                CachedTransform.localPosition = Vector3.Lerp(startPos, endPos, timer);
                CachedTransform.localRotation = Quaternion.Lerp(startRot,endRot,timer);
                CachedTransform.localScale = Vector3.Lerp(startScale, endScale, timer);
                
                if (timer >= 1f)  break;
                
                yield return CachedWaitFrame;
            }

            if (destroy)
                Destroy(gameObject);
           
        }

        #endregion

        #region Pointer Events
        Coroutine tooltipCR = null;
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
            if (CardData.KeywordsList.Count<=0) yield break;

            yield return new WaitForSeconds(0.25f);

            var tooltipManager = TooltipManager.Instance;
            foreach (var cardDataSpecialKeyword in CardData.KeywordsList)
            {
                var specialKeyword = tooltipManager.SpecialKeywordData.SpecialKeywordBaseList.Find(x=>x.SpecialKeyword == cardDataSpecialKeyword);
                if (specialKeyword != null)
                    ShowTooltipInfo(tooltipManager,specialKeyword.GetContent(),specialKeyword.GetHeader(),descriptionRoot,CursorType.Default,CollectionManager ? CollectionManager.HandController.cam : Camera.main);
            }
        }
        public virtual void ShowTooltipInfo(TooltipManager tooltipManager, string content, string header = "", Transform tooltipStaticTransform = null, CursorType targetCursor = CursorType.Default,Camera cam = null, float delayShow =0)
        {
            tooltipManager.ShowTooltip(content,header,tooltipStaticTransform,targetCursor,cam,delayShow);
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