// Decompiled with JetBrains decompiler
// Type: Outfitted.OutfittedMod
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

#nullable disable
namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class OutfittedMod
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

		static OutfittedMod()
		{
			OutfittedMod.isSaveStorageSettingsEnabled = ModLister.GetActiveModWithIdentifier("savestoragesettings.kv.rw") != null;
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
			float num1 = 0.1f + apparel.def.apparel.scoreOffset + OutfittedMod.ApparelScoreRawPriorities(apparel, currentApparelPolicy);
			if (currentApparelPolicy.AutoWorkPriorities)
				num1 += OutfittedMod.ApparelScoreAutoWorkPriorities(pawn, apparel);
			if (apparel.def.useHitPoints)
				num1 *= OutfittedMod.HitPointsPercentScoreFactorCurve.Evaluate((float)apparel.HitPoints / (float)apparel.MaxHitPoints);
			float num2 = num1 + apparel.GetSpecialApparelScoreOffset();
			if (pawn != null && currentApparelPolicy != null)
				num2 += OutfittedMod.ApparelScoreRawInsulation(pawn, apparel, currentApparelPolicy, neededWarmth);
			if (currentApparelPolicy.PenaltyWornByCorpse && apparel.WornByCorpse && ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.DeadMansApparel, true))
			{
				num2 -= 0.5f;
				if ((double)num2 > 0.0)
					num2 *= 0.1f;
			}
			return num2;
		}

		private static float ApparelScoreRawPriorities(Apparel apparel, ExtendedOutfit outfit)
		{
			return !outfit.StatPriorities.Any<StatPriority>() ? 0.0f : outfit.StatPriorities.Select(sp => new
			{
				weight = sp.Weight,
				value = apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetStatValue(sp.Stat),
				def = sp.Stat.defaultBaseValue
			}).Average(sp => ((double)Math.Abs(sp.def) < 0.001 ? sp.value : (sp.value - sp.def) / sp.def) * Mathf.Pow(sp.weight, 3f));
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
				FloatRange insulationStats1 = OutfittedMod.GetInsulationStats(apparel);
				floatRange2.min += insulationStats1.min;
				floatRange2.max += insulationStats1.max;
				if (num2 == 0)
				{
					foreach (Apparel apparel1 in pawn.apparel.WornApparel)
					{
						if (!ApparelUtility.CanWearTogether(apparel.def, apparel1.def, pawn.RaceProps.body))
						{
							FloatRange insulationStats2 = OutfittedMod.GetInsulationStats(apparel1);
							floatRange2.min -= insulationStats2.min;
							floatRange2.max -= insulationStats2.max;
						}
					}
				}
				FloatRange floatRange3 = new FloatRange(Mathf.Max(floatRange1.min - targetTemperatures.min, 0.0f), Mathf.Max(targetTemperatures.max - floatRange1.max, 0.0f));
				FloatRange floatRange4 = new FloatRange(Mathf.Max(floatRange2.min - targetTemperatures.min, 0.0f), Mathf.Max(targetTemperatures.max - floatRange2.max, 0.0f));
				num1 = OutfittedMod.InsulationFactorCurve.Evaluate(floatRange3.min - floatRange4.min) + OutfittedMod.InsulationFactorCurve.Evaluate(floatRange3.max - floatRange4.max);
			}
			else
			{
				switch (neededWarmth)
				{
					case NeededWarmth.Warm:
						float statValue1 = apparel.GetStatValue(StatDefOf.Insulation_Heat);
						num1 = OutfittedMod.InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue1);
						break;
					case NeededWarmth.Cool:
						float statValue2 = apparel.GetStatValue(StatDefOf.Insulation_Cold);
						num1 = OutfittedMod.InsulationTemperatureScoreFactorCurve_Need.Evaluate(statValue2);
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
				Log.Error(string.Format("OutfittedMod.Notify_OutfitChanged: {0}", (object)ex));
			}
		}
	}
}
