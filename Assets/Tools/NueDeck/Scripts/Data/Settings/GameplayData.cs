using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Settings
{
    [CreateAssetMenu(fileName = "Gameplay Data", menuName = "NueDeck/Settings/GameplayData", order = 0)]
    public class GameplayData : ScriptableObject
    {
        [Header("Gameplay Settings")] 
        [SerializeField] private int drawCount = 4;
        [SerializeField] private int initialMana = 5;
        [SerializeField] private int maxMana = 10;
        [SerializeField] private int maxSouls = 5;
        [SerializeField] private List<AllyBase> initalAllyList;
        [SerializeField] private int _collisionDamageOnPush = 1;
        [SerializeField] public int RandomSeed = 0;

        [Header("Decks")] 
        [SerializeField] private DeckData initalDeck;
        [SerializeField] private int maxCardOnHand;
        
        [Header("Card Settings")] 
        [SerializeField] private DeckData allCardsList;
        [SerializeField] private CardBase cardPrefab;

        [Header("Customization Settings")] 
        [SerializeField] private string defaultName = "Nue";
        [SerializeField] private bool useStageSystem;
        public Color InstantTextColor;
        public Color FatigueTextColor;
        public Color EraseTextColor;
        public Color ChannelTextColor;
        public Color NoEffectTextColor;
        
        [Header("Modifiers")]
        [SerializeField] private bool isRandomHand = false;
        [SerializeField] private int randomCardCount;
        
        #region Encapsulation
        public int DrawCount => drawCount;
        public int InitialMana => initialMana;
        public int MaxMana => maxMana;
        public int MaxSouls => maxSouls;
        public int PushCollisionDamage => _collisionDamageOnPush;
        public bool IsRandomHand => isRandomHand;
        public List<AllyBase> InitalAllyList => initalAllyList;
        public DeckData InitalDeck { get => initalDeck; set => initalDeck = value; }
        public int RandomCardCount => randomCardCount;
        public int MaxCardOnHand => maxCardOnHand;
        public DeckData AllCardsList => allCardsList;
        public CardBase CardPrefab => cardPrefab;
        public string DefaultName => defaultName;
        public bool UseStageSystem => useStageSystem;
        #endregion
    }
}