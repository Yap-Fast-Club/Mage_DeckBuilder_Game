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


    private int currentTurn = 0; // Tracks the current turn within the wave
    public int CurrentTurn => currentTurn;
    private CombatManager _combatManager => CombatManager.Instance;


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
            _combatManager.OnAllyTurnStarted -= OnPlayerTurnStarted; // Unsubscribe from the event when the wave ends

            if (currentWaveIndex < waves.Count)
            {
                StartWave();
            }
        }
    }

    private void SpawnEnemy(EnemyBase enemyPrefab)
    {
        // Pick a random spawn position from the five positions on the right side
        int randomIndex = Random.Range(0, spawnPositions.Count);
        var clone = Instantiate(enemyPrefab, spawnPositions[randomIndex].position, Quaternion.identity);

        clone.BuildCharacter();
        _combatManager.CurrentEnemiesList.Add(clone);
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
