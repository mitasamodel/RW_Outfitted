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
		private const float nakedOffset = 1f;

		internal static bool showApparelScores;
		internal static bool isSaveStorageSettingsEnabled;
		internal static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve
		{
			{ new CurvePoint(0.0f, 0.0f), true },
			{ new CurvePoint(0.2f, 0.2f), true },
			{ new CurvePoint(0.22f, 0.6f), true },
			{ new CurvePoint(0.5f, 0.6f), true },
			{ new CurvePoint(0.52f, 1f), true }
		};

		private static readonly SimpleCurve InsulationTemperatureScoreFactorCurve_Need = new SimpleCurve
		{
			{ new CurvePoint(0.0f, 1f), true },
			{ new CurvePoint(30f, 4f), true }
		};

		private static readonly SimpleCurve InsulationFactorCurve = new SimpleCurve
		{
			{ new CurvePoint(-20f, -3f), true },
			{ new CurvePoint(-10f, -2f), true },
			{ new CurvePoint(10f, 2f), true },
			{ new CurvePoint(20f, 3f), true }
		};


		static Outfitted()
		{
#if DEBUG
			Logger.Init();
#endif
			isSaveStorageSettingsEnabled = ModLister.GetActiveModWithIdentifier("savestoragesettings.kv.rw") != null;
			new Harmony("rimworld.outfitted").PatchAll();
			Log.Message("[Outfitted] loaded");
		}

		internal static void PruneNullStatPriorities(ExtendedOutfit outfit)
		{
			if (outfit?.StatPriorities == null) return;
			outfit.StatPriorities.RemoveAll(sp => sp == null || sp.Stat == null);
		}

		public static float ApparelScoreExtra(Pawn pawn, Apparel apparel, NeededWarmth neededWarmth)
		{
			if (!(pawn.outfits.CurrentApparelPolicy is ExtendedOutfit currentApparelPolicy))
			{
				Log.ErrorOnce("Outfitted :: Not an ExtendedOutfit, something went wrong.", 399441);
				return 0.0f;
			}

			// Starting score.
			float num = OutfittedMod.Settings.disableStartScore ? 0f : 0.1f;

			// Score offset.
			num += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.def.apparel.scoreOffset;

			// Score from appaerl itself.
			num += ApparelScoreRawPriorities(apparel, currentApparelPolicy);

			// If Pawn need pants / shirt.
			num += ApparelScorePawnNeedThis(pawn, apparel);

			if (currentApparelPolicy.AutoWorkPriorities)
				num += ApparelScoreAutoWorkPriorities(pawn, apparel);

			if (apparel.def.useHitPoints)
			{
				float hp = (float)apparel.HitPoints / apparel.MaxHitPoints;
				num *= HitPointsPercentScoreFactorCurve.Evaluate(Mathf.Clamp01(hp));
			}

			num += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.GetSpecialApparelScoreOffset();

			if (pawn != null && currentApparelPolicy != null)
				num += ApparelScoreRawInsulation(pawn, apparel, currentApparelPolicy, neededWarmth);

			num = ModifiedWornByCorpse(pawn, apparel, currentApparelPolicy, num);
			return num;
		}

		internal static float ModifiedWornByCorpse(Pawn pawn, Apparel apparel, ExtendedOutfit currentApparelPolicy, float num)
		{
			if (currentApparelPolicy.PenaltyWornByCorpse && apparel.WornByCorpse && ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.DeadMansApparel, true))
			{
				num -= 0.5f;
				if (num > 0.0f)
					num *= 0.1f;
			}

			return num;
		}

		/// <summary>
		/// Return additional score if apparel needed due to nudity 
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="apparel"></param>
		/// <param name="whatIfNotWorn">Run the scoring like if this apparel will be removed and will be worn again</param>
		/// <returns></returns>
		internal static float ApparelScorePawnNeedThis(Pawn pawn, Apparel apparel, bool whatIfNotWorn = false)
		{
			if (pawn == null || apparel?.def?.apparel == null) return 0f;

			// Exclude this apparel if it is a run to get real score.
			Apparel exclude = whatIfNotWorn ? apparel : null;

			// Pawn wants pants
			if (IsDefPants(apparel.def))
			{
				if (PawnCareAboutNaked(pawn) && !PawnWearPants(pawn, exclude))
				{
					return nakedOffset;
				}
			}
			// Pawn wants shirt
			else if (IsDefShirt(apparel.def))
			{
				if (PawnCareAboutNaked(pawn) && PawnCareAboutTorso(pawn) && !PawnWearShirt(pawn, exclude))
				{
					return nakedOffset;
				}
			}

			return 0f;

			//var skinLayer = ApparelLayerDefOf.OnSkin;
			//var legs = BodyPartGroupDefOf.Legs;
		}

		private static bool PawnCareAboutTorso(Pawn pawn)
		{
			if (pawn == null) return false;
			//if (!ModsConfig.IdeologyActive) return pawn.gender == Gender.Female;

			// For now.
			// TODO: How to check Thought "naked" for female if Ideology is active and it is acceptable to not wear a shirt.
			return pawn.gender == Gender.Female;
		}

		private static bool PawnCareAboutNaked(Pawn pawn)
		{
			return ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.Naked, true);
		}

		// Pawn wear shirt or smth, what covers torso.
		private static bool PawnWearShirt(Pawn pawn, Apparel exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => ap != exclude && IsDefShirt(ap.def));
		}

		// Pawn wear pants or smth, what covers same area.
		private static bool PawnWearPants(Pawn pawn, Apparel exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => ap != exclude && IsDefPants(ap.def));
		}

		// Can be used as pants?
		private static bool IsDefPants(ThingDef def)
		{
			var apparel = def?.apparel;
			if (apparel == null) return false;

			return apparel.layers.Contains(ApparelLayerDefOf.OnSkin) && apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs);
		}

		// Can be used as shirt?
		private static bool IsDefShirt(ThingDef def)
		{
			var apparel = def?.apparel;
			if (apparel == null) return false;

			return apparel.layers.Contains(ApparelLayerDefOf.OnSkin) && apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso);
		}

		// Can be called also for equipped apparel.
		internal static float ApparelScoreRawPriorities(Apparel apparel, ExtendedOutfit outfit)
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

			// CE
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

		internal static float ApparelScoreAutoWorkPriorities(Pawn pawn, Apparel apparel)
		{
			return WorkPriorities.WorktypeStatPriorities(pawn)
				.Select(sp => (apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetStatValue(sp.Stat) - sp.Stat.defaultBaseValue) * sp.Weight).Sum();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="apparel"></param>
		/// <param name="outfit"></param>
		/// <param name="neededWarmth"></param>
		/// <param name="whatIfNotWorn">Run the scoring like if this apparel will be removed and will be worn again. Is used to get correct score of apparel.</param>
		/// <returns></returns>
		internal static float ApparelScoreRawInsulation(
			Pawn pawn,
			Apparel apparel,
			ExtendedOutfit outfit,
			NeededWarmth neededWarmth,
			bool whatIfNotWorn = false
		)
		{
			float num1;
			if (outfit.targetTemperaturesOverride)
			{
				// Current comfortable temperature.
				FloatRange curComfTemp = pawn.ComfortableTemperatureRange();

				// Target temperature.
				FloatRange targetTemp;
				if (outfit.AutoTemp)
				{
					float seasonalTemp = pawn.Map.mapTemperature.SeasonalTemp;
					targetTemp = new FloatRange(seasonalTemp - outfit.autoTempOffset, seasonalTemp + outfit.autoTempOffset);
				}
				else
					targetTemp = outfit.targetTemperatures;

				FloatRange newApparelInsulation = GetInsulationStats(apparel);

				// Remove current apparel temps to get the real score of already worn apparel.
				if (whatIfNotWorn)
				{
					curComfTemp.min -= newApparelInsulation.min;
					curComfTemp.max -= newApparelInsulation.max;
				}

				// New apparel insulation.
				FloatRange newComfTemp = curComfTemp;
				newComfTemp.min += newApparelInsulation.min;
				newComfTemp.max += newApparelInsulation.max;

				// Check if can be worn together with existed.
				// Skip if it is check for already worn apparel for whatIfNotWorn.
				if (!whatIfNotWorn && pawn.apparel != null && !pawn.apparel.WornApparel.Contains(apparel))
				{
					foreach (Apparel apWorn in pawn.apparel.WornApparel)
					{
						if (!ApparelUtility.CanWearTogether(apparel.def, apWorn.def, pawn.RaceProps.body))
						{
							// Cannot be worn together: reduce insulation by apparel, which will be removed.
							FloatRange insulationWorn = GetInsulationStats(apWorn);
							newComfTemp.min -= insulationWorn.min;
							newComfTemp.max -= insulationWorn.max;
						}
					}
				}
				FloatRange floatRange3 = new FloatRange(Mathf.Max(curComfTemp.min - targetTemp.min, 0.0f), Mathf.Max(targetTemp.max - curComfTemp.max, 0.0f));
				FloatRange floatRange4 = new FloatRange(Mathf.Max(newComfTemp.min - targetTemp.min, 0.0f), Mathf.Max(targetTemp.max - newComfTemp.max, 0.0f));
				num1 = InsulationFactorCurve.Evaluate(floatRange3.min - floatRange4.min) + InsulationFactorCurve.Evaluate(floatRange3.max - floatRange4.max);
			}
			else
			{
				switch (neededWarmth)
				{
					case NeededWarmth.Warm:
						float statValue1 = apparel.GetStatValue(StatDefOf.Insulation_Heat);
						num1 = InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue1);
						break;
					case NeededWarmth.Cool:
						float statValue2 = apparel.GetStatValue(StatDefOf.Insulation_Cold);
						num1 = InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue2);
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
				foreach (Pawn pawn in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer).Where<Pawn>(i => i != null && i.outfits != null && i.outfits.CurrentApparelPolicy != null && i.outfits.CurrentApparelPolicy.id == id))
					pawn.mindState?.Notify_OutfitChanged();
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Outfitted.Notify_OutfitChanged: {0}", ex));
			}
		}
	}
}
