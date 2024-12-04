using NueGames.NueDeck.ThirdParty.NueTooltip.Core;
using NueGames.NueDeck.ThirdParty.NueTooltip.Interfaces;
using UnityEngine.EventSystems;

namespace NueGames.NueDeck.ThirdParty.NueTooltip.Triggers
{
    public class TooltipTrigger2D : TooltipTriggerBase,I2DTooltipTarget
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.enabled)
                ShowTooltipInfo();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.enabled)
                HideTooltipInfo(TooltipManager.Instance);
        }
    }
}