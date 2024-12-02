using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class EarnManaAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EarnMana;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            
            if (CombatManager != null)
            {
                CombatManager.IncreaseMana(Mathf.RoundToInt(actionParameters.Value));
                CardBlackboard.LastPlayedInfo.GainedMana += Mathf.RoundToInt(actionParameters.Value);
            }
            else
                Debug.LogError("There is no CombatManager");

            if (FxManager != null)
                FxManager.PlayFx(actionParameters.SelfCharacter.transform, FxType.Buff);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}