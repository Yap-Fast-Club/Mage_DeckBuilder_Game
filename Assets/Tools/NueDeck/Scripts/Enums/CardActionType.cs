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
        EarnMana,
        LifeSteal,
        Stun,
        Exhaust,
        Push,
        EmptyAction,
        DealDamageForEveryMana,
        LoseHealth,
        EarnManaForEverySoul,
        PickErase,   
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

    }


    public enum ActionRepeatType
    {
        None,
        Repeat,
        RepeatPerReserveMana,
        RepeatPerSouls,
        RepeatPerDamageDealt
    }
}