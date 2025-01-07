using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Utils;
using System.Linq;
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


            var shuffledDrawPile = CollectionManager.DrawPile.Where(c => actionParameters.AreaValue > 0 ? c.Rarity == targetRarity : c).ToList();

            if (shuffledDrawPile.Count == 0)
            {
                FxManager.SpawnFloatingText(CombatManager.DefaultTextSpawnRoot, "No card(s) to erase", duration: 3);
                return;
            }

            shuffledDrawPile.Shuffle();

            for (int i = 0; i < Mathf.Min(shuffledDrawPile.Count, actionParameters.Value); i++)
            {
                var drawnCard = CollectionManager.DrawCard(shuffledDrawPile[i].Id, addToHand: false);
                CollectionManager.HandController.StopAllCoroutines();
                drawnCard?.Exhaust(false);
            }


            //CollectionManager.HandController.RemoveCardFromHand(cardInHand.CardData);

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}