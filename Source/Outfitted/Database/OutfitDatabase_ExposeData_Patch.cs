using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Outfitted.RW_JustUtils;

namespace Outfitted
{
	[HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.ExposeData), MethodType.Normal)]
	internal static class OutfitDatabase_ExposeData_Patch
	{
		private static void Postfix(OutfitDatabase __instance, List<ApparelPolicy> ___outfits)
		{
			//if (Scribe.mode != LoadSaveMode.LoadingVars || ___outfits.Any(i => i is ExtendedOutfit))
			//	return;

			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				bool flag = false;
				int cnt = 0;
				StandardOutfits.EntriesFreshStart();

				// Convert vanilla outfits.
				for (int i = 0; i < ___outfits.Count; i++)
				{
					if (!(___outfits[i] is ExtendedOutfit))
					{
						___outfits[i] = DBHelpers.ConvertVanillaOutfit(___outfits[i]);
						DBHelpers.AddBasicStats(___outfits[i] as ExtendedOutfit);
						cnt++;
						flag = true;
					}
				}
#if DEBUG
				LoggerMy.LogNL($"[OutfitDatabase_ExposeData_Patch] Game loaded. Converted {cnt} vanilla outfits.");
#endif

				// Generate additional starting outfits if there were vanilla outfits.
				if (flag && OutfittedMod.Settings.generateStartingOutfits)
				{
#if DEBUG
					LoggerMy.LogNL("[OutfitDatabase_ExposeData_Patch] Generate starting outfits.");
#endif
					StandardOutfits.GenerateStartingOutfits(__instance, false);
				}
			}
		}
	}
}
