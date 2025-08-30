using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Outfitted
{
	[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
	public static class JobGiver_OptimizeApparel_TryGiveJob_Patch
	{
		public static void Postfix(JobGiver_OptimizeApparel __instance, Pawn pawn, ref Job __result)
		{
			ApparelPolicy currentApparelPolicy = pawn.outfits.CurrentApparelPolicy;
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			bool inVacuum = PawnIsInVacuum(pawn);



			//Job job2 = JobMaker.MakeJob(JobDefOf.RemoveApparel, apparel);
			//job2.haulDroppedApparel = true;
			//return job2;
		}

		private static bool PawnIsInVacuum(Pawn pawn)
		{
			return pawn.MapHeld.Biome.inVacuum && pawn.Position.GetVacuum(pawn.MapHeld) >= 0.5f;
		}

		private static bool ApparelForVacuum(Apparel apparel)
		{
			if (StatDefOf_Rimworld.VacuumResistance == null) return false;
			return apparel.GetStatValue(StatDefOf_Rimworld.VacuumResistance, applyPostProcess: true, 60) > 0f;
		}
	}
}
