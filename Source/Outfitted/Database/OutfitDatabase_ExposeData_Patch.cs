using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Outfitted.Database
{
	[HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.ExposeData), MethodType.Normal)]
	internal static class OutfitDatabase_ExposeData_Patch
	{
		private static void Postfix(OutfitDatabase __instance, List<ApparelPolicy> ___outfits)
		{
			if (Scribe.mode != LoadSaveMode.LoadingVars || ___outfits.Any<ApparelPolicy>((Predicate<ApparelPolicy>)(i => i is ExtendedOutfit)))
				return;
			foreach (ApparelPolicy outfit in ___outfits.ToList<ApparelPolicy>())
			{
				___outfits.Remove(outfit);
				___outfits.Add(OutfitDatabase_ExposeData_Patch.ReplaceKnownVanillaOutfits(outfit));
			}
			OutfitDatabase_GenerateStartingOutfits_Patch.GenerateStartingOutfits(__instance, false);
		}

		private static ApparelPolicy ReplaceKnownVanillaOutfits(ApparelPolicy outfit)
		{
			ExtendedOutfit extendedOutfit = new ExtendedOutfit(outfit);
			switch (extendedOutfit.label)
			{
				case "Worker":
					extendedOutfit.AddRange((IEnumerable<StatPriority>)new List<StatPriority>()
					{
						new StatPriority(global::Outfitted.StatDefOf.MoveSpeed, 0.0f),
						new StatPriority(global::Outfitted.StatDefOf.WorkSpeedGlobal, 1f)
					});
					break;
				case "Soldier":
					extendedOutfit.AddRange((IEnumerable<StatPriority>)new List<StatPriority>()
					{
						new StatPriority(global::Outfitted.StatDefOf.ShootingAccuracyPawn, 2f),
						new StatPriority(global::Outfitted.StatDefOf.AccuracyShort, 1f),
						new StatPriority(global::Outfitted.StatDefOf.AccuracyMedium, 1f),
						new StatPriority(global::Outfitted.StatDefOf.AccuracyLong, 1f),
						new StatPriority(global::Outfitted.StatDefOf.MoveSpeed, 1f),
						new StatPriority(global::Outfitted.StatDefOf.ArmorRating_Blunt, 0.0f),
						new StatPriority(global::Outfitted.StatDefOf.ArmorRating_Sharp, 1f),
						new StatPriority(global::Outfitted.StatDefOf.MeleeDodgeChance, 0.0f),
						new StatPriority(global::Outfitted.StatDefOf.AimingDelayFactor, -2f),
						new StatPriority(global::Outfitted.StatDefOf.RangedWeapon_Cooldown, -2f),
						new StatPriority(global::Outfitted.StatDefOf.PainShockThreshold, 2f)
					});
					break;
				case "Nudist":
					extendedOutfit.AddRange((IEnumerable<StatPriority>)new List<StatPriority>()
					{
						new StatPriority(global::Outfitted.StatDefOf.MoveSpeed, 1f),
						new StatPriority(global::Outfitted.StatDefOf.WorkSpeedGlobal, 2f)
					});
					break;
				default:
					extendedOutfit.AddRange((IEnumerable<StatPriority>)new List<StatPriority>()
					{
						new StatPriority(global::Outfitted.StatDefOf.MoveSpeed, 1f),
						new StatPriority(global::Outfitted.StatDefOf.WorkSpeedGlobal, 2f),
						new StatPriority(global::Outfitted.StatDefOf.ArmorRating_Blunt, 1f),
						new StatPriority(global::Outfitted.StatDefOf.ArmorRating_Sharp, 1f)
					});
					break;
			}
			return (ApparelPolicy)extendedOutfit;
		}
	}
}
