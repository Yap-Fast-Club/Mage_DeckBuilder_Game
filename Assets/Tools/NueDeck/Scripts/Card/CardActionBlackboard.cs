using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardActionBlackboard
    {
        public int SpentMana = 0;
        public int SpentSouls = 0;
        public int GainedSouls = 0;

        public bool ResetPower = false;
        public bool ResetFocus = false;

        public CardActionBlackboard(CardData cardData)
        {
            //Initial Data Set
            ResetFocus = (cardData.Type == CardType.Spell);

            SpentMana = cardData.FocusedManaCost;
            SpentSouls = 0;
            GainedSouls = 0;
            ResetPower = false;
        }
    }
}