using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NueGames.NueDeck.Scripts.Managers;
public class BlockCardHand : MonoBehaviour
{
    public void CanSelectCard(bool flag)
    {
        GameManager.Instance.PersistentGameplayData.CanSelectCards = flag;
        GameManager.Instance.PersistentGameplayData.STOP = !flag;
    }
}
