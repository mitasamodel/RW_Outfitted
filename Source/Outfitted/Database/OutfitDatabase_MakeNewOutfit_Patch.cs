using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Outfitted.Database
{
	[HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.MakeNewOutfit))]
	internal static class OutfitDatabase_MakeNewOutfit_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			ConstructorInfo oldConstructor = AccessTools.Constructor(typeof(ApparelPolicy), new Type[2]
			{
				typeof (int),
				typeof (string)
			}, false);
			ConstructorInfo newConstructor = AccessTools.Constructor(typeof(ExtendedOutfit), new Type[2]
			{
				typeof (int),
				typeof (string)
			}, false);
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Newobj && oldConstructor.Equals(instruction.operand))
					instruction.operand = newConstructor;
				yield return instruction;
			}
		}
	}
}
