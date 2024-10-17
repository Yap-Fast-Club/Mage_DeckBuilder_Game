using NueGames.NueDeck.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    //[HideInInspector, SerializeField] string _name;
    //public int WaveNumber;

    public List<EnemySpawnInfo> waveEnemies;
    public List<TurnSpawnInfo> waveTurns;

    public TurnSpawnInfo GetTurnInfoFor(int turnNumber)
    {
        var turnInfo = waveTurns.Find(t => t.TurnNumber == turnNumber);

        if (turnInfo == null)
        {
            //Empty Turn
            turnInfo = new TurnSpawnInfo()
            {
                TurnNumber = turnNumber,
                EnemyLayout = new int[5] { 0, 0, 0, 0, 0 }
            };
        }

        return turnInfo;
    }
}


[System.Serializable]
public class EnemySpawnInfo
{
    public int weight;
    public EnemyBase Prefab;
}

[System.Serializable]
public class TurnSpawnInfo : ISerializationCallbackReceiver
{
    [HideInInspector, SerializeField] private string _name;

    public int TurnNumber;

    public int[] EnemyLayout = new int[5];

    public int GetEnemyCount()
    {
        int enemySum = 0;

        for (int i = 0; i < EnemyLayout.Length; i++)
        {
            if (EnemyLayout[i] != 0)
                enemySum++;
        }

        return enemySum;
    }

    void OnValidate()
    {
        _name = "Turn " + TurnNumber;
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() { }


}


