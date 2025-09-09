using HarmonyLib;
using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Outfitted
{
	/// <summary>
	/// Patch the parent of Dialog_ManageApparelPolicies.
	/// This is where the whole Window is being drawn.
	/// </summary>
	[HarmonyPatch(typeof(Dialog_ManagePolicies<ApparelPolicy>), nameof(Dialog_ManagePolicies<ApparelPolicy>.DoWindowContents))]
	internal static class Dialog_ManagePolicies_DoWindowContents_Patch
	{
		private static void Postfix(Dialog_ManagePolicies<ApparelPolicy> __instance, Rect inRect)
		{
			if (__instance is Dialog_ManageApparelPolicies)
			{
				ExtendedOutfit extendedOutfit =
					AccessTools.Property(typeof(Dialog_ManagePolicies<ApparelPolicy>), "SelectedPolicy")
					.GetValue(__instance, null) as ExtendedOutfit;

				// Draw top buttons: Copy settings and Generate policies.
				Rect topButtonsRect = new Rect(inRect.x, inRect.y, inRect.width, Dialog_Policies.ButtonHeight);
				Dialog_Policies.DrawOutfittedButtons(topButtonsRect, extendedOutfit);

				if (extendedOutfit != null)
				{
					// Title.
					Rect titleRect = new Rect(
						inRect.xMax - Dialog_Policies.RightPanelWidth,
						inRect.y + Dialog_Policies.WinTitleHeight + Dialog_Policies.WinTipHeight + Dialog_Policies.GapHor,
						Dialog_Policies.RightPanelWidth, Dialog_Policies.WinTitleHeight);
					Dialog_Policies.DrawOutfittedTitle(titleRect);

					// Main column.
					Rect contentRect = titleRect;
					contentRect.y = titleRect.yMax + 4f;
					contentRect.yMax = inRect.yMax - Window.CloseButSize.y - Dialog_Policies.GapHor;
					Dialog_Policies.DrawOutfittedContent(contentRect, extendedOutfit);
				}
			}
		}
	}
}
