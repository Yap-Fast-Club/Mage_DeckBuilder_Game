using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

namespace NueGames.NueDeck.Scripts.Data.Collection.RewardData
{
    [CreateAssetMenu(fileName = "Card Reward Data", menuName = "NueDeck/Collection/Rewards/CardRW", order = 0)]
    public class CardRewardData : RewardDataBase
    {
        public WeightedListContainer<CardData> weightedCardRewards;

        [SerializeField] List<(int, CardData)> cardWeights;



#if UNITY_EDITOR
        [Header("Load From Deck"), InfoBox("If you want a new list, clear the previous list setting the count to 0", EInfoBoxType.Warning)  ]
        [SerializeField] DeckData _deckToLoadFrom;
        [Button]
        private void Load()
        {
            if (_deckToLoadFrom == null) return;

            _deckToLoadFrom.CardList.ForEach(card =>
            {
                weightedCardRewards.Items.Add(new WeightedListContainer<CardData>.WeightedItem<CardData>() { Item = card, Weight = 1 });
            }
            );
        }

#endif
    }




}