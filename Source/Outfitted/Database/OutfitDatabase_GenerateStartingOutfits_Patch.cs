using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace Outfitted.Database
{
	[HarmonyPatch(typeof(OutfitDatabase), "GenerateStartingOutfits")]
	public static class OutfitDatabase_GenerateStartingOutfits_Patch
	{
		private static bool Prefix(OutfitDatabase __instance)
		{
			if (OutfittedMod.Settings.generateStartingOutfits)
			{
				try
				{
					GenerateStartingOutfits(__instance);
				}
				catch (Exception ex)
				{
					Log.Error("Can't generate outfits: " + ex?.ToString());
				}
				return false;
			}
			else return true;
		}

		// All apparel created with RW MakeNewOutfit() method will get
		// basic stats from Helpers.AddBasicStats().
		internal static void GenerateStartingOutfits(OutfitDatabase db, bool vanilla = true)
		{
			if (vanilla)
			{
				ConfigureOutfit(
					MakeOutfit(db, "Anything", true),
					StatDefOfHelper.MakeDict(
						(StatDefOf_Rimworld.MoveSpeed, 1f),
						(StatDefOf_Rimworld.WorkSpeedGlobal, 2f),
						(StatDefOf_Rimworld.ArmorRating_Blunt, 1f),
						(StatDefOf_Rimworld.ArmorRating_Sharp, 1f)
					)
				);

				ConfigureOutfitWorker(
					MakeOutfit(db, "Worker", true),
					StatDefOfHelper.MakeDict(
						(StatDefOf_Rimworld.MoveSpeed, 0f),
						(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
					)
				);
			}

			ConfigureOutfitWorker(
				MakeOutfit(db, "Doctor"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MedicalSurgerySuccessChance, 2f),
					(StatDefOf_Rimworld.MedicalOperationSpeed, 2f),
					(StatDefOf_Rimworld.MedicalTendQuality, 2f),
					(StatDefOf_Rimworld.MedicalTendSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Warden"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.NegotiationAbility, 2f),
					(StatDefOf_Rimworld.SocialImpact, 1f),
					(StatDefOf_Rimworld.TradePriceImprovement, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Handler"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.TrainAnimalChance, 2f),
					(StatDefOf_Rimworld.TameAnimalChance, 2f),
					(StatDefOf_Rimworld.ArmorRating_Sharp, 0f),
					(StatDefOf_Rimworld.MeleeDodgeChance, 1f),
					(StatDefOf_Rimworld.MeleeHitChance, 0f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.MeleeDPS, 0f),
					(StatDefOf_Rimworld.AccuracyTouch, 0f),
					(StatDefOf_Rimworld.MeleeWeapon_CooldownMultiplier, -2f),
					(StatDefOf_Rimworld.MeleeWeapon_DamageMultiplier, 0f),
					(StatDefOf_Rimworld.PainShockThreshold, 2f),
					(StatDefOf_Rimworld.AnimalGatherYield, 2f),
					(StatDefOf_Rimworld.AnimalGatherSpeed, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Cook"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.DrugCookingSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryFleshSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryFleshEfficiency, 2f),
					(StatDefOf_Rimworld.CookSpeed, 2f),
					(StatDefOf_Rimworld.FoodPoisonChance, -2f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitSoldier(
				MakeOutfit(db, "Hunter"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.ShootingAccuracyPawn, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.AccuracyShort, 1f),
					(StatDefOf_Rimworld.AccuracyMedium, 1f),
					(StatDefOf_Rimworld.AccuracyLong, 1f),
					(StatDefOf_Rimworld.MeleeDPS, 0f),
					(StatDefOf_Rimworld.MeleeHitChance, 0f),
					(StatDefOf_Rimworld.ArmorRating_Blunt, 0f),
					(StatDefOf_Rimworld.ArmorRating_Sharp, 0f),
					(StatDefOf_Rimworld.RangedWeapon_Cooldown, -2f),
					(StatDefOf_Rimworld.AimingDelayFactor, -2f),
					(StatDefOf_Rimworld.PainShockThreshold, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Builder"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.FixBrokenDownBuildingSuccessChance, 2f),
					(StatDefOf_Rimworld.ConstructionSpeed, 2f),
					(StatDefOf_Rimworld.ConstructSuccessChance, 2f),
					(StatDefOf_Rimworld.SmoothingSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 0f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Grower"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.PlantHarvestYield, 2f),
					(StatDefOf_Rimworld.PlantWorkSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Miner"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MiningYield, 2f),
					(StatDefOf_Rimworld.MiningSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Smith"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Tailor"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Artist"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Crafter"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryMechanoidSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryMechanoidEfficiency, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Hauler"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 2f),
					(StatDefOf_Rimworld.CarryingCapacity, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Cleaner"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			);

			ConfigureOutfitWorker(
				MakeOutfit(db, "Researcher"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.ResearchSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			);

			ConfigureOutfitSoldier(
				MakeOutfit(db, "Brawler"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 2f),
					(StatDefOf_Rimworld.AimingDelayFactor, -2f),
					(StatDefOf_Rimworld.MeleeDPS, 2f),
					(StatDefOf_Rimworld.MeleeHitChance, 2f),
					(StatDefOf_Rimworld.MeleeDodgeChance, 2f),
					(StatDefOf_Rimworld.ArmorRating_Blunt, 0f),
					(StatDefOf_Rimworld.ArmorRating_Sharp, 1f),
					(StatDefOf_Rimworld.AccuracyTouch, 2f),
					(StatDefOf_Rimworld.MeleeWeapon_DamageMultiplier, 2f),
					(StatDefOf_Rimworld.MeleeWeapon_CooldownMultiplier, -2f),
					(StatDefOf_Rimworld.PainShockThreshold, 2f)
				)
			);

			if (!vanilla)
				return;

			ConfigureOutfitSoldier(
				MakeOutfit(db, "Soldier"),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.ShootingAccuracyPawn, 2f),
					(StatDefOf_Rimworld.AccuracyShort, 1f),
					(StatDefOf_Rimworld.AccuracyMedium, 1f),
					(StatDefOf_Rimworld.AccuracyLong, 1f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.ArmorRating_Blunt, 0f),
					(StatDefOf_Rimworld.ArmorRating_Sharp, 1f),
					(StatDefOf_Rimworld.MeleeDodgeChance, 0f),
					(StatDefOf_Rimworld.AimingDelayFactor, -2f),
					(StatDefOf_Rimworld.RangedWeapon_Cooldown, -2f),
					(StatDefOf_Rimworld.PainShockThreshold, 2f)
				)
			);

			ConfigureOutfitNudist(
				MakeOutfit(db, "Nudist", true),
				StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			);

		}

		private static ExtendedOutfit MakeOutfit(
			OutfitDatabase database,
			string name,
			bool autoWorkPriorities = false,
			bool autoTemp = true)
		{
			ExtendedOutfit extendedOutfit = database.MakeNewOutfit() as ExtendedOutfit;
			extendedOutfit.label = ("Outfit" + name).Translate();
			extendedOutfit.AutoWorkPriorities = autoWorkPriorities;
			extendedOutfit.AutoTemp = autoTemp;
			return extendedOutfit;
		}

		private static void ConfigureOutfit(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			//outfit.AddRange(priorities.Select(i => new StatPriority(i.Key, i.Value, i.Value)));
			outfit.AddRange(priorities.Select(i => new StatPriority(i.Key, i.Value)));      // Leave stats unlocked (can be deleted).
		}

		private static void ConfigureOutfitFiltered(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities,
			Func<ThingDef, bool> filter)
		{
			outfit.filter.SetDisallowAll();
			outfit.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(filter))
				outfit.filter.SetAllow(thingDef, true);
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfit(outfit, priorities);
		}

		private static void ConfigureOutfitTagged(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities,
			string tag)
		{
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(
				outfit,
				priorities,
				d => d.apparel?.defaultOutfitTags?.Contains(tag) ?? false);
		}

		private static void ConfigureOutfitWorker(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitTagged(outfit, priorities, "Worker");
		}

		private static void ConfigureOutfitSoldier(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitTagged(outfit, priorities, "Soldier");
		}

		private static void ConfigureOutfitNudist(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			BodyPartGroupDef[] forbid = new BodyPartGroupDef[2]
			{
				BodyPartGroupDefOf.Legs,
				BodyPartGroupDefOf.Torso
			};
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(
				outfit,
				priorities,
				d =>
				{
					ApparelProperties apparel = d.apparel;
					return apparel != null &&
						apparel.bodyPartGroups.All(g => !forbid.Contains(g));
				});
		}
	}
}
