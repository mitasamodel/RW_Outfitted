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
		private static readonly ConstructorInfo CtorApparel =
			AccessTools.Constructor(typeof(ApparelPolicy), new[] { typeof(int), typeof(string) });

		private static readonly MethodInfo HelperMethod =
			AccessTools.Method(typeof(OutfitDatabase_MakeNewOutfit_Patch), nameof(Helper));

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var matcher = new CodeMatcher(instructions);

			// Find ctor.
			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Newobj, CtorApparel))
				.ThrowIfInvalid("Can not find ctor for ApparelPolicy");

			// Replace calling ctor by colling our method.
			matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call, HelperMethod));

			return matcher.InstructionEnumeration();
		}

		/// <summary>
		/// Initialize new outfit and add some basic stats.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		private static ApparelPolicy Helper(int id, string label)
		{
			ExtendedOutfit outfit = new ExtendedOutfit(id, label);
			Helpers.AddBasicsStats(outfit);

			return outfit;
		}
	}
}
