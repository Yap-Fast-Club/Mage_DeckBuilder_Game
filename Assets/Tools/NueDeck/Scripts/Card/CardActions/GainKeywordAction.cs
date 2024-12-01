using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class GainKeywordAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.GainKeyword;
        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            CombatManager.CurrentMainAlly.CharacterStats.ApplyStatus((StatusType)actionParameters.AreaValue, (int)actionParameters.Value);
        }
    }
}