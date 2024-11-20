using System;
using NueGames.NueDeck.Scripts.Collection;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace NueGames.NueDeck.Scripts.Card
{
    public class ChoiceCard : MonoBehaviour,IPointerEnterHandler,IPointerDownHandler,IPointerExitHandler,IPointerUpHandler
    {
        [SerializeField] private float showScaleRate = 1.15f;
        private CardBase _cardBase;
        private Vector3 _initalScale;
        public Action OnCardChose;
        public GameManager GameManager => GameManager.Instance;
        public UIManager UIManager => UIManager.Instance;
        public CollectionManager CollectionManager => CollectionManager.Instance;
        
        public void BuildReward(CardData cardData)
        {
            _cardBase = GetComponent<CardBase>();
            _initalScale = transform.localScale;
            _cardBase.SetCard(cardData);
            _cardBase.UpdateCardText();
        }


        private void OnChoice()
        {
            if (UIManager != null)
                UIManager.RewardCanvas.ChoicePanel.DisablePanel();
            OnCardChose?.Invoke();
        }

        public void AddCardToHand()
        {
            if (GameManager != null)
            {
                GameManager.PersistentGameplayData.CurrentCardsList.Add(_cardBase.CardData);
            }

            if (CollectionManager)
            {
                var clone = GameManager.BuildAndGetCard(_cardBase.CardData, CollectionManager.HandController.drawTransform);
                CollectionManager.HandController.AddCardToHand(clone, 0);
                CollectionManager.HandPile.Add(_cardBase.CardData);
                CollectionManager.DrawPile.Remove(_cardBase.CardData);
            }

        }

        public void RemoveCardFromDeck()
        {
            if (GameManager != null)
            {
                GameManager.PersistentGameplayData.CurrentCardsList.Remove(_cardBase.CardData);
            }

            if (CollectionManager)
            {
                //CollectionManager.HandController.RemoveCardFromHand();
                CollectionManager.HandPile.Remove(_cardBase.CardData);
                CollectionManager.DrawPile.Remove(_cardBase.CardData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = _initalScale * showScaleRate;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
           
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = _initalScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnChoice();
            
        }
    }
}
