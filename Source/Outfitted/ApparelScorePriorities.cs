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
		private const float StatMinValue = -9999999f;
		private const float StatMaxValue = 9999999f;
		private const float StatRangeScore = 6.4f;      // By Max will give 100 points.

		// Can be called also for equipped apparel.
		internal static float RawPriorities(Pawn pawn, Apparel apparel, ExtendedOutfit outfit)
		{
#if DEBUG
			DebugDeepScorePriorities.Start(apparel.def.defName);
#endif
			if (outfit.StatPriorities.Count == 0)
			{
#if DEBUG
				DebugDeepScorePriorities.AddToLog("\tNo StatPriorities.\n");
#endif
				return 0f;
			}

			bool isWorn = apparel.Wearer == pawn;
			float sum = 0f;
			int count = 0;
			foreach (var sp in outfit.StatPriorities)
			{
				StatDef stat = sp.Stat;
				float weight = sp.Weight;
				if (stat == null || weight == 0f) continue;

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
				DebugDeepScorePriorities.AddToLog($"\t" +
					$"[{apparel.def.defName}] " +
					$"[{stat.defName}] " +
					$"[{stat.category}] " +
					$"Wei[{weight:F1}]\n");
				DebugDeepScorePriorities.AddToLog($"\t\t" +
					$"Pawn[{pawnStat:F1}] " +
					$"Base[{statBase:F1}] " +
					$"Eval[{statBaseEval:F1}] " +
					$"Min[{(statMin != StatMinValue ? statMin.ToString("F1") : "")}] " +
					$"Max[{(statMax != StatMaxValue ? statMax.ToString("F1") : "")}] " +
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
					// How much this apparel benefits however is depends on some mods adjustments.
					normalized = pawnStat != 0 ? modsAdjusted / pawnStat : modsAdjusted;
				}

				float scaledDelta = normalized;
				if (statMin != StatMinValue && statMax != StatMaxValue)
				{
					if (normalized < statMin || normalized > statMax)
					{
						int hash = Gen.HashCombineInt(Gen.HashCombineInt(apparel.thingIDNumber, stat.index), 0xaecde);
						Logger.Log_ErrorOnce($"Is out of bounds [min, max][{apparel.def.defName}][{stat.defName}][{normalized}]", hash);
					}
					else
						scaledDelta = Mathf.InverseLerp(statMin, statMax, normalized) * StatRangeScore;
				}

				float score = scaledDelta * weight * weight * weight;
				sum += score;
				count++;

#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\t" +
					$"Raw[{raw:F2}] " +
					$"Offset[{rawOffset:F2}] " +
					$"Delta[{delta:F2}] " +
					$"Adj[{modsAdjusted:F2}] " +
					$"NormP[{normalized:F2}] " +
					$"Scaled[{scaledDelta:F2}] " +
					$"Score[{score:F2}] " +
					$"SUM[{sum:F2}] " +
					$"COUNT[{count}]\n");
#endif
			}

			// Depending on setting return either sum or average.
			return OutfittedMod.Settings.sumScoresInsteadOfAverage ? sum : (count == 0 ? 0f : sum / count);
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
				DebugDeepScorePriorities.AddToLog($"\t\tCE_Adjusted[{value:F2}]\n");
#endif
			}
			return value;
		}

		//private static float 

		// Check if ThingDef has stat modifier applied when equipped.
		private static bool DefHasEquippedOffset(ThingDef def, StatDef stat)
		{
			var list = def.equippedStatOffsets;
			if (list != null)
			{
				foreach (var statApparel in list)
				{
					if (statApparel.stat == stat && Math.Abs(statApparel.value) > float.Epsilon) return true;
				}
			}
			return false;
		}
	}
}
