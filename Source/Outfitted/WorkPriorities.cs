using RimWorld.Planet;
using RimWorld;
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
			_instance = this;
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
					WorktypeStatPriorities(x.worktype)
					));

			if (!source.Any())
				return new List<StatPriority>();
			IntRange intRange = new IntRange(source.Min(s => s.Priority), source.Max(s => s.Priority));
			List<StatPriority> list = new List<StatPriority>();
			float num1 = 0.0f;
			foreach (var data in source)
			{
				float num2 = intRange.min == intRange.max ? 1f : (float)(1.0 - (float)(data.Priority - intRange.min) / (intRange.max - intRange.min));
				foreach (StatPriority weight in data.Weights)
				{
					StatPriority statWeight = weight;
					StatPriority statPriority1 = list.FirstOrDefault(sp => sp.Stat == statWeight.Stat);
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
			if (list.Any() && (double)num1 != 0.0)
			{
				foreach (StatPriority statPriority in list)
					statPriority.Weight *= 10f / num1;
			}
			return list;
		}

		public static List<StatPriority> WorktypeStatPriorities(WorkTypeDef worktype)
		{
			WorktypePriorities worktypePriorities = _worktypePriorities.Find(wp => wp.worktype == worktype);
			if (worktypePriorities == null)
			{
				Log.Warning($"Outfitted :: Created worktype stat priorities for '{worktype.defName}' after initial init. This should never happen!");
				worktypePriorities = new WorktypePriorities(worktype, DefaultPriorities(worktype));
				_worktypePriorities.Add(worktypePriorities);
			}
			return worktypePriorities.priorities;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref _worktypePriorities, "worktypePriorities", LookMode.Deep);
		}

		public override void FinalizeInit(bool fromLoad)
		{
			base.FinalizeInit(fromLoad);
			if (_worktypePriorities != null && _worktypePriorities.Count > 0)
				return;
			_worktypePriorities = new List<WorktypePriorities>();
			foreach (WorkTypeDef worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading)
				_worktypePriorities.Add(new WorktypePriorities(worktype, DefaultPriorities(worktype)));
		}

		private static List<StatPriority> DefaultPriorities(WorkTypeDef worktype)
		{
			List<StatPriority> list = new List<StatPriority>();
			if (worktype == WorkTypeDefOf.Art)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.BasicWorker)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Cleaning)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.MoveSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 1f));
			}
			if (worktype == WorkTypeDefOf.Cooking)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.CookSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.FoodPoisonChance, -2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ButcheryFleshSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ButcheryFleshEfficiency, 1f));
			}
			if (worktype == WorkTypeDefOf.Construction)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.ConstructionSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ConstructSuccessChance, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.FixBrokenDownBuildingSuccessChance, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.SmoothingSpeed, 1f));
			}
			if (worktype == WorkTypeDefOf.Crafting)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.DrugSynthesisSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.DrugCookingSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ButcheryMechanoidSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ButcheryMechanoidEfficiency, 1f));
			}
			if (worktype == WorkTypeDefOf.Doctor)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.MedicalTendSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.MedicalTendQuality, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.MedicalOperationSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.MedicalSurgerySuccessChance, 2f));
			}
			if (worktype == WorkTypeDefOf.Firefighter)
				list.Add(new StatPriority(StatDefOf_Rimworld.MoveSpeed, 2f));
			if (worktype == WorkTypeDefOf.Growing)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.PlantWorkSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.PlantHarvestYield, 2f));
			}
			if (worktype == WorkTypeDefOf.Handling)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.MoveSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.TameAnimalChance, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.TrainAnimalChance, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AnimalGatherSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AnimalGatherYield, 1f));
			}
			if (worktype == WorkTypeDefOf.Hauling)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.CarryingCapacity, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.MoveSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Hunting)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.MoveSpeed, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.ShootingAccuracyPawn, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AimingDelayFactor, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.HuntingStealth, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AccuracyTouch, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AccuracyShort, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AccuracyMedium, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.AccuracyLong, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.RangedWeapon_Cooldown, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.RangedWeapon_DamageMultiplier, 1f));
			}
			if (worktype == WorkTypeDefOf.Mining)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.MiningSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.MiningYield, 2f));
			}
			if (worktype == WorkTypeDefOf.PlantCutting)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.PlantWorkSpeed, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.PlantHarvestYield, 2f));
			}
			if (worktype == WorkTypeDefOf.Research)
				list.Add(new StatPriority(StatDefOf_Rimworld.ResearchSpeed, 2f));
			if (worktype == WorkTypeDefOf.Smithing)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Tailoring)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.WorkSpeedGlobal, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.GeneralLaborSpeed, 2f));
			}
			if (worktype == WorkTypeDefOf.Warden)
			{
				list.Add(new StatPriority(StatDefOf_Rimworld.NegotiationAbility, 2f));
				list.Add(new StatPriority(StatDefOf_Rimworld.TradePriceImprovement, 1f));
				list.Add(new StatPriority(StatDefOf_Rimworld.SocialImpact, 2f));
			}
			list.RemoveDuplicates();
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
