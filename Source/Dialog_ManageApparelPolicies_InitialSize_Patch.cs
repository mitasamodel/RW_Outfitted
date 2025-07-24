// Decompiled with JetBrains decompiler
// Type: Outfitted.Dialog_ManageApparelPolicies_InitialSize_Patch
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Outfitted
{
	[HarmonyPatch(typeof(Dialog_ManageApparelPolicies), "get_InitialSize")]
	public static class Window_InitialSize_Patch
	{
		public static bool Prefix(Window __instance, ref Vector2 __result)
		{
			if (__instance is Dialog_ManageApparelPolicies)
			{
#if DEBUG
				Log.Message("[Outfitted] Resized Dialog_ManageApparelPolicies to 900x700");
#endif
				__result = new Vector2(900f, 700f);
				return false;
			}

			return true; // fallback to original method
		}
	}

}
