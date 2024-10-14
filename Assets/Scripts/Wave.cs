using NueGames.NueDeck.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string WaveName;
    public List<EnemySpawnInfo> EnemiesToSpawn; // List of enemies and their turn info
}

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyBase EnemyPrefab; // The enemy prefab to spawn
    public int TurnNumber;         // The turn this enemy should spawn
}
