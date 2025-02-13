﻿using System;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.UI;
using NueGames.NueDeck.ThirdParty.NueTooltip.Core;
using NueGames.NueDeck.ThirdParty.NueTooltip.CursorSystem;
using NueGames.NueDeck.ThirdParty.NueTooltip.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Characters
{
    [RequireComponent(typeof(Canvas))]
    public abstract class CharacterCanvas : MonoBehaviour,I2DTooltipTarget
    {
        [Header("References")]
        [SerializeField] protected Transform statusIconRoot;
        [SerializeField] protected Transform partialHighlightRoot;
        [SerializeField] protected Transform fullHighlightRoot;
        [SerializeField] protected Transform descriptionRoot;
        [SerializeField] protected StatusIconsData statusIconsData;
        [SerializeField] protected TextMeshProUGUI currentHealthText;
        [SerializeField] protected TextMeshProUGUI currentAttackText;
        [SerializeField] protected TextMeshProUGUI currentMovementText;
        [SerializeField] protected TextMeshProUGUI currentDelayText;
        [SerializeField] protected TextMeshProUGUI currentSoulsText;
        
        #region Cache

        protected Dictionary<StatusType, StatusIconBase> StatusDict = new Dictionary<StatusType, StatusIconBase>();

        protected Canvas TargetCanvas;
        
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected CombatManager CombatManager => CombatManager.Instance;
        protected CollectionManager CollectionManager => CollectionManager.Instance;
        protected UIManager UIManager => UIManager.Instance;

        #endregion
        
        #region Setup

        public void InitCanvas()
        {
            partialHighlightRoot.gameObject.SetActive(false);
            fullHighlightRoot.gameObject.SetActive(false);
            
            for (int i = 0; i < Enum.GetNames(typeof(StatusType)).Length; i++)
                StatusDict.Add((StatusType) i, null);

            TargetCanvas = GetComponent<Canvas>();

            if (TargetCanvas)
                TargetCanvas.worldCamera = Camera.main;
        }

        #endregion
        
        #region Public Methods
        public void ApplyStatus(StatusType targetStatus, int value)
        {
            if (StatusDict[targetStatus] == null)
            {
                var targetData = statusIconsData.StatusIconList.FirstOrDefault(x => x.IconStatus == targetStatus);
                
                if (targetData == null) return;
                
                var clone = Instantiate(statusIconsData.StatusIconBasePrefab, statusIconRoot);
                clone.SetStatus(targetData);
                StatusDict[targetStatus] = clone;
            }
            
            StatusDict[targetStatus].SetStatusValue(value);
        }

        public void ClearStatus(StatusType targetStatus)
        {
            if (StatusDict[targetStatus])
            {
                Destroy(StatusDict[targetStatus].gameObject);
            }
           
            StatusDict[targetStatus] = null;
        }
        
        public void UpdateStatusText(StatusType targetStatus, int value)
        {
            if (StatusDict[targetStatus] == null) return;
          
            StatusDict[targetStatus].StatusValueText.text = $"{value}";
        }
        
        public void UpdateHealthText(int currentHealth,int maxHealth) =>  currentHealthText.text = $"{currentHealth}/{maxHealth}";
        public void UpdateAttackGUI (int currentAttack) =>  currentAttackText.text = $"{currentAttack}";
        public void UpdateMovementGUI (int currentMovement) =>  currentMovementText.text = $"{currentMovement}";
        public void UpdateDelayGUI(int currentDelay)
        {
            if (currentDelay == 0)
            {
                currentDelayText.transform.parent.gameObject.SetActive(false);
            }
            currentDelayText.text = $"{currentDelay}";
        }
        public void UpdateSoulsGUI (int currentSouls) =>  currentSoulsText.text = $"{currentSouls}";
        public void SetHighlight(bool open, bool partial = true)
        {
            if (partial)
            {
                partialHighlightRoot.gameObject.SetActive(open);
                fullHighlightRoot.gameObject.SetActive(false);

            }
            else
            {
                partialHighlightRoot.gameObject.SetActive(open);
                fullHighlightRoot.gameObject.SetActive(open);
            }
        }
       
        #endregion

        #region Pointer Events
        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowTooltipInfo();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltipInfo(TooltipManager.Instance);
        }

        #endregion

        #region Tooltip
        public void ShowTooltipInfo()
        {
            var tooltipManager = TooltipManager.Instance;
            var specialKeywords = new List<SpecialKeywords>();
            
            foreach (var statusIcon in StatusDict)
            {
                if (statusIcon.Value == null) continue;
               
                var statusData = statusIcon.Value.MyStatusIconData;
                foreach (var statusDataSpecialKeyword in statusData.SpecialKeywords)
                {
                    if (specialKeywords.Contains(statusDataSpecialKeyword)) continue;
                    specialKeywords.Add(statusDataSpecialKeyword);
                }
            }
            
            foreach (var specialKeyword in specialKeywords)
            {
                var specialKeywordData =tooltipManager.SpecialKeywordData.SpecialKeywordBaseList.Find(x => x.SpecialKeyword == specialKeyword);
                if (specialKeywordData != null)
                    ShowTooltipInfo(tooltipManager,specialKeywordData.GetContent(),specialKeywordData.GetHeader(),descriptionRoot);
            }
            
        }
        public void ShowTooltipInfo(TooltipManager tooltipManager, string content, string header = "", Transform tooltipStaticTransform = null, CursorType targetCursor = CursorType.Default,Camera cam = null, float delayShow =0)
        {
            tooltipManager.ShowTooltip(content,header,tooltipStaticTransform,targetCursor,cam,delayShow);
        }

        public void HideTooltipInfo(TooltipManager tooltipManager)
        {
            tooltipManager.HideTooltip();
        }
        

        #endregion
       
    }
}