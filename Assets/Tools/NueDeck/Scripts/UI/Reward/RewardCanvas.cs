using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI.Reward
{
    public class RewardCanvas : CanvasBase
    {
        [Header("References")]
        [SerializeField] private RewardContainerData rewardContainerData;
        [SerializeField] private Transform rewardRoot;
        [SerializeField] private RewardContainer rewardContainerPrefab;
        [SerializeField] private Transform rewardPanelRoot;
        [Header("Choice")]
        [SerializeField] private Transform choice2DCardSpawnRoot;
        [SerializeField] private ChoiceCard choiceCardUIPrefab;
        [SerializeField] private ChoicePanel choicePanel;
        
        private readonly List<RewardContainer> _currentRewardsList = new List<RewardContainer>();
        private readonly List<ChoiceCard> _spawnedChoiceList = new List<ChoiceCard>();
        private readonly List<CardData> _cardRewardList = new List<CardData>();

        public ChoicePanel ChoicePanel => choicePanel;
        
        #region Public Methods

        public void PrepareCanvas()
        {
            rewardPanelRoot.gameObject.SetActive(true);
        }
        public void BuildReward(RewardType rewardType)
        {
            var rewardClone = Instantiate(rewardContainerPrefab, rewardRoot);
            _currentRewardsList.Add(rewardClone);
            
            switch (rewardType)
            {
                case RewardType.Gold:
                    var rewardGold = rewardContainerData.GetRandomGoldReward(out var goldRewardData);
                    rewardClone.BuildReward(goldRewardData.RewardSprite,goldRewardData.RewardDescription);
                    rewardClone.RewardButton.onClick.AddListener(()=>GetGoldReward(rewardClone,rewardGold));
                    break;
                case RewardType.Card:
                    var rewardCardList = rewardContainerData.GetRandomCardRewardList(out var cardRewardData);
                    _cardRewardList.Clear();
                    foreach (var cardData in rewardCardList)
                        _cardRewardList.Add(cardData);
                    rewardClone.BuildReward(cardRewardData.RewardSprite,cardRewardData.RewardDescription);
                    rewardClone.RewardButton.onClick.AddListener(()=>GetCardReward(rewardClone,3));
                    break;
                case RewardType.Relic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null);
            }
        }

        public void InstantReward(RewardType rewardType)
        {
            var rewardClone = Instantiate(rewardContainerPrefab, rewardRoot);
            _currentRewardsList.Add(rewardClone);

            switch (rewardType)
            {
                case RewardType.Gold:
                    var rewardGold = rewardContainerData.GetRandomGoldReward(out var goldRewardData);
                    rewardClone.BuildReward(goldRewardData.RewardSprite, goldRewardData.RewardDescription);
                    rewardClone.RewardButton.onClick.AddListener(() => GetGoldReward(rewardClone, rewardGold));
                    break;
                case RewardType.Card:
                    var rewardCardList = rewardContainerData.GetRandomCardRewardList(out var cardRewardData);
                    _cardRewardList.Clear();
                    foreach (var cardData in rewardCardList)
                        _cardRewardList.Add(cardData);
                    rewardClone.BuildReward(cardRewardData.RewardSprite, cardRewardData.RewardDescription);
                    GetInstantCardReward(rewardClone, 3);
                    break;
                case RewardType.Relic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null);
            }
        }

        public void InstantRemoveCard()
        {
            var rewardClone = Instantiate(rewardContainerPrefab, rewardRoot);
            _currentRewardsList.Add(rewardClone);

            rewardContainerData.GetRandomCardRewardList(out var cardRewardData);
            var removeCardList = new List<CardData>(GameManager.PersistentGameplayData.CurrentCardsList);

            _cardRewardList.Clear();
            foreach (var cardData in removeCardList)
                _cardRewardList.Add(cardData);
            rewardClone.BuildReward(cardRewardData.RewardSprite, cardRewardData.RewardDescription);
            GetInstantCardRemove(rewardClone, 3);
               
        }

        public override void ResetCanvas()
        {
            ResetRewards();
            ResetChoice();
            UIManager.InformationCanvas.UpdateSoulsGUI();
        }

        private void ResetRewards()
        {
            foreach (var rewardContainer in _currentRewardsList)
                Destroy(rewardContainer.gameObject);

            _currentRewardsList?.Clear();
        }

        private void ResetChoice()
        {
            foreach (var choice in _spawnedChoiceList)
            {
                Destroy(choice.gameObject);
            }

            _spawnedChoiceList?.Clear();
            ChoicePanel.DisablePanel();
        }

        #endregion
        
        #region Private Methods
        private void GetGoldReward(RewardContainer rewardContainer,int amount)
        {
            GameManager.PersistentGameplayData.CurrentGold += amount;
            _currentRewardsList.Remove(rewardContainer);
            UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
            Destroy(rewardContainer.gameObject);
        }


        private void GetCardReward(RewardContainer rewardContainer,int amount = 3)
        {
            ChoicePanel.gameObject.SetActive(true);
            
            for (int i = 0; i < amount; i++)
            {
                Transform spawnTransform = choice2DCardSpawnRoot;
              
                var choice = Instantiate(choiceCardUIPrefab, spawnTransform);
                
                var reward = _cardRewardList.RandomItem();
                choice.BuildReward(reward);
                choice.OnCardChose += ResetChoice;
                choice.OnCardChose += ResetCanvas;
                
                _cardRewardList.Remove(reward);
                _spawnedChoiceList.Add(choice);
                _currentRewardsList.Remove(rewardContainer);
                
            }
            
            Destroy(rewardContainer.gameObject);
        }
        #endregion

        private void GetInstantCardReward(RewardContainer rewardContainer, int amount = 3)
        {
            ChoicePanel.gameObject.SetActive(true);

            for (int i = 0; i < amount; i++)
            {
                Transform spawnTransform = choice2DCardSpawnRoot;

                var choice = Instantiate(choiceCardUIPrefab, spawnTransform);

                var reward = _cardRewardList.RandomItem();
                choice.BuildReward(reward);
                choice.OnCardChose += choice.AddCardToHand;
                choice.OnCardChose += ResetCanvas;
                choice.OnCardChose += () => this.gameObject.SetActive(false);

                _cardRewardList.Remove(reward);
                _spawnedChoiceList.Add(choice);
                _currentRewardsList.Remove(rewardContainer);

            }

            Destroy(rewardContainer.gameObject);
        }


        private void GetInstantCardRemove(RewardContainer rewardContainer, int amount = 3)
        {
            ChoicePanel.gameObject.SetActive(true);

            for (int i = 0; i < amount; i++)
            {
                Transform spawnTransform = choice2DCardSpawnRoot;

                var choice = Instantiate(choiceCardUIPrefab, spawnTransform);

                var reward = _cardRewardList.RandomItem();
                choice.BuildReward(reward);
                choice.OnCardChose += choice.RemoveCardFromDeck;
                choice.OnCardChose += ResetCanvas;
                choice.OnCardChose += () => this.gameObject.SetActive(false);

                _cardRewardList.Remove(reward);
                _spawnedChoiceList.Add(choice);
                _currentRewardsList.Remove(rewardContainer);

            }

            Destroy(rewardContainer.gameObject);
        }


       
    }
}