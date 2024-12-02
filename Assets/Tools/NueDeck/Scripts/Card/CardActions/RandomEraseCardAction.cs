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

        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var targetRarity = actionParameters.AreaValue switch
            {
                1 => RarityType.Common,
                2 => RarityType.Rare,
                3 => RarityType.Legendary,
                _ => RarityType.Common
            };



            if (CollectionManager.DrawPile.Count > 0)
            {
                var randomCardData = CollectionManager.DrawPile.RandomItem();
                CollectionManager.DrawCard(randomCardData);

                CardBase cardInHand=null;
                if (actionParameters.AreaValue == 0)
                    cardInHand = CollectionManager.HandController.hand.Find(c => c.CardData == randomCardData);
                else if (actionParameters.AreaValue <= 3)
                    cardInHand = CollectionManager.HandController.hand.Find(c => c.CardData == randomCardData && c.CardData.Rarity == targetRarity);

                if (cardInHand == null)
                    return;

                CollectionManager.HandController.StopAllCoroutines();
                CollectionManager.HandController.RemoveCardFromHand(cardInHand.CardData);

                cardInHand.Exhaust(false);
            }

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}