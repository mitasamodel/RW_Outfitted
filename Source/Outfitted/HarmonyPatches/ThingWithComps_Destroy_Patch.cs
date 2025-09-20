using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted.HarmonyPatches
{
	[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.Destroy))]
	internal static class ThingWithComps_Destroy_Patch
	{
		internal static void Postfix(ThingWithComps __instance)
		{
			if ( __instance is Apparel apparel)
				StatWorkerOutfitted.ClearCacheFor(apparel);
		}
	}
}
