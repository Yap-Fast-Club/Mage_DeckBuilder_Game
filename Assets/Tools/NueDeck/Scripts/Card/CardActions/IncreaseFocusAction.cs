using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class IncreaseFocusAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.GainFocus;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;
            
            if (!newTarget) return;
            
            newTarget.CharacterStats.ApplyStatus(StatusType.Focus, Mathf.RoundToInt(actionParameters.Value));
            
            if (FxManager != null) 
                FxManager.PlayFx(newTarget.transform, FxType.Buff);

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}