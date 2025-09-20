using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Outfitted
{
	internal static class ApparelScorePriorities
	{
		private const float StatMinValue = -9999999f;
		private const float StatMaxValue = 9999999f;
		private const float StatRangeScore = 6.4f;      // By Max will give 100 points.
		internal static readonly HashSet<(Apparel apparel, StatDef statDef)> GetStatForApparelIdNoDefault = 
			new HashSet<(Apparel apparel, StatDef statDef)>();

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

				// Base value.
				float statBase = stat.defaultBaseValue;
				float statBaseEval = stat.postProcessCurve?.Evaluate(statBase) ?? statBase;
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t" +
					$"[{apparel.def.defName}] " +
					$"[{stat.defName}] " +
					$"[{stat.category}] " +
					$"Wei[{weight:F1}]\n");
				DebugDeepScorePriorities.AddToLog($"\t\t" +
					$"Pawn[{pawn.GetStatValue(stat):F1}] " +
					$"Base[{statBase:F1}] " +
					$"Eval[{statBaseEval:F1}] " +
					$"Min[{(statMin != StatMinValue ? statMin.ToString("F1") : "")}] " +
					$"Max[{(statMax != StatMaxValue ? statMax.ToString("F1") : "")}] " +
					$"NoStuff[{apparel.def.GetStatValueAbstract(stat, null):F1}] " +
					$"Stuff[{apparel.def.GetStatValueAbstract(stat, apparel.Stuff):F1}] " +
					$"Q[{apparel.GetStatValue(stat):F1}] " +
					$"Eq[{apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat):F1}] " +
					$"EqQ[{StatWorker.StatOffsetFromGear(apparel, stat):F1}]\n");

				string sel = "";
#endif
				float raw = 0f;
				float rawOffset = 0f;
				float delta = 0f;
				float scaledDelta = 0f;
				float score = 0f;

				// Base is zero. Apparel score is absolute.
				if (statBase == 0f)
				{
#if DEBUG
					sel = "Abs";
#endif
					raw = apparel.GetStatValue(stat);
					rawOffset = StatWorker.StatOffsetFromGear(apparel, stat);
				}
				else
				{
#if DEBUG
					sel = "Else";
#endif
					GetStatForApparelIdNoDefault.Add((apparel, stat));
					try
					{
						raw = apparel.GetStatValue(stat);
					}
					finally
					{
						GetStatForApparelIdNoDefault.Remove((apparel, stat));
					}
					rawOffset = StatWorker.StatOffsetFromGear(apparel, stat);
				}

				delta = raw + rawOffset;
				if (statMin != StatMinValue && statMax != StatMaxValue)
					scaledDelta = Mathf.InverseLerp(statMin, statMax, delta) * StatRangeScore;
				else
					scaledDelta = delta;
				score = scaledDelta * weight * weight * weight;
				sum += score;
				count++;

#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t\t" +
					$"Sel[{sel}] " +
					$"Raw[{raw:F2}] " +
					$"Offset[{rawOffset:F2}] " +
					$"Delta[{delta:F2}] " +
					$"Scaled[{scaledDelta:F2}] " +
					$"Score[{score:F2}] " +
					$"SUM[{sum:F2}] " +
					$"COUNT[{count}]\n");
#endif

				// Base to compare with. Most stats have direct effect: armor, shield value, etc.
				// Non-zero will be for stats, for which Pawn has some different base value: carry capacity, tend quality, etc.
				//float statBase = 0f;

				//float defBase = Math.Max(sp.Stat.defaultBaseValue, sp.Stat.minValue);

				// For some reason vanilla RW has not-zero base stat for 'EnergyShieldEnergyMax' or 'EnergyShieldRechargeRate'.
				// For us it is important to have any positive value (or negative based on selected weight).
				// Therefore statbase for category 'Apparel' is always zero.
				//if (sp.Stat.category == StatCategoryDefOf.Apparel ||
				//	sp.Stat.category == StatCategoryDefOf.Basics)
				//	defBase = 0f;

				//				float evalDefBase = sp.Stat.postProcessCurve?.Evaluate(statBase) ?? statBase;
				//				float baseValue = Math.Max(Math.Abs(evalDefBase), 0.001f);
				////#if DEBUG
				////				DebugDeepScorePriorities.AddToLog($"\t[{apparel.def.defName}] [{sp.Stat.defName}] " +
				////					$"[{sp.Stat.category}] " +
				////					$"Wei[{weight:F1}] defBase[{statBase:F1}] Eval[{evalDefBase:F1}] base[{baseValue:F1}]\n");
				////#endif
				//				//float raw = ApparelScore(pawn, apparel, sp.Stat);
				//				float delta = (evalDefBase < 0.001f) ? raw : (raw - evalDefBase) / baseValue;
				//				float score = delta * weight * weight * weight;
				//				sum += score;
				//				count++;
				//#if DEBUG
				//				DebugDeepScorePriorities.AddToLog($"Delta[{delta:F2}] Score[{score:F2}] Sum[{sum:F2}] Count [{count}]\n");
				//#endif
			}

			// Depending on setting return either sum or average.
			return OutfittedMod.Settings.sumScoresInsteadOfAverage ? sum : (count == 0 ? 0f : sum / count);

			//// Original
			//return !outfit.StatPriorities.Any<StatPriority>() ? 0.0f : outfit.StatPriorities.Select(sp => new
			//{
			//	weight = sp.Weight,
			//	value = apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetStatValue(sp.Stat),
			//	def = sp.Stat.defaultBaseValue
			//})
			//	.Average(sp => (
			//		(double)Math.Abs(sp.def) < 0.001 ? sp.value : (sp.value - sp.def) / Math.Abs(sp.def)) * Mathf.Pow(sp.weight, 3f)
			//	);
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
