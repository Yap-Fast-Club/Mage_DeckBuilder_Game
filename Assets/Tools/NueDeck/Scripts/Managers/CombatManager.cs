using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Card.CardActions;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Characters.Enemies;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.UI;
using NueGames.NueDeck.Scripts.Utils.Background;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Managers
{
    public class CombatManager : MonoBehaviour
    {
        private CombatManager(){}
        public static CombatManager Instance { get; private set; }

        [Header("References")] 
        [SerializeField] private BackgroundContainer backgroundContainer;
        [SerializeField] private List<Transform> enemyPosList;
        [SerializeField] private List<Transform> allyPosList;
        [SerializeField] private TargetDetector _targetDetector;
        [SerializeField] private CardData _channelCardData;
        [SerializeField] private CardBase _channelCardBase;

        [Header("Custom")]
        [SerializeField] private bool _capManaAtMax = true;
 
        
        #region Cache
        public List<EnemyBase> CurrentEnemiesList { get; private set; } = new List<EnemyBase>();
        public List<AllyBase> CurrentAlliesList { get; private set; }= new List<AllyBase>();

        public Action OnAllyTurnStarted;
        public Action OnEnemyTurnStarted;
        public Action<EnemyBase> EnemySpawned;
        public Action<EnemyBase> EnemyDeath;
        public List<Transform> EnemyPosList => enemyPosList;

        public List<Transform> AllyPosList => allyPosList;

        public AllyBase CurrentMainAlly => CurrentAlliesList.Count>0 ? CurrentAlliesList[0] : null;

        public WavesEncounter CurrentEncounter { get; private set; }

        public TargetDetector TargetDetector => _targetDetector;
        
        public CombatStateType CurrentCombatStateType
        {
            get => _currentCombatStateType;
            private set
            {
                ExecuteCombatState(value);
                _currentCombatStateType = value;
            }
        }
        
        private CombatStateType _currentCombatStateType;
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected PersistentGameplayData persistentData => GameManager.PersistentGameplayData;
        protected UIManager UIManager => UIManager.Instance;
        protected WaveManager WaveManager => WaveManager.Instance;

        protected CollectionManager CollectionManager => CollectionManager.Instance;

        public CardBase ChannelCard => _channelCardBase;

        #endregion
        
        
        #region Setup
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            } 
            else
            {
                Instance = this;
                CurrentCombatStateType = CombatStateType.PrepareCombat;
            }
        }

        private void Start()
        {
            StartCombat();
        }

        public void StartCombat()
        {
            BuildEnemies();
            BuildAllies();
            backgroundContainer.OpenSelectedBackground();
          
            CollectionManager.SetGameDeck();

            ChannelCard.SetCard(Instantiate(_channelCardData));

            CollectionManager.DrawCards(persistentData.DrawCount);
           
            UIManager.CombatCanvas.gameObject.SetActive(true);
            UIManager.InformationCanvas.gameObject.SetActive(true);
            CurrentCombatStateType = CombatStateType.AllyTurn;
        }


        private void ExecuteCombatState(CombatStateType targetStateType)
        {
            switch (targetStateType)
            {
                case CombatStateType.PrepareCombat:
                    StartCoroutine(PrepareCombatRoutine());
                    break;
                case CombatStateType.AllyTurn:

                    OnAllyTurnStarted?.Invoke();

                    StartCoroutine(AllyTurnRoutine());
                    break;
                case CombatStateType.EnemyTurn:

                    OnEnemyTurnStarted?.Invoke();

                    persistentData.CanSelectCards = false;
                    StartCoroutine(nameof(EnemyTurnRoutine));
                    
                    
                    break;
                case CombatStateType.EndCombat:
                    UIManager.CombatCanvas.Unbind();
                    AudioManager.StopMusic();
                    AudioManager.PlayMusic(AudioActionType.MenuMusic);
                    persistentData.CanSelectCards = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetStateType), targetStateType, null);
            }
        }
        #endregion

        #region Public Methods
        public void EndTurn()
        {
            CurrentCombatStateType = CombatStateType.EnemyTurn;
        }

       

        public void OnAllyDeath(AllyBase targetAlly)
        {
            var targetAllyData = persistentData.AllyList.Find(x =>
                x.AllyCharacterData.CharacterID == targetAlly.AllyCharacterData.CharacterID);
            if (persistentData.AllyList.Count>1)
                persistentData.AllyList.Remove(targetAllyData);
            CurrentAlliesList.Remove(targetAlly);
            UIManager.InformationCanvas.ResetCanvas();
            if (CurrentAlliesList.Count<=0)
                LoseCombat();
        }
        public void OnEnemyDeath(EnemyBase targetEnemy)
        {
            EnemyDeath?.Invoke(targetEnemy);
            CurrentEnemiesList.Remove(targetEnemy);
            persistentData.CurrentSouls += targetEnemy.GetComponent<SoulContainer>().SoulAmount;
            UIManager.InformationCanvas.UpdateSoulsGUI(targetEnemy);
        }

        public void DeactivateCardHighlights()
        {
            foreach (var currentEnemy in CurrentEnemiesList)
                currentEnemy.EnemyCanvas.SetHighlight(false);

            foreach (var currentAlly in CurrentAlliesList)
                currentAlly.AllyCanvas.SetHighlight(false);
        }
        public void IncreaseMana(int target)
        {
            int totalMana = persistentData.CurrentMana + target;

            persistentData.CurrentMana = _capManaAtMax ? Mathf.Min(totalMana, persistentData.MaxMana) : totalMana;

            UIManager.CombatCanvas.SetPileTexts();
        }

        public void CheckForSoulReward()
        {
            if (persistentData.CurrentSouls >= persistentData.MaxSouls)
            {
                if (!UIManager.RewardCanvas.gameObject.activeInHierarchy){
                    UIManager.RewardCanvas.gameObject.SetActive(true);
                    UIManager.RewardCanvas.PrepareCanvas(plannedCalls: persistentData.CurrentSouls / persistentData.MaxSouls);
                }

                persistentData.CurrentSouls -= persistentData.MaxSouls;

                UIManager.RewardCanvas.InstantReward(RewardType.Card, OnComplete: CheckForSoulReward);
            }
            else
            {
                UIManager.RewardCanvas.gameObject.SetActive(false); 
            }
        }
        public void HighlightCardTarget(ActionTargetType targetTypeTargetType, int areaValue = 1)
        {
            switch (targetTypeTargetType)
            {
                case ActionTargetType.Enemy:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.Ally:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.AllEnemies:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true, false);
                    break;
                case ActionTargetType.AllAllies:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.RandomEnemy:
                    foreach (var currentEnemy in CurrentEnemiesList)
                        currentEnemy.EnemyCanvas.SetHighlight(true);
                    break;
                case ActionTargetType.LowestHPEnemy:
                    CurrentEnemiesList.Select(e => (e, e.CharacterStats.CurrentHealth))
                                            .OrderBy(x => x.CurrentHealth)
                                            .FirstOrDefault()
                                            .e.EnemyCanvas.SetHighlight(true, false);
                    break;
                case ActionTargetType.ClosestEnemies:
                    CurrentEnemiesList.OrderBy(e => e.transform.position.x)
                                            .Take(areaValue)
                                            .ToList()
                                            .ForEach(e => e.EnemyCanvas.SetHighlight(true, false));
                    break;
                case ActionTargetType.RandomAlly:
                    foreach (var currentAlly in CurrentAlliesList)
                        currentAlly.AllyCanvas.SetHighlight(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetTypeTargetType), targetTypeTargetType, null);
            }
        }
        #endregion
        
        #region Private Methods
        private void BuildEnemies()
        {
            CurrentEncounter = GameManager.EncounterData.GetEncounterWaves(
                persistentData.CurrentStageId,
                persistentData.CurrentEncounterId,
                persistentData.IsFinalEncounter);


            WaveManager.Instance.SetLevelWaves(CurrentEncounter.LevelWaves);
            WaveManager.Instance.StartWave();
        }
        private void BuildAllies()
        {
            for (var i = 0; i < persistentData.AllyList.Count; i++)
            {
                var clone = Instantiate(persistentData.AllyList[i], AllyPosList.Count >= i ? AllyPosList[i] : AllyPosList[0]);
                clone.BuildCharacter();
                CurrentAlliesList.Add(clone);
            }
        }
        private void LoseCombat()
        {
            if (CurrentCombatStateType == CombatStateType.EndCombat) return;
            
            CurrentCombatStateType = CombatStateType.EndCombat;
            
            CollectionManager.DiscardHand();
            CollectionManager.DiscardPile.Clear();
            CollectionManager.DrawPile.Clear();
            CollectionManager.HandPile.Clear();
            CollectionManager.HandController.hand.Clear();
            UIManager.CombatCanvas.gameObject.SetActive(true);
            UIManager.CombatCanvas.CombatLosePanel.SetActive(true);
        }
        private void WinCombat()
        {
            if (CurrentCombatStateType == CombatStateType.EndCombat) return;
          
            CurrentCombatStateType = CombatStateType.EndCombat;
           
            foreach (var allyBase in CurrentAlliesList)
            {
                persistentData.SetAllyHealthData(allyBase.AllyCharacterData.CharacterID,
                    allyBase.CharacterStats.CurrentHealth, allyBase.CharacterStats.MaxHealth);
            }
            
            CollectionManager.ClearPiles();
            
           
            if (persistentData.IsFinalEncounter)
            {
                UIManager.CombatCanvas.CombatWinPanel.SetActive(true);
            }
            else
            {
                UIManager.CombatCanvas.NextCombatPanel.SetActive(true);
                CurrentMainAlly.CharacterStats.ClearAllStatus();
                persistentData.CurrentEncounterId++;
                //UIManager.CombatCanvas.gameObject.SetActive(false);
                //UIManager.RewardCanvas.gameObject.SetActive(true);
                //UIManager.RewardCanvas.PrepareCanvas();
                //UIManager.RewardCanvas.BuildReward(RewardType.Gold);
                //UIManager.RewardCanvas.BuildReward(RewardType.Card);
            }
           
        }
        #endregion
        
        #region Routines

        private IEnumerator AllyTurnRoutine()
        {
            var waitDelay = new WaitForSeconds(0.1f);

            yield return waitDelay;
            yield return new WaitWhile(() => persistentData.STOP == true);

            //Check channeled Cards
            yield return StartCoroutine(ChanneledCardUseRoutine());
            ChannelCard.CardData.CardActionDataList.Clear();

            UIManager.CombatCanvas.EnableHandell(true);
            persistentData.CanSelectCards = true;
            CheckForSoulReward();

            persistentData.TurnDebt--;

            if (CurrentEnemiesList.Count <= 0 && WaveManager.CurrentWaveIsFinal() && WaveManager.CurrentWaveIsCompleted())
                WinCombat();

            if (persistentData.TurnDebt > 0)
            {
                EndTurn();
                yield break;
            }

        }

        private IEnumerator ChanneledCardUseRoutine()
        {
            AudioManager.Instance.PlayOneShot(AudioActionType.CardPlayed);
            bool resetPower = false;
            foreach (var actionData in ChannelCard.CardData.CardActionDataList)
            {
                yield return new WaitForSeconds(actionData.ActionDelay);
                var targetList = CardBase.DetermineTargets(null, CurrentEnemiesList, CurrentAlliesList, actionData);

                var action = CardActionProcessor.GetAction(actionData.CardActionType);
                foreach (var target in targetList)
                    action.DoAction(new CardActionParameters(actionData.ActionValue, actionData.ActionAreaValue, target, CurrentMainAlly, ChannelCard.CardData, ChannelCard));

                if (action is AttackAction)
                    resetPower = true;
            }

            if (resetPower)
               CurrentMainAlly.CharacterStats.ClearStatus(StatusType.Power);

            yield return new WaitForSeconds(0.1f);
        }


        private IEnumerator PrepareCombatRoutine()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);
            yield return new WaitUntil(() => UIManager.Instance != null);

            GameManager.PersistentGameplayData.CurrentMana = 5;
            GameManager.PersistentGameplayData.HandellCount = 0;
            GameManager.PersistentGameplayData.TurnDebt = 0;

            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(AudioActionType.CombatMusic);
            CurrentMainAlly.CharacterStats.SetCurrentHealth(CurrentMainAlly.CharacterStats.MaxHealth);
            UIManager.CombatCanvas.Bind();
            UIManager.Instance.CombatCanvas.SetPileTexts();
            UIManager.Instance.InformationCanvas.ResetCanvas();
        }

        private IEnumerator EnemyTurnRoutine()
        {
            UIManager.Instance.CombatCanvas.EnableHandell(false);
            var waitDelay = new WaitForSeconds(0.1f);

            yield return waitDelay;
            yield return new WaitWhile(() => persistentData.STOP == true);

            List<EnemyBase> enemiesToProcess = new List<EnemyBase>(CurrentEnemiesList);

            foreach (var currentEnemy in enemiesToProcess)
            {
                yield return currentEnemy.StartCoroutine(nameof(EnemyExample.ActionRoutine));
                yield return waitDelay;
            }


            if (CurrentCombatStateType != CombatStateType.EndCombat)
                CurrentCombatStateType = CombatStateType.AllyTurn;
        }
        #endregion
    }
}