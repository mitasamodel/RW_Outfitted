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
		// Can be called also for equipped apparel.
		internal static float RawPriorities(Apparel apparel, ExtendedOutfit outfit)
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
				if (sp?.Stat == null) continue;
				float weight = sp.Weight;
				float defBase = Math.Max(sp.Stat.defaultBaseValue, sp.Stat.minValue);

				// For some reason vanilla RW has not-zero base stat for 'EnergyShieldEnergyMax' or 'EnergyShieldRechargeRate'.
				// For us it is important to have any positive value (or negative based on selected weight).
				// Therefore statbase for category 'Apparel' is always ero.
				if (sp.Stat.category == StatCategoryDefOf.Apparel)
					defBase = 0f;

				float evalDefBase = sp.Stat.postProcessCurve?.Evaluate(defBase) ?? defBase;
				float defAbs = Math.Abs(evalDefBase);
				float baseValue = Math.Max(defAbs, 0.001f);
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"\t[{apparel.def.defName}] [{sp.Stat.defName}] " +
					$"[{sp.Stat.category}] " +
					$"Wei[{weight:F1}] defBase[{defBase:F1}] Eval[{evalDefBase:F1}] base[{baseValue:F1}]\n");
#endif
				float raw = ApparelScore(apparel, sp.Stat);
				float delta = (evalDefBase < 0.001f) ? raw : (raw - evalDefBase) / baseValue;
				float score = delta * weight * weight * weight;
				sum += score;
				count++;
#if DEBUG
				DebugDeepScorePriorities.AddToLog($"Delta[{delta:F2}] Score[{score:F2}] Sum[{sum:F2}] Count [{count}]\n");
#endif
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
		public static float ApparelScore(Apparel apparel, StatDef stat, bool basedOnQuality = true)
		{
#if DEBUG
			DebugDeepScorePriorities.AddToLog($"\t\tBase[{stat.defaultBaseValue:F1}] " +
				$"Min[{stat.minValue:F1}] " +
				$"NoStuff[{apparel.def.GetStatValueAbstract(stat, null):F1}] " +
				$"Norm[{apparel.def.GetStatValueAbstract(stat, apparel.Stuff):F1}] " +
				$"Q[{apparel.GetStatValue(stat):F1}] " +
				$"Eq[{apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat):F1}] " +
				$"EqQ[{StatWorker.StatOffsetFromGear(apparel, stat):F1}]\n");
#endif
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
