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
			if (!outfit.StatPriorities.Any())
			{
#if DEBUG
				DebugDeepScorePriorities.AddToLog("\tNo StatPriorities.\n");
#endif
				return 0f;
			}

			float sum = 0f;
			int count = 0;
			foreach (var sp in outfit.StatPriorities)
			{
				StatDef stat = sp.Stat;
				if (stat == null) continue;

				// Stat weight - how important stat is (positive or negative).
				float weight = sp.Weight;

				// Range (if exists).
				float statMin = stat.minValue;
				float statMax = stat.maxValue;

				// Stats.
				float pawnStat = pawn.GetStatValue(stat);
				float raw = apparel.GetStatValue(stat);
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
					normalized = pawnStat != 0 ? modsAdjusted / pawnStat : modsAdjusted;

				float scaledDelta = normalized;
				if (statMin != StatMinValue && statMax != StatMaxValue)
				{
					if (normalized < statMin || normalized > statMax)
						Logger.Log_ErrorOnce($"Is out of bounds [min, max][{apparel.def.defName}][{stat.defName}][{normalized}]", 0xaecde);
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
					value -= apparel.GetStatValue(StatDefOf_CE.WornBulk);
				else if (stat == StatDefOf_CE.CarryWeight)
					value -= apparel.GetStatValue(StatDefOf_Rimworld.Mass);
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\tCE_Adjusted[{value:F2}]\n");
#endif
			}
			return value;
		}

		/// <summary>
		/// Score based on effect of apparel.
		/// Example: CE's default CarryBulk is never 0 (it is 20) and "normal" formula will return higher than 20 for all "good" items
		/// even if this item does not modify CarryBulk (it is checked in StatOffsetFromGear instead).
		/// </summary>
		/// <param name="apparel"></param>
		/// <param name="stat"></param>
		/// <param name="basedOnQuality"></param>
		/// <returns></returns>
		public static float ApparelScore(Pawn pawn, Apparel apparel, StatDef stat, bool basedOnQuality = true)
		{
			float result;
			// Apparel provides gear offset for the stat.
			// That means only offset depends on quality/material.
			// Base value comes from stat default.
			if (DefHasEquippedOffset(apparel.def, stat))
			{
				result = basedOnQuality ? StatWorker.StatOffsetFromGear(apparel, stat) : apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat);
				result += stat.defaultBaseValue;
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\tEquipped[{result:F2}] ");
#endif
			}

			// Pawn-category stats with no equipped offset (example: CarryBulk with no offset).
			// Apparel itself doesn't provide any bonus.
			// Stat is always here "default", but to be safe, take it from apparel, not from stat itself.
			else if (stat.category == StatCategoryDefOf.BasicsPawn)
			{
				result = apparel.def.GetStatValueAbstract(stat, null);
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\tPawn-category[{result:F2}] ");
#endif
			}

			// All other.
			// Stat is not default; it is defined not as gear offset, but as stat itself.
			// Example: armor, cold/warm insulation. Depends on gear itsef, not modifying Pawn's stats.
			else
			{
				//var def = stat.defaultBaseValue;
				//var appVal = apparel.def.GetStatValueAbstract(stat, apparel.Stuff);

				//if (apparel.def.StatBaseDefined(stat))
				//	result = apparel.GetStatValue(stat);
				//else if (def != appVal)
				//	result = appVal;
				//else
				//	result = def;
				result = basedOnQuality ? apparel.GetStatValue(stat) : apparel.def.GetStatValueAbstract(stat, apparel.Stuff);
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\tNormal[{result:F2}] ");
#endif
			}

			// CE: CarryBulk -> WornBulk; CarryWeight -> Mass
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (stat == StatDefOf_CE.CarryBulk)
					result -= apparel.GetStatValue(StatDefOf_CE.WornBulk);
				else if (stat == StatDefOf_CE.CarryWeight)
					result -= apparel.GetStatValue(StatDefOf_Rimworld.Mass);
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"CE_Adjusted[{result:F2}] ");
#endif
			}

			return result;
		}

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
