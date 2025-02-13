using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthBuffToAllEnemies : MonoBehaviour
{

    [SerializeField] int maxHealthToGive = 2;

    private List<EnemyBase> _currentAffectedEnemies = new List<EnemyBase>();

    void Start()
    {
        //apply buff to already present enemies
        CombatManager.Instance.CurrentEnemiesList.ForEach(e =>
        {
            if (e == this.GetComponent<EnemyBase>()) return;

            GiveBuff(e);
        });

        CombatManager.Instance.EnemySpawned += GiveBuff;
        CombatManager.Instance.EnemyDeath += RemoveBuff;
    }

    private void OnDisable()
    {
        CombatManager.Instance.EnemySpawned -= GiveBuff;
        CombatManager.Instance.EnemyDeath -= RemoveBuff;

        for (int i = _currentAffectedEnemies.Count - 1; i >= 0; i--)
        {
            RemoveBuff(_currentAffectedEnemies[i]);
        }
    }

    private void GiveBuff(EnemyBase enemy)
    {
        enemy.CharacterStats.MaxHealth += maxHealthToGive;
        enemy.CharacterStats.SetCurrentHealth(enemy.CharacterStats.CurrentHealth + maxHealthToGive);
        _currentAffectedEnemies.Add(enemy);
    }

    private void RemoveBuff(EnemyBase enemy)
    {
        enemy.CharacterStats.MaxHealth -= maxHealthToGive;
        enemy.CharacterStats.SetCurrentHealth(Mathf.Min(enemy.CharacterStats.CurrentHealth, enemy.CharacterStats.MaxHealth));
        _currentAffectedEnemies.Remove(enemy);
    }



}
