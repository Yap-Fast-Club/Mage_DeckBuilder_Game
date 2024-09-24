 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField] int _maxHealth;
    [SerializeField] int _curHealth;

    private void Awake()
    {
        _curHealth = _maxHealth;
    }

    public void SufferDamage(int damagePoints)
    {
        _curHealth -= damagePoints;

        _curHealth = Mathf.Max(0, _curHealth);

        if (_curHealth == 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
