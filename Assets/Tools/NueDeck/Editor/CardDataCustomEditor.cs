using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Collection.RewardData;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using System.Security.AccessControl;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using static Codice.Client.BaseCommands.KnownCommandOptions;

namespace NueGames.NueDeck.Editor
{

    public static class EditorUtilities
    {
#if UNITY_EDITOR
        public static T LoadScriptableObject<T>(string path) where T : ScriptableObject
        {
            AssetDatabase.Refresh(); // Ensure the asset database is up-to-date
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif

    }
    public class CardDataAssetHandler
    {
#if UNITY_EDITOR


        [OnOpenAsset]
        public static bool OpenEditor(int instanceId, int line)
        {
            CardData obj = EditorUtility.InstanceIDToObject(instanceId) as CardData;
            if (obj != null)
            {
                CardEditorWindow.OpenCardEditor(obj);
                return true;
            }
            return false;
        }
#endif
    }

    [CustomEditor(typeof(CardData))]
    public class CardDataCustomEditor : UnityEditor.Editor
    {
#if UNITY_EDITOR

        RarityType selectedRarity;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open in editor"))
            {
                CardEditorWindow.OpenCardEditor((CardData)target);
            }


        }

#endif
    }


    [CustomEditor(typeof(DeckData))]
    public class DeckDataCustomEditor : UnityEditor.Editor
    {
#if UNITY_EDITOR

        RarityType selectedRarity;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targetDeck = (DeckData)target;


            if (GUILayout.Button("Load On GamePlay Settings"))
            {
                GameplayData gamePlayData = EditorUtilities.LoadScriptableObject<GameplayData>("Assets/Config/NueDeck Data/Settings/Gameplay Settings.asset");

                if (gamePlayData != null)
                {
                    gamePlayData.InitalDeck = targetDeck;

                    Debug.Log($"Updated InitialDeck in GamePlayData to {targetDeck.DeckName}");
                }
                else
                {
                    Debug.LogError("Failed to find GamePlayData ScriptableObject at Assets/Config/Nue Data/Settings/Gameplay Settings.asset");
                }
            }


            EditorGUILayout.Space(20);

            selectedRarity = (RarityType)EditorGUILayout.EnumPopup(selectedRarity);

            if (GUILayout.Button("AutoLoad Cards"))
            {
                DeckData allCardsDeck = EditorUtilities.LoadScriptableObject<GameplayData>("Assets/Config/NueDeck Data/Settings/Gameplay Settings.asset").AllCardsList;

                foreach (var card in allCardsDeck.CardList)
                {
                    if (card.Rarity == selectedRarity && !targetDeck.CardList.Contains(card))
                        targetDeck.CardList.Add(card);
                }
            }

            serializedObject.ApplyModifiedProperties();

        }

#endif
    }
}