using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class LoseHealthAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.LoseHealth;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var value = Mathf.RoundToInt(actionParameters.Value);
            actionParameters.SelfCharacter.CharacterStats.Damage(value);
            
            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.SelfCharacter.transform,FxType.Attack);
                FxManager.SpawnFloatingText(actionParameters.SelfCharacter.TextSpawnRoot,value.ToString());
            }
           
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}