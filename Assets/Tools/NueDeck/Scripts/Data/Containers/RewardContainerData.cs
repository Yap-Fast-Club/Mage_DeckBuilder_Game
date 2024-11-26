
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.NueExtentions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Data.Containers
{
    [CreateAssetMenu(fileName = "Reward Container", menuName = "NueDeck/Containers/Reward", order = 4)]
    public class RewardContainerData : ScriptableObject
    {
        [SerializeField] private List<CardRewardData> cardRewardDataList;
        [SerializeField] private List<GoldRewardData> goldRewardDataList;
        public List<CardRewardData> CardRewardDataList => cardRewardDataList;
        public List<GoldRewardData> GoldRewardDataList => goldRewardDataList;

        public List<CardData> GetRandomCardReward(out CardRewardData rewardData)
        {
            //CAMBIAR ESTO
            rewardData = CardRewardDataList.RandomItem();
            
            List<CardData> cardList = new List<CardData>();
            int attempts = 0;

            while (cardList.Count <= 3 && attempts <= 100)
            {
                attempts++;

                var card = rewardData.weightedCardRewards.GetRandomItem();

                if(!cardList.Contains(card))
                    cardList.Add(rewardData.weightedCardRewards.GetRandomItem());
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