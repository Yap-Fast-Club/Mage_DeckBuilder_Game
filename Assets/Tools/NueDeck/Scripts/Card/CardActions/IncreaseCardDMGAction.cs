using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class IncreaseCardDMGAction : CardActionBase
    {
        Dictionary<(string, CardActionType), float> mods => GameManager.Instance.PersistentGameplayData.ActionMods;
        public override CardActionType ActionType => CardActionType.IncreaseCardDMG;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var duple = (actionParameters.AreaValue.ToString(), CardActionType.DealDamage);
            if (mods.ContainsKey(duple))
                mods[duple] += actionParameters.Value;
            else
                mods.Add(duple, actionParameters.Value);

            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }

    public class DecreaseCardCost : CardActionBase
    {
        Dictionary<(string, CardActionType), float> mods => GameManager.Instance.PersistentGameplayData.ActionMods;
        public override CardActionType ActionType => CardActionType.DecreaseCardCost;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            var duple = (actionParameters.AreaValue.ToString(), CardActionType.SpendMana);
            if (mods.ContainsKey(duple))
                mods[duple] += actionParameters.Value;
            else
                mods.Add(duple, actionParameters.Value);

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }
}