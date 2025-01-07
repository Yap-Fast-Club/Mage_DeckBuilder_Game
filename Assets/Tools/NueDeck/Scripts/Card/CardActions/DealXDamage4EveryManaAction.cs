using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class DealXDamage4EveryManaAction: DealDamageAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.DealDamageForEveryMana;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var value = actionParameters.Value * PersistendData.CurrentMana;

            base.DoAction(new CardActionParameters(value, actionParameters), blackboard);

        }
    }
}