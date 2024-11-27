using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;
using NueGames.NueDeck.Scripts.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class TransformAllCardsCardAction : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.TransformAllCards;

        public override void DoAction(CardActionParameters actionParameters)
        {
            string toEraseID = actionParameters.AreaValue.ToString();
            string toCreateID = actionParameters.Value.ToString();

            var allCopiesInDeck = CollectionManager.DrawPile.FindAll(c => c.Id == toEraseID).Concat(CollectionManager.HandPile.FindAll(c => c.Id == toEraseID)).ToList();

            if (allCopiesInDeck == null || allCopiesInDeck.Count == 0 || CollectionManager.DrawPile.Count == 0) return;

            CardData cardToCreateData = GameManager.GameplayData.AllCardsList.CardList.Find(c => c.Id == toCreateID);
            for ( int i = 0; i < allCopiesInDeck.Count; i++ )
            {
                GameManager.PersistentGameplayData.CurrentCardsList.Add(cardToCreateData);
                CollectionManager.DrawPile.Add(cardToCreateData);
            }
            CollectionManager.UpdateDrawPile();

            int prevHandSize = CollectionManager.HandPile.Count;

            foreach (var card in allCopiesInDeck) 
            {
                if (!CollectionManager.HandPile.Contains(card))
                    CollectionManager.DrawCard(card, ignoreLimit: true);

                var cardInHand = CollectionManager.HandController.hand.Find(c => c.CardData == card);
                CollectionManager.HandController.RemoveCardFromHand(card);

                cardInHand.Exhaust(false);
            }

            int postHandSize = CollectionManager.HandPile.Count;

            for ( int i = 0; i < prevHandSize - postHandSize;i++)
            {
                CollectionManager.DrawCard(cardToCreateData);
            }


            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}