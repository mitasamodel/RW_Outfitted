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

		public static void ITab_InventoryDrawThingRowCEPostfix(ITab_Pawn_Gear __instance, ref float y, float width, Thing thing, bool showDropButtonIfPrisoner)
		{
			float rowHeight = 28f;
			//GearDisplayScore(__instance, new Rect(0, y - rowHeight, width, rowHeight), thing, CE: true);
		}

		internal static void GearDisplayScore(Pawn pawn, Rect rowRect, Thing thing, bool CE = false)
		{
			if (!(thing is Apparel apparel) ||
				!Outfitted.showApparelScores ||
				!(pawn.RaceProps?.Humanlike == true) ||
				//(!pawn.IsColonistPlayerControlled || !pawn.IsPrisonerOfColony || !pawn.IsSlaveOfColony)
				!pawn.IsColonistPlayerControlled)
				return;

			List<Apparel> wornAp = pawn.apparel?.WornApparel;
			if (wornAp == null || wornAp.Count == 0)
				return;
			if (!wornAp.Any(wa => wa == apparel)) return;


			float smallGap = 6f;
			float scoreWidth = 45f;



			Rect freeSpaceRect = rowRect;

			freeSpaceRect.width -= (24f + 24f + 24f + 60f);
			Utils_GUI.DrawBox(freeSpaceRect, Color.grey);

			Rect scoreRect = freeSpaceRect.ToTheRight(scoreWidth);
			scoreRect.x -= smallGap;
			Utils_GUI.DrawBox(scoreRect, Color.white);

			float score = CacheWornApparel.GetScore(pawn,apparel);
			Utils_GUI.LabelMiddleRight(scoreRect, $"{score:F1}", BeautyDrawer.BeautyColor(score, 3f), GameFont.Tiny);
		}
	}


	[HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
	public static class ITab_Pawn_Gear_DrawThingRow_Patch
	{
		// cache the private property getter and a fast delegate
		private static readonly MethodInfo SelPawnForGearGetter =
			AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		// The same as Pawn pawn = (Pawn)SelPawnForGearGetter.Invoke(__instance, null);
		// But faster (cached reflection).
		private static readonly Func<ITab_Pawn_Gear, Pawn> GetSelPawnForGear =
			AccessTools.MethodDelegate<Func<ITab_Pawn_Gear, Pawn>>(SelPawnForGearGetter);

		public static void Postfix(
			ITab_Pawn_Gear __instance,
			ref float y,
			float width,
			Thing thing,
			bool inventory
		)
		{
			float rowHeight = 28f;
			ConditionalPatches.GearDisplayScore(GetSelPawnForGear(__instance), new Rect(0, y - rowHeight, width, rowHeight), thing);
		}
	}

	[HarmonyPatch(typeof(ITab_Pawn_Gear), "FillTab")]
	public static class ITab_Pawn_Gear_FillTab_Patch
	{

	}
}

