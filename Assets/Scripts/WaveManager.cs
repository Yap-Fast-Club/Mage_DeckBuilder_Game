using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    public List<Wave> waves; // List to store all waves
    public int currentWaveIndex = 0; // Index of the current wave

    [Header("Spawn Settings")]
    public List<Transform> spawnPositions; // The five positions on the right side of the grid


    private int currentTurn = 0;

    public int CurrentTurn => currentTurn;
    private CombatManager _combatManager => CombatManager.Instance;


    public int GetRemainingEnemies()
    {
        int totalEnemies = 0;

        var currentWave = waves[currentWaveIndex];

        for (int i = currentTurn; i <= GetMaxTurnForWave(currentWave); i++)
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

    private void Start()
    {
        StartWave(); // Start the first wave
    }

    public void StartWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            currentTurn = 0; 
            _combatManager.OnAllyTurnStarted += OnPlayerTurnStarted; 
            _combatManager.OnEnemyTurnStarted += OnEnemyTurnStarted; 
        }
    }

    private void OnPlayerTurnStarted()
    {
        SpawnEnemiesForCurrentTurn();

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



    private void SpawnEnemiesForCurrentTurn()
    {
        Wave currentWave = waves[currentWaveIndex];

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


        // Check if the wave is completed
        if (currentTurn > GetMaxTurnForWave(currentWave))
        {

            currentWaveIndex++;
            _combatManager.OnAllyTurnStarted -= OnPlayerTurnStarted; // Unsubscribe from the event when the wave ends

            if (currentWaveIndex < waves.Count)
            {
                StartWave();
            }
        }
    }

    private void SpawnEnemy(EnemyBase enemyPrefab, int spawnPosition)
    {
        // Pick a random spawn position from the five positions on the right side
        var clone = Instantiate(enemyPrefab, spawnPositions[spawnPosition].position, Quaternion.identity);

        clone.BuildCharacter();
        _combatManager.CurrentEnemiesList.Add(clone);
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
}
