﻿using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardActionParameters
    {
        public float Value;
        public readonly float AreaValue;
        public readonly CharacterBase TargetCharacter;
        public readonly CharacterBase SelfCharacter;
        public readonly CardData CardData;
        public readonly CardBase CardBase;
        public readonly AudioActionType ActionAudioType;
        public CardActionParameters(float value,CharacterBase target, CharacterBase self,CardData cardData, CardBase cardBase)
        {
            Value = value;
            TargetCharacter = target;
            SelfCharacter = self;
            CardData = cardData;
            CardBase = cardBase;
            AreaValue = 1;
        }

        public CardActionParameters(float newValue, CardActionParameters copyFrom)
        {
            Value = newValue;
            TargetCharacter = copyFrom.TargetCharacter;
            SelfCharacter = copyFrom.SelfCharacter;
            CardData = copyFrom.CardData;
            CardBase = copyFrom.CardBase;
            AreaValue = copyFrom.AreaValue;
            ActionAudioType = copyFrom.ActionAudioType;
        }

        public CardActionParameters(float value, float areaValue, CharacterBase target, CharacterBase self, CardData cardData, CardBase cardBase, AudioActionType audioType)
        {
            Value = value;
            TargetCharacter = target;
            SelfCharacter = self;
            CardData = cardData;
            CardBase = cardBase;
            AreaValue = areaValue;
            ActionAudioType = audioType;
        }
    }
    public abstract class CardActionBase
    {
        protected CardActionBase(){}
        public abstract CardActionType ActionType { get;}
        public abstract void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard);
        
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected CombatManager CombatManager => CombatManager.Instance;
        protected CollectionManager CollectionManager => CollectionManager.Instance;
        
    }
    
    
   
}