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

    [Header("References")]
    public CombatManager combatManager; // Reference to the CombatManager

    private int currentTurn = 0; // Tracks the current turn within the wave
    public int CurrentTurn => currentTurn;
    public int GetRemainingEnemies()
    {
        int totalEnemies = 0;
        foreach (var enemyInfo in waves[currentWaveIndex].EnemiesToSpawn)
        {
            if (enemyInfo.TurnNumber >= currentTurn)
            {
                totalEnemies++;
            }
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
            currentTurn = 0; // Reset the turn counter for the wave
            combatManager.OnAllyTurnStarted += OnTurnStarted; // Subscribe to the event for turn-based spawning
        }
    }

    private void OnTurnStarted()
    {
        currentTurn++;
        SpawnEnemiesForCurrentTurn();

        // Update the UI
        if (WaveUIManager.Instance != null)
        {
            WaveUIManager.Instance.UpdateWaveUI();
        }
    }
    private void SpawnEnemiesForCurrentTurn()
    {
        Wave currentWave = waves[currentWaveIndex];

        // Find all enemies scheduled to spawn on this turn
        foreach (var enemyInfo in currentWave.EnemiesToSpawn)
        {
            if (enemyInfo.TurnNumber == currentTurn)
            {
                SpawnEnemy(enemyInfo.EnemyPrefab);
            }
        }

        // Check if the wave is completed
        if (currentTurn >= GetMaxTurnForWave(currentWave))
        {
            currentWaveIndex++;
            combatManager.OnAllyTurnStarted -= OnTurnStarted; // Unsubscribe from the event when the wave ends

            if (currentWaveIndex < waves.Count)
            {
                StartWave(); // Start the next wave if available
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Pick a random spawn position from the five positions on the right side
        int randomIndex = Random.Range(0, spawnPositions.Count);
        Instantiate(enemyPrefab, spawnPositions[randomIndex].position, Quaternion.identity);
    }

    private int GetMaxTurnForWave(Wave wave)
    {
        int maxTurn = 0;
        foreach (var enemyInfo in wave.EnemiesToSpawn)
        {
            if (enemyInfo.TurnNumber > maxTurn)
            {
                maxTurn = enemyInfo.TurnNumber;
            }
        }
        return maxTurn;
    }
}
