using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;

namespace Outfitted.HarmonyPatches
{
	[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetBaseValueFor))]
	internal static class StatWorker_GetBaseValueFor_Patch
	{
		/// <summary>
		/// Remove defaultBaseValue if it is for us.
		/// </summary>
		internal static bool Prefix(ref float __result, StatRequest request, StatDef ___stat)
		{
			// Check if this request is for thing.
			if (!request.HasThing) return true;
			// Apparel.
			if (!(request.Thing is Apparel)) return true;
			// Check if it is our request.
			if (!ApparelScorePriorities.GetStatForApparelIdNoDefault.Contains((request.Thing as Apparel, ___stat))) return true;

			// If there are StatBases.
			if (request.StatBases != null)
			{
				for (int i = 0; i < request.StatBases.Count; i++)
				{
					var sb = request.StatBases[i];
					if (sb.stat == ___stat)
					{
						__result = sb.value;
						return false;
					}
				}
			}

			// Otherwise zero.
			__result = 0f;
			return false;
		}
	}
}
