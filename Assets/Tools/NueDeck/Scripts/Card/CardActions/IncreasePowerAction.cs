using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class IncreasePowerAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.GainPower;
        public override void DoAction(CardActionParameters actionParameters)
        {
            CombatManager.CurrentMainAlly.CharacterStats.ApplyStatus(StatusType.Power,Mathf.RoundToInt(actionParameters.Value));
            
            if (FxManager != null) 
                FxManager.PlayFx(CombatManager.CurrentMainAlly.transform, FxType.Str);

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}