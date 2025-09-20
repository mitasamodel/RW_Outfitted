using Outfitted.RW_JustUtils;
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
	internal class ApparelScoreInsulation
	{
		private static readonly SimpleCurve InsulationTemperatureScoreFactorCurve_Need = new SimpleCurve
		{
			{ new CurvePoint(0.0f, 1f), true },
			{ new CurvePoint(30f, 4f), true }
		};

		private static readonly SimpleCurve InsulationFactorCurve = new SimpleCurve
		{
			//{ new CurvePoint(-20f, -3f), true },
			{ new CurvePoint(-60f, -9.5f), true },
			{ new CurvePoint(-10f, -2f), true },
			{ new CurvePoint(10f, 2f), true },
			//{ new CurvePoint(20f, 3f), true }
			{ new CurvePoint(60f, 9.5f), true }
		};

		internal static float RawInsulation(
			Pawn pawn,
			Apparel apparel,
			ExtendedOutfit outfit,
			NeededWarmth neededWarmth
		)
		{
			bool deepLogger = false && (
				apparel.def.defName.ContainsIgnoreCase("parka") ||
				apparel.def.defName.ContainsIgnoreCase("duster"));

			float num1;
			if (outfit.targetTemperaturesOverride)
			{
				if (deepLogger) Logger.LogNL($"[{apparel.def.defName}]");

				// Current comfortable temperature.
				FloatRange curComfTemp = pawn.ComfortableTemperatureRange();
				if (deepLogger) Logger.LogNL($"curComfTemp min[{curComfTemp.min}] max[{curComfTemp.max}]");

				// Target temperature.
				FloatRange targetTemp;
				if (outfit.AutoTemp)
				{
					var map = pawn?.MapHeld ?? pawn?.Map;
					if (map != null)
					{
						float seasonalTemp = map.mapTemperature.SeasonalTemp;
						float conTempOffset = OutfittedMod.Settings.insScoreBasedOnMapConditions ? GetTempOffset(map) : 0f;
						float mapTemp = seasonalTemp + conTempOffset;
						targetTemp = new FloatRange(mapTemp - outfit.autoTempOffset, mapTemp + outfit.autoTempOffset);
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
				if (deepLogger) Logger.LogNL($"targetTemp min[{targetTemp.min}] max[{targetTemp.max}]");

				// Conflict insulation.
				FloatRange conflictsIns = new FloatRange(0f, 0f);
				if (pawn.apparel != null)
				{
					foreach (Apparel apWorn in pawn.apparel.WornApparel)
					{
						if (!ApparelUtility.CanWearTogether(apparel.def, apWorn.def, pawn.RaceProps.body))
						{
							FloatRange insulationWorn = GetInsulationStats(apWorn);
							conflictsIns.min += insulationWorn.min;
							conflictsIns.max += insulationWorn.max;
						}
					}
				}
				if (deepLogger) Logger.LogNL($"conflictsIns min[{conflictsIns.min}] max[{conflictsIns.max}]");

				// Remove stats from conflicting insulation.
				// The same is done for already worn apparel too to get its proper score.
				curComfTemp.min -= conflictsIns.min;
				curComfTemp.max -= conflictsIns.max;
				if (deepLogger) Logger.LogNL($"Adjust curComfTemp min[{curComfTemp.min}] max[{curComfTemp.max}]");

				// Add new apparel.
				FloatRange newApparelInsulation = GetInsulationStats(apparel);
				if (deepLogger) Logger.LogNL($"newApparelInsulation min[{newApparelInsulation.min}] max[{newApparelInsulation.max}]");

				// New apparel insulation.
				FloatRange newComfTemp = curComfTemp;
				newComfTemp.min += newApparelInsulation.min;
				newComfTemp.max += newApparelInsulation.max;
				if (deepLogger) Logger.LogNL($"Add newComfTemp min[{newComfTemp.min}] max[{newComfTemp.max}]");

				FloatRange curNeed = new FloatRange(
					Mathf.Max(curComfTemp.min - targetTemp.min, 0.0f),
					Mathf.Max(targetTemp.max - curComfTemp.max, 0.0f));

				FloatRange newComf = new FloatRange(
					Mathf.Max(newComfTemp.min - targetTemp.min, 0.0f),
					Mathf.Max(targetTemp.max - newComfTemp.max, 0.0f));

				num1 = InsulationFactorCurve.Evaluate(curNeed.min - newComf.min) +
					InsulationFactorCurve.Evaluate(curNeed.max - newComf.max);
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

		/// <summary>
		/// Offset by any external conditions (heat wave, cold snap, etc.).
		/// </summary>
		/// <param name="map"></param>
		/// <returns></returns>
		private static float GetTempOffset(Map map)
		{
			if (map == null) return 0f;
			var conditions = map.GameConditionManager.ActiveConditions;
			if (conditions == null) return 0f;
			float offset = 0f;
			foreach (var condition in conditions)
			{
				offset += condition.TemperatureOffset();
			}
			return offset;
		}

		private static FloatRange GetInsulationStats(Apparel apparel)
		{
			return new FloatRange(-apparel.GetStatValue(StatDefOf.Insulation_Cold), apparel.GetStatValue(StatDefOf.Insulation_Heat));
		}
	}
}
