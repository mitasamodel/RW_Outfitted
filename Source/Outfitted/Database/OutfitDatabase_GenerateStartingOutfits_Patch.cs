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
		private static void Postfix(OutfitDatabase __instance)
		{
			if (OutfittedMod.Settings.generateStartingOutfits)
			{
				try
				{
					StandardOutfits.EntriesFreshStart();
					StandardOutfits.GenerateStartingOutfits(__instance);
				}
				catch (Exception ex)
				{
					Log.Error("Can't generate outfits: " + ex?.ToString());
				}
			}
		}
	}
}
