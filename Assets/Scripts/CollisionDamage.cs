using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionDamage : MonoBehaviour
{
    private Collider _collider;

    [SerializeField, Tag] 
    private string _target;

    public int Damage = 1;
    [SerializeField]
    private bool _autoDestroy = true;



    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_target)) return;

        var otherCharacter = other.GetComponent<CharacterBase>();

        otherCharacter?.CharacterStats.Damage(Damage);

        if (_autoDestroy)
        {
            gameObject.GetComponent<CharacterBase>()?.CharacterStats.Damage(9999, true);
            //TODO: soul stuff for enemies
        }

    }

}
