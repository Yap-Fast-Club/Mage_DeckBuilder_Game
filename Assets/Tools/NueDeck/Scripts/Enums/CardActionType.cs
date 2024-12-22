namespace NueGames.NueDeck.Scripts.Enums
{
    public enum CardActionType
    {
        DealDamage,
        Heal,
        Block,
        GainPower,
        IncreaseMaxHealth,
        Draw,
        GainMana,
        LifeSteal,
        Stun,
        Exhaust,
        Push,
        EmptyAction,
        DealDamageForEveryMana,
        LoseHealth,
        EarnManaForEverySoul,
        ChooseErase,   
        DealDamageForEverySoul,
        SpendSouls,
        RandomErase,
        GainFocus,
        DiscardHand,
        TransformAllCards,
        IncreaseCardDMG,
        IncreaseMaxMana,
        GainSouls,
        HealForEverySoul,
        GainKeyword,
        DecreaseCardCost,
        SpendMana,
        ExcessAttack,
        DealDamageForEverySpentSoul,
        ReduceMovementBy,
        AddWeight

    }


    public enum ActionRepeatType
    {
        None,
        Repeat,
        RepeatPerReserveMana,
        RepeatPerSouls,
        RepeatPerDamageDealt,
        RepeatPerDeadEnemy,
        AfterCardPlayed,
        AfterSpellPlayed,
        AfterIncantPlayed,
        AfterCardDrawn,
        AfterCardDiscarded,
        OnDrawn,
        OnDiscarded

    }
}