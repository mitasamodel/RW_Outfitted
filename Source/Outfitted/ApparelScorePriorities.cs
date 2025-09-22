using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Logger = Outfitted.RW_JustUtils.Logger;

namespace Outfitted
{
	internal static class ApparelScorePriorities
	{
		// Can be called also for equipped apparel.
		internal static float RawPriorities(Pawn pawn, Apparel apparel, ExtendedOutfit outfit)
		{
#if DEBUG
			MyDebug.debugDeepScorePriorities.Start(apparel.def.defName);
#endif
			if (outfit.StatPriorities.Count == 0)
			{
#if DEBUG
				MyDebug.debugDeepScorePriorities.AddToLog("\tNo StatPriorities.\n");
#endif
				return 0f;
			}

			bool isWorn = apparel.Wearer == pawn;
			float sum = 0f;
			int count = 0;
			foreach (var sp in outfit.StatPriorities)
			{
				float weight = sp.Weight;
				float scaledDelta = ApparelScore.GetFinalDelta(pawn, apparel, sp, isWorn);
				float score = scaledDelta * weight * weight * weight;
				sum += score;
				count++;

#if DEBUG
				MyDebug.debugDeepScorePriorities.AddToLog(
					$"Score[{score:F2}] " +
					$"SUM[{sum:F2}] " +
					$"COUNT[{count}]\n");
#endif
			}

			// Depending on setting return either sum or average.
			return OutfittedMod.Settings.sumScoresInsteadOfAverage ? sum : (count == 0 ? 0f : sum / count);
		}
	}
}
