namespace NueGames.NueDeck.Scripts.Enums
{
    public enum ActionTargetType
    {
        Enemy,
        Ally,
        AllEnemies,
        AllAllies,
        RandomEnemy,
        RandomAlly,
        Terrain,
        RandomTerrain,
        AllTerrain,
        LowestHPEnemy,
        ClosestEnemies,
        ClosestAndConsecutives,
        EnemyAndBehind
    }


    public enum ActionAreaType
    {
        SingleTarget,
        Radius,
        Line,
        Proximity
    }
}