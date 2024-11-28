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
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (CombatManager != null)
            {
                var value = actionParameters.Value * PersistendData.CurrentSouls;
                CombatManager.IncreaseMana(Mathf.RoundToInt(value));
            }
            else
                Debug.LogError("There is no CombatManager");

            if (FxManager != null)
                FxManager.PlayFx(actionParameters.SelfCharacter.transform, FxType.Buff);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}