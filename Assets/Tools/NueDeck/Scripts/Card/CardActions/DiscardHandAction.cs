using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class DiscardHandAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.DiscardHand;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (CollectionManager != null)
                CollectionManager.DiscardHand();
            else
                Debug.LogError("There is no CollectionManager");
            

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}