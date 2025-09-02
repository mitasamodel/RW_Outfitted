using CombatExtended;
using HarmonyLib;
using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using LudeonTK;

namespace Outfitted
{
	//[HarmonyPatch(typeof(ITab_Inventory), "DrawThingRowCE")]
	internal static class ConditionalPatches
	{
		internal static void PatchAll(Harmony harmony)
		{
			MethodInfo original = AccessTools.TypeByName("CombatExtended.ITab_Inventory")?.GetMethod("DrawThingRowCE");
			MethodInfo postfix = typeof(ConditionalPatches).GetMethod(nameof(ITab_InventoryDrawThingRowCEPostfix));
			if (original != null) harmony.Patch(original, postfix: new HarmonyMethod(postfix));
		}

		public static void ITab_InventoryDrawThingRowCEPostfix(ref float y, float width, Thing thing, bool showDropButtonIfPrisoner)
		{
			float rowHeight = 28f;
			GearDisplayScore(new Rect(0, y - rowHeight, width, rowHeight), thing, CE: true);
		}

		internal static void GearDisplayScore(Rect rowRect, Thing thing, bool CE = false)
		{
			float smallGap = 6f;
			Rect freeSpaceRect = rowRect;

			freeSpaceRect.width -= (24f + 24f + 24f + 60f);
			Utils_GUI.DrawBox(freeSpaceRect, Color.grey);

			string sampleText = "-123.5";
			Rect scoreRect = freeSpaceRect.ToTheRight(Text.CalcSize(sampleText).x);
			scoreRect.x -= smallGap;
			Utils_GUI.DrawBox(scoreRect, Color.white);

			Utils_GUI.LabelRight(scoreRect, "-123.4");
			Log.Message("Size: " + Text.CalcSize(sampleText).x);
		}
	}


	[HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
	public static class ITab_Pawn_Gear_DrawThingRow_Patch
	{
		// Precisely select: private void DrawThingRow(ref float y, float width, Thing thing, bool inventory = false)
		//static MethodBase TargetMethod()
		//{
		//	return AccessTools.Method(
		//		typeof(ITab_Pawn_Gear),
		//		"DrawThingRow",
		//		new[] { typeof(float).MakeByRefType(), typeof(float), typeof(Thing), typeof(bool) }
		//	);
		//}

		public static void Postfix(
			ITab_Pawn_Gear __instance,
			ref float y,
			float width,
			Thing thing,
			bool inventory
		)
		{
			float rowHeight = 28f;
			ConditionalPatches.GearDisplayScore(new Rect(0, y - rowHeight, width, rowHeight), thing);
		}
	}

	[HarmonyPatch(typeof(ITab_Pawn_Gear), "FillTab")]
	public static class ITab_Pawn_Gear_FillTab_Patch
	{

	}
}

