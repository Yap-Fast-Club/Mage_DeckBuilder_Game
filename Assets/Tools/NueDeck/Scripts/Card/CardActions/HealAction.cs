﻿using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class HealAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Heal;

        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;
            
            actionParameters.SelfCharacter.CharacterStats.Heal(Mathf.RoundToInt(actionParameters.Value));

            if (FxManager != null) 
                FxManager.PlayFx(newTarget.transform, FxType.Heal);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}