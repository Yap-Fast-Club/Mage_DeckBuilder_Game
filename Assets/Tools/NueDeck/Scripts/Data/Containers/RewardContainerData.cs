
using System;
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Reward Container", menuName = "NueDeck/Containers/Reward", order = 4)]
    public class RewardContainerData : ScriptableObject
    {
        [SerializeField] private int initialCardPackIndex = 0;
        [SerializeField] private int cardPackRandomRange = 5;
        [SerializeField] private List<CardRewardData> cardRewardDataList;

        [SerializeField] private List<GoldRewardData> goldRewardDataList;
        public List<CardRewardData> CardRewardDataList => cardRewardDataList;
        public List<GoldRewardData> GoldRewardDataList => goldRewardDataList;

     

        private PersistentGameplayData _persistentData => GameManager.Instance.PersistentGameplayData;

        public List<CardData> GetRandomCardReward(out CardRewardData rewardData)
        {
            if (_persistentData.RandomInitialized == false)
            {
                cardRewardDataList.ForEach(crd => crd.weightedCardRewards.Init(GameManager.Instance.Random));
                _persistentData.RandomInitialized = true;
            }

            int min = initialCardPackIndex + _persistentData.OfferedCardProbIndex;
            int max = min + cardPackRandomRange;

            min = Mathf.Min(min, CardRewardDataList.Count - cardPackRandomRange);
            max = Mathf.Min(max, CardRewardDataList.Count);

            rewardData = CardRewardDataList.SystemRandomItem(min, max);

            List<CardData> cardList = rewardData.GetRandomCards(3);
            int indexAdvance = rewardData.ProbabilityIndexAdvance;
            _persistentData.OfferedCardProbIndex += indexAdvance;

            Debug.Log($"{rewardData.name} Reward! (index: {CardRewardDataList.IndexOf(rewardData)}). " +
                       $"\n(Advance: {_persistentData.OfferedCardProbIndex}, now {cardRewardDataList[_persistentData.OfferedCardProbIndex].name})\")");

            return cardList;
        } 

        public int GetRandomGoldReward(out GoldRewardData rewardData)
        { 
            rewardData = GoldRewardDataList.RandomItem();
            var value =Random.Range(rewardData.MinGold, rewardData.MaxGold);

            return value;
        } 
       
    }

}