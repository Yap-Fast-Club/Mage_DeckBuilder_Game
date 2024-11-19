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

    [SerializeField]private int _damage = 1;
    [SerializeField]
    private bool _autoDestroy = true;



    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void SetDamage (int damage)
    {
        _damage = damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_target)) return;

        var thisCaracter = this.GetComponent<CharacterBase>();
        var otherCharacter = other.GetComponent<CharacterBase>();

        otherCharacter?.CharacterStats.Damage(_damage);

        if (_autoDestroy)
        {
            thisCaracter?.CharacterStats.SetCurrentSouls(0);
            thisCaracter?.CharacterStats.Damage(999999, true);
        }

    }

}
