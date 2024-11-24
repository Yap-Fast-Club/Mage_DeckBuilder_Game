using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection.RewardData
{
    [CreateAssetMenu(fileName = "Card Reward Data", menuName = "NueDeck/Collection/Rewards/CardRW", order = 0)]
    public class CardRewardData : RewardDataBase
    {
        [SerializeField] private List<CardData> rewardCardList;

        public List<CardData> RewardCardList => rewardCardList;

        public WeightedList<CardData> weightedCardRewards;

        [SerializeField] List<(int, CardData)> cardWeights;
    }

    [Serializable]
    public class WeightedList<T> : ISerializationCallbackReceiver
    {
        [AllowNesting, SerializeField, ReadOnly, Label("Total Weight")]
        private int InspectorTotalWeight = 0;

        public List<WeightedItem<T>> Items;

        public int TotalWeight { get => Items.Sum(i => i.Weight);}


        void OnValidate()
        {
            UpdateInspectorPercentages();
        }
        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() => this.UpdateInspectorPercentages();

        public void UpdateInspectorPercentages()
        {
            InspectorTotalWeight = TotalWeight;

            if (TotalWeight > 0)
            {
                foreach (var item in Items)
                {
                    item.PercentInList = $" ({((float)item.Weight / TotalWeight * 100).ToString("##")}%)";
                }
            }
        }
    }

    [Serializable]
    public class WeightedItem<T> : ISerializationCallbackReceiver
    {
        [HideInInspector] public string Name;
        [HideInInspector] public string PercentInList;

        public int Weight;
        public T Item;

        void OnValidate()
        {
            if (Item is UnityEngine.Object obj)
                Name = $"{obj.name}";
            else
                Name = $"{Item.ToString()}";

            Name += $".     [W:{Weight}]{PercentInList}";

        }
        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

    }



}