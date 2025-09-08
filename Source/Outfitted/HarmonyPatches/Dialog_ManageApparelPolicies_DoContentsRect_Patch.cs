using HarmonyLib;
using RimWorld;
using Outfitted.RW_JustUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using LudeonTK;

namespace Outfitted
{
	[HarmonyPatch(typeof(Dialog_ManageApparelPolicies), "DoContentsRect")]
	internal static class Dialog_ManageApparelPolicies_DoContentsRect_Patch
	{
		private static void Prefix(ref Rect rect) => rect.width = Dialog_Policies.MiddlePanelWidth;

		//private static void Postfix(Rect rect, Dialog_ManageApparelPolicies __instance)
		//{
		//	ExtendedOutfit extendedOutfit =
		//			AccessTools.Property(typeof(Dialog_ManagePolicies<ApparelPolicy>), "SelectedPolicy")
		//			.GetValue(__instance, null) as ExtendedOutfit;

		//	Dialog_Policies.DrawOutfittedContent(rect, extendedOutfit);
		//}
	}
}
