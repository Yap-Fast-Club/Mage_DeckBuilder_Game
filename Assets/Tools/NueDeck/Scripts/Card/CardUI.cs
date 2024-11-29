using NueGames.NueDeck.ThirdParty.NueTooltip.Core;
using NueGames.NueDeck.ThirdParty.NueTooltip.CursorSystem;
using System.Collections;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card
{
    public class CardUI : CardBase
    {
        protected override IEnumerator ShowTooltipInfo()
        {
            if (!descriptionRoot) yield break;
            if (CardData.KeywordsList.Count <= 0) yield break;

            yield return new WaitForSeconds(0.25f);

            var tooltipManager = TooltipManager.Instance;
            foreach (var cardDataSpecialKeyword in CardData.KeywordsList)
            {
                var specialKeyword = tooltipManager.SpecialKeywordData.SpecialKeywordBaseList.Find(x => x.SpecialKeyword == cardDataSpecialKeyword);
                if (specialKeyword != null)
                    ShowTooltipInfo(tooltipManager, specialKeyword.GetContent(), specialKeyword.GetHeader(), descriptionRoot, CursorType.Default, CollectionManager ? CollectionManager.HandController.cam : Camera.main);
            }
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