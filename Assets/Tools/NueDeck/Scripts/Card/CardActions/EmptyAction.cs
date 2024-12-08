using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using System.Linq;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class EmptyAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.EmptyAction;
        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            if (!actionParameters.TargetCharacter) return;

            if (AudioManager != null)
                AudioManager.PlayOneShot(actionParameters.ActionAudioType);
        }
    }



    public class AddWeightToCardInRewards : CardActionBase
    {
        public override CardActionType ActionType => CardActionType.AddWeight;

        public override void DoAction(CardActionParameters actionParameters, CardBlackboard blackboard)
        {
            string cardID = actionParameters.AreaValue.ToString();
            CardData cardToAddWeight = GameManager.GameplayData.AllCardsList.CardList.Find(c => c.Id == cardID);

            UIManager.Instance.RewardCanvas.rewardContainerData.AddWeightTo(cardToAddWeight, (int)actionParameters.Value);
        }
    }
}