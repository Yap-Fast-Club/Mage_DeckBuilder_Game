using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class AttackAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Attack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;
            
            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;
            
            var value = actionParameters.Value + selfCharacter.CharacterStats.StatusDict[StatusType.Power].StatusValue ;


            targetCharacter.CharacterStats.Damage(Mathf.RoundToInt(value));

            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.TargetCharacter.transform,FxType.Attack);
                FxManager.SpawnFloatingText(actionParameters.TargetCharacter.TextSpawnRoot,value.ToString());
            }
           
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }

    public class ExcessAttackAction : AttackAction
    {
        public override CardActionType ActionType => CardActionType.ExcessAttack;

        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;
            if (actionParameters.Value <= 0) return;

            var previousTargetHealth = actionParameters.TargetCharacter.CharacterStats.CurrentHealth;

            base.DoAction(actionParameters);

            actionParameters.Value -= previousTargetHealth;
            
        }
    }
}