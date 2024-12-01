using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class IncreaseMaxHealthAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.IncreaseMaxHealth;
        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;
            
            if (!newTarget) return;
            
            newTarget.CharacterStats.IncreaseMaxHealth(Mathf.RoundToInt(actionParameters.Value));

            if (FxManager != null) 
                FxManager.PlayFx(newTarget.transform, FxType.Buff);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }

    public class IncreaseMaxManaAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.IncreaseMaxMana;
        public override void DoAction(CardActionParameters actionParameters, CardActionBlackboard blackboard)
        {
            //var newTarget = actionParameters.TargetCharacter
            //    ? actionParameters.TargetCharacter
            //    : actionParameters.SelfCharacter;

            //if (!newTarget) return;

            GameManager.Instance.PersistentGameplayData.TemporalMaxMana += (int)actionParameters.Value;


            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}