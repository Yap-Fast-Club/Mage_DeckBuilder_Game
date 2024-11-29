using NueGames.NueDeck.ThirdParty.NueTooltip.Core;
using NueGames.NueDeck.ThirdParty.NueTooltip.CursorSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardUI : CardBase
    {
       

        public override void OnPointerDown(PointerEventData eventData)
        {
            HideTooltipInfo(TooltipManager.Instance);
        }

        public override void ShowTooltipInfo(TooltipManager tooltipManager, string content, string header = "", Transform tooltipStaticTransform = null, CursorType targetCursor = CursorType.Default, Camera cam = null, float delayShow = 0)
        {
            tooltipManager.ShowTooltip(content, header, null, targetCursor, null, delayShow);
        }

        public override void HideTooltipInfo(TooltipManager tooltipManager)
        {
            if (tooltipCR != null)
                StopCoroutine(tooltipCR);
            tooltipCR = null;
            tooltipManager.HideTooltip();
        }
    }
}