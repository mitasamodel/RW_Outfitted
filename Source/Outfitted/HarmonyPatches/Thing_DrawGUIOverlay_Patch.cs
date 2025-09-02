using HarmonyLib;
using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using LudeonTK;

namespace Outfitted
{
	[HarmonyPatch(typeof(Thing), nameof(Thing.DrawGUIOverlay))]
	internal static class Thing_DrawGUIOverlay_Patch
	{
		private static void Postfix(Thing __instance)
		{
			if (!Outfitted.showApparelScores ||
				!(Find.Selector.SingleSelectedThing is Pawn singleSelectedThing)
				|| !singleSelectedThing.IsColonistPlayerControlled ||
				!(__instance is Apparel apparel) ||
				!(singleSelectedThing.outfits.CurrentApparelPolicy is ExtendedOutfit currentApparelPolicy)
				|| !currentApparelPolicy.filter.Allows(apparel))
				return;

			List<float> wornScoresCache = CacheWornApparel.GetScore(singleSelectedThing);
			float beauty = JobGiver_OptimizeApparel.ApparelScoreGain(singleSelectedThing, apparel, wornScoresCache);
			GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(apparel, 0.0f), beauty.ToString("F1"), BeautyDrawer.BeautyColor(beauty, 3f));
		}
	}
}
