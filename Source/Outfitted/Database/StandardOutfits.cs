using HarmonyLib;
using Outfitted.Database;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace Outfitted
{
	public class StandardOutfitEntry
	{
		public string Label;
		public string ApparelTag = null;
		public bool AutoWorkPrio = false;
		public Dictionary<StatDef, float> StatsWeight = new Dictionary<StatDef, float>();
		public bool Used = false;
	}

	// On top of these the "Basic" is always applied (DBHelpers.AddBasicStats).
	internal static class StandardOutfits
	{
		private static readonly List<StandardOutfitEntry> _standardOutfits = new List<StandardOutfitEntry>()
		{
			new StandardOutfitEntry()
			{
				Label = "Slave",
				ApparelTag = "Slave",
				AutoWorkPrio = true,
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.SlaveSuppressionFallRate, -2f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry()
			{
				Label = "Spacefarer",
				ApparelTag = "Spacefarer",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.VacuumResistance, 2.5f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry()
			{
				Label = "Anything",
				AutoWorkPrio = true,
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f),
					(StatDefOf_Rimworld.ArmorRating_Blunt, 1f),
					(StatDefOf_Rimworld.ArmorRating_Sharp, 1f)
				)
			},
			new StandardOutfitEntry()
			{
				Label = "Worker",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 0.5f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry()
			{
				Label = "Doctor",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MedicalSurgerySuccessChance, 2f),
					(StatDefOf_Rimworld.MedicalOperationSpeed, 2f),
					(StatDefOf_Rimworld.MedicalTendQuality, 2f),
					(StatDefOf_Rimworld.MedicalTendSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Warden",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.NegotiationAbility, 2f),
					(StatDefOf_Rimworld.SocialImpact, 1f),
					(StatDefOf_Rimworld.TradePriceImprovement, 2f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Handler",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
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
			},
			new StandardOutfitEntry
			{
				Label = "Cook",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.DrugCookingSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryFleshSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryFleshEfficiency, 2f),
					(StatDefOf_Rimworld.CookSpeed, 2f),
					(StatDefOf_Rimworld.FoodPoisonChance, -2f),
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Hunter",
				ApparelTag = "Soldier",
				StatsWeight = StatDefOfHelper.MakeDict(
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
			},
			new StandardOutfitEntry
			{
				Label = "Builder",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.FixBrokenDownBuildingSuccessChance, 2f),
					(StatDefOf_Rimworld.ConstructionSpeed, 2f),
					(StatDefOf_Rimworld.ConstructSuccessChance, 2f),
					(StatDefOf_Rimworld.SmoothingSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 0f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Grower",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.PlantHarvestYield, 2f),
					(StatDefOf_Rimworld.PlantWorkSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Miner",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MiningYield, 2f),
					(StatDefOf_Rimworld.MiningSpeed, 2f),
					(StatDefOf_Rimworld.MoveSpeed, 0f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Smith",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Tailor",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Artist",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Crafter",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.GeneralLaborSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryMechanoidSpeed, 2f),
					(StatDefOf_Rimworld.ButcheryMechanoidEfficiency, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Hauler",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 2f),
					(StatDefOf_Rimworld.CarryingCapacity, 2f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Cleaner",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Researcher",
				ApparelTag = "Worker",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.ResearchSpeed, 2f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 1f)
				)
			},
			new StandardOutfitEntry
			{
				Label = "Brawler",
				ApparelTag = "Soldier",
				StatsWeight = StatDefOfHelper.MakeDict(
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
			},
			new StandardOutfitEntry
			{
				Label = "Soldier",
				ApparelTag = "Soldier",
				StatsWeight = StatDefOfHelper.MakeDict(
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
			},
			new StandardOutfitEntry
			{
				Label = "Nudist",
				ApparelTag = "Nudist",
				StatsWeight = StatDefOfHelper.MakeDict(
					(StatDefOf_Rimworld.MoveSpeed, 1f),
					(StatDefOf_Rimworld.WorkSpeedGlobal, 2f)
				)
			}
		};

		public static void EntriesFreshStart()
		{
			foreach(var entry in _standardOutfits)
			{
				entry.Used = false;
			}
		}

		/// <summary>
		/// Modify existed outfit.
		/// </summary>
		/// <param name="outfit"></param>
		public static void ApplyStandardOutfit(ExtendedOutfit outfit)
		{
			DBHelpers.AddBasicStats(outfit);
			foreach (var entry in _standardOutfits)
			{
				if (entry.Used) continue;
				if (outfit.label == entry.Label)
				{
#if DEBUG
					Logger.LogNL($"[ApplyStandardOutfit] Existed outfit {entry.Label}. Modifying.");
#endif
					entry.Used = true;
					ApplyEntry(outfit, entry);
				}
			}
		}

		private static void ApplyEntry(ExtendedOutfit outfit, StandardOutfitEntry entry)
		{
			// Filter apparel if applicable.
			if (entry.ApparelTag != null)
			{
				outfit.filter.SetDisallowAll();
				outfit.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
				var apparelDefs = DefDatabase<ThingDef>.AllDefs
					.Where(def => def.apparel?.defaultOutfitTags?.Contains(entry.ApparelTag) ?? false);
				foreach (ThingDef thingDef in apparelDefs)
					outfit.filter.SetAllow(thingDef, true);
			}

			// Auto work priorities.
			outfit.AutoWorkPriorities = entry.AutoWorkPrio;

			// Set stat weights.
			foreach (var stat in entry.StatsWeight)
				outfit.SafeSetStatPriority(stat.Key, stat.Value);
		}

		internal static void GenerateStartingOutfits(OutfitDatabase db, bool vanilla = true)
		{
			if (!(AccessTools.Field(typeof(OutfitDatabase), "outfits")?.GetValue(db) is List<ApparelPolicy> outfits))
			{
				Logger.Log_Error("[GenerateStartingOutfits] Unexpected non-ApparelPolicy field.");
				Verse.Log.Warning("[Outfitted] Please report it to mod author.");
				return;
			}

			if (outfits == null)
			{
				Logger.Log_Error("[GenerateStartingOutfits] Unexpected null outfits list.");
				Verse.Log.Warning("[Outfitted] Please report it to mod author.");
				return;
			}

			// Adjust existed outfits. Match by label.
			for (int i = 0; i < outfits.Count; i++)
			{
				var outfit = outfits[i] as ExtendedOutfit;
				ApplyStandardOutfit(outfit);
			}

			// Create new outfits.
			foreach (var entry in _standardOutfits)
			{
				if (!entry.Used)
				{
#if DEBUG
					Logger.LogNL($"[GenerateStartingOutfits] New outfit {entry.Label}.");
#endif
					var outfit = db.MakeNewOutfit() as ExtendedOutfit;
					outfit.label = entry.Label;
					ApplyEntry(outfit, entry);
				}
			}
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
			ConfigureOutfit(outfit, priorities);
		}

		private static void ConfigureOutfitTagged(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities,
			string tag)
		{
			ConfigureOutfitFiltered(
				outfit,
				priorities,
				d => d.apparel?.defaultOutfitTags?.Contains(tag) ?? false);
		}

		private static void ConfigureOutfitWorker(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			ConfigureOutfitTagged(outfit, priorities, "Worker");
		}

		private static void ConfigureOutfitSoldier(
			ExtendedOutfit outfit,
			Dictionary<StatDef, float> priorities)
		{
			ConfigureOutfitTagged(outfit, priorities, "Soldier");
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
			ConfigureOutfitFiltered(
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
