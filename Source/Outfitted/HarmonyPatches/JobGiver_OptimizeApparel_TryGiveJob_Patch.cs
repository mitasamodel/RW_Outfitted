using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Outfitted
{
	[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
	public static class JobGiver_OptimizeApparel_TryGiveJob_Patch
	{
		private static readonly MethodInfo List_Clear =
			AccessTools.Method(typeof(List<float>), nameof(List<float>.Clear));

		private static readonly MethodInfo ApparelScoreRaw =
			AccessTools.Method(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.ApparelScoreRaw));

		private static readonly MethodInfo BuildWornScore =
			AccessTools.Method(typeof(Outfitted), nameof(Outfitted.BuildWornScore));

		private static readonly FieldInfo WornScoresField =
		   AccessTools.Field(typeof(JobGiver_OptimizeApparel), "wornApparelScores");

		private static readonly MethodInfo List_GetItem =
			AccessTools.PropertyGetter(typeof(List<Apparel>), "Item");

		private static readonly MethodInfo List_AddSingle =
			AccessTools.Method(typeof(List<float>), nameof(List<float>.Add));

		/// <summary>
		/// Replace worn score calculation by our method.
		/// </summary>
		/// <param name="instructions"></param>
		/// <param name="generator"></param>
		/// <returns></returns>
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var matcher = new CodeMatcher(instructions, generator);

			// Find wornApparelScores.Clear()
			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Ldsfld, WornScoresField),     // JobGiver_OptimizeApparel.wornApparelScores
				new CodeMatch(OpCodes.Callvirt, List_Clear))   // List<float>.Clear()
				.ThrowIfInvalid("Can not find wornApparelScores.Clear()");

			// Remove both instructions.
			matcher.RemoveInstructions(2);

			// Insert out method: BuildWornScore
			matcher.Insert(
				new CodeInstruction(OpCodes.Ldarg_1),                   // pawn
				new CodeInstruction(OpCodes.Call, BuildWornScore),      // call helper
				new CodeInstruction(OpCodes.Stsfld, WornScoresField)    // wornApparelScores (private static)
			);

			// Find loop initialization: int i = 0
			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Ldc_I4_0),        // push 0 to the stack
				new CodeMatch(ci => ci.opcode == OpCodes.Stloc || ci.opcode == OpCodes.Stloc_S))   // i = 0
				.ThrowIfInvalid("Can not find the beginning of the loop");
			int loopStart = matcher.Pos;

			// Find loop termination
			matcher.MatchStartForward(
				new CodeMatch(ci => ci.opcode == OpCodes.Blt || ci.opcode == OpCodes.Blt_S))    // jump
				.ThrowIfInvalid("Can not find the end of the loop");
			int loopEnd = matcher.Pos;

			// Remove the whole loop
			matcher.RemoveInstructionsInRange(loopStart, loopEnd);

			return matcher.InstructionEnumeration();

			//if (matcher.IsValid)
			//{
			//	Log.Message($"[MyMod] Found instruction at index {matcher.Pos}: {matcher.Instruction}");
			//}

			//// Find the loop: wornApparelScores.Add(ApparelScoreRaw(pawn, wornApparel[i]))
			//matcher.MatchStartForward(
			//	new CodeMatch(OpCodes.Ldsfld, WornScoresField),     // wornApparelScores
			//	new CodeMatch(OpCodes.Ldarg_1),                     // pawn
			//	new CodeMatch(ci => ci.opcode == OpCodes.Ldloc_2 || ci.opcode == OpCodes.Ldloc_S), // wornApparel local
			//	new CodeMatch(ci => ci.opcode == OpCodes.Ldloc || ci.opcode == OpCodes.Ldloc_S),    // i
			//	new CodeMatch(OpCodes.Callvirt, List_GetItem),   // wornApparel[i]
			//	new CodeMatch(OpCodes.Call, ApparelScoreRaw),   // ApparelScoreRaw(pawn, item)
			//	new CodeMatch(OpCodes.Callvirt, List_AddSingle))        // wornApparelScores.Add(score)
			//	.ThrowIfInvalid("Could not find worn loop");
		}



		public static void Postfix(JobGiver_OptimizeApparel __instance, Pawn pawn, ref Job __result)
		{
			// Vanilla has assigned a job already.
			if (__result != null) return;

			// Mirror vanilla simple checks.
			if (pawn?.outfits == null ||
				pawn.Faction != Faction.OfPlayer ||
				(pawn.IsMutant && pawn.mutant.Def.disableApparel) ||
				pawn.IsQuestLodger()) return;

			// Worn apparel.
			List<Apparel> worn = pawn?.apparel?.WornApparel;
			if (worn == null || worn.Count == 0) return;

			// Outfitted policy.
			if (!(pawn.outfits.CurrentApparelPolicy is ExtendedOutfit currentPolicy)) return;

			// Candidate to be removed.
			Apparel candidate = null;
			float massCandidate = float.MinValue;

			foreach (var ap in worn)
			{
				// Respect policy & vanilla constraints for auto-dropping.
				// These handled by vanilla.
				if (!currentPolicy.filter.Allows(ap)) continue;
				if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(ap)) continue;
				if (pawn.apparel.IsLocked(ap)) continue;

				// If in vacuum, skip anything providing vacuum protection.
				if (PawnIsInVacuum(pawn) && ProtectsFromVacuum(ap)) continue;

				// Check worn scores. All positive stay.
				float score = CacheWornApparel.GetScore(pawn, ap);
				if (score > 0f) continue;

				// Select the heaviest.
				float mass = ap.GetStatValue(StatDefOf_Rimworld.Mass, true);
				if ( mass > massCandidate )
				{
					massCandidate = mass;
					candidate = ap;
				}
			}

			// Issue a Job.
			if ( candidate != null )
			{
				Job job = JobMaker.MakeJob(JobDefOf.RemoveApparel, candidate);
				job.haulDroppedApparel = true;
				__result = job;
			}
		}

		private static bool PawnIsInVacuum(Pawn pawn)
		{
			return pawn.MapHeld.Biome.inVacuum && pawn.Position.GetVacuum(pawn.MapHeld) >= 0.5f;
		}

		private static bool ProtectsFromVacuum(Apparel apparel)
		{
			if (StatDefOf_Rimworld.VacuumResistance == null) return false;
			return apparel.GetStatValue(StatDefOf_Rimworld.VacuumResistance, applyPostProcess: true, 60) > 0f;
		}


	}
}
