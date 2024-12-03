using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class GainKeywordAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.GainKeyword;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            CombatManager.CurrentMainAlly.CharacterStats.ApplyStatus((StatusType)actionParameters.AreaValue, (int)actionParameters.Value);
        }
    }


    public class ReduceMovementAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.ReduceMovementBy;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var targetCurMov = actionParameters.TargetCharacter.CharacterStats.CurrentMovement;

            actionParameters.TargetCharacter.CharacterStats.SetCurrentMovement(Mathf.Max(1, targetCurMov - (int)actionParameters.Value));


            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}