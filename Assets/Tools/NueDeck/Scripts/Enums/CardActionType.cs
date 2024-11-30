﻿namespace NueGames.NueDeck.Scripts.Enums
{
    public enum CardActionType
    {
        Attack,
        Heal,
        Block,
        Power,
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
        Focus,
        DiscardHand,
        TransformAllCards,
        IncreaseCardDMG,
        IncreaseMaxMana,
        GainSouls,
        HealForEverySoul
    }


    public enum ActionRepeatType
    {
        None,
        Repeat,
        RepeatPerReserveMana,
        RepeatPerSouls
    }
}