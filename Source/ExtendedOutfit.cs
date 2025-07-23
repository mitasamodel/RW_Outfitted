// Decompiled with JetBrains decompiler
// Type: Outfitted.ExtendedOutfit
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

#nullable disable
namespace Outfitted
{
  public class ExtendedOutfit : ApparelPolicy, IExposable
  {
    public bool targetTemperaturesOverride;
    public FloatRange targetTemperatures = new FloatRange(-100f, 100f);
    public bool PenaltyWornByCorpse = true;
    public bool AutoWorkPriorities;
    private bool _autoTemp;
    public int autoTempOffset = 20;
    private static IEnumerable<StatCategoryDef> blacklistedCategories = (IEnumerable<StatCategoryDef>) new List<StatCategoryDef>()
    {
      StatCategoryDefOf.BasicsNonPawn,
      StatCategoryDefOf.Building,
      StatCategoryDefOf.StuffStatFactors
    };
    private static readonly IEnumerable<StatDef> blacklistedStats = (IEnumerable<StatDef>) new List<StatDef>()
    {
      StatDefOf.ComfyTemperatureMin,
      StatDefOf.ComfyTemperatureMax,
      StatDefOf.Insulation_Cold,
      StatDefOf.Insulation_Heat,
      StatDefOf.StuffEffectMultiplierInsulation_Cold,
      StatDefOf.StuffEffectMultiplierInsulation_Heat,
      StatDefOf.StuffEffectMultiplierArmor
    };
    private List<StatPriority> statPriorities = new List<StatPriority>();

    public bool AutoTemp
    {
      get => this._autoTemp;
      set
      {
        this._autoTemp = value;
        if (!this._autoTemp)
          return;
        this.targetTemperaturesOverride = true;
      }
    }

    internal static IEnumerable<StatDef> AllAvailableStats
    {
      get
      {
        return (IEnumerable<StatDef>) DefDatabase<StatDef>.AllDefs.Where<StatDef>((Func<StatDef, bool>) (i => !ExtendedOutfit.blacklistedCategories.Contains<StatCategoryDef>(i.category))).Except<StatDef>(ExtendedOutfit.blacklistedStats).ToList<StatDef>();
      }
    }

    public IEnumerable<StatDef> UnassignedStats
    {
      get
      {
        return ExtendedOutfit.AllAvailableStats.Except<StatDef>(this.StatPriorities.Select<StatPriority, StatDef>((Func<StatPriority, StatDef>) (i => i.Stat)));
      }
    }

    public IEnumerable<StatPriority> StatPriorities
    {
      get => (IEnumerable<StatPriority>) this.statPriorities;
    }

    public ExtendedOutfit(int uniqueId, string label)
      : base(uniqueId, label)
    {
    }

    public ExtendedOutfit(ApparelPolicy outfit)
      : base(outfit.id, outfit.label)
    {
      this.filter.CopyAllowancesFrom(outfit.filter);
    }

    public ExtendedOutfit()
    {
    }

    public void AddStatPriority(StatDef def, float priority, float defaultPriority = float.NaN)
    {
      this.statPriorities.Insert(0, new StatPriority(def, priority, defaultPriority));
    }

    public void AddRange(IEnumerable<StatPriority> priorities)
    {
      this.statPriorities.AddRange(priorities);
    }

    public void RemoveStatPriority(StatDef def)
    {
      this.statPriorities.RemoveAll((Predicate<StatPriority>) (i => i.Stat == def));
    }

    void IExposable.ExposeData()
    {
      Scribe_Values.Look<int>(ref this.id, "id");
      Scribe_Values.Look<string>(ref this.label, "label");
      Scribe_Deep.Look<ThingFilter>(ref this.filter, "filter");
      Scribe_Values.Look<bool>(ref this.targetTemperaturesOverride, "targetTemperaturesOverride");
      Scribe_Values.Look<FloatRange>(ref this.targetTemperatures, "targetTemperatures");
      Scribe_Values.Look<bool>(ref this.PenaltyWornByCorpse, "PenaltyWornByCorpse", true);
      Scribe_Collections.Look<StatPriority>(ref this.statPriorities, "statPriorities", LookMode.Deep);
      Scribe_Values.Look<bool>(ref this.AutoWorkPriorities, "AutoWorkPriorities");
      Scribe_Values.Look<bool>(ref this._autoTemp, "AutoTemp");
      Scribe_Values.Look<int>(ref this.autoTempOffset, "autoTempOffset");
    }

    public void CopyFrom(ExtendedOutfit outfit)
    {
      this.filter.CopyAllowancesFrom(outfit.filter);
      this.targetTemperaturesOverride = outfit.targetTemperaturesOverride;
      this.targetTemperatures = outfit.targetTemperatures;
      this.PenaltyWornByCorpse = outfit.PenaltyWornByCorpse;
      this.statPriorities.Clear();
      foreach (StatPriority statPriority in outfit.statPriorities)
        this.statPriorities.Add(new StatPriority(statPriority.Stat, statPriority.Weight, statPriority.Default));
      this.AutoWorkPriorities = outfit.AutoWorkPriorities;
      this._autoTemp = outfit._autoTemp;
      this.autoTempOffset = outfit.autoTempOffset;
    }
  }
}
