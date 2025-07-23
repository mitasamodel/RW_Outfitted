// Decompiled with JetBrains decompiler
// Type: Outfitted.Database.OutfitDatabase_GenerateStartingOutfits_Patch
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

#nullable disable
namespace Outfitted.Database
{
  [HarmonyPatch(typeof (OutfitDatabase), "GenerateStartingOutfits")]
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
            Outfitted.StatDefOf.MoveSpeed,
            1f
          },
          {
            Outfitted.StatDefOf.WorkSpeedGlobal,
            2f
          },
          {
            Outfitted.StatDefOf.ArmorRating_Blunt,
            1f
          },
          {
            Outfitted.StatDefOf.ArmorRating_Sharp,
            1f
          }
        });
        OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Worker", true), new Dictionary<StatDef, float>()
        {
          {
            Outfitted.StatDefOf.MoveSpeed,
            0.0f
          },
          {
            Outfitted.StatDefOf.WorkSpeedGlobal,
            1f
          }
        });
      }
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Doctor"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MedicalSurgerySuccessChance,
          2f
        },
        {
          Outfitted.StatDefOf.MedicalOperationSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.MedicalTendQuality,
          2f
        },
        {
          Outfitted.StatDefOf.MedicalTendSpeed,
          1f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Warden"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.NegotiationAbility,
          2f
        },
        {
          Outfitted.StatDefOf.SocialImpact,
          1f
        },
        {
          Outfitted.StatDefOf.TradePriceImprovement,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Handler"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.TrainAnimalChance,
          2f
        },
        {
          Outfitted.StatDefOf.TameAnimalChance,
          2f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Sharp,
          0.0f
        },
        {
          Outfitted.StatDefOf.MeleeDodgeChance,
          1f
        },
        {
          Outfitted.StatDefOf.MeleeHitChance,
          0.0f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          0.0f
        },
        {
          Outfitted.StatDefOf.MeleeDPS,
          0.0f
        },
        {
          Outfitted.StatDefOf.AccuracyTouch,
          0.0f
        },
        {
          Outfitted.StatDefOf.MeleeWeapon_CooldownMultiplier,
          -2f
        },
        {
          Outfitted.StatDefOf.MeleeWeapon_DamageMultiplier,
          0.0f
        },
        {
          Outfitted.StatDefOf.PainShockThreshold,
          2f
        },
        {
          Outfitted.StatDefOf.AnimalGatherYield,
          2f
        },
        {
          Outfitted.StatDefOf.AnimalGatherSpeed,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Cook"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.DrugCookingSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.ButcheryFleshSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.ButcheryFleshEfficiency,
          2f
        },
        {
          Outfitted.StatDefOf.CookSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.FoodPoisonChance,
          -2f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          1f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Hunter"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.ShootingAccuracyPawn,
          2f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyShort,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyMedium,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyLong,
          1f
        },
        {
          Outfitted.StatDefOf.MeleeDPS,
          0.0f
        },
        {
          Outfitted.StatDefOf.MeleeHitChance,
          0.0f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Blunt,
          0.0f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Sharp,
          0.0f
        },
        {
          Outfitted.StatDefOf.RangedWeapon_Cooldown,
          -2f
        },
        {
          Outfitted.StatDefOf.AimingDelayFactor,
          -2f
        },
        {
          Outfitted.StatDefOf.PainShockThreshold,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Builder"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.FixBrokenDownBuildingSuccessChance,
          2f
        },
        {
          Outfitted.StatDefOf.ConstructionSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.ConstructSuccessChance,
          2f
        },
        {
          Outfitted.StatDefOf.SmoothingSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          0.0f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          0.0f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Grower"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.PlantHarvestYield,
          2f
        },
        {
          Outfitted.StatDefOf.PlantWorkSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          0.0f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Miner"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MiningYield,
          2f
        },
        {
          Outfitted.StatDefOf.MiningSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          0.0f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Smith"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.GeneralLaborSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Tailor"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.GeneralLaborSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Artist"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.GeneralLaborSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Crafter"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.GeneralLaborSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.ButcheryMechanoidSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.ButcheryMechanoidEfficiency,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Hauler"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MoveSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.CarryingCapacity,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Cleaner"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MoveSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitWorker(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Researcher"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.ResearchSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
          1f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Brawler"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MoveSpeed,
          2f
        },
        {
          Outfitted.StatDefOf.AimingDelayFactor,
          -2f
        },
        {
          Outfitted.StatDefOf.MeleeDPS,
          2f
        },
        {
          Outfitted.StatDefOf.MeleeHitChance,
          2f
        },
        {
          Outfitted.StatDefOf.MeleeDodgeChance,
          2f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Blunt,
          0.0f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Sharp,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyTouch,
          2f
        },
        {
          Outfitted.StatDefOf.MeleeWeapon_DamageMultiplier,
          2f
        },
        {
          Outfitted.StatDefOf.MeleeWeapon_CooldownMultiplier,
          -2f
        },
        {
          Outfitted.StatDefOf.PainShockThreshold,
          2f
        }
      });
      if (!vanilla)
        return;
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitSoldier(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Soldier"), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.ShootingAccuracyPawn,
          2f
        },
        {
          Outfitted.StatDefOf.AccuracyShort,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyMedium,
          1f
        },
        {
          Outfitted.StatDefOf.AccuracyLong,
          1f
        },
        {
          Outfitted.StatDefOf.MoveSpeed,
          1f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Blunt,
          0.0f
        },
        {
          Outfitted.StatDefOf.ArmorRating_Sharp,
          1f
        },
        {
          Outfitted.StatDefOf.MeleeDodgeChance,
          0.0f
        },
        {
          Outfitted.StatDefOf.AimingDelayFactor,
          -2f
        },
        {
          Outfitted.StatDefOf.RangedWeapon_Cooldown,
          -2f
        },
        {
          Outfitted.StatDefOf.PainShockThreshold,
          2f
        }
      });
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitNudist(OutfitDatabase_GenerateStartingOutfits_Patch.MakeOutfit(db, "Nudist", true), new Dictionary<StatDef, float>()
      {
        {
          Outfitted.StatDefOf.MoveSpeed,
          1f
        },
        {
          Outfitted.StatDefOf.WorkSpeedGlobal,
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
      extendedOutfit.label = (string) ("Outfit" + name).Translate();
      extendedOutfit.AutoWorkPriorities = autoWorkPriorities;
      extendedOutfit.AutoTemp = autoTemp;
      return extendedOutfit;
    }

    private static void ConfigureOutfit(
      ExtendedOutfit outfit,
      Dictionary<StatDef, float> priorities)
    {
      outfit.AddRange(priorities.Select<KeyValuePair<StatDef, float>, StatPriority>((Func<KeyValuePair<StatDef, float>, StatPriority>) (i => new StatPriority(i.Key, i.Value, i.Value))));
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
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(outfit, priorities, (Func<ThingDef, bool>) (d => d.apparel?.defaultOutfitTags?.Contains(tag) ?? false));
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
      OutfitDatabase_GenerateStartingOutfits_Patch.ConfigureOutfitFiltered(outfit, priorities, (Func<ThingDef, bool>) (d =>
      {
        ApparelProperties apparel = d.apparel;
        return apparel != null && apparel.bodyPartGroups.All<BodyPartGroupDef>((Func<BodyPartGroupDef, bool>) (g => !((IEnumerable<BodyPartGroupDef>) forbid).Contains<BodyPartGroupDef>(g)));
      }));
    }
  }
}
