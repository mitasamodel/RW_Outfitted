using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Outfitted
{
	public class ExtendedOutfit : ApparelPolicy, IExposable
	{
		public bool targetTemperaturesOverride = true;
		public FloatRange targetTemperatures = new FloatRange(-100f, 100f);
		public bool PenaltyWornByCorpse = true;
		public bool AutoWorkPriorities;
		private bool _autoTemp = true;
		public int autoTempOffset = 20;
		private static IEnumerable<StatCategoryDef> blacklistedCategories = (IEnumerable<StatCategoryDef>)new List<StatCategoryDef>()
		{
			StatCategoryDefOf.BasicsNonPawn,
			StatCategoryDefOf.Building,
			StatCategoryDefOf.StuffStatFactors
		};
		private static readonly IEnumerable<StatDef> blacklistedStats = (IEnumerable<StatDef>)new List<StatDef>()
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
			get => _autoTemp;
			set
			{
				_autoTemp = value;
				if (!_autoTemp)
					return;
				targetTemperaturesOverride = true;
			}
		}

		internal static IEnumerable<StatDef> AllAvailableStats
		{
			get
			{
				return (IEnumerable<StatDef>)DefDatabase<StatDef>.AllDefs.Where<StatDef>((Func<StatDef, bool>)(i => !ExtendedOutfit.blacklistedCategories.Contains<StatCategoryDef>(i.category))).Except<StatDef>(ExtendedOutfit.blacklistedStats).ToList<StatDef>();
			}
		}

		public IEnumerable<StatDef> UnassignedStats
		{
			get
			{
				return AllAvailableStats.Except(StatPriorities.Select(i => i.Stat));
			}
		}

		public List<StatPriority> StatPriorities
		{
			get => statPriorities;
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
			this.statPriorities.RemoveAll((Predicate<StatPriority>)(i => i.Stat == def));
		}

		void IExposable.ExposeData()
		{
			Scribe_Values.Look(ref id, "id");
			Scribe_Values.Look(ref label, "label");
			Scribe_Deep.Look(ref filter, "filter");
			Scribe_Values.Look(ref targetTemperaturesOverride, "targetTemperaturesOverride");
			Scribe_Values.Look(ref targetTemperatures, "targetTemperatures");
			Scribe_Values.Look(ref PenaltyWornByCorpse, "PenaltyWornByCorpse", true);
			Scribe_Collections.Look(ref statPriorities, "statPriorities", LookMode.Deep);
			Scribe_Values.Look(ref AutoWorkPriorities, "AutoWorkPriorities");
			Scribe_Values.Look(ref _autoTemp, "AutoTemp");
			Scribe_Values.Look(ref autoTempOffset, "autoTempOffset");

			// Safeguard. Remove all nulls.
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
				Outfitted.PruneNullStatPriorities(this);
		}

		//public void CopyFrom(ExtendedOutfit outfit)
		public override void CopyFrom(Policy policy)
		{
			base.CopyFrom(policy);
			//filter.CopyAllowancesFrom(outfit.filter);
			if (policy is ExtendedOutfit outfit)
			{
				targetTemperaturesOverride = outfit.targetTemperaturesOverride;
				targetTemperatures = outfit.targetTemperatures;
				PenaltyWornByCorpse = outfit.PenaltyWornByCorpse;
				statPriorities.Clear();
				foreach (StatPriority statPriority in outfit.statPriorities)
					statPriorities.Add(new StatPriority(statPriority.Stat, statPriority.Weight));
				AutoWorkPriorities = outfit.AutoWorkPriorities;
				_autoTemp = outfit._autoTemp;
				autoTempOffset = outfit.autoTempOffset;
			}
		}
	}
}
