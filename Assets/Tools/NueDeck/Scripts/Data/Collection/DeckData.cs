using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Data.Settings;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection
{
    [CreateAssetMenu(fileName = "Deck Data", menuName = "NueDeck/Collection/Deck", order = 1)]
    public class DeckData : ScriptableObject
    {
        [SerializeField] private string deckId;
        [SerializeField] private string deckName;

        [SerializeField] private List<CardData> cardList;
        public List<CardData> CardList => cardList;

        public string DeckId => deckId;

        public string DeckName => deckName;
#if UNITY_EDITOR

        [Button]
        public void LoadOnGamePlaySettings()
        {
            GameplayData gamePlayData = LoadScriptableObject<GameplayData>("Assets/Config/NueDeck Data/Settings/Gameplay Settings.asset");

            if (gamePlayData != null)
            {
                gamePlayData.InitalDeck = this;

                Debug.Log($"Updated InitialDeck in GamePlayData to {this.name}");
            }
            else
            {
                Debug.LogError("Failed to find GamePlayData ScriptableObject at Assets/Config/Nue Data/Settings/Gameplay Settings.asset");
            }
        }

        private static T LoadScriptableObject<T>(string path) where T : ScriptableObject
        {
            AssetDatabase.Refresh(); // Ensure the asset database is up-to-date
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif
    }
}