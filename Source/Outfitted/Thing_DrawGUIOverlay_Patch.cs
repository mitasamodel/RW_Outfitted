using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Outfitted
{
	[HarmonyPatch(typeof(Thing), nameof(Thing.DrawGUIOverlay))]
	internal static class Thing_DrawGUIOverlay_Patch
	{
		private static int cachedId = -1;
		private static int cachedTick = -1;
		private static List<float> cachedScores = new List<float>();

		private static void Postfix(Thing __instance)
		{
			if (!Outfitted.showApparelScores ||
				!(Find.Selector.SingleSelectedThing is Pawn singleSelectedThing)
				|| !singleSelectedThing.IsColonistPlayerControlled ||
				!(__instance is Apparel apparel) ||
				!(singleSelectedThing.outfits.CurrentApparelPolicy is ExtendedOutfit currentApparelPolicy)
				|| !currentApparelPolicy.filter.Allows(apparel))
				return;

			List<float> wornScoresCache = CachedScoresForPawn(singleSelectedThing);
			float beauty = JobGiver_OptimizeApparel.ApparelScoreGain(singleSelectedThing, apparel, wornScoresCache);
			//if ((double)Math.Abs(beauty) <= 0.0099999997764825821)
			//	return;
			GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(apparel, 0.0f), beauty.ToString("F1"), BeautyDrawer.BeautyColor(beauty, 3f));
		}

		private static List<float> CachedScoresForPawn(Pawn pawn)
		{
			if (Thing_DrawGUIOverlay_Patch.cachedId != pawn.thingIDNumber || Thing_DrawGUIOverlay_Patch.cachedTick < GenTicks.TicksGame)
			{
				Thing_DrawGUIOverlay_Patch.cachedScores = Thing_DrawGUIOverlay_Patch.ScoresForPawn(pawn);
				Thing_DrawGUIOverlay_Patch.cachedId = pawn.thingIDNumber;
				Thing_DrawGUIOverlay_Patch.cachedTick = GenTicks.TicksGame;
			}
			return Thing_DrawGUIOverlay_Patch.cachedScores;
		}

		private static List<float> ScoresForPawn(Pawn pawn)
		{
			List<float> floatList = new List<float>();
			for (int index = 0; index < pawn.apparel.WornApparel.Count; ++index)
				floatList.Add(JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, pawn.apparel.WornApparel[index]));
			return floatList;
		}
	}
}
