using CombatExtended;
using HarmonyLib;
using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

#if DEBUG
namespace Outfitted
{
	public static class MyDebug
	{
		internal static bool DeepScorePriorities { get; } = true;
		internal static bool ApparelStatsCache { get; } = true;

		internal static Apparel SelectedApparel { get; set; }
		private static ExtendedOutfit _outfit;
		private static Pawn _selectedPawn;

		public static void ClickedOn(ISelectable selectable)
		{
			if (selectable is Pawn pawn)
			{
				// Only humanlike. Avoid animals.
				if (!(pawn.RaceProps?.Humanlike == true) || !(pawn.Faction == Faction.OfPlayer))
					return;

				_selectedPawn = pawn;
				if (pawn.outfits.CurrentApparelPolicy is ExtendedOutfit outfit)
					_outfit = outfit;

				DebugDeepScorePriorities.Clear();
				ShowScoreWornApparel(pawn);
				DebugDeepScorePriorities.ShowLog();
			}
			else if (selectable is Apparel apparel)
			{
				SelectedApparel = apparel;
				DebugDeepScorePriorities.Clear();
				ShowScoreApp(SelectedApparel, _selectedPawn, _outfit);
				DebugDeepScorePriorities.ShowLog();
			}

			if (SelectedApparel == null || _outfit == null || _selectedPawn == null) return;

			//ShowApparelStatScores();
		}

		private static void ShowScoreWornApparel(Pawn pawn)
		{
			RW_JustUtils.LoggerMy.LogNL($"[{pawn.Name}] Apparel score:");
			List<Apparel> wornApparel = pawn?.apparel?.WornApparel;
			if (wornApparel == null || wornApparel.Count == 0)
			{
				RW_JustUtils.LoggerMy.LogNL($"No apparel");
				return;
			}

			ExtendedOutfit policy = pawn.outfits.CurrentApparelPolicy as ExtendedOutfit;
			Map map = pawn.MapHeld ?? pawn.Map;
			var seasonTemp = map.mapTemperature.SeasonalTemp;
			var tempOffset = GetTempOffset(map);
			RW_JustUtils.LoggerMy.LogNL($"SeasonalTemp [{seasonTemp}] Offset [{tempOffset}] Final [{seasonTemp + tempOffset}]");
			foreach (var ap in wornApparel)
			{
				ShowScoreApp(ap, pawn, policy, whatIfNotWorn: true);
			}
			RW_JustUtils.LoggerMy.LogNL($"\tComfort: Min[{pawn.ComfortableTemperatureRange().min}] Max[{pawn.ComfortableTemperatureRange().max}]");
		}

		private static float GetTempOffset(Map map)
		{
			float temp = 0f;
			if (map == null) return 0f;
			var conditions = map.GameConditionManager.ActiveConditions;
			if (conditions == null) return 0f;
			foreach (var condition in conditions)
			{
				temp += condition.TemperatureOffset();
				RW_JustUtils.LoggerMy.LogNL($"[{condition.def.defName}] [{condition.TemperatureOffset()}] [{condition.def.temperatureOffset}]");
			}
			//GameConditionDefOf
			return temp;
		}

		private static void ShowScoreApp(Apparel ap, Pawn pawn, ExtendedOutfit policy, bool whatIfNotWorn = false)
		{
			if (ap == null || pawn == null || policy == null) return;

			if (!whatIfNotWorn)
				RW_JustUtils.LoggerMy.Log($"\tDef[{ap.def?.defName}] ");
			else
				RW_JustUtils.LoggerMy.Log($"\tDef[{ap.def?.defName}] ");

			float totalRaw = 0f;
			if (whatIfNotWorn)
			{
				using (PawnContext.WhatIfNotWornScope(pawn))
					totalRaw = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			}
			else totalRaw = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			RW_JustUtils.LoggerMy.Log($"RWScore[{totalRaw:F2}] ");

			var wornScore = CacheWornApparel.GetScoreList(pawn);
			if (!whatIfNotWorn)
				RW_JustUtils.LoggerMy.Log($"RWGain[{JobGiver_OptimizeApparel.ApparelScoreGain(pawn, ap, wornScore):F2}] ");

			float num = 0f;

			num += OutfittedMod.Settings.disableStartScore ? 0f : 0.1f;
			num += OutfittedMod.Settings.disableScoreOffset ? 0f : ap.def.apparel.scoreOffset;
			if (num != 0) RW_JustUtils.LoggerMy.Log($"Base[{num:F2}] ");

			// Priority stats.
			float prio = ApparelScorePriorities.RawPriorities(pawn, ap, policy);
			RW_JustUtils.LoggerMy.Log($"Prio[{prio:F2}] ");
			num += prio;

			// Pawn need this.
			float need = ApparelScoreNeeds.PawnNeedThis(pawn, ap);
			RW_JustUtils.LoggerMy.Log($"Need[{need:F2}] ");
			num += need;

			// Ideology
			float ideo = ApparelScoreNeeds.PawnNeedIdeology(pawn, ap);
			if (ideo != 0) RW_JustUtils.LoggerMy.Log($"Ideo[{ideo:F2}] ");
			num += ideo;

			// Auto work.
			float autoWork = 0f;
			if (policy.AutoWorkPriorities)
				autoWork += Outfitted.ApparelScoreAutoWorkPriorities(pawn, ap);
			if (autoWork != 0) RW_JustUtils.LoggerMy.Log($"Work[{autoWork:F2}] ");
			num += autoWork;

			// HP.
			float hp = 1f;
			if (ap.def.useHitPoints)
			{
				hp = (float)ap.HitPoints / ap.MaxHitPoints;
				num *= Outfitted.HitPointsPercentScoreFactorCurve.Evaluate(Mathf.Clamp01(hp));
			}
			if (hp != 1) RW_JustUtils.LoggerMy.Log($"HP[{hp:F2}] Num[{num:F2}] ");

			// Special score offset.
			float offset = OutfittedMod.Settings.disableScoreOffset ? 0 : ap.GetSpecialApparelScoreOffset();
			num += offset;
			if (offset != 0) RW_JustUtils.LoggerMy.Log($"Offset[{offset:F2}] ");

			// Insulation.
			float insulation = ApparelScoreInsulation.RawInsulation(pawn, ap, policy, NeededWarmth.Any);
			num += insulation;
			RW_JustUtils.LoggerMy.Log($"Ins[{insulation:F2}] ");

			// Corpse.
			num = ApparelScoreNeeds.ModifiedWornByCorpse(pawn, ap, policy, num);
			RW_JustUtils.LoggerMy.Log($"Final[{num:F2}] ");


			RW_JustUtils.LoggerMy.LogNL("");
		}
	}

	internal static class DebugSelectionLogger
	{
		// Keep noisy logs under a single switch if you want to toggle later.
		public static bool Enabled => true;

		public static void LogForMapSelection(ISelectable selectable)
		{
			if (!Enabled || selectable == null) return;

			// Pawn selected
			if (selectable is Pawn pawn)
			{
				// On-map pawns
				if (pawn.Spawned)
				{
					Log.Message(
						$"[DebugSelection] Pawn selected: {pawn.LabelShortCap} | Faction={(pawn.Faction?.Name ?? "None")} | " +
						$"Pos={pawn.Position} | Map={pawn.Map?.ToString() ?? "null"}");
				}
				else
				{
					// Pawns inside containers, caravans, etc.
					Log.Message(
						$"[DebugSelection] Pawn selected (not spawned): {pawn.LabelShortCap} | Faction={(pawn.Faction?.Name ?? "None")}");
				}
				return;
			}

			// Any other Thing (items on the ground, buildings, corpses, etc.)
			if (selectable is Thing thing)
			{
				// Treat “item on the ground” as: a Thing that is spawned and not a Pawn.
				var defName = thing.def?.defName ?? "null";
				var label = thing.LabelCap;
				var mapStr = thing.Map?.ToString() ?? "null";
				var posStr = thing.Spawned ? thing.Position.ToString() : "(not spawned)";

				// Special-case corpse so you can see who it was
				if (thing is Corpse corpse)
				{
					var inner = corpse.InnerPawn != null ? corpse.InnerPawn.LabelShortCap : "unknown";
					Log.Message($"[DebugSelection] Corpse selected: {label} (of {inner}) | Def={defName} | Pos={posStr} | Map={mapStr}");
				}
				else
				{
					Log.Message($"[DebugSelection] Thing selected: {label} | Def={defName} | Pos={posStr} | Map={mapStr}");
				}
				return;
			}

			// Zones and other ISelectable (rooms, etc.)
			if (selectable is Zone zone)
			{
				Log.Message($"[DebugSelection] Zone selected: {zone.label ?? "(no label)"} | Cells={zone.cells.Count}");
				return;
			}

			// Fallback
			Log.Message($"[DebugSelection] ISelectable selected: {selectable.GetType().FullName}");
		}
	}

	[HarmonyPatch(typeof(Selector), nameof(Selector.Select), new[] { typeof(object), typeof(bool), typeof(bool) })]
	internal static class Selector_Select_Patch
	{
		private static int _lastLoggedFrame;
		private static void Postfix(object obj, bool __state)
		{
			if (UnityEngine.Time.frameCount == _lastLoggedFrame) return;
			_lastLoggedFrame = UnityEngine.Time.frameCount;

			if (obj is ISelectable s) MyDebug.ClickedOn(s);
		}
	}

	[StaticConstructorOnStartup]
	public static class LogSomeStuff
	{
		static LogSomeStuff()
		{
			LogStatPredicate(
				stat => stat.postProcessCurve != null && 
					stat.postProcessCurve.Evaluate(stat.defaultBaseValue) != stat.defaultBaseValue,
				"Stats with postProcessCurve:");

			//LogStatPredicate(stat => stat.cacheable == true, "cacheable stats");
			//LogStatPredicate(stat => stat.immutable == true, "immutable stats");

			//// PostProcessed value differs from defaultBase.
			//LogStatPredicate(def =>
			//	def.postProcessCurve != null &&
			//	def.defaultBaseValue != def.postProcessCurve.Evaluate(def.defaultBaseValue), "eval differ from base:");

			//LogApparelStatModifierPredicate(sm => sm.stat.defaultBaseValue != 0, "modifiers with non-zero base.");
			//LogAllApparelComps();
			//LogStatPredicate(def => def.statFactors != null, "statFactors exist:", def => def.statFactors);
			//LogStatPredicate(def => def.parts != null, "parts exist:", def => def.parts);

			//LogStatPredicate(
			//	def => def.workerClass != null && !(def.workerClass == typeof(RimWorld.StatWorker)),
			//	"workers (except RimWorld.StatWorker)",
			//	def => new[] { def.workerClass });

			//// Log all Apparels, where defaultBase > statBase.
			//// Exclude some (already noted).
			//{
			//	static bool filter(StatModifier sb) =>
			//		sb.stat.defaultBaseValue != 0f &&
			//		sb.stat.defaultBaseValue > sb.value &&
			//		!(sb.stat.defName == "MaxHitPoints" ||
			//			sb.stat.defName == "Mass" ||
			//			sb.stat.defName == "EquipDelay");
			//	LogApparels_Predicate(
			//		def => def.statBases != null &&
			//			def.statBases.Any(sb => filter(sb)),
			//		"Apparels: statBase < defaultBase",
			//		def => def.statBases
			//			.Where(sb => filter(sb))
			//			.Select(sb => $"" +
			//			$"{sb.stat.defName} " +
			//			$"V[{sb.value}] " +
			//			$"VEval[{sb.stat.postProcessCurve.Evaluate(sb.value)}] " +
			//			$"B[{sb.stat.defaultBaseValue}] " +
			//			$"Eval[{sb.stat.postProcessCurve.Evaluate(sb.stat.defaultBaseValue)}] " +
			//			$"Cat[{sb.stat.category}]"));
			//}

			//// Check evaluated values.
			//{
			//	static bool filter(StatModifier sb) =>
			//		sb.stat.postProcessCurve != null &&
			//		(sb.stat.defaultBaseValue != sb.stat.postProcessCurve.Evaluate(sb.stat.defaultBaseValue) ||
			//			sb.value != sb.stat.postProcessCurve.Evaluate(sb.value));
			//	LogApparels_Predicate(
			//		def => def.statBases != null &&
			//			def.statBases.Any(sb => filter(sb)),
			//		"Apparels: val != eval",
			//		def => def.statBases
			//			.Where(sb => filter(sb))
			//			.Select(sb => $"" +
			//				$"{sb.stat.defName} " +
			//				$"V[{sb.value}] " +
			//				$"VEval[{sb.stat.postProcessCurve.Evaluate(sb.value)}] " +
			//				$"B[{sb.stat.defaultBaseValue}] " +
			//				$"BEval[{sb.stat.postProcessCurve.Evaluate(sb.stat.defaultBaseValue)}] " +
			//				$"Cat[{sb.stat.category}]"));

			//	LogApparels_Predicate(
			//		def => def.equippedStatOffsets != null &&
			//			def.equippedStatOffsets.Any(sb => filter(sb)),
			//		"Apparels: Eq val != eval",
			//		def => def.equippedStatOffsets
			//			.Where(sb => filter(sb))
			//			.Select(sb => $"" +
			//				$"{sb.stat.defName} " +
			//				$"V[{sb.value}] " +
			//				$"VEval[{sb.stat.postProcessCurve.Evaluate(sb.value)}] " +
			//				$"B[{sb.stat.defaultBaseValue}] " +
			//				$"BEval[{sb.stat.postProcessCurve.Evaluate(sb.stat.defaultBaseValue)}] " +
			//				$"Cat[{sb.stat.category}]"));
			//}

			//LogStatPredicate(def => def.defaultBaseValue == 0f, "zero base");
			//LogStatPredicate(def => def.category == StatCategoryDefOf.BasicsNonPawnImportant, "'BasicsNonPawnImportant' cat.");
			//LogStatPredicate(def => def.category == StatCategoryDefOf.Basics, "'Basics' cat.");
			//LogStatPredicate(def => def.category == StatCategoryDefOf.BasicsPawn, "'BasicsPawn' cat.");
			//LogStatPredicate(def => def.category?.ToString().ContainsIgnoreCase("pawn") ?? false, "has 'pawn' in cat name.");
		}

		private static void LogAllApparelComps()
		{
			var hashset = new HashSet<string>();
			var things = DefDatabase<ThingDef>.AllDefsListForReading
				.Where(def => def.IsApparel && def.comps != null);


			foreach (var thing in things)
			{
				foreach (var comp in thing.comps)
				{
					hashset.Add(comp.compClass.Name);
				}
			}
			LoggerMy.LogNL($"All apparel comps:");
			foreach (var item in hashset)
			{
				LoggerMy.LogNL($"{item}");
			}
			LoggerMy.LogNL();
		}

		private static void LogApparelStatPredicate(Func<StatModifier, bool> predicate, string str = null)
		{
			var defs = DefDatabase<ThingDef>.AllDefsListForReading
				.Where(def => def.IsApparel &&
					def.statBases != null &&
					def.statBases.Any(predicate));

			LoggerMy.LogNL($"StatModifiers: {str ?? predicate.ToString()}");
			foreach (var def in defs)
			{
				LoggerMy.LogNL($"[{def.defName}]");
				foreach (var sm in def.statBases.Where(predicate))
				{
					var dd = sm.stat.defaultBaseValue;
					var min = sm.stat.minValue;
					LoggerMy.LogNL($"\t[{sm.stat.defName}] [{sm.stat.category}] Value[{sm.value}] Base[{dd}] Min[{min}]");
				}
			}
			LoggerMy.LogNL("");
		}

		private static void LogApparelStatModifierPredicate(Func<StatModifier, bool> predicate, string str = null)
		{
			var defs = DefDatabase<ThingDef>.AllDefsListForReading
				.Where(def => def.IsApparel &&
					def.equippedStatOffsets != null &&
					def.equippedStatOffsets.Any(predicate));

			LoggerMy.LogNL($"StatModifiers: {str ?? predicate.ToString()}");
			foreach (var def in defs)
			{
				LoggerMy.LogNL($"[{def.defName}]");
				foreach (var sm in def.equippedStatOffsets.Where(predicate))
				{
					var dd = sm.stat.defaultBaseValue;
					var min = sm.stat.minValue;
					LoggerMy.LogNL($"\t[{sm.stat.defName}] [{sm.stat.category}] Value[{sm.value}] Base[{dd}] Min[{min}]");
				}
			}
			LoggerMy.LogNL("");
		}

		private static void LogStatPredicate(
			Func<StatDef, bool> predicate,
			string str = null,
			Func<StatDef, IEnumerable> selector = null)
		{
			var defs = DefDatabase<StatDef>.AllDefsListForReading
				.Where(predicate);
			LoggerMy.LogNL($"StatDefs: {str ?? predicate.ToString()}");
			foreach (var def in defs)
			{
				var dd = def.defaultBaseValue;
				var min = def.minValue;
				var max = def.maxValue;
				var eval = def.postProcessCurve?.Evaluate(dd) ?? dd;
				LoggerMy.LogNL($"" +
					$"[{def.defName}] " +
					$"[{def.category}] " +
					$"Base[{dd}] " +
					$"Eval[{eval}] " +
					$"Min[{min}] " +
					$"Max[{max}] " +
					$"Select[{Math.Max(dd, min)}]");

				// Additional items to log for each def.
				if (selector != null)
				{
					var items = selector(def);
					LoggerMy.Log("\t");
					foreach (var item in items)
						LoggerMy.Log($"[{item}] ");
					LoggerMy.LogNL();
				}
			}
			LoggerMy.LogNL("");
		}

		private static void LogApparels_Predicate(
			Func<ThingDef, bool> predicate,
			string str = null,
			Func<ThingDef, IEnumerable> selector = null)
		{
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			var defs = DefDatabase<ThingDef>.AllDefsListForReading
				.Where(def => def.IsApparel && predicate(def));
			LoggerMy.LogNL($"Apparels: {str ?? predicate.ToString()}");
			foreach (var def in defs)
			{
				LoggerMy.LogNL($"" +
					$"[{def.defName}] " +
					$"[{def.category}] ");

				// Additional items to log for each def.
				if (selector != null)
				{
					var items = selector(def) ?? Array.Empty<object>();
					LoggerMy.Log("\t");
					foreach (var item in items)
						LoggerMy.Log($"[{item}] ");
					LoggerMy.LogNL();
				}
			}
			LoggerMy.LogNL("");
		}

		private static void LogStatDefs()
		{
			var stats = DefDatabase<StatDef>.AllDefsListForReading
							.GroupBy(stat => stat.modContentPack.PackageId);
			var list = new List<string>()
			{
				"ludeon.rimworld",
				"ludeon.rimworld.royalty",
				"ludeon.rimworld.ideology",
				"ludeon.rimworld.biotech",
				"ludeon.rimworld.anomaly",
				"ludeon.rimworld.odyssey",
				"ceteam.combatextended"
			};

			LoggerMy.LogNL($"New StatDefs:");
			foreach (var group in stats)
			{
				if (!list.Contains(group.Key))
				{
					LoggerMy.LogNL($"// {group.Key}");
					foreach (var stat in group)
					{
						LoggerMy.LogNL($"[MayRequire(\"{group.Key}\")] public static StatDef {stat.defName};");
					}
				}
			}
		}

		private static void ListAllStats()
		{
			foreach (var stat in DefDatabase<StatDef>.AllDefsListForReading)
			{
				//if (stat.defaultBaseValue != 0)
				{
					LoggerMy.LogNL($"Stat [{stat.defName}] Cat[{stat.category}] Default[{stat.defaultBaseValue}] Mod[{stat.modContentPack.PackageId}] ");
				}
			}
		}
	}
}
#endif