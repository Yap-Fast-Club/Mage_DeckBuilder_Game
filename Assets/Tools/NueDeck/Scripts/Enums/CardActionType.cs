namespace NueGames.NueDeck.Scripts.Enums
{
    public enum CardActionType
    {
        Attack,
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
        SpendMana
    }


    public enum ActionRepeatType
    {
        None,
        Repeat,
        RepeatPerReserveMana,
        RepeatPerSouls
    }
}