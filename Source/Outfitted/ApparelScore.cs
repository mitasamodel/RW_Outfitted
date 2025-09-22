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
	public static class ApparelScore
	{
		private const float AbsoluteStatMinValue = -9999999f;
		private const float AbsoluteStatMaxValue = 9999999f;
		private const float StatRangeScore = 6.4f;      // By Max will give 100 points.

		public static float GetFinalDelta(Pawn pawn, Apparel apparel, StatPriority sp, bool isWorn)
		{
			StatDef stat = sp.Stat;
			float weight = sp.Weight;
			if (stat == null || weight == 0f) return 0f;

			// Range (if exists).
			float statMin = stat.minValue;
			float statMax = stat.maxValue;

			// Stats.
			float pawnStat = pawn.GetStatValue(stat);
			float raw = apparel.GetOutfittedStatValue(stat);
			float rawOffset = StatWorker.StatOffsetFromGear(apparel, stat);
#if DEBUG
			float statBase = stat.defaultBaseValue;
			float statBaseEval = stat.postProcessCurve?.Evaluate(statBase) ?? statBase;
			MyDebug.debugDeepScorePriorities.AddToLog($"\t" +
				$"[{apparel.def.defName}] " +
				$"[{stat.defName}] " +
				$"[{stat.category}] " +
				$"Wei[{weight:F1}]\n");
			MyDebug.debugDeepScorePriorities.AddToLog($"\t\t" +
				$"Pawn[{pawnStat:F1}] " +
				$"Base[{statBase:F1}] " +
				$"Eval[{statBaseEval:F1}] " +
				$"Min[{(statMin != AbsoluteStatMinValue ? statMin.ToString("F1") : "")}] " +
				$"Max[{(statMax != AbsoluteStatMaxValue ? statMax.ToString("F1") : "")}] " +
				$"NoStuff[{apparel.def.GetStatValueAbstract(stat, null):F1}] " +
				$"Stuff[{apparel.def.GetStatValueAbstract(stat, apparel.Stuff):F1}] " +
				$"Raw[{raw:F1}] " +
				$"Eq[{rawOffset:F1}]\n");
#endif
			float delta = raw + rawOffset;
			float modsAdjusted = AdjustForMods(apparel, stat, delta);
			float normalized = modsAdjusted;
			if (stat.category == StatCategoryDefOf.BasicsPawn)
			{
				// We need te decrease the stat by the full scale to get correct ratio.
				if (isWorn)
					pawnStat -= delta;
				// How much this apparel benefits, however, does depend on some mods adjustments.
				normalized = pawnStat != 0 ? modsAdjusted / pawnStat : modsAdjusted;

				// Here will be a slight error caused by Pawn's postprocess curve. But this error is really small
				// and it will provide slightly higher rating for already worn apparel, so no endless loops.
				// Example: weight capacity depends on Pawn's health (postprocess).
				// It applays reduction AFTER all stats summarized (e.g. 95% from total 140).
			}

			float scaledDelta = normalized;
			if (statMin != AbsoluteStatMinValue && statMax != AbsoluteStatMaxValue)
			{
				if (normalized < statMin || normalized > statMax)
				{
					int hash = Gen.HashCombineInt(Gen.HashCombineInt(apparel.thingIDNumber, stat.index), 0xaecde);
					Logger.Log_ErrorOnce($"Stat is out of bounds [min, max][{apparel.def.defName}][{stat.defName}][{normalized}]", hash);
				}
				else
					scaledDelta = Mathf.InverseLerp(statMin, statMax, normalized) * StatRangeScore;
			}

#if DEBUG
			MyDebug.debugDeepScorePriorities.AddToLog($"\t\t" +
				$"Raw[{raw:F2}] " +
				$"Offset[{rawOffset:F2}] " +
				$"Delta[{delta:F2}] " +
				$"Adj[{modsAdjusted:F2}] " +
				$"NormP[{normalized:F2}] " +
				$"Scaled[{scaledDelta}] ");
#endif

			return scaledDelta;
		}

		private static float AdjustForMods(Apparel apparel, StatDef stat, float value)
		{
			// CE: CarryBulk -> WornBulk; CarryWeight -> Mass
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (stat == StatDefOf_CE.CarryBulk)
					value -= apparel.GetOutfittedStatValue(StatDefOf_CE.WornBulk);
				else if (stat == StatDefOf_CE.CarryWeight)
					value -= apparel.GetOutfittedStatValue(StatDefOf_Rimworld.Mass);
#if DEBUG
				MyDebug.debugDeepScorePriorities.AddToLog($"\t\tCE_Adjusted[{value:F2}]\n");
#endif
			}
			return value;
		}
	}
}
