using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	internal static class ApparelScorePriorities
	{
		// Can be called also for equipped apparel.
		internal static float RawPriorities(Apparel apparel, ExtendedOutfit outfit)
		{
			if (!outfit.StatPriorities.Any()) return 0f;

			float sum = 0f;
			int count = 0;
			foreach (var sp in outfit.StatPriorities)
			{
				if (sp?.Stat == null) continue;
				float weight = sp.Weight;
				float defaultAbs = Math.Abs(sp.Stat.defaultBaseValue);
				float baseValue = Math.Max(defaultAbs, 0.001f);
				float raw = ApparelScore(apparel, sp.Stat);
				float delta = (defaultAbs < 0.001f) ? raw : (raw - sp.Stat.defaultBaseValue) / baseValue;

				sum += delta * weight * weight * weight;
				count++;
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
			float result;
			// Apparel provides gear offset for the stat.
			// That means only offset depends on quality/material.
			// Base value comes from stat default.
			if (DefHasEquippedOffset(apparel.def, stat))
			{
				result = basedOnQuality ? StatWorker.StatOffsetFromGear(apparel, stat) : apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat);
				result += stat.defaultBaseValue;
			}

			// Pawn-category stats with no equipped offset (example: CarryBulk with no offset).
			// Apparel itself doesn't provide any bonus.
			// Stat is always here "default", but to be safe, take it from apparel, not from stat itself.
			else if (stat.category == StatCategoryDefOf.BasicsPawn)
				result = apparel.def.GetStatValueAbstract(stat, null);

			// All other.
			// Stat is not default; it is defined not as gear offset, but as stat itself.
			// Example: armor, cold/warm insulation. Depends on gear itsef, not modifying Pawn's stats.
			else
				result = basedOnQuality ? apparel.GetStatValue(stat) : apparel.def.GetStatValueAbstract(stat, apparel.Stuff);

			// CE: CarryBulk -> WornBulk; CarryWeight -> Mass
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (stat == StatDefOf_CE.CarryBulk)
					result -= apparel.GetStatValue(StatDefOf_CE.WornBulk);
				else if (stat == StatDefOf_CE.CarryWeight)
					result -= apparel.GetStatValue(StatDefOf_Rimworld.Mass);
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
