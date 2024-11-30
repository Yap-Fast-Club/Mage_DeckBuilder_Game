using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Data.Collection
{
    public enum CardType { Spell, Incantation, Curse}

    [CreateAssetMenu(fileName = "Card Data",menuName = "NueDeck/Collection/Card",order = 0)]
    public class CardData : ScriptableObject
    {
        [Header("Card Profile")] 
        [SerializeField] private string id;
        [ContextMenuItem("Update Asset name","UpdateFileName")]
        [SerializeField] private string cardName;
        [SerializeField] private CardType type;
        [SerializeField] private int manaCost;
        [SerializeField] private int turnCost = 1;
        [SerializeField, ShowAssetPreview] private Sprite cardSprite;
        [SerializeField] private RarityType rarity;
        
        [Header("Action Settings")]
        [SerializeField] private bool usableWithoutTarget;
        [SerializeField] private bool exhaustAfterPlay;
        [SerializeField] private bool _channel = false;
        [SerializeField] private List<CardActionData> cardActionDataList;
        
        [Header("Description")]
        [SerializeField] private List<CardDescriptionData> cardDescriptionDataList;
        [SerializeField] private List<SpecialKeywords> specialKeywordsList;
        
        [Header("Fx")]
        [SerializeField] private AudioActionType audioType;

        [Header("Special")]
        public CardData EvolveToCard;

        #region Cache
        public string Id => id;
        public bool UsableWithoutTarget => usableWithoutTarget;
        public bool Channel => _channel;
        public int ManaCost => manaCost;
        public int TurnCost => turnCost;
        public string CardName => cardName;
        public Sprite CardSprite => cardSprite;
        public List<CardActionData> CardActionDataList => cardActionDataList;
        public List<CardDescriptionData> CardDescriptionDataList => cardDescriptionDataList;
        public List<SpecialKeywords> KeywordsList => specialKeywordsList;
        public AudioActionType AudioType => audioType;
        public string MyDescription { get; set; }
        public CardType Type => type;
        public RarityType Rarity => rarity;

        public bool ExhaustAfterPlay => exhaustAfterPlay;

        private Color InstantTextColor => GameManager.Instance.GameplayData.InstantTextColor;
        private Color FatigueTextColor => GameManager.Instance.GameplayData.FatigueTextColor;
        private Color EraseTextColor => GameManager.Instance.GameplayData.EraseTextColor;
        private Color ChannelTextColor => GameManager.Instance.GameplayData.ChannelTextColor;

        #endregion
        
        #region Methods
        public void UpdateDescription(bool showMods = true)
        {
            var str = new StringBuilder();

            if (turnCost == 0)
            {
                str.Append(ColorExtentions.ColorString("Instant\n", InstantTextColor));
                if (!KeywordsList.Contains(SpecialKeywords.Instant))
                    KeywordsList.Add(SpecialKeywords.Instant);
            }

            if (Channel)
            {
                if (CombatManager.Instance.CurrentMainAlly.CharacterStats.StatusDict[StatusType.AntiChannel].StatusValue == 0)
                    str.Append(ColorExtentions.ColorString("Channel\n", ChannelTextColor));
                else
                    str.Append(ColorExtentions.ColorString("<s>Channel</s>\n", ChannelTextColor));

                if (!KeywordsList.Contains(SpecialKeywords.Channel))    
                    KeywordsList.Add(SpecialKeywords.Channel);
            }

            if (turnCost > 1)
            {
                str.Append(ColorExtentions.ColorString("Fatigue\n", FatigueTextColor));
                if (!KeywordsList.Contains(SpecialKeywords.Fatigue))
                    KeywordsList.Add(SpecialKeywords.Fatigue);
            }


            foreach (var descriptionData in cardDescriptionDataList)
            {
                if (showMods)
                    str.Append(descriptionData.UseModifier && showMods
                        ? descriptionData.GetModifiedValue(this)
                        : descriptionData.GetDescription());
                else
                    str.Append(descriptionData.UseModifier && showMods
                        ? descriptionData.GetValueNoMods(this)
                        : descriptionData.GetDescription());
            }

          

            if (exhaustAfterPlay)
            {
                str.Append(ColorExtentions.ColorString("\nErase", EraseTextColor));
                if (!KeywordsList.Contains(SpecialKeywords.Erase))
                    KeywordsList.Add(SpecialKeywords.Erase);
            }

            UpdateSpecialKeyowrds();

            MyDescription = str.ToString();
        }

        private void UpdateSpecialKeyowrds()
        {
            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.Focus))
                if (!KeywordsList.Contains(SpecialKeywords.Focus))
                    KeywordsList.Add(SpecialKeywords.Focus);

            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.Power))
                if (!KeywordsList.Contains(SpecialKeywords.Power))
                    KeywordsList.Add(SpecialKeywords.Power);

            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.PickErase))
                if (!KeywordsList.Contains(SpecialKeywords.Erase))
                    KeywordsList.Add(SpecialKeywords.Erase);

            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.RandomErase))
                if (!KeywordsList.Contains(SpecialKeywords.Erase))
                    KeywordsList.Add(SpecialKeywords.Erase);

            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.Push))
                if (!KeywordsList.Contains(SpecialKeywords.Push))
                    KeywordsList.Add(SpecialKeywords.Push);

            if (CardActionDataList.Any(a => a.CardActionType == CardActionType.DealDamageForEveryMana))
                if (!KeywordsList.Contains(SpecialKeywords.ReserveMana))
                    KeywordsList.Add(SpecialKeywords.ReserveMana);

        }

        public string GetDescriptionForTooltip()
        {
            StringBuilder str = Type switch
            {
                CardType.Spell => new StringBuilder($"(Spell) [{ManaCost} Mana] "),
                CardType.Incantation => new StringBuilder("(Incant.) "),
                _ => new StringBuilder()
            };

            foreach (var descriptionData in cardDescriptionDataList)
            {
                str.Append(descriptionData.UseModifier 
                    ? descriptionData.GetValueNoMods(this, false)
                    : descriptionData.GetDescription());
            }
            return str.ToString();
        }


        #endregion

        #region Editor Methods
#if UNITY_EDITOR
        public void EditCardName(string newName) => cardName = newName;
        public void EditId(string newId) => id = newId;
        public void EditManaCost(int newCost) => manaCost = newCost;
        public void EditTurnCost(int newCost) => turnCost = newCost;
        public void EditType(CardType targetType) => type = targetType;
        public void EditRarity(RarityType targetRarity) => rarity = targetRarity;
        public void EditCardSprite(Sprite newSprite) => cardSprite = newSprite;
        public void EditUsableWithoutTarget(bool newStatus) => usableWithoutTarget = newStatus;
        public void EditExhaustAfterPlay(bool newStatus) => exhaustAfterPlay = newStatus;
        public void EditChannelBool(bool newStatus) => _channel = newStatus;
        public void EditCardActionDataList(List<CardActionData> newCardActionDataList) =>
            cardActionDataList = newCardActionDataList;
        public void EditCardDescriptionDataList(List<CardDescriptionData> newCardDescriptionDataList) =>
            cardDescriptionDataList = newCardDescriptionDataList;
        public void EditSpecialKeywordsList(List<SpecialKeywords> newSpecialKeywordsList) =>
            specialKeywordsList = newSpecialKeywordsList;
        public void EditAudioType(AudioActionType newAudioActionType) => audioType = newAudioActionType;

        

        private void UpdateFileName()
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, $"{id}-{cardName}");
            AssetDatabase.SaveAssets();
        }
#endif

        #endregion

    }

    [Serializable]
    public class CardActionData
    {
        [SerializeField] private CardActionType cardActionType;
        [SerializeField] private ActionTargetType actionTargetType;
        [SerializeField] private ActionAreaType actionAreaType;
        [SerializeField] private int actionAreaValue;
        [SerializeField] private float actionValue;
        [SerializeField] private float actionDelay;

        public ActionTargetType ActionTargetType => actionTargetType;
        public ActionAreaType ActionAreaType => actionAreaType;
        public int ActionAreaValue => actionAreaValue;
        public CardActionType CardActionType => cardActionType;
        public float ActionValue => actionValue;
        public float ActionDelay => actionDelay;

        public float GetModifiedValue(CardData cardData)
        {
            var duple = (cardData.Id, cardActionType);

            if( GameManager.Instance.PersistentGameplayData.ActionMods.ContainsKey(duple))
                return actionValue + GameManager.Instance.PersistentGameplayData.ActionMods[duple];
            else
                return actionValue;
        }

        #region Editor

#if UNITY_EDITOR
        public void EditActionType(CardActionType newType) =>  cardActionType = newType;
        public void EditActionTarget(ActionTargetType newTargetType) => actionTargetType = newTargetType;
        public void EditActionArea(ActionAreaType newAreaType, int newAreaValue) { actionAreaType = newAreaType; actionAreaValue = newAreaValue; }
        public void EditActionValue(float newValue) => actionValue = newValue;
        public void EditActionDelay(float newValue) => actionDelay = newValue;

#endif


        #endregion
    }

    [Serializable]
    public class CardDescriptionData
    {
        [Header("Text")]
        [SerializeField] private string descriptionText;
        [SerializeField] private bool enableOverrideColor;
        [SerializeField] private Color overrideColor = Color.black;
       
        [Header("Modifer")]
        [SerializeField] private bool useModifier;
        [SerializeField] private int modifiedActionValueIndex;
        [SerializeField] private StatusType modiferStats;
        [SerializeField] private bool usePrefixOnModifiedValue;
        [SerializeField] private string modifiedValuePrefix = "*";
        [SerializeField] private bool overrideColorOnValueScaled;

        public string DescriptionText => descriptionText;
        public bool EnableOverrideColor => enableOverrideColor;
        public Color OverrideColor => overrideColor;
        public bool UseModifier => useModifier;
        public int ModifiedActionValueIndex => modifiedActionValueIndex;
        public StatusType ModiferStats => modiferStats;
        public bool UsePrefixOnModifiedValue => usePrefixOnModifiedValue;
        public string ModifiedValuePrefix => modifiedValuePrefix;
        public bool OverrideColorOnValueScaled => overrideColorOnValueScaled;
        
        private CombatManager CombatManager => CombatManager.Instance;

        public string GetDescription()
        {
            var str = new StringBuilder();
            
            str.Append(DescriptionText);
            
            if (EnableOverrideColor && !string.IsNullOrEmpty(str.ToString())) 
                str.Replace(str.ToString(),ColorExtentions.ColorString(str.ToString(),OverrideColor));
            
            return str.ToString();
        }

        public string GetValueNoMods(CardData cardData, bool overrideColor = true)
        {
            if (cardData.CardActionDataList.Count <= 0) return "";

            if (ModifiedActionValueIndex >= cardData.CardActionDataList.Count)
                modifiedActionValueIndex = cardData.CardActionDataList.Count - 1;

            if (ModifiedActionValueIndex < 0)
                modifiedActionValueIndex = 0;

            var str = new StringBuilder();
            var value = cardData.CardActionDataList[ModifiedActionValueIndex].GetModifiedValue(cardData);
                

            str.Append(value);

            if (EnableOverrideColor && overrideColor)
            {
                str.Replace(str.ToString(), ColorExtentions.ColorString(str.ToString(), OverrideColor));
            }

            return str.ToString();
        }

        public string GetModifiedValue(CardData cardData)
        {
            if (cardData.CardActionDataList.Count <= 0) return "";
            
            if (ModifiedActionValueIndex>=cardData.CardActionDataList.Count)
                modifiedActionValueIndex = cardData.CardActionDataList.Count - 1;

            if (ModifiedActionValueIndex<0)
                modifiedActionValueIndex = 0;
            
            var str = new StringBuilder();
            var value = cardData.CardActionDataList[ModifiedActionValueIndex].GetModifiedValue(cardData);
            var modifer = 0;
            if (CombatManager)
            {
                var player = CombatManager.CurrentMainAlly;
               
                if (player)
                {
                    if (ModiferStats == StatusType.EnchantmentAndLeftMana)
                    {
                        modifer = player.CharacterStats.StatusDict[StatusType.Power].StatusValue + ((GameManager.Instance.PersistentGameplayData.CurrentMana 
                            - Mathf.Max(0, cardData.ManaCost - player.CharacterStats.StatusDict[StatusType.Focus].StatusValue) - 1) * (int)value);
                        modifer = modifer < 0 ? 0 : modifer;
                    }
                    else if (ModiferStats == StatusType.SoulScale)
                    {
                        modifer = (GameManager.Instance.PersistentGameplayData.CurrentSouls -1) * ((int)value);
                    }
                    else if (ModiferStats == StatusType.EnchantmentAndSoulScale)
                    {
                        modifer = player.CharacterStats.StatusDict[StatusType.Power].StatusValue + (GameManager.Instance.PersistentGameplayData.CurrentSouls - 1) * ((int)value);
                    }
                    else if (ModiferStats == StatusType.EnemyAmount)
                    {
                        modifer = CombatManager.CurrentEnemiesList.Count();
                    }
                    else
                    {
                        modifer = player.CharacterStats.StatusDict[ModiferStats].StatusValue;
                    }
                    value += modifer;

                    if (modifer != 0)
                    {
                        if (usePrefixOnModifiedValue)
                            str.Append(modifiedValuePrefix);
                    }
                }
            }
           
            str.Append(value);

            if (EnableOverrideColor)
            {
                if (OverrideColorOnValueScaled)
                {
                    if (modifer != 0)
                        str.Replace(str.ToString(),ColorExtentions.ColorString(str.ToString(),OverrideColor));
                }
                else
                {
                    str.Replace(str.ToString(),ColorExtentions.ColorString(str.ToString(),OverrideColor));
                }
               
            }
            
            return str.ToString();
        }

        #region Editor
#if UNITY_EDITOR
        
        public string GetDescriptionEditor()
        {
            var str = new StringBuilder();
            
            str.Append(DescriptionText);
            
            return str.ToString();
        }

        public string GetModifiedValueEditor(CardData cardData)
        {
            if (cardData.CardActionDataList.Count <= 0) return "";
            
            if (ModifiedActionValueIndex>=cardData.CardActionDataList.Count)
                modifiedActionValueIndex = cardData.CardActionDataList.Count - 1;

            if (ModifiedActionValueIndex<0)
                modifiedActionValueIndex = 0;
            
            var str = new StringBuilder();
            var value = cardData.CardActionDataList[ModifiedActionValueIndex].ActionValue;
            if (CombatManager)
            {
                var player = CombatManager.CurrentMainAlly;
                if (player)
                {
                    var modifer =player.CharacterStats.StatusDict[ModiferStats].StatusValue;
                    value += modifer;
                
                    if (modifer!= 0)
                        str.Append("*");
                }
            }
           
            str.Append(value);
          
            return str.ToString();
        }
        
        public void EditDescriptionText(string newText) => descriptionText = newText;
        public void EditEnableOverrideColor(bool newStatus) => enableOverrideColor = newStatus;
        public void EditOverrideColor(Color newColor) => overrideColor = newColor;
        public void EditUseModifier(bool newStatus) => useModifier = newStatus;
        public void EditModifiedActionValueIndex(int newIndex) => modifiedActionValueIndex = newIndex;
        public void EditModiferStats(StatusType newStatusType) => modiferStats = newStatusType;
        public void EditUsePrefixOnModifiedValues(bool newStatus) => usePrefixOnModifiedValue = newStatus;
        public void EditPrefixOnModifiedValues(string newText) => modifiedValuePrefix = newText;
        public void EditOverrideColorOnValueScaled(bool newStatus) => overrideColorOnValueScaled = newStatus;

#endif
#endregion
    }
}