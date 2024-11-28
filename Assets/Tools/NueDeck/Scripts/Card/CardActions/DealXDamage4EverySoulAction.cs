using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class DealXDamage4EverySoulAction : AttackAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.DealDamageForEverySoul;
        public override void DoAction(CardActionParameters actionParameters)
        {
            var value = actionParameters.Value * PersistendData.CurrentSouls;

            if (!actionParameters.TargetCharacter) return;

            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;

            value += selfCharacter.CharacterStats.StatusDict[StatusType.Power].StatusValue;

            targetCharacter.CharacterStats.Damage(Mathf.RoundToInt(value));

            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.TargetCharacter.transform, FxType.Attack);
                FxManager.SpawnFloatingText(actionParameters.TargetCharacter.TextSpawnRoot, value.ToString());
            }

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);

        }
    }

    public class HealX4EverySoulAction : HealAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.HealForEverySoul;
        public override void DoAction(CardActionParameters actionParameters)
        {
            actionParameters.Value = actionParameters.Value * PersistendData.CurrentSouls;

            base.DoAction(actionParameters);

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);

        }
    }
}