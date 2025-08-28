using CombatExtended;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class Outfitted
	{
		internal static bool showApparelScores;
		internal static bool isSaveStorageSettingsEnabled;
		private static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve()
	{
	  {
		new CurvePoint(0.0f, 0.0f),
		true
	  },
	  {
		new CurvePoint(0.2f, 0.2f),
		true
	  },
	  {
		new CurvePoint(0.22f, 0.6f),
		true
	  },
	  {
		new CurvePoint(0.5f, 0.6f),
		true
	  },
	  {
		new CurvePoint(0.52f, 1f),
		true
	  }
	};
		private static readonly SimpleCurve InsulationTemperatureScoreFactorCurve_Need = new SimpleCurve()
	{
	  {
		new CurvePoint(0.0f, 1f),
		true
	  },
	  {
		new CurvePoint(30f, 4f),
		true
	  }
	};
		private static readonly SimpleCurve InsulationFactorCurve = new SimpleCurve()
	{
	  {
		new CurvePoint(-20f, -3f),
		true
	  },
	  {
		new CurvePoint(-10f, -2f),
		true
	  },
	  {
		new CurvePoint(10f, 2f),
		true
	  },
	  {
		new CurvePoint(20f, 3f),
		true
	  }
	};

		static Outfitted()
		{
			Outfitted.isSaveStorageSettingsEnabled = ModLister.GetActiveModWithIdentifier("savestoragesettings.kv.rw") != null;
			new Harmony("rimworld.outfitted").PatchAll();
			Log.Message("[Outfitted] loaded");
		}

		public static float ApparelScoreExtra(Pawn pawn, Apparel apparel, NeededWarmth neededWarmth)
		{
			if (!(pawn.outfits.CurrentApparelPolicy is ExtendedOutfit currentApparelPolicy))
			{
				Log.ErrorOnce("Outfitted :: Not an ExtendedOutfit, something went wrong.", 399441);
				return 0.0f;
			}

			// Starting score.
			float num1 = OutfittedMod.Settings.disableStartScore ? 0f : 0.1f;

			// Score offset.
			num1 += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.def.apparel.scoreOffset;

			num1 += Outfitted.ApparelScoreRawPriorities(apparel, currentApparelPolicy);

			if (currentApparelPolicy.AutoWorkPriorities)
				num1 += Outfitted.ApparelScoreAutoWorkPriorities(pawn, apparel);
			if (apparel.def.useHitPoints)
				num1 *= Outfitted.HitPointsPercentScoreFactorCurve.Evaluate((float)apparel.HitPoints / (float)apparel.MaxHitPoints);
			float num2 = OutfittedMod.Settings.disableScoreOffset ? num1 : num1 + apparel.GetSpecialApparelScoreOffset();
			if (pawn != null && currentApparelPolicy != null)
				num2 += Outfitted.ApparelScoreRawInsulation(pawn, apparel, currentApparelPolicy, neededWarmth);
			if (currentApparelPolicy.PenaltyWornByCorpse && apparel.WornByCorpse && ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.DeadMansApparel, true))
			{
				num2 -= 0.5f;
				if ((double)num2 > 0.0)
					num2 *= 0.1f;
			}
			return num2;
			//return num1;
		}

		private static float ApparelScoreRawPriorities(Apparel apparel, ExtendedOutfit outfit)
		{
			if (!outfit.StatPriorities.Any<StatPriority>()) return 0f;

			return outfit.StatPriorities.Average(sp =>
			{
				float weight = sp.Weight;
				float baseValue = Math.Max(Math.Abs(sp.Stat.defaultBaseValue), 0.001f);
				bool hasEquippedOffset = DefHasEquippedOffset(apparel.def, sp.Stat);

				float raw = ApparelScore(apparel, sp.Stat);

				float delta;
				if (hasEquippedOffset)
					delta = raw / baseValue;
				else
					delta = (Math.Abs(sp.Stat.defaultBaseValue) < 0.001f) ? raw : (raw - sp.Stat.defaultBaseValue) / baseValue;

				return delta * (float)Math.Pow(weight, 3f);
			});


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
			// Apparel provides gear offset for this stat.
			if (DefHasEquippedOffset(apparel.def, stat))
			{
				result = basedOnQuality ? StatWorker.StatOffsetFromGear(apparel, stat) : apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat);
			}

			// Pawn-category stats with no equipped offset (example: CarryBulk with no offset).
			else if (stat.category == StatCategoryDefOf.BasicsPawn)
				result = apparel.def.GetStatValueAbstract(stat, null);

			// All other. Example: armor, cold/warm insulation. Depends on gear itsef, not modifying Pawn's stats.
			else
				result = basedOnQuality ? apparel.GetStatValue(stat) : apparel.def.GetStatValueAbstract(stat, apparel.Stuff);

			// CE
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (stat == CE_CompatDefOf.CarryBulk)
					result -= apparel.GetStatValue(CE_CompatDefOf.WornBulk);
				else if (stat == CE_CompatDefOf.CarryWeight)
					result -= apparel.GetStatValue(RimWorld.StatDefOf.Mass);
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

		private static float ApparelScoreAutoWorkPriorities(Pawn pawn, Apparel apparel)
		{
			return WorkPriorities.WorktypeStatPriorities(pawn).Select<StatPriority, float>((Func<StatPriority, float>)(sp => (apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetStatValue(sp.Stat) - sp.Stat.defaultBaseValue) * sp.Weight)).Sum();
		}

		private static float ApparelScoreRawInsulation(
		  Pawn pawn,
		  Apparel apparel,
		  ExtendedOutfit outfit,
		  NeededWarmth neededWarmth)
		{
			float num1;
			if (outfit.targetTemperaturesOverride)
			{
				int num2 = pawn.apparel.WornApparel.Contains(apparel) ? 1 : 0;
				FloatRange floatRange1 = pawn.ComfortableTemperatureRange();
				FloatRange floatRange2 = floatRange1;
				if (outfit.AutoTemp)
				{
					float seasonalTemp = pawn.Map.mapTemperature.SeasonalTemp;
					outfit.targetTemperatures = new FloatRange(seasonalTemp - (float)outfit.autoTempOffset, seasonalTemp + (float)outfit.autoTempOffset);
				}
				FloatRange targetTemperatures = outfit.targetTemperatures;
				FloatRange insulationStats1 = Outfitted.GetInsulationStats(apparel);
				floatRange2.min += insulationStats1.min;
				floatRange2.max += insulationStats1.max;
				if (num2 == 0)
				{
					foreach (Apparel apparel1 in pawn.apparel.WornApparel)
					{
						if (!ApparelUtility.CanWearTogether(apparel.def, apparel1.def, pawn.RaceProps.body))
						{
							FloatRange insulationStats2 = Outfitted.GetInsulationStats(apparel1);
							floatRange2.min -= insulationStats2.min;
							floatRange2.max -= insulationStats2.max;
						}
					}
				}
				FloatRange floatRange3 = new FloatRange(Mathf.Max(floatRange1.min - targetTemperatures.min, 0.0f), Mathf.Max(targetTemperatures.max - floatRange1.max, 0.0f));
				FloatRange floatRange4 = new FloatRange(Mathf.Max(floatRange2.min - targetTemperatures.min, 0.0f), Mathf.Max(targetTemperatures.max - floatRange2.max, 0.0f));
				num1 = Outfitted.InsulationFactorCurve.Evaluate(floatRange3.min - floatRange4.min) + Outfitted.InsulationFactorCurve.Evaluate(floatRange3.max - floatRange4.max);
			}
			else
			{
				switch (neededWarmth)
				{
					case NeededWarmth.Warm:
						float statValue1 = apparel.GetStatValue(StatDefOf.Insulation_Heat);
						num1 = Outfitted.InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue1);
						break;
					case NeededWarmth.Cool:
						float statValue2 = apparel.GetStatValue(StatDefOf.Insulation_Cold);
						num1 = Outfitted.InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue2);
						break;
					default:
						num1 = 1f;
						break;
				}
			}
			return num1;
		}

		private static FloatRange GetInsulationStats(Apparel apparel)
		{
			return new FloatRange(-apparel.GetStatValue(StatDefOf.Insulation_Cold), apparel.GetStatValue(StatDefOf.Insulation_Heat));
		}

		internal static void Notify_OutfitChanged(int id)
		{
			try
			{
				if (PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer) == null)
					return;
				foreach (Pawn pawn in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer).Where<Pawn>((Func<Pawn, bool>)(i => i != null && i.outfits != null && i.outfits.CurrentApparelPolicy != null && i.outfits.CurrentApparelPolicy.id == id)))
					pawn.mindState?.Notify_OutfitChanged();
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Outfitted.Notify_OutfitChanged: {0}", (object)ex));
			}
		}
	}
}
