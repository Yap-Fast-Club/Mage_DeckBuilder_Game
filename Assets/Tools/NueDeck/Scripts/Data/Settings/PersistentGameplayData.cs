using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Settings
{
    [Serializable]
    public class PersistentGameplayData
    {
        private readonly GameplayData _gameplayData;
        
        [SerializeField] private int currentGold;
        [SerializeField] private int drawCount;
        [SerializeField] private int maxMana;
        [SerializeField] private int currentMana;
        [SerializeField] private int maxSouls;
        [SerializeField] private int currentSouls;
        [SerializeField] private int turnDebt;
        [SerializeField] private int handellCount;
        [SerializeField] private bool canUseCards;
        [SerializeField] private bool canSelectCards;
        [SerializeField] public bool _stop;
        [SerializeField] private bool isRandomHand;
        [SerializeField] private List<AllyBase> allyList;
        [SerializeField] private int currentStageId;
        [SerializeField] private int currentEncounterId;
        [SerializeField] private bool isFinalEncounter;
        [SerializeField] private List<CardData> currentCardsList;
        [SerializeField] private List<AllyHealthData> allyHealthDataDataList;
        public bool RandomInitialized = false;
        public int OfferedCardRewards = 0;

        public PersistentGameplayData(GameplayData gameplayData)
        {
            _gameplayData = gameplayData;

            InitData();
        }
        
        public void SetAllyHealthData(string id,int newCurrentHealth, int newMaxHealth)
        {
            var data = allyHealthDataDataList.Find(x => x.CharacterId == id);
            var newData = new AllyHealthData();
            newData.CharacterId = id;
            newData.CurrentHealth = newCurrentHealth;
            newData.MaxHealth = newMaxHealth;
            if (data != null)
            {
                allyHealthDataDataList.Remove(data);
                allyHealthDataDataList.Add(newData);
            }
            else
            {
                allyHealthDataDataList.Add(newData);
            }
        } 
        private void InitData()
        {
            DrawCount = _gameplayData.DrawCount;
            MaxMana = _gameplayData.MaxMana;
            MaxSouls = _gameplayData.MaxSouls;
            CurrentSouls = 0;
            CurrentMana = _gameplayData.InitialMana;
            CanUseCards = true;
            CanSelectCards = true;
            STOP = false;
            IsRandomHand = _gameplayData.IsRandomHand;
            AllyList = new List<AllyBase>(_gameplayData.InitalAllyList);
            CurrentEncounterId = 0;
            CurrentStageId = 0;
            CurrentGold = 0;
            turnDebt = 0;
            CurrentCardsList = new List<CardData>();
            IsFinalEncounter = false;
            allyHealthDataDataList = new List<AllyHealthData>();
            RandomInitialized = false;
            OfferedCardRewards = 0;
        }

    #region Encapsulation

    public bool STOP
        {
            get => _stop;
            set => _stop = value;
        }

        public int DrawCount
        {
            get => drawCount;
            set => drawCount = value;
        }

        public int MaxMana
        {
            get => maxMana;
            set => maxMana = value;
        }

        public int CurrentMana
        {
            get => currentMana;
            set => currentMana = value < 0 ? 0 : value;
        }
        public int MaxSouls
        {
            get => maxSouls;
            set => maxSouls = value;
        }

        public int CurrentSouls
        {
            get => currentSouls;
            set => currentSouls = value < 0 ? 0 : value;
        }
        public int TurnDebt
        {
            get => turnDebt;
            set => turnDebt = Mathf.Max(0, value);
        }

        public int HandellCount
        {
            get => handellCount;
            set => handellCount = Mathf.Max(0, value);
        }

        public int HandellThreshold
        {
            get => 3;
        }

        public bool HandellIsActive => handellCount >= HandellThreshold;
        public bool CanUseCards
        {
            get => canUseCards;
            set => canUseCards = value;
        }

        public bool CanSelectCards
        {
            get => canSelectCards;
            set => canSelectCards = value;
        }

        public bool IsRandomHand
        {
            get => isRandomHand;
            set => isRandomHand = value;
        }

        public List<AllyBase> AllyList
        {
            get => allyList;
            set => allyList = value;
        }

        public int CurrentStageId
        {
            get => currentStageId;
            set => currentStageId = value;
        }

        public int CurrentEncounterId
        {
            get => currentEncounterId;
            set => currentEncounterId = value;
        }

        public bool IsFinalEncounter
        {
            get => isFinalEncounter;
            set => isFinalEncounter = value;
        }

        public List<CardData> CurrentCardsList
        {
            get => currentCardsList;
            set => currentCardsList = value;
        }

        public List<AllyHealthData> AllyHealthDataList
        {
            get => allyHealthDataDataList;
            set => allyHealthDataDataList = value;
        }
        public int CurrentGold
        {
            get => currentGold;
            set => currentGold = value;
        }
        
        #endregion
    }
}