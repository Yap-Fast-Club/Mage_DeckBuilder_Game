
using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.Managers;
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

        private int _seed { get; set;}

        public List<CardData> GetRandomCardReward(out CardRewardData rewardData)
        {
            if (GameManager.Instance.PersistentGameplayData.RandomInitialized == false)
            {
                cardRewardDataList.ForEach(crd => crd.weightedCardRewards.Init(_seed));
                GameManager.Instance.PersistentGameplayData.RandomInitialized = true;
            }

            //CAMBIAR ESTO
            rewardData = CardRewardDataList.RandomItem();
            
            List<CardData> cardList = new List<CardData>();
            int attempts = 0;
                Debug.Log(rewardData.name);

            while (cardList.Count <= 4 && attempts <= 100)
            {
                attempts++;
                var card = rewardData.weightedCardRewards.GetRandomItem();
                Debug.Log(card.name);
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