using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class EmptyAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EmptyAction;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            if (!actionParameters.TargetCharacter) return;

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}