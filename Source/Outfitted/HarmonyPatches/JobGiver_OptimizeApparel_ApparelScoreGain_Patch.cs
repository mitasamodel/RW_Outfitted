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

namespace Outfitted
{
	[HarmonyPatch(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.ApparelScoreGain))]
	internal static class JobGiver_OptimizeApparel_ApparelScoreGain_Patch
	{
		private static readonly MethodInfo ApparelScoreRawMI =
			AccessTools.Method(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.ApparelScoreRaw));

		/// <summary>
		/// After original call:
		/// float num = ApparelScoreRaw(pawn, ap);
		/// check the 'num' and if it is 0 or less, then return from the execution.
		/// </summary>
		/// <param name="instructions"></param>
		/// <param name="il"></param>
		/// <returns></returns>
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			var matcher = new CodeMatcher(instructions, il);

			// Find the method call and the result.
			matcher.MatchEndForward(
				new CodeMatch(OpCodes.Call, ApparelScoreRawMI),		// Call for a method.
				new CodeMatch(ci => ci.IsStloc()))					// Store the result in local variable.
				.ThrowIfInvalid("Can not find call ApparelScoreRaw()");

			// Get the index of current local variable
			int numLocalIndex = matcher.InstructionAt(0).LocalIndex();

			// Define a label to which code normally continue (jump over our new return instructions).
			var continueLabel = il.DefineLabel();

			// Advance one step, right after storing the local variable.
			matcher.Advance(1);

			// Insert instructions:
			//	ldloc num			<- Push value to the stack
			//	ldc.r4 0			<- Push 0f as float32 to the stack
			//	bgt.s continue		<- If first pushed val > second pushed val, goto branch (num > 0)
			//	ldloc num			<- Push value back to the stack
			//	ret					<- Return
			//	NOP continue:		<- nop with label
			matcher.Insert(
				new CodeInstruction(OpCodes.Ldloc_S, (short)numLocalIndex),
				new CodeInstruction(OpCodes.Ldc_R4, 0f),
				new CodeInstruction(OpCodes.Bgt_S, continueLabel),
				new CodeInstruction(OpCodes.Ldloc_S, (short)numLocalIndex),
				new CodeInstruction(OpCodes.Ret),
				new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel));

			return matcher.InstructionEnumeration();
		}
	}
}
