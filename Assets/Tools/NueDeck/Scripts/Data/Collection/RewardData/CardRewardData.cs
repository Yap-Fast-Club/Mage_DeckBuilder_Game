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
    public class CardRewardData : RewardDataBase, ISerializationCallbackReceiver
    {

        public int ProbabilityIndexAdvance { get; private set; }

        [Header("Default Weights")]
        [SerializeField] int _defaultCommonWeight = 4;
        [SerializeField] int _defaultRareWeight = 3;
        [SerializeField] int _defaultSpecialWeight = 2;
        [SerializeField] int _defaultLegendaryWeight = 1;

        [Header("Nerd Stuff")]
        [SerializeField] float _commonProbIndexMod = 0.7f;
        [SerializeField] float _rareProbIndexMod = 0.5f;
        [SerializeField] float _legendaryProbIndexMod = -0.7f;
        [SerializeField] float _specialProbIndexMod = -0.2f;

        [AllowNesting, SerializeField, ReadOnly, Label("Spell Amount")]
        private int InspectorSpellAmount = 0;
        [AllowNesting, SerializeField, ReadOnly, Label("Incant Amount")]
        private int InspectorIncantAmount = 0;

        public WeightedListContainer<CardData> weightedCardRewards;


        public void Add(CardData data)
        {
            int defaultWeight = data.Rarity switch
            {
                Enums.RarityType.Common => _defaultCommonWeight,
                Enums.RarityType.Rare => _defaultRareWeight,
                Enums.RarityType.Legendary => _defaultLegendaryWeight,
                Enums.RarityType.Special => _defaultSpecialWeight,
                _ => 1
            };

            weightedCardRewards.Items.Add(new WeightedListContainer<CardData>.WeightedItem<CardData>() { Item = data, Weight = defaultWeight });
        }

        public List<CardData> GetRandomCards(int amount)
        {
            var cardList = new List<CardData>();
            int attempts = 0;
            float indexMod = 0;

            while (cardList.Count < amount && attempts <= 100)
            {
                attempts++;
                var card = weightedCardRewards.GetRandomItem();
                if (!cardList.Contains(card))
                {
                    cardList.Add(card);
                    indexMod += card.Rarity switch
                    {
                        Enums.RarityType.Common => _commonProbIndexMod,
                        Enums.RarityType.Rare => _rareProbIndexMod,
                        Enums.RarityType.Legendary => _legendaryProbIndexMod,
                        Enums.RarityType.Special => _specialProbIndexMod,
                        _ => 0
                    };
                }
            }

            ProbabilityIndexAdvance = (int)indexMod;
            return cardList;
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.UpdateInspectorRewardInfo();
        void ISerializationCallbackReceiver.OnAfterDeserialize() => this.UpdateInspectorRewardInfo();

        private void UpdateInspectorRewardInfo()
        {
            InspectorSpellAmount = 0;
            InspectorIncantAmount = 0;

            InspectorIncantAmount = weightedCardRewards.Items.Where(wi => wi.Item.Type == CardType.Incantation).Count();
            InspectorSpellAmount = weightedCardRewards.Items.Where(wi => wi.Item.Type == CardType.Spell).Count();
        }










#if UNITY_EDITOR
        [Header("Load From Deck"), InfoBox("If you want a new list, clear the previous list setting the count to 0", EInfoBoxType.Warning)  ]
        [SerializeField] DeckData _deckToLoadFrom;
        [Button]
        private void Load()
        {
            if (_deckToLoadFrom == null) return;

            _deckToLoadFrom.CardList.ForEach(card =>
            {
                int defaultWeight = card.Rarity switch
                {
                    Enums.RarityType.Common => _defaultCommonWeight,
                    Enums.RarityType.Rare => _defaultRareWeight,
                    Enums.RarityType.Legendary => _defaultLegendaryWeight,
                    Enums.RarityType.Special => _defaultSpecialWeight,
                    _ => 1
                };
                weightedCardRewards.Items.Add(new WeightedListContainer<CardData>.WeightedItem<CardData>() { Item = card, Weight = defaultWeight });
            }
            );
        }

#endif
    }




}