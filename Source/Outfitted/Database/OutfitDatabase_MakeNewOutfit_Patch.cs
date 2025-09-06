using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Outfitted.Database
{
	[HarmonyPatch(typeof(OutfitDatabase), nameof(OutfitDatabase.MakeNewOutfit))]
	internal static class OutfitDatabase_MakeNewOutfit_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var ctorApparel = AccessTools.Constructor(typeof(ApparelPolicy), new[] { typeof(int), typeof(string) });
			var ctorExtended = AccessTools.Constructor(typeof(ExtendedOutfit), new[] { typeof(int), typeof(string) });

			var matcher = new CodeMatcher(instructions);

			// Find ctor.
			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Newobj, ctorApparel))
				.ThrowIfInvalid("Can not find ctor for ApparelPolicy");

			// Replace operand.
			matcher.SetOperandAndAdvance(ctorExtended);

			return matcher.InstructionEnumeration();
		}
	}
}
