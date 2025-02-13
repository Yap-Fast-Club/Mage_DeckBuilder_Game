﻿using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class LifeStealAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.LifeSteal;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            if (!actionParameters.TargetCharacter) return;

            var value = Mathf.RoundToInt(actionParameters.Value +
                                         actionParameters.SelfCharacter.CharacterStats.StatusDict[StatusType.Power]
                                             .StatusValue);
            actionParameters.TargetCharacter.CharacterStats.Damage(value);
            actionParameters.SelfCharacter.CharacterStats.Heal(value);
            
            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.TargetCharacter.transform,FxType.Attack);
                FxManager.PlayFx(actionParameters.SelfCharacter.transform,FxType.Heal);
                FxManager.SpawnFloatingText(actionParameters.TargetCharacter.TextSpawnRoot,value.ToString());
            }
           
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}