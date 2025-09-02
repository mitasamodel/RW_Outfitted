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
		[TweakValue("Outfitted",0,60)]
		static int _skipTicksOverlay = 6;
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
			GenMapUI.DrawThingLabel(GenMapUI.LabelDrawPosFor(apparel, 0.0f), beauty.ToString("F1"), BeautyDrawer.BeautyColor(beauty, 3f));
		}

		private static List<float> CachedScoresForPawn(Pawn pawn)
		{
			if (cachedId != pawn.thingIDNumber || cachedTick < GenTicks.TicksGame)
			{
				cachedScores = Outfitted.BuildWornScore(pawn);
				cachedId = pawn.thingIDNumber;
				cachedTick = GenTicks.TicksGame + _skipTicksOverlay;
			}
			return cachedScores;
		}
	}
}
