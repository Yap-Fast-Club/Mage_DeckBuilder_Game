using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class EraseCardAction : CardActionBase
    {
        PersistentGameplayData PersistendData => GameManager.Instance.PersistentGameplayData;

        public override CardActionType ActionType => CardActionType.Erase;

        public override void DoAction(CardActionParameters actionParameters)
        {
            UIManager.Instance.RewardCanvas.gameObject.SetActive(true);
            UIManager.Instance.RewardCanvas.PrepareCanvas();
            UIManager.Instance.RewardCanvas.InstantRemoveCard();

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}