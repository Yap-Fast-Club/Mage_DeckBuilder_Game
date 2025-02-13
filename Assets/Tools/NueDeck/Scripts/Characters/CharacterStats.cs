﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Characters
{
    public class StatusStats
    { 
        public StatusType StatusType { get; set; }
        public int StatusValue { get; set; }
        public bool DecreaseOverTurn { get; set; } // If true, decrease on turn end
        public bool IsPermanent { get; set; } // If true, status can not be cleared during combat
        public bool IsActive { get; set; }
        public bool CanNegativeStack { get; set; }
        public bool ClearAtNextTurn { get; set; }
        
        public Action OnTriggerAction;
        public StatusStats(StatusType statusType,int statusValue,bool decreaseOverTurn = false, bool isPermanent = false,bool isActive = false,bool canNegativeStack = false,bool clearAtNextTurn = false)
        {
            StatusType = statusType;
            StatusValue = statusValue;
            DecreaseOverTurn = decreaseOverTurn;
            IsPermanent = isPermanent;
            IsActive = isActive;
            CanNegativeStack = canNegativeStack;
            ClearAtNextTurn = clearAtNextTurn;
        }
    }
    public class CharacterStats
    { 
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentDamage { get; set;}
        public int CurrentMovement { get; set;}
        public int CurrentMoveDelay { get; set; }
        public int MaxMovement => 9;
        public int CurrentSouls { get; set; }
        public bool IsStunned { get;  set; }
        public bool IsDead { get; private set; }
       
        public Action OnDeath;
        public Action<int, int> OnHealthChanged;
        public Action<int> OnAttackDamageChanged;
        public Action<int> OnMovementChanged;
        public Action<int> OnDelayChanged;
        public Action<int> OnSoulsChanged;
        private readonly Action<StatusType,int> OnStatusChanged;
        private readonly Action<StatusType, int> OnStatusApplied;
        private readonly Action<StatusType> OnStatusCleared;
        public Action OnHealAction;
        public Action OnTakeDamageAction;
        public Action OnShieldGained;
         
        public readonly Dictionary<StatusType, StatusStats> StatusDict = new Dictionary<StatusType, StatusStats>();

        private Dictionary<StatusType, StatusStats> SavedStatus => GameManager.Instance.PersistentGameplayData.SavedStatus;
        
        #region Setup
        public CharacterStats(int maxHealth, CharacterCanvas characterCanvas)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            SetAllStatus();
            
            OnHealthChanged += characterCanvas.UpdateHealthText;
            OnStatusChanged += characterCanvas.UpdateStatusText;
            OnStatusApplied += characterCanvas.ApplyStatus;
            OnStatusCleared += characterCanvas.ClearStatus;
        }

        public CharacterStats(int maxHealth, int curDamage, int curMovement, int moveDelay, int curSouls, CharacterCanvas characterCanvas)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            CurrentMovement = curMovement;
            CurrentMoveDelay = moveDelay;
            CurrentDamage = curDamage;
            CurrentSouls = curSouls;
            SetAllStatus();

            OnHealthChanged += characterCanvas.UpdateHealthText;
            OnAttackDamageChanged += characterCanvas.UpdateAttackGUI;
            OnMovementChanged += characterCanvas.UpdateMovementGUI;
            OnDelayChanged += characterCanvas.UpdateDelayGUI;
            OnSoulsChanged += characterCanvas.UpdateSoulsGUI;
            OnStatusChanged += characterCanvas.UpdateStatusText;
            OnStatusApplied += characterCanvas.ApplyStatus;
            OnStatusCleared += characterCanvas.ClearStatus;


            foreach (var statusEntry in SavedStatus)
            {
                UnityEngine.Debug.Log(statusEntry.Key);
                UnityEngine.Debug.Log(statusEntry.Value.StatusValue);
                ApplyStatus(statusEntry.Key, statusEntry.Value.StatusValue);
            }
        }


        private void SetAllStatus()
        {
            for (int i = 0; i < Enum.GetNames(typeof(StatusType)).Length; i++)
            {
                StatusDict.Add((StatusType) i, new StatusStats((StatusType) i, 0));
            }

            StatusDict[StatusType.Poison].DecreaseOverTurn = true;
            StatusDict[StatusType.Poison].OnTriggerAction += DamagePoison;

            StatusDict[StatusType.Block].ClearAtNextTurn = true;

            StatusDict[StatusType.Power].CanNegativeStack = true;
            StatusDict[StatusType.Dexterity].CanNegativeStack = true;
            StatusDict[StatusType.Focus].CanNegativeStack = true;
            
            StatusDict[StatusType.Stun].DecreaseOverTurn = true;
            StatusDict[StatusType.Stun].OnTriggerAction += CheckStunStatus;

            StatusDict[StatusType.Hibernate].IsPermanent = true;
        }
        #endregion
        
        #region Public Methods
        public void ApplyStatus(StatusType targetStatus,int value)
        {
            if (StatusDict[targetStatus].IsActive)
            {
                StatusDict[targetStatus].StatusValue += value;
                OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
                
            }
            else
            {
                StatusDict[targetStatus].StatusValue = value;
                StatusDict[targetStatus].IsActive = true;
                OnStatusApplied?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
            }
        }
        public void TriggerAllStatus()
        {
            for (int i = 0; i < Enum.GetNames(typeof(StatusType)).Length; i++)
                TriggerStatus((StatusType) i);
        }
        
        public void SetCurrentHealth(int targetCurrentHealth)
        {
            CurrentHealth = targetCurrentHealth <=0 ? 1 : targetCurrentHealth;
            OnHealthChanged?.Invoke(CurrentHealth,MaxHealth);
        }

        public void SetCurrentMovement(int targetMovement)
        {
            CurrentMovement = targetMovement <= 0 ? 1 : 
                             targetMovement  > MaxMovement ? MaxMovement :
                             targetMovement;

            OnMovementChanged?.Invoke(CurrentMovement);
        }

        public void SetCurrentMoveDelay(int targetDelay)
        {
            CurrentMoveDelay = targetDelay < 0 ? 1 : targetDelay;

            OnDelayChanged?.Invoke(CurrentMoveDelay);
        }

        public void SetCurrentDamage(int targetDamage)
        {
            CurrentDamage = targetDamage < 0 ? 0 : targetDamage;
            OnAttackDamageChanged?.Invoke(CurrentDamage);
        }

        public void SetCurrentSouls(int targetSouls)
        {
            CurrentSouls = targetSouls < 0 ? 0 : targetSouls;
            OnSoulsChanged?.Invoke(CurrentSouls);
        }

        public void Heal(int value)
        {
            CurrentHealth += value;
            if (CurrentHealth>MaxHealth)  CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth,MaxHealth);
        }
        
        public void Damage(int value, bool canPierceArmor = false)
        {
            if (IsDead) return;
            value = value < 0 ? 0 : value;
            OnTakeDamageAction?.Invoke();
            var remainingDamage = value;
            
            if (!canPierceArmor)
            {
                if (StatusDict[StatusType.Block].IsActive)
                {
                    ApplyStatus(StatusType.Block,-value);

                    remainingDamage = 0;
                    if (StatusDict[StatusType.Block].StatusValue <= 0)
                    {
                        remainingDamage = StatusDict[StatusType.Block].StatusValue * -1;
                        ClearStatus(StatusType.Block);
                    }
                }
            }
            
            CurrentHealth -= remainingDamage;
            
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnDeath?.Invoke();
                IsDead = true;
            }
            OnHealthChanged?.Invoke(CurrentHealth,MaxHealth);
        }
        
        public void IncreaseMaxHealth(int value)
        {
            MaxHealth += value;
            OnHealthChanged?.Invoke(CurrentHealth,MaxHealth);
        }

        public void ClearAllStatus()
        {
            foreach (var status in StatusDict)
            {
                if (status.Value.IsPermanent && status.Value.StatusValue > 0)
                {
                    bool addedNew = SavedStatus.TryAdd(status.Key, status.Value);

                    if (!addedNew)
                        SavedStatus[status.Key] =  status.Value;
                }
                else
                    ClearStatus(status.Key);
            }
        }
           
        public void ClearStatus(StatusType targetStatus)
        {
            StatusDict[targetStatus].IsActive = false;
            StatusDict[targetStatus].StatusValue = 0;
            OnStatusCleared?.Invoke(targetStatus);
        }

        #endregion

        #region Private Methods
        private void TriggerStatus(StatusType targetStatus)
        {
            StatusDict[targetStatus].OnTriggerAction?.Invoke();
            
            //One turn only statuses
            if (StatusDict[targetStatus].ClearAtNextTurn)
            {
                ClearStatus(targetStatus);
                OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
                return;
            }
            
            //Check status
            if (StatusDict[targetStatus].StatusValue <= 0)
            {
                if (StatusDict[targetStatus].CanNegativeStack)
                {
                    if (StatusDict[targetStatus].StatusValue == 0 && !StatusDict[targetStatus].IsPermanent)
                        ClearStatus(targetStatus);
                }
                else
                {
                    if (!StatusDict[targetStatus].IsPermanent)
                        ClearStatus(targetStatus);
                }
            }
            
            if (StatusDict[targetStatus].DecreaseOverTurn) 
                StatusDict[targetStatus].StatusValue--;
            
            if (StatusDict[targetStatus].StatusValue == 0)
                if (!StatusDict[targetStatus].IsPermanent)
                    ClearStatus(targetStatus);
            
            OnStatusChanged?.Invoke(targetStatus, StatusDict[targetStatus].StatusValue);
        }
        
     
        private void DamagePoison()
        {
            if (StatusDict[StatusType.Poison].StatusValue<=0) return;
            Damage(StatusDict[StatusType.Poison].StatusValue,true);
        }
        
        public void CheckStunStatus()
        {
            if (StatusDict[StatusType.Stun].StatusValue <= 0)
            {
                IsStunned = false;
                return;
            }
            
            IsStunned = true;
        }
        
        #endregion
    }
}