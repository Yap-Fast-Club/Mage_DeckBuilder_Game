﻿using NueGames.NueDeck.Scripts.Data.Settings;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Utils;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class ErasePickCardAction : CardActionBase
    {

        public override CardActionType ActionType => CardActionType.ChooseErase;

        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            UIManager.Instance.RewardCanvas.gameObject.SetActive(true);
            UIManager.Instance.RewardCanvas.PrepareCanvas();
            UIManager.Instance.RewardCanvas.InstantRemoveCard();

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}