using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class DealXDamage4EveryManaAction: AttackAction
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.DealDamageForEveryMana;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;

            var value = actionParameters.Value * PersistendData.CurrentMana;


            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;
            
            value +=  selfCharacter.CharacterStats.StatusDict[StatusType.Power].StatusValue;


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
}