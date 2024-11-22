using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class RandomEraseCardAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.RandomErase;

        public override void DoAction(CardActionParameters actionParameters)
        {
            if (CollectionManager.DrawPile.Count > 0)
            {
                var randomCardData = CollectionManager.DrawPile.RandomItem();
                Debug.Log("yeah");
                CollectionManager.DrawCard(randomCardData);

                var cardInHand = CollectionManager.HandController.hand.Find(c => c.CardData == randomCardData);
                CollectionManager.HandController.StopAllCoroutines();
                CollectionManager.HandController.RemoveCardFromHand(0);

                cardInHand.Exhaust(false);
            }

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}