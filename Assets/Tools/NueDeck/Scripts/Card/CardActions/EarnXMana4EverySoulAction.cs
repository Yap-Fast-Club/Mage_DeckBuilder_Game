using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class EarnXMana4EverySoulAction : EarnManaAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.EarnManaForEverySoul;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var value = actionParameters.Value * PersistendData.CurrentSouls;
            base.DoAction(new CardActionParameters(value, actionParameters), blackboard);
        }
    }
}