
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
        private int _seed { get; set;}

        public List<CardData> GetRandomCardReward(out CardRewardData rewardData)
        {
            if (_persistentData.RandomInitialized == false)
            {
                cardRewardDataList.ForEach(crd => crd.weightedCardRewards.Init(_seed));
                _persistentData.RandomInitialized = true;
            }

            int min = initialCardPackIndex + _persistentData.OfferedCardRewards;
            int max = min + cardPackRandomRange;

            min = Mathf.Min(min, CardRewardDataList.Count - cardPackRandomRange);
            max = Mathf.Min(max, CardRewardDataList.Count);

            rewardData = CardRewardDataList.RandomItem(min, max);
            _persistentData.OfferedCardRewards += rewardData.ProbabilityIndexAdvance;
            Debug.Log($"{rewardData.name} Reward!");

            List<CardData> cardList = new List<CardData>();
            int attempts = 0;

            while (cardList.Count <= 4 && attempts <= 100)
            {
                attempts++;
                var card = rewardData.weightedCardRewards.GetRandomItem();
                if(!cardList.Contains(card))
                    cardList.Add(card);
            }

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