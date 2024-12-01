using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class DealXDamage4EverySoulAction : DealDamageAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.DealDamageForEverySoul;
        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            var value = actionParameters.Value * PersistendData.CurrentSouls;

            base.DoAction(new CardActionParameters(value, actionParameters),blackboard);
        }
    }

    public class HealX4EverySoulAction : HealAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.HealForEverySoul;
        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            actionParameters.Value = actionParameters.Value * PersistendData.CurrentSouls;

            base.DoAction(actionParameters, blackboard);

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);

        }
    }


    public class DealDamageXEverySpentSoulAction : DealDamageAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.DealDamageForEverySpentSoul;

        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            int spentSouls = blackboard.SpentSouls;

            var value = actionParameters.Value * spentSouls;

            base.DoAction(new CardActionParameters(value, actionParameters), blackboard);

        }

    }
}