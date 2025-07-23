// Decompiled with JetBrains decompiler
// Type: Outfitted.PlaySettings_DoPlaySettingsGlobalControls_Patch
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using HarmonyLib;
using RimWorld;
using Verse;

#nullable disable
namespace Outfitted
{
  [HarmonyPatch(typeof (PlaySettings), "DoPlaySettingsGlobalControls")]
  internal static class PlaySettings_DoPlaySettingsGlobalControls_Patch
  {
    private static void Postfix(WidgetRow row, bool worldView)
    {
      if (worldView)
        return;
      row.ToggleableIcon(ref OutfittedMod.showApparelScores, ResourceBank.Textures.ShirtBasic, ResourceBank.Strings.OutfitShow, SoundDefOf.Mouseover_ButtonToggle);
    }
  }
}
