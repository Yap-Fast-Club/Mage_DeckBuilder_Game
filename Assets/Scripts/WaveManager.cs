using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using NaughtyAttributes;
using ReadOnly = NaughtyAttributes.ReadOnlyAttribute;
using Unity.VisualScripting;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private List<Wave> _waves; // List to store all waves
    [ReadOnly]
    public int currentWaveIndex = 0; // Index of the current wave

    public int MaxWaveIndex => _waves.Count-1;

    [Header("Spawn Settings")]
    public List<Transform> spawnPositions; // The five positions on the right side of the grid
    public List<Renderer> portals; // The five positions on the right side of the grid
    [SerializeField] Color _portalEnemy;
    [SerializeField] Color _portalNoEnemy;
    [SerializeField] Color _portalWait;
    [SerializeField] Color _portalOff;

    private int currentTurn = 0;

    public int CurrentTurn => currentTurn;
    public int CurrentMaxTurn => GetMaxTurnForWave(_currentWave);

    private CombatManager _combatManager => CombatManager.Instance;

    private Wave _currentWave => _waves[currentWaveIndex];

    public void SetLevelWaves(List<Wave> waves)
    {
        _waves = waves;
    }

    public int GetRemainingEnemies()
    {
        int totalEnemies = 0;

        var currentWave = _waves[currentWaveIndex];

        for (int i = currentTurn+1; i <= GetMaxTurnForWave(currentWave); i++)
        {
            totalEnemies += currentWave.GetTurnInfoFor(i).GetEnemyCount();
        }

        return totalEnemies;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public void StartWave()
    {
        if (currentWaveIndex < _waves.Count)
        {
            currentTurn = 0; 
            _combatManager.OnAllyTurnStarted += OnPlayerTurnStarted; 
            _combatManager.OnEnemyTurnStarted += OnEnemyTurnStarted; 
        }
        if (WaveUIManager.Instance != null)
        {
            WaveUIManager.Instance.UpdateWaveUI();
            WaveUIManager.Instance.SoftWinPanel.SetActive(false);
        }
    }

    private void OnPlayerTurnStarted()
    {
        SpawnEnemiesForCurrentTurn();
        PreviewNextTurn();

        // Update the UI
        if (WaveUIManager.Instance != null)
        {
            WaveUIManager.Instance.UpdateWaveUI();
        }
    }

    private void OnEnemyTurnStarted()
    {
        currentTurn++;
    }

    private void PreviewNextTurn()
    {
        Wave currentWave = _waves[currentWaveIndex];

        int[] enemyLayout = new int[5];

        //Find all enemies scheduled to spawn next turn
        if (GetMaxTurnForWave(currentWave) == currentTurn)
        {
            if(!CurrentWaveIsFinal())
                enemyLayout = _waves[currentWaveIndex+1].GetTurnInfoFor(0).EnemyLayout;
        }
        else
            enemyLayout = currentWave.GetTurnInfoFor(currentTurn+1).EnemyLayout;

        for (int row = 0; row < enemyLayout.Length; row++)
        {
            var enemyNumber = enemyLayout[row];

            if (enemyNumber == 0)
                portals[row].material.color = _portalNoEnemy;
            else
                portals[row].material.color = _portalEnemy;

            if (GetRemainingEnemies() == 0)
                if(CurrentWaveIsFinal())
                    portals[row].material.color = _portalOff;
                else
                    portals[row].material.color = _portalWait;

        }

     
    }

    private void SpawnEnemiesForCurrentTurn()
    {
        Wave currentWave = _waves[currentWaveIndex];

        //Find all enemies scheduled to spawn on this turn
        var enemyLayout = currentWave.GetTurnInfoFor(currentTurn).EnemyLayout;


        for (int col = 0; col < enemyLayout.Length; col++)
        {
            var enemyNumber = enemyLayout[col];

            if (enemyNumber == 0) continue;

            //if enemy -1 random btw weights
            //if enemy -2 50% empty 50%  random btw weights
            //if enemy -3 25% empty 75% random btw weights
            //if enemy -4 75% empty 25% random btw weights

            var enemyIndex = Mathf.Min(enemyNumber, currentWave.waveEnemies.Count) - 1;

            SpawnEnemy(currentWave.waveEnemies[enemyIndex].Prefab, col);
        }


        if (GetRemainingEnemies() == 0)
            if (CurrentWaveIsFinal() && CombatManager.Instance.CurrentEnemiesList.Count == 0)
                WaveUIManager.Instance.SoftWinPanel.SetActive(true);

        // Check if the wave is completed
        if (CurrentWaveIsCompleted())
        {
            _combatManager.OnAllyTurnStarted -= OnPlayerTurnStarted; 
            _combatManager.OnEnemyTurnStarted -= OnEnemyTurnStarted;

            if (!CurrentWaveIsFinal())
            {
                currentWaveIndex++;
                StartWave();
            }
            else
            {
                WaveUIManager.Instance.SoftWinPanel.SetActive(false);
            }

        }
    }

    private void SpawnEnemy(EnemyBase enemyPrefab, int spawnPosition)
    {
        // Pick a random spawn position from the five positions on the right side
        var clone = Instantiate(enemyPrefab, spawnPositions[spawnPosition].position, Quaternion.identity);

        clone.BuildCharacter();
        _combatManager.CurrentEnemiesList.Add(clone);
        AudioManager.Instance.PlayOneShot(NueGames.NueDeck.Scripts.Enums.AudioActionType.PortalSpawn);
        _combatManager.EnemySpawned?.Invoke(clone);
    }

    private int GetMaxTurnForWave(Wave wave)
    {
        int maxTurn = 0;

        foreach (var turnInfo in wave.waveTurns)
        {
            if (turnInfo.TurnNumber > maxTurn)
            {
                maxTurn = turnInfo.TurnNumber;
            }
        }
        return maxTurn;
    }

    public bool CurrentWaveIsCompleted()
    {
        return currentTurn > GetMaxTurnForWave(_waves[currentWaveIndex]);
    }

    public bool CurrentWaveIsFinal()
    {
        return currentWaveIndex + 1 >= _waves.Count;
    }

    public void FinishCurrentWaveNow()
    {
        currentTurn = GetMaxTurnForWave(_waves[currentWaveIndex]) + 1;
        CombatManager.Instance.EndTurn();
    }

}
