// Decompiled with JetBrains decompiler
// Type: Outfitted.Database.OutfitDatabase_ExposeData_Patch
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
  [HarmonyPatch(typeof (OutfitDatabase), "ExposeData")]
  internal static class OutfitDatabase_ExposeData_Patch
  {
    private static void Postfix(OutfitDatabase __instance, List<ApparelPolicy> ___outfits)
    {
      if (Scribe.mode != LoadSaveMode.LoadingVars || ___outfits.Any<ApparelPolicy>((Predicate<ApparelPolicy>) (i => i is ExtendedOutfit)))
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
          extendedOutfit.AddRange((IEnumerable<StatPriority>) new List<StatPriority>()
          {
            new StatPriority(Outfitted.StatDefOf.MoveSpeed, 0.0f),
            new StatPriority(Outfitted.StatDefOf.WorkSpeedGlobal, 1f)
          });
          break;
        case "Soldier":
          extendedOutfit.AddRange((IEnumerable<StatPriority>) new List<StatPriority>()
          {
            new StatPriority(Outfitted.StatDefOf.ShootingAccuracyPawn, 2f),
            new StatPriority(Outfitted.StatDefOf.AccuracyShort, 1f),
            new StatPriority(Outfitted.StatDefOf.AccuracyMedium, 1f),
            new StatPriority(Outfitted.StatDefOf.AccuracyLong, 1f),
            new StatPriority(Outfitted.StatDefOf.MoveSpeed, 1f),
            new StatPriority(Outfitted.StatDefOf.ArmorRating_Blunt, 0.0f),
            new StatPriority(Outfitted.StatDefOf.ArmorRating_Sharp, 1f),
            new StatPriority(Outfitted.StatDefOf.MeleeDodgeChance, 0.0f),
            new StatPriority(Outfitted.StatDefOf.AimingDelayFactor, -2f),
            new StatPriority(Outfitted.StatDefOf.RangedWeapon_Cooldown, -2f),
            new StatPriority(Outfitted.StatDefOf.PainShockThreshold, 2f)
          });
          break;
        case "Nudist":
          extendedOutfit.AddRange((IEnumerable<StatPriority>) new List<StatPriority>()
          {
            new StatPriority(Outfitted.StatDefOf.MoveSpeed, 1f),
            new StatPriority(Outfitted.StatDefOf.WorkSpeedGlobal, 2f)
          });
          break;
        default:
          extendedOutfit.AddRange((IEnumerable<StatPriority>) new List<StatPriority>()
          {
            new StatPriority(Outfitted.StatDefOf.MoveSpeed, 1f),
            new StatPriority(Outfitted.StatDefOf.WorkSpeedGlobal, 2f),
            new StatPriority(Outfitted.StatDefOf.ArmorRating_Blunt, 1f),
            new StatPriority(Outfitted.StatDefOf.ArmorRating_Sharp, 1f)
          });
          break;
      }
      return (ApparelPolicy) extendedOutfit;
    }
  }
}
