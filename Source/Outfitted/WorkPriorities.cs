// Decompiled with JetBrains decompiler
// Type: Outfitted.WorkPriorities
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Outfitted
{
	public class WorkPriorities : WorldComponent
	{
		private static List<WorktypePriorities> _worktypePriorities;
		private static WorkPriorities _instance;

		public WorkPriorities(World world)
		  : base(world)
		{
			WorkPriorities._instance = this;
			Log.Message("WorldComponent created!");
		}

		public static List<StatPriority> WorktypeStatPriorities(Pawn pawn)
		{
			IEnumerable<WorktypePriorityWeights> source = DefDatabase<WorkTypeDef>.AllDefsListForReading
				.Select(wtd => new
				{
					priority = pawn?.workSettings?.GetPriority(wtd) ?? 0,
					worktype = wtd
				})
				.Where(x => x.priority > 0)
				.Select(x => new WorktypePriorityWeights(
					x.priority,
					x.worktype,
					WorkPriorities.WorktypeStatPriorities(x.worktype)
					));

			if (!source.Any())
				return new List<StatPriority>();
			IntRange intRange = new IntRange(source.Min(s => s.Priority), source.Max(s => s.Priority));
			List<StatPriority> list = new List<StatPriority>();
			float num1 = 0.0f;
			foreach (var data in source)
			{
				float num2 = intRange.min == intRange.max ? 1f : (float)(1.0 - (double)(data.Priority - intRange.min) / (double)(intRange.max - intRange.min));
				foreach (StatPriority weight in data.Weights)
				{
					StatPriority statWeight = weight;
					StatPriority statPriority1 = list.FirstOrDefault<StatPriority>((Predicate<StatPriority>)(sp => sp.Stat == statWeight.Stat));
					if (statPriority1 != null)
					{
						statPriority1.Weight += num2 * statWeight.Weight;
					}
					else
					{
						StatPriority statPriority2 = new StatPriority(statWeight.Stat, num2 * statWeight.Weight);
						list.Add(statPriority2);
					}
					num1 += statWeight.Weight * num2;
				}
			}
			if (list.Any<StatPriority>() && (double)num1 != 0.0)
			{
				foreach (StatPriority statPriority in list)
					statPriority.Weight *= 10f / num1;
			}
			return list;
		}

		public static List<StatPriority> WorktypeStatPriorities(WorkTypeDef worktype)
		{
			WorktypePriorities worktypePriorities = WorkPriorities._worktypePriorities.Find((Predicate<WorktypePriorities>)(wp => wp.worktype == worktype));
			if (worktypePriorities == null)
			{
				Log.Warning("Outfitted :: Created worktype stat priorities for '" + worktype.defName + "' after initial init. This should never happen!");
				worktypePriorities = new WorktypePriorities(worktype, WorkPriorities.DefaultPriorities(worktype));
				WorkPriorities._worktypePriorities.Add(worktypePriorities);
			}
			return worktypePriorities.priorities;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<WorktypePriorities>(ref WorkPriorities._worktypePriorities, "worktypePriorities", LookMode.Deep);
		}

		public override void FinalizeInit(bool fromLoad)
		{
			base.FinalizeInit(fromLoad);
			if (WorkPriorities._worktypePriorities != null && WorkPriorities._worktypePriorities.Count > 0)
				return;
			WorkPriorities._worktypePriorities = new List<WorktypePriorities>();
			foreach (WorkTypeDef worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading)
				WorkPriorities._worktypePriorities.Add(new WorktypePriorities(worktype, WorkPriorities.DefaultPriorities(worktype)));
		}

		private static List<StatPriority> DefaultPriorities(WorkTypeDef worktype)
		{
			List<StatPriority> list = new List<StatPriority>();
			if (worktype == WorkTypeDefOf.Art)
			{
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.BasicWorker)
			{
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Cleaning)
			{
				list.Add(new StatPriority(StatDefOf.MoveSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 1f));
			}
			if (worktype == WorkTypeDefOf.Cooking)
			{
				list.Add(new StatPriority(StatDefOf.CookSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.FoodPoisonChance, -2f));
				list.Add(new StatPriority(StatDefOf.ButcheryFleshSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.ButcheryFleshEfficiency, 1f));
			}
			if (worktype == WorkTypeDefOf.Construction)
			{
				list.Add(new StatPriority(StatDefOf.ConstructionSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.ConstructSuccessChance, 2f));
				list.Add(new StatPriority(StatDefOf.FixBrokenDownBuildingSuccessChance, 1f));
				list.Add(new StatPriority(StatDefOf.SmoothingSpeed, 1f));
			}
			if (worktype == WorkTypeDefOf.Crafting)
			{
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 2f));
				list.Add(new StatPriority(StatDefOf.DrugSynthesisSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.DrugCookingSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.ButcheryMechanoidSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.ButcheryMechanoidEfficiency, 1f));
			}
			if (worktype == WorkTypeDefOf.Doctor)
			{
				list.Add(new StatPriority(StatDefOf.MedicalTendSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.MedicalTendQuality, 2f));
				list.Add(new StatPriority(StatDefOf.MedicalOperationSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.MedicalSurgerySuccessChance, 2f));
			}
			if (worktype == WorkTypeDefOf.Firefighter)
				list.Add(new StatPriority(StatDefOf.MoveSpeed, 2f));
			if (worktype == WorkTypeDefOf.Growing)
			{
				list.Add(new StatPriority(StatDefOf.PlantWorkSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.PlantHarvestYield, 2f));
			}
			if (worktype == WorkTypeDefOf.Handling)
			{
				list.Add(new StatPriority(StatDefOf.MoveSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.TameAnimalChance, 2f));
				list.Add(new StatPriority(StatDefOf.TrainAnimalChance, 2f));
				list.Add(new StatPriority(StatDefOf.AnimalGatherSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.AnimalGatherYield, 1f));
			}
			if (worktype == WorkTypeDefOf.Hauling)
			{
				list.Add(new StatPriority(StatDefOf.CarryingCapacity, 2f));
				list.Add(new StatPriority(StatDefOf.MoveSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Hunting)
			{
				list.Add(new StatPriority(StatDefOf.MoveSpeed, 1f));
				list.Add(new StatPriority(StatDefOf.ShootingAccuracyPawn, 2f));
				list.Add(new StatPriority(StatDefOf.AimingDelayFactor, 1f));
				list.Add(new StatPriority(StatDefOf.HuntingStealth, 2f));
				list.Add(new StatPriority(StatDefOf.AccuracyTouch, 1f));
				list.Add(new StatPriority(StatDefOf.AccuracyShort, 1f));
				list.Add(new StatPriority(StatDefOf.AccuracyMedium, 1f));
				list.Add(new StatPriority(StatDefOf.AccuracyLong, 2f));
				list.Add(new StatPriority(StatDefOf.RangedWeapon_Cooldown, 1f));
				list.Add(new StatPriority(StatDefOf.RangedWeapon_DamageMultiplier, 1f));
			}
			if (worktype == WorkTypeDefOf.Mining)
			{
				list.Add(new StatPriority(StatDefOf.MiningSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.MiningYield, 2f));
			}
			if (worktype == WorkTypeDefOf.PlantCutting)
			{
				list.Add(new StatPriority(StatDefOf.PlantWorkSpeed, 2f));
				list.Add(new StatPriority(StatDefOf.PlantHarvestYield, 2f));
			}
			if (worktype == WorkTypeDefOf.Research)
				list.Add(new StatPriority(StatDefOf.ResearchSpeed, 2f));
			if (worktype == WorkTypeDefOf.Smithing)
			{
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Tailoring)
			{
				list.Add(new StatPriority(StatDefOf.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Warden)
			{
				list.Add(new StatPriority(StatDefOf.NegotiationAbility, 2f));
				list.Add(new StatPriority(StatDefOf.TradePriceImprovement, 1f));
				list.Add(new StatPriority(StatDefOf.SocialImpact, 2f));
			}
			list.RemoveDuplicates<StatPriority>();
			return list;
		}
	}

	public class WorktypePriorityWeights
	{
		public int Priority;
		public WorkTypeDef WorkType;
		public List<StatPriority> Weights;

		public WorktypePriorityWeights(int priority, WorkTypeDef workType, List<StatPriority> weights)
		{
			Priority = priority;
			WorkType = workType;
			Weights = weights;
		}
	}
}
