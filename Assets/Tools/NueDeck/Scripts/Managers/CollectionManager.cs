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
using static UnityEditor.Progress;
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

        public List<CardData> DrawPile { get; private set; } = new List<CardData>();
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

            AddNewCardToDrawPile(card);
            DrawCard(card);
        }
        public void AddNewCardToDrawPile(CardData card)
        {
            card = Instantiate(card);  //clone
            GameManager.PersistentGameplayData.CurrentCardsList.Add(card);
            DrawPile.Add(card);
            _weightedDrawPile.Add(card, 10);

        }

        #region Public Methods
        public void DrawCards(int targetDrawCount)
        {
            var currentDrawCount = 0;

            for (var i = 0; i < targetDrawCount; i++)
            {
                if (GameManager.GameplayData.MaxCardOnHand<=HandPile.Count)
                    return;
                

                if (DrawPile.Count <= 0)
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

        public void DrawCard(CardData card, bool ignoreLimit = false)
        {
            if (GameManager.GameplayData.MaxCardOnHand <= HandPile.Count && !ignoreLimit)
                return;

            var clone = GameManager.BuildAndGetCard(card, HandController.drawTransform);
            HandController.AddCardToHand(clone, 0);
            HandPile.Add(card);
            UIManager.CombatCanvas.SetPileTexts();
            clone.UpdateCardText();
            AudioManager.Instance.PlayOneShot(Enums.AudioActionType.Draw);

            DrawPile.Remove(card);
            _weightedDrawPile.SetWeight(card, 10);
            _weightedDrawPile.Remove(card);
            _weightedDrawPile.AddWeightToAll(2);
        }
        public void DiscardHand()
        {
            _weightedDrawPile.AddWeightToAll(4);

            foreach (var cardBase in HandController.hand) 
                cardBase.Discard();

            HandController.hand.Clear();
        }
        
        public void OnCardDiscarded(CardBase targetCard)
        {
            HandPile.Remove(targetCard.CardData);
            DrawPile.Add(targetCard.CardData);
            _weightedDrawPile.Add(targetCard.CardData, 10);
            UIManager.CombatCanvas.SetPileTexts();
        }
        
        public void OnCardExhausted(CardBase targetCard)
        {
            HandPile.Remove(targetCard.CardData);
            ExhaustPile.Add(targetCard.CardData);
            UIManager.CombatCanvas.SetPileTexts();

            GameManager.PersistentGameplayData.CurrentCardsList.Remove(targetCard.CardData);
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
            foreach (var i in GameManager.PersistentGameplayData.CurrentCardsList)
            {
                DrawPile.Add(i);
            }

            _weightedDrawPile = new(DrawPile.Select(c  => new WeightedListItem<CardData>(c, 10)).ToList(), GameManager.DrawRandom);
        }

        public void UpdateDrawPile()
        {
            foreach (var c in GameManager.PersistentGameplayData.CurrentCardsList)
                if (!DrawPile.Contains(c) && !HandPile.Contains(c))
                    DrawPile.Add(c);
        }

        public void ClearPiles()
        {
            DiscardPile.Clear();
            DrawPile.Clear();
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
                DrawPile.Add(i);
            
            DiscardPile.Clear();
        }
        private void ReshuffleDrawPile()
        {
            foreach (var i in DrawPile) 
                DiscardPile.Add(i);
            
            DrawPile.Clear();
        }
        #endregion

    }
}