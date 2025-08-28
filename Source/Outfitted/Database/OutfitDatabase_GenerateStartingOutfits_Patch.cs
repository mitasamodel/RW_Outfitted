using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Outfitted.Database
{
	[HarmonyPatch(typeof(OutfitDatabase), "GenerateStartingOutfits")]
	public static class OutfitDatabase_GenerateStartingOutfits_Patch
	{
		private static bool Prefix(OutfitDatabase __instance)
		{
			try
			{
				OutfitDatabase_GenerateStartingOutfits_Patch.GenerateStartingOutfits(__instance);
			}
			catch (Exception ex)
			{
				Log.Error("Can't generate outfits: " + ex?.ToString());
			}
			return false;
		}

		internal static void GenerateStartingOutfits(OutfitDatabase db, bool vanilla = true)
		{
			if (vanilla)
			{
				OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfit(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Anything", true), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MoveSpeed,
1f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
2f
},
{
global::Outfitted.StatDefOf.ArmorRating_Blunt,
1f
},
{
global::Outfitted.StatDefOf.ArmorRating_Sharp,
1f
}
});
				OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Worker", true), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MoveSpeed,
0.0f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			}
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Doctor"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MedicalSurgerySuccessChance,
2f
},
{
global::Outfitted.StatDefOf.MedicalOperationSpeed,
2f
},
{
global::Outfitted.StatDefOf.MedicalTendQuality,
2f
},
{
global::Outfitted.StatDefOf.MedicalTendSpeed,
1f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Warden"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.NegotiationAbility,
2f
},
{
global::Outfitted.StatDefOf.SocialImpact,
1f
},
{
global::Outfitted.StatDefOf.TradePriceImprovement,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Handler"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.TrainAnimalChance,
2f
},
{
global::Outfitted.StatDefOf.TameAnimalChance,
2f
},
{
global::Outfitted.StatDefOf.ArmorRating_Sharp,
0.0f
},
{
global::Outfitted.StatDefOf.MeleeDodgeChance,
1f
},
{
global::Outfitted.StatDefOf.MeleeHitChance,
0.0f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
0.0f
},
{
global::Outfitted.StatDefOf.MeleeDPS,
0.0f
},
{
global::Outfitted.StatDefOf.AccuracyTouch,
0.0f
},
{
global::Outfitted.StatDefOf.MeleeWeapon_CooldownMultiplier,
-2f
},
{
global::Outfitted.StatDefOf.MeleeWeapon_DamageMultiplier,
0.0f
},
{
global::Outfitted.StatDefOf.PainShockThreshold,
2f
},
{
global::Outfitted.StatDefOf.AnimalGatherYield,
2f
},
{
global::Outfitted.StatDefOf.AnimalGatherSpeed,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Cook"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.DrugCookingSpeed,
2f
},
{
global::Outfitted.StatDefOf.ButcheryFleshSpeed,
2f
},
{
global::Outfitted.StatDefOf.ButcheryFleshEfficiency,
2f
},
{
global::Outfitted.StatDefOf.CookSpeed,
2f
},
{
global::Outfitted.StatDefOf.FoodPoisonChance,
-2f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
1f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Hunter"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.ShootingAccuracyPawn,
2f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
1f
},
{
global::Outfitted.StatDefOf.AccuracyShort,
1f
},
{
global::Outfitted.StatDefOf.AccuracyMedium,
1f
},
{
global::Outfitted.StatDefOf.AccuracyLong,
1f
},
{
global::Outfitted.StatDefOf.MeleeDPS,
0.0f
},
{
global::Outfitted.StatDefOf.MeleeHitChance,
0.0f
},
{
global::Outfitted.StatDefOf.ArmorRating_Blunt,
0.0f
},
{
global::Outfitted.StatDefOf.ArmorRating_Sharp,
0.0f
},
{
global::Outfitted.StatDefOf.RangedWeapon_Cooldown,
-2f
},
{
global::Outfitted.StatDefOf.AimingDelayFactor,
-2f
},
{
global::Outfitted.StatDefOf.PainShockThreshold,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Builder"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.FixBrokenDownBuildingSuccessChance,
2f
},
{
global::Outfitted.StatDefOf.ConstructionSpeed,
2f
},
{
global::Outfitted.StatDefOf.ConstructSuccessChance,
2f
},
{
global::Outfitted.StatDefOf.SmoothingSpeed,
2f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
0.0f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
0.0f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Grower"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.PlantHarvestYield,
2f
},
{
global::Outfitted.StatDefOf.PlantWorkSpeed,
2f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
0.0f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Miner"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MiningYield,
2f
},
{
global::Outfitted.StatDefOf.MiningSpeed,
2f
},
{
global::Outfitted.StatDefOf.MoveSpeed,
0.0f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Smith"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.GeneralLaborSpeed,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Tailor"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.GeneralLaborSpeed,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Artist"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.GeneralLaborSpeed,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Crafter"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.GeneralLaborSpeed,
2f
},
{
global::Outfitted.StatDefOf.ButcheryMechanoidSpeed,
2f
},
{
global::Outfitted.StatDefOf.ButcheryMechanoidEfficiency,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Hauler"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MoveSpeed,
2f
},
{
global::Outfitted.StatDefOf.CarryingCapacity,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Cleaner"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MoveSpeed,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
2f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Researcher"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.ResearchSpeed,
2f
},
{
global::Outfitted.StatDefOf.WorkSpeedGlobal,
1f
}
});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Brawler"), new Dictionary<StatDef, float>()
{
{
global::Outfitted.StatDefOf.MoveSpeed,
2f
},
{
global::Outfitted.StatDefOf.AimingDelayFactor,
-2f
},
{
global::Outfitted.StatDefOf.MeleeDPS,
2f
},
{
global::Outfitted.StatDefOf.MeleeHitChance,
2f
},
{
global::Outfitted.StatDefOf.MeleeDodgeChance,
2f
},
{
global::Outfitted.StatDefOf.ArmorRating_Blunt,
0.0f
},
{
global::Outfitted.StatDefOf.ArmorRating_Sharp,
1f
},
{
global::Outfitted.StatDefOf.AccuracyTouch,
2f
},
{
global::Outfitted.StatDefOf.MeleeWeapon_DamageMultiplier,
2f
},
{
global::Outfitted.StatDefOf.MeleeWeapon_CooldownMultiplier,
-2f
},
{
global::Outfitted.StatDefOf.PainShockThreshold,
2f
}
});
			if (!vanilla)
				return;
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Soldier"), new Dictionary<StatDef, float>()
			{
				{
					global::Outfitted.StatDefOf.ShootingAccuracyPawn,
					2f
				},
				{
					global::Outfitted.StatDefOf.AccuracyShort,
					1f
				},
				{
					global::Outfitted.StatDefOf.AccuracyMedium,
					1f
				},
				{
					global::Outfitted.StatDefOf.AccuracyLong,
					1f
				},
				{
					global::Outfitted.StatDefOf.MoveSpeed,
					1f
				},
				{
					global::Outfitted.StatDefOf.ArmorRating_Blunt,
					0.0f
				},
				{
					global::Outfitted.StatDefOf.ArmorRating_Sharp,
					1f
				},
				{
					global::Outfitted.StatDefOf.MeleeDodgeChance,
					0.0f
				},
				{
					global::Outfitted.StatDefOf.AimingDelayFactor,
					-2f
				},
				{
					global::Outfitted.StatDefOf.RangedWeapon_Cooldown,
					-2f
				},
				{
					global::Outfitted.StatDefOf.PainShockThreshold,
					2f
				}
			});
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitNudist(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Nudist", true), new Dictionary<StatDef, float>()
			{
				{
					global::Outfitted.StatDefOf.MoveSpeed,
					1f
				},
				{
					global::Outfitted.StatDefOf.WorkSpeedGlobal,
					2f
				}
			});
		}

		private static ExtendedOutfit MakeOutfit(
		OutfitDatabase database,
		string name,
		bool autoWorkPriorities = false,
		bool autoTemp = true)
		{
			ExtendedOutfit extendedOutfit = database.MakeNewOutfit() as ExtendedOutfit;
			extendedOutfit.label = (string)("Outfit" + name).Translate();
			extendedOutfit.AutoWorkPriorities = autoWorkPriorities;
			extendedOutfit.AutoTemp = autoTemp;
			return extendedOutfit;
		}

		private static void ConfigureOutfit(
		ExtendedOutfit outfit,
		Dictionary<StatDef, float> priorities)
		{
			outfit.AddRange(priorities.Select<KeyValuePair<StatDef, float>, StatPriority>((Func<KeyValuePair<StatDef, float>, StatPriority>)(i => new StatPriority(i.Key, i.Value, i.Value))));
		}

		private static void ConfigureOutfitFiltered(
		ExtendedOutfit outfit,
		Dictionary<StatDef, float> priorities,
		Func<ThingDef, bool> filter)
		{
			outfit.filter.SetDisallowAll();
			outfit.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, false);
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where<ThingDef>(filter))
				outfit.filter.SetAllow(thingDef, true);
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfit(outfit, priorities);
		}

		private static void ConfigureOutfitTagged(
		ExtendedOutfit outfit,
		Dictionary<StatDef, float> priorities,
		string tag)
		{
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(outfit, priorities, (Func<ThingDef, bool>)(d => d.apparel?.defaultOutfitTags?.Contains(tag) ?? false));
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
			OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(outfit, priorities, (Func<ThingDef, bool>)(d =>
			{
				ApparelProperties apparel = d.apparel;
				return apparel != null && apparel.bodyPartGroups.All<BodyPartGroupDef>((Func<BodyPartGroupDef, bool>)(g => !((IEnumerable<BodyPartGroupDef>)forbid).Contains<BodyPartGroupDef>(g)));
			}));
		}
	}
}
