using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using Unity.VisualScripting;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardBlackboard
    {
        public static CharacterBase CurrentTarget { get; set; }

        public static CardBlackboard LastPlayedInfo { get; private set; }


        public int SpentMana = 0;
        public int GainedMana = 0;
        public int SpentSouls = 0;
        public int GainedSouls = 0;
        public int DamageDealt = 0;

        public bool ResetPower = false;
        public bool ResetFocus = false;

        public CardBlackboard(CardData cardData)
        {
            //Initial Data Set
            CardBlackboard.LastPlayedInfo = this;

            ResetFocus = (cardData.Type == CardType.Spell);

            SpentMana = cardData.FocusedManaCost;
            SpentSouls = 0;
            GainedSouls = 0;
            GainedMana = 0;
            DamageDealt = 0;
            ResetPower = false;
        }
    }

    
}