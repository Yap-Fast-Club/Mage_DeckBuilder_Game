using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SoulContainer : MonoBehaviour
{
    [SerializeField] private int _soulAmount = 1;

    public int SoulAmount => _soulAmount;

    public void SetSoulAmount(int amount) => _soulAmount = amount;


    public void Empty()
    {
        _soulAmount = 0; 
    }
}
