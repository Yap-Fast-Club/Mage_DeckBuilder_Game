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
        EnemyAndLineBehind,
        EnemyAndAllBehind,
        LineBehindEnemy,
        AllBehindEnemy
    }


    public enum ActionAreaType
    {
        SingleTarget,
        Radius,
        Line,
        Proximity
    }
}