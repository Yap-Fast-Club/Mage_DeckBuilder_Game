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
        [SerializeField] private TextMeshProUGUI _soulsTextField;
        [SerializeField] private TextMeshProUGUI nameTextField;
        [SerializeField] private TextMeshProUGUI healthTextField;
        [SerializeField] private List<RectTransform> _soulContainers = new List<RectTransform>();
        [SerializeField] private List<RectTransform> _soulOnIcons = new List<RectTransform>();
        [SerializeField] private RectTransform _soulOffRoot = new RectTransform();

        public GameObject RandomizedDeckObject => randomizedDeckObject;
        public TextMeshProUGUI RoomTextField => roomTextField;
        public TextMeshProUGUI GoldTextField => goldTextField;
        public TextMeshProUGUI SoulsTextField => _soulsTextField;
        public TextMeshProUGUI NameTextField => nameTextField;
        public TextMeshProUGUI HealthTextField => healthTextField;

        private PersistentGameplayData _persistentGameplayData => GameManager.PersistentGameplayData;
        private Queue<Coroutine> _gainSoulsCRs = new Queue<Coroutine>();
        
        #region Setup
        private void Awake()
        {
            ResetCanvas();
        }
        #endregion
        
        #region Public Methods
        public void SetRoomText(int roomNumber,bool useStage = false, int stageNumber = -1) => 
            RoomTextField.text = useStage ? $"Room {stageNumber}/{roomNumber}" : $"Room {roomNumber}";

        public void SetGoldText(int value)=>GoldTextField.text = $"{value}";

        public void InstantUpdateSoulsGUI()
        {
            SoulsTextField.text = $"{_persistentGameplayData.CurrentSouls}";

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
            _soulsOnImg.fillAmount =  (float)_persistentGameplayData.CurrentSouls / (float)_persistentGameplayData.MaxSouls;

            int soulAmout = deadEnemy.CharacterStats.CurrentSouls;

            for (int i = _persistentGameplayData.CurrentSouls - soulAmout; i <_persistentGameplayData.CurrentSouls ; i++)
            {
                int soulIndex = Mathf.Min(i, _soulOnIcons.Count - 1);
                soulIndex = Mathf.Max(0, soulIndex);

                var enemyScreenPos = Camera.main.WorldToScreenPoint(deadEnemy.transform.position);

                deadEnemy.EnemyCanvas.IntentImage.rectTransform.SetParent(_soulOffRoot.parent, true);
                _soulOnIcons[soulIndex].position = enemyScreenPos;


                _gainSoulsCRs.Enqueue(StartCoroutine(GainSoulCR(soulIndex)));
            }

        }

        public void SetNameText(string name) => NameTextField.text = $"{name}";

        public void SetHealthText(int currentHealth,int maxHealth) => HealthTextField.text = $"{currentHealth}/{maxHealth}";

        public override void ResetCanvas()
        {
            RandomizedDeckObject.SetActive(_persistentGameplayData.IsRandomHand);
            SetHealthText(_persistentGameplayData.AllyList[0].AllyCharacterData.MaxHealth,_persistentGameplayData.AllyList[0].AllyCharacterData.MaxHealth);
            SetNameText(GameManager.GameplayData.DefaultName);
            SetRoomText(_persistentGameplayData.CurrentEncounterId+1,GameManager.GameplayData.UseStageSystem,_persistentGameplayData.CurrentStageId+1);
            UIManager.InformationCanvas.SetGoldText(_persistentGameplayData.CurrentGold);
            UIManager.InformationCanvas.InstantUpdateSoulsGUI();
        }
        #endregion


        protected virtual IEnumerator GainSoulCR(int soulIndex)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (_gainSoulsCRs.Count > 0)
                yield return new WaitForEndOfFrame();
            else
            {
                _persistentGameplayData.CanSelectCards = false;
                _persistentGameplayData.STOP = true;
            }

            RectTransform soulIcon = _soulOnIcons[soulIndex];
            RectTransform soulContainer = _soulContainers[Mathf.Min(soulIndex, _soulContainers.Count - 1)];

            var startPos = soulIcon.position;
            var endPos = soulContainer.position;
            var startScale = soulContainer.localScale * 0.1f;
            var endScale = soulContainer.localScale * 1;

            var timer = 0f;

            while (true)
            {
                timer += Time.deltaTime * 3;
                soulIcon.position = Vector3.Lerp(startPos + Vector3.down * 10, startPos, timer);
                soulIcon.localScale = Vector3.Lerp(startScale, endScale, timer);

                if (timer >= 1f) break;

                yield return new WaitForEndOfFrame();
            }

            startPos = soulIcon.position;
            Debug.Log(startPos);
            timer = 0;

            while (true)
            {
                timer += Time.deltaTime * 2;
                soulIcon.position = Vector3.Lerp(startPos, endPos, timer);

                if (timer >= 1f) break;

                yield return new WaitForEndOfFrame();
            }

            _gainSoulsCRs.Dequeue();
            yield return new WaitForSeconds(0.5f);

            if (_gainSoulsCRs.Count == 0)
            {
                _persistentGameplayData.CanSelectCards = true;
                _persistentGameplayData.STOP = false;
            }
        }
    }
}