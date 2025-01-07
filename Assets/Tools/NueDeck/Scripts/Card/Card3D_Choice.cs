using System.Collections;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Managers;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card
{
    public class Card3D_Choice : Card3D
    {
        public override void UpdateCardText()
        {
            CardData.UpdateDescription(showMods: false);


            nameTextField.text = CardData.CardName;
            descTextField.text = CardData.MyDescription;

            if (CardData.Type == CardType.Incantation)
            {
                manaTextField.transform.parent.gameObject.SetActive(false);
                return;
            }

        }
    }
}