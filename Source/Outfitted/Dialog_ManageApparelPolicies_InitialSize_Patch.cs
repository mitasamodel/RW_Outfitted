using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Outfitted
{
	[HarmonyPatch(typeof(Dialog_ManageApparelPolicies), nameof(Dialog_ManageApparelPolicies.InitialSize), MethodType.Getter)]
	public static class Window_InitialSize_Patch
	{
		public static bool Prefix(Window __instance, ref Vector2 __result)
		{
			if (__instance is Dialog_ManageApparelPolicies)
			{
				__result = new Vector2(900f, 700f);
				return false;
			}

			return true; // fallback to original method
		}
	}

	[HarmonyPatch(typeof(Dialog_ManageApparelPolicies), MethodType.Constructor, argumentTypes: new[] { typeof(ApparelPolicy) })]
	public static class Dialog_ManageApparelPolicies_Ctor_Patch
	{
		public static void Postfix(Dialog_ManageApparelPolicies __instance)
		{
			if ( OutfittedMod.Settings.draggableWindow) __instance.draggable = true;
			if (OutfittedMod.Settings.nonBlockingWindow)
			{
				__instance.closeOnClickedOutside = false;
				__instance.absorbInputAroundWindow = false;
				__instance.preventCameraMotion = false;
				__instance.forcePause = false;
			}
		}
	}
}
