using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KaimiraGames;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Collection;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Data.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NueGames.NueDeck.Scripts.Managers
{
    public class CollectionManager : MonoBehaviour
    {
        public CollectionManager(){}
      
        public static CollectionManager Instance { get; private set; }

        [Header("Controllers")] 
        [SerializeField] private HandController handController;

        public Action<CardBase> CardPlayed;


        #region Cache

        public List<CardData> DrawPile => _weightedDrawPile.ToList();
        public List<CardData> HandPile { get; private set; } = new List<CardData>();
        public List<CardData> DiscardPile { get; private set; } = new List<CardData>();


        private WeightedList<CardData> _weightedDrawPile;

        public List<CardData> ExhaustPile { get; private set; } = new List<CardData>();
        public HandController HandController => handController;
        protected GameManager GameManager => GameManager.Instance;


        protected UIManager UIManager => UIManager.Instance;

        #endregion
       
        #region Setup
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        public void AddNewCardToHand(CardData card)
        {
            var cardCopy = Instantiate(card);  //clone
            GameManager.PersistentGameplayData.CurrentCardsList.Add(card);
            _weightedDrawPile.Add(cardCopy, 10);
            DrawCard(cardCopy);
        }
        public void AddNewCardToDrawPile(CardData card)
        {
            var cardCopy = Instantiate(card);  //clone
            GameManager.PersistentGameplayData.CurrentCardsList.Add(cardCopy);
            _weightedDrawPile.Add(cardCopy, 10);
        }

        public void ClearDrawPile()
        {
            _weightedDrawPile.Clear();
        }

        #region Public Methods
        public void DrawCards(int targetDrawCount)
        {
            var currentDrawCount = 0;

            for (var i = 0; i < targetDrawCount; i++)
            {
                if (GameManager.GameplayData.MaxCardOnHand <= HandPile.Count)
                    return;
                

                if (_weightedDrawPile.Count <= 0)
                {
                    var nDrawCount = targetDrawCount - currentDrawCount;
                    
                    if (nDrawCount >= DiscardPile.Count) 
                        nDrawCount = DiscardPile.Count;
                    
                    ReshuffleDiscardPile();
                    DrawCards(nDrawCount);
                    break;
                }

                var randomCard = _weightedDrawPile.Next();
                DrawCard(randomCard);
                currentDrawCount++;
            }
            
            foreach (var cardObject in HandController.hand)
                cardObject.UpdateCardText();
        }


        public CardBase DrawCard(string cardID, bool ignoreLimit = false, bool addToHand = true)
        {
            if (!_weightedDrawPile.Any(c => c.Id == cardID)) return null;

            return DrawCard(_weightedDrawPile.First(c => c.Id == cardID), ignoreLimit, addToHand);
        }

        private CardBase DrawCard(CardData card, bool ignoreLimit = false, bool addToHand = true)
        {
            if (GameManager.GameplayData.MaxCardOnHand <= HandPile.Count && !ignoreLimit)
                return null;

            var clone = GameManager.BuildAndGetCard(card, HandController.drawTransform);
            if ( addToHand)
            {
                HandController.AddCardToHand(clone, 0);
                HandPile.Add(card);
                UIManager.CombatCanvas.SetPileTexts();
                AudioManager.Instance.PlayOneShot(Enums.AudioActionType.Draw);
                _weightedDrawPile.AddWeightToAll(1);
            }


            clone.UpdateCardText();
            _weightedDrawPile.Remove(card);

            return clone;
        }
        public void DiscardHand()
        {
            _weightedDrawPile.AddWeightToAll(2);

            foreach (var cardBase in HandController.hand) 
                cardBase.Discard();

            HandController.hand.Clear();
        }
        
        public void OnCardDiscarded(CardBase targetCard)
        {
            HandPile.Remove(targetCard.CardData);
            _weightedDrawPile.Add(targetCard.CardData, 10);
            UIManager.CombatCanvas.SetPileTexts();
        }
        
        public void OnCardExhausted(CardBase targetCard)
        {
            HandPile.Remove(targetCard.CardData);
            ExhaustPile.Add(targetCard.CardData);
            UIManager.CombatCanvas.SetPileTexts();

            var cardToRemove = GameManager.PersistentGameplayData.CurrentCardsList.Find(c => targetCard.CardData.Id == c.Id);

            GameManager.PersistentGameplayData.CurrentCardsList.Remove(cardToRemove);
        }


        public void OnCardPlayed(CardBase targetCard)
        {
            if (targetCard.CardData.ExhaustAfterPlay)
                targetCard.Exhaust();
            else
                targetCard.Discard();

            foreach (var cardObject in HandController.hand)
                cardObject.UpdateCardText();

            CardPlayed?.Invoke(targetCard);

        }
        public void SetGameDeck()
        {
            var initialDeckCopies = GameManager.PersistentGameplayData.CurrentCardsList.Select(c => Instantiate(c));

            _weightedDrawPile = new(initialDeckCopies.Select(c => new WeightedListItem<CardData>(c, 10)).ToList(), GameManager.DrawRandom);
        }


        public void ClearPiles()
        {
            DiscardPile.Clear();
            _weightedDrawPile.Clear();
            HandPile.Clear();
            ExhaustPile.Clear();
            HandController.hand.Clear();
        }

        public int WeightOf(CardData card)
        {
            if (!_weightedDrawPile.Contains(card)) return 0;

            return _weightedDrawPile.GetWeightOf(card);
        }
        #endregion

        #region Private Methods
        private void ReshuffleDiscardPile()
        {
            foreach (var i in DiscardPile) 
                _weightedDrawPile.Add(i, 10);
            
            DiscardPile.Clear();
        }
        private void ReshuffleDrawPile()
        {
            foreach (var i in _weightedDrawPile) 
                DiscardPile.Add(i);
            
            _weightedDrawPile.Clear();
        }
        #endregion

    }
}