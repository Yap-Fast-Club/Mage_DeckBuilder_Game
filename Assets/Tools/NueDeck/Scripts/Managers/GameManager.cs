using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.EnemyBehaviour;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Managers
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    { 
        public GameManager(){}
        public static GameManager Instance { get; private set; }
        
        [Header("Settings")]
        [SerializeField, Expandable] private GameplayData gameplayData;
        [SerializeField, Expandable] private EncounterData encounterData;
        [SerializeField, Expandable] private SceneData sceneData;


        #region Cache
        public SceneData SceneData => sceneData;
        public EncounterData EncounterData => encounterData;
        public GameplayData GameplayData => gameplayData;
        public PersistentGameplayData PersistentGameplayData { get; private set; }
        protected UIManager UIManager => UIManager.Instance;
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
                transform.parent = null;
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CardActionProcessor.Initialize();
                EnemyActionProcessor.Initialize();
                InitGameplayData();
                SetInitalHand();
            }
        }
        #endregion
        
        #region Public Methods
        public void InitGameplayData()
        { 
            PersistentGameplayData = new PersistentGameplayData(gameplayData);
            if (UIManager)
                UIManager.InformationCanvas.ResetCanvas();
           
        } 
        public CardBase BuildAndGetCard(CardData targetData, Transform parent)
        {
            var clone = Instantiate(GameplayData.CardPrefab, parent);
            clone.SetCard(targetData);
            return clone;
        }
        public void SetInitalHand()
        {
            PersistentGameplayData.CurrentCardsList.Clear();
            
            if (PersistentGameplayData.IsRandomHand)
                for (var i = 0; i < GameplayData.RandomCardCount; i++)
                    PersistentGameplayData.CurrentCardsList.Add(GameplayData.AllCardsList.CardList.RandomItem());
            else
                foreach (var cardData in GameplayData.InitalDeck.CardList)
                    PersistentGameplayData.CurrentCardsList.Add(cardData);
        }
        public void NextEncounter()
        {
            PersistentGameplayData.CurrentEncounterId++;
            if (PersistentGameplayData.CurrentEncounterId>=EncounterData.EnemyEncounterList[PersistentGameplayData.CurrentStageId].NormalEncounterList.Count)
            {
                PersistentGameplayData.CurrentEncounterId = Random.Range(0,
                    EncounterData.EnemyEncounterList[PersistentGameplayData.CurrentStageId].NormalEncounterList.Count);
            }
        }
        public void OnExitApp()
        {
            
        }
        #endregion


        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P))
            {
                PersistentGameplayData.CanSelectCards = true;
                PersistentGameplayData.STOP = false;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
            {
                CombatManager.Instance.CurrentMainAlly.CharacterStats.Heal(20);
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.W))
            {
                CombatManager.Instance.WinCombat();
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.O))
            {
                PersistentGameplayData.STOP = true;

                if (!UIManager.RewardCanvas.gameObject.activeInHierarchy)
                {
                    UIManager.RewardCanvas.gameObject.SetActive(true);
                    UIManager.RewardCanvas.PrepareCanvas(plannedCalls: 1);
                }

                UIManager.RewardCanvas.InstantReward(RewardType.Card, ()=> { PersistentGameplayData.STOP = false; UIManager.RewardCanvas.gameObject.SetActive(false); });
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.M))
            {
                this.GetComponent<SceneChanger>()?.OpenMapScene();
            }


        }
    }
}
