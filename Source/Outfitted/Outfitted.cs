using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
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

		public static float ApparelScoreExtra(Pawn pawn, Apparel apparel, NeededWarmth neededWarmth, bool whatIfNotWorn = false)
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
			num += ApparelScorePriorities.RawPriorities(apparel, currentApparelPolicy);

			// If Pawn need pants / shirt.
			num += ApparelScoreNeeds.PawnNeedThis(pawn, apparel, whatIfNotWorn);

			if (currentApparelPolicy.AutoWorkPriorities)
				num += ApparelScoreAutoWorkPriorities(pawn, apparel);

			if (apparel.def.useHitPoints)
			{
				float hp = (float)apparel.HitPoints / apparel.MaxHitPoints;
				num *= HitPointsPercentScoreFactorCurve.Evaluate(Mathf.Clamp01(hp));
			}

			num += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.GetSpecialApparelScoreOffset();

			if (pawn != null && currentApparelPolicy != null)
				num += ApparelScoreRawInsulation(pawn, apparel, currentApparelPolicy, neededWarmth, whatIfNotWorn);

			num = ApparelScoreNeeds.ModifiedWornByCorpse(pawn, apparel, currentApparelPolicy, num);
			return num;
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
					var map = pawn?.MapHeld ?? pawn?.Map;
					if (map != null)
					{
						float seasonalTemp = map.mapTemperature.SeasonalTemp;
						targetTemp = new FloatRange(seasonalTemp - outfit.autoTempOffset, seasonalTemp + outfit.autoTempOffset);
					}
					else
					{
						// No map (caravan, world-only context, etc.).
						// Use the user-set manual range rather than guessing world temps.
						targetTemp = outfit.targetTemperatures;
					}
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

		public static void BuildWornScore(Pawn pawn, List<float> wornScores)
		{
			if (pawn == null || pawn.apparel == null || pawn.apparel.WornApparel == null) return;
			if (wornScores is null) throw new ArgumentNullException(nameof(wornScores));
			var worn = pawn.apparel.WornApparel;
			wornScores.Capacity = wornScores.Count + worn.Count;
			using (PawnContext.WhatIfNotWornScope(pawn))
			{
				foreach (var ap in worn)
				{
					if (ap == null)
					{
						Logger.Log_Warning("BuildWornScore: Unexpected Apparel-null in worn list.");
						continue;
					}
					wornScores.Add(JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap));
				}
			}
		}
	}
}
