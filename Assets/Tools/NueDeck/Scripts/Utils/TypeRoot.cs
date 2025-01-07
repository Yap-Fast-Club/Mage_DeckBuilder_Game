using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Utils
{
    public class TypeRoot : MonoBehaviour
    {
        [SerializeField] private CardType type;

        public CardType Type => type;
    }
}