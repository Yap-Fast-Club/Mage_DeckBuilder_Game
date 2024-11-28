using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Characters;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Encounter Data", menuName = "NueDeck/Containers/EncounterData", order = 4)]
    public class EncounterData : ScriptableObject
    {
        [Header("Settings")] 
        [SerializeField] private bool encounterRandomlyAtStage;
        [SerializeField] private List<EnemyEncounterStage> enemyEncounterList;

        public bool EncounterRandomlyAtStage => encounterRandomlyAtStage;
        public List<EnemyEncounterStage> EnemyEncounterList => enemyEncounterList;

        public string GetCurrentLevelName(int stageId = 0,int encounterId =0,bool isFinal = false)
        {
            var selectedStage = EnemyEncounterList.First(x => x.StageId == stageId);
            if (isFinal) return selectedStage.BossEncounterList.RandomItem().LevelName;

            return EncounterRandomlyAtStage
                ? selectedStage.NormalEncounterList.RandomItem().LevelName
                : selectedStage.NormalEncounterList[encounterId] != null ? selectedStage.NormalEncounterList[encounterId].LevelName : selectedStage.NormalEncounterList.RandomItem().LevelName;
        }

        public WavesEncounter GetEncounterWaves(int stageId = 0,int encounterId =0,bool isFinal = false)
        {
            var selectedStage = EnemyEncounterList.First(x => x.StageId == stageId);
            if (isFinal) return selectedStage.BossEncounterList.RandomItem();
           
            return EncounterRandomlyAtStage
                ? selectedStage.NormalEncounterList.RandomItem()
                : selectedStage.NormalEncounterList[encounterId] ?? selectedStage.NormalEncounterList.RandomItem();
        }
    }


    [Serializable]
    public class EnemyEncounterStage
    {
        [SerializeField] private string name;
        [SerializeField] private int stageId;
        [SerializeField] private List<WavesEncounter> normalEncounterList;
        [SerializeField] private List<WavesEncounter> bossEncounterList;
        public string Name => name;
        public int StageId => stageId;
        public List<WavesEncounter> NormalEncounterList => normalEncounterList;
        public List<WavesEncounter> BossEncounterList => bossEncounterList;
    }

    [Serializable]
    public class WavesEncounter : EncounterBase
    {
        [SerializeField, Expandable] private LevelWavesSO _levelWavesData;

        public List<Wave> LevelWaves => _levelWavesData.Waves;
        public string LevelName => _levelWavesData.LevelName;
    }

    
    [Serializable]
    public class EnemyEncounter : EncounterBase
    {
        [SerializeField] private List<EnemyCharacterData> enemyList;
        public List<EnemyCharacterData> EnemyList => enemyList;
    }
    
    [Serializable]
    public abstract class EncounterBase
    {
        [SerializeField] private BackgroundTypes targetBackgroundType;

        public BackgroundTypes TargetBackgroundType => targetBackgroundType;
    }
}