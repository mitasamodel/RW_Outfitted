using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Verse;

namespace Outfitted
{
	[HarmonyPatch(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.ApparelScoreRaw))]
	internal static class JobGiver_OptimizeApparel_ApparelScoreRaw_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo add = AccessTools.Method(typeof(JobGiver_OptimizeApparel_ApparelScoreRaw_Patch), "ApparelScoreExtra", null, null);
			MethodInfo find = AccessTools.PropertyGetter(typeof(Thing), "Stuff");
			FieldInfo fld = AccessTools.Field(typeof(JobGiver_OptimizeApparel), "neededWarmth");
			foreach (CodeInstruction ins in instructions)
			{
				if (ins.opcode == OpCodes.Callvirt && find.Equals(ins.operand))     // if (ap.Stuff == ThingDefOf.Human.race.leatherDef)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);    // push Pawn
					yield return new CodeInstruction(OpCodes.Ldsfld, fld);      // push neededWarmth
					yield return new CodeInstruction(OpCodes.Call, add);        // call ApparelScoreExtra
					yield return new CodeInstruction(OpCodes.Stloc_0, null);    // store result in "num" (overwrite)
					yield return new CodeInstruction(OpCodes.Ldarg_1, null);    // push ap (was previously on the stack; will be used by original code futher)
				}
				yield return ins;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float ApparelScoreExtra(Apparel ap, Pawn pawn, NeededWarmth neededWarmth)
		{
			bool whatIfNotWorn = PawnContext.GetWhatIfNotWorn(pawn);
			return Outfitted.ApparelScoreExtra(pawn, ap, neededWarmth, whatIfNotWorn);
		}
	}
}
