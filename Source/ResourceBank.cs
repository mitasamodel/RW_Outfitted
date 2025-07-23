// Decompiled with JetBrains decompiler
// Type: Outfitted.ResourceBank
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using UnityEngine;
using Verse;

#nullable disable
namespace Outfitted
{
  public static class ResourceBank
  {
    [StaticConstructorOnStartup]
    public static class Textures
    {
      public static readonly Texture2D AddButton = ContentFinder<Texture2D>.Get("add");
      public static readonly Texture2D BgColor = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.2f, 1f));
      public static readonly Texture2D DeleteButton = ContentFinder<Texture2D>.Get("delete");
      public static readonly Texture2D Drop = ContentFinder<Texture2D>.Get("UI/Buttons/Drop");
      public static readonly Texture2D FloatRangeSliderTex = ContentFinder<Texture2D>.Get("UI/Widgets/RangeSlider");
      public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton");
      public static readonly Texture2D ResetButton = ContentFinder<Texture2D>.Get("reset");
      public static readonly Texture2D White = SolidColorMaterials.NewSolidColorTexture(Color.white);
      public static readonly Texture2D ShirtBasic = ContentFinder<Texture2D>.Get("Things/Pawn/Humanlike/Apparel/ShirtBasic/ShirtBasic");
    }

    public static class Strings
    {
      public static readonly string PreferedTemperature = ResourceBank.Strings.TL(nameof (PreferedTemperature));
      public static readonly string TemperatureRangeReset = ResourceBank.Strings.TL(nameof (TemperatureRangeReset));
      public static readonly string PreferedStats = ResourceBank.Strings.TL(nameof (PreferedStats));
      public static readonly string StatPriorityAdd = ResourceBank.Strings.TL(nameof (StatPriorityAdd));
      public static readonly string None = ResourceBank.Strings.TL(nameof (None));
      public static readonly string OutfitShow = ResourceBank.Strings.TL(nameof (OutfitShow));
      public static readonly string PenaltyWornByCorpse = ResourceBank.Strings.TL(nameof (PenaltyWornByCorpse));
      public static readonly string PenaltyWornByCorpseTooltip = ResourceBank.Strings.TL(nameof (PenaltyWornByCorpseTooltip));
      public static readonly string AutoWorkPriorities = ResourceBank.Strings.TL(nameof (AutoWorkPriorities));
      public static readonly string AutoWorkPrioritiesTooltip = ResourceBank.Strings.TL(nameof (AutoWorkPrioritiesTooltip));
      public static readonly string AutoTemp = ResourceBank.Strings.TL(nameof (AutoTemp));
      public static readonly string AutoTempTooltip = ResourceBank.Strings.TL(nameof (AutoTempTooltip));
      public static readonly string AutoTempOffset = ResourceBank.Strings.TL(nameof (AutoTempOffset));
      public static readonly string AutoTempOffsetTooltip = ResourceBank.Strings.TL(nameof (AutoTempOffsetTooltip));

      private static string TL(string s) => (string) s.Translate();

      private static string TL(string s, string arg) => (string) s.Translate((NamedArgument) arg);

      public static string StatPriorityDelete(string labelCap)
      {
        return ResourceBank.Strings.TL(nameof (StatPriorityDelete), labelCap);
      }

      public static string StatPriorityReset(string labelCap)
      {
        return ResourceBank.Strings.TL(nameof (StatPriorityReset), labelCap);
      }
    }
  }
}
