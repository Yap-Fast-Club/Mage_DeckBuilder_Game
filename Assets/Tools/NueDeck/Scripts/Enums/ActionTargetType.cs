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
        EnemyRowAndColumn
    }


    public enum ActionAreaType
    {
        SingleTarget,
        Radius,
        Line,
        Proximity
    }
}