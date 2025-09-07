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
			if (Scribe.mode != LoadSaveMode.LoadingVars || ___outfits.Any(i => i is ExtendedOutfit))
				return;

			// Convert vanilla outfits.
			foreach (ApparelPolicy outfit in ___outfits.ToList())
			{
				___outfits.Remove(outfit);
				___outfits.Add(Helpers.GenerateBasedOnKnownVanillaOutfits(outfit));
			}

			// Generate additional starting outfits.
			if (OutfittedMod.Settings.generateStartingOutfits)
				OutfitDatabase_GenerateStartingOutfits_Patch.GenerateStartingOutfits(__instance, false);
		}
	}
}
