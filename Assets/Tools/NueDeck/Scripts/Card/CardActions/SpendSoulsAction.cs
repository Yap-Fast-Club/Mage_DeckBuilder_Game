using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class SpendSoulsAction : CardActionBase
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.SpendSouls;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (CombatManager != null)
            {
                PersistendData.CurrentSouls -= (int)actionParameters.Value;
                UIManager.Instance.InformationCanvas.InstantUpdateSoulsGUI();
            }
            else
                Debug.LogError("There is no CombatManager");

            if (FxManager != null)
                FxManager.PlayFx(actionParameters.SelfCharacter.transform, FxType.Buff);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }

    public class GainSoulsAction : CardActionBase
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.GainSouls;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (CombatManager != null)
            {
                PersistendData.CurrentSouls += (int)actionParameters.Value;
                UIManager.Instance.InformationCanvas.InstantUpdateSoulsGUI();
            }
            else
                Debug.LogError("There is no CombatManager");


            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}