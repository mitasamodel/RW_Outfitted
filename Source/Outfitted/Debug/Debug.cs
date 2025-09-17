using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using CombatExtended;
using System.Diagnostics.Eventing.Reader;
using Outfitted.RW_JustUtils;

#if DEBUG
namespace Outfitted
{
	public static class ScoreDebug
	{
		internal static bool DeepScorePriorities { get; } = true;

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
			Logger.LogNL($"[{pawn.Name}] Apparel score:");
			List<Apparel> wornApparel = pawn?.apparel?.WornApparel;
			if (wornApparel == null || wornApparel.Count == 0)
			{
				Logger.LogNL($"No apparel");
				return;
			}

			ExtendedOutfit policy = pawn.outfits.CurrentApparelPolicy as ExtendedOutfit;
			Map map = pawn.MapHeld ?? pawn.Map;
			var seasonTemp = map.mapTemperature.SeasonalTemp;
			var tempOffset = GetTempOffset(map);
			Logger.LogNL($"SeasonalTemp [{seasonTemp}] Offset [{tempOffset}] Final [{seasonTemp + tempOffset}]");
			foreach (var ap in wornApparel)
			{
				ShowScoreApp(ap, pawn, policy, whatIfNotWorn: true);
			}
			Logger.LogNL($"\tComfort: Min[{pawn.ComfortableTemperatureRange().min}] Max[{pawn.ComfortableTemperatureRange().max}]");
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
				Logger.LogNL($"[{condition.def.defName}] [{condition.TemperatureOffset()}] [{condition.def.temperatureOffset}]");
			}
			//GameConditionDefOf
			return temp;
		}

		private static void ShowScoreApp(Apparel ap, Pawn pawn, ExtendedOutfit policy, bool whatIfNotWorn = false)
		{
			if (ap == null || pawn == null || policy == null) return;

			if (!whatIfNotWorn)
				Logger.Log($"\tDef[{ap.def?.defName}] ");
			else
				Logger.Log($"\tDef[{ap.def?.defName}] ");

			float totalRaw = 0f;
			if (whatIfNotWorn)
			{
				using (PawnContext.WhatIfNotWornScope(pawn))
					totalRaw = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			}
			else totalRaw = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			Logger.Log($"RWScore[{totalRaw:F2}] ");

			var wornScore = CacheWornApparel.GetScoreList(pawn);
			if (!whatIfNotWorn)
				Logger.Log($"RWGain[{JobGiver_OptimizeApparel.ApparelScoreGain(pawn, ap, wornScore):F2}] ");

			float num = 0f;

			num += OutfittedMod.Settings.disableStartScore ? 0f : 0.1f;
			num += OutfittedMod.Settings.disableScoreOffset ? 0f : ap.def.apparel.scoreOffset;
			if (num != 0) Logger.Log($"Base[{num:F2}] ");

			// Priority stats.
			float prio = ApparelScorePriorities.RawPriorities(ap, policy);
			Logger.Log($"Prio[{prio:F2}] ");
			num += prio;

			// Pawn need this.
			float need = ApparelScoreNeeds.PawnNeedThis(pawn, ap);
			Logger.Log($"Need[{need:F2}] ");
			num += need;

			// Ideology
			float ideo = ApparelScoreNeeds.PawnNeedIdeology(pawn, ap);
			if (ideo != 0) Logger.Log($"Ideo[{ideo:F2}] ");
			num += ideo;

			// Auto work.
			float autoWork = 0f;
			if (policy.AutoWorkPriorities)
				autoWork += Outfitted.ApparelScoreAutoWorkPriorities(pawn, ap);
			if (autoWork != 0) Logger.Log($"Work[{autoWork:F2}] ");
			num += autoWork;

			// HP.
			float hp = 1f;
			if (ap.def.useHitPoints)
			{
				hp = (float)ap.HitPoints / ap.MaxHitPoints;
				num *= Outfitted.HitPointsPercentScoreFactorCurve.Evaluate(Mathf.Clamp01(hp));
			}
			if (hp != 1) Logger.Log($"HP[{hp:F2}] Num[{num:F2}] ");

			// Special score offset.
			float offset = OutfittedMod.Settings.disableScoreOffset ? 0 : ap.GetSpecialApparelScoreOffset();
			num += offset;
			if (offset != 0) Logger.Log($"Offset[{offset:F2}] ");

			// Insulation.
			float insulation = ApparelScoreInsulation.RawInsulation(pawn, ap, policy, NeededWarmth.Any);
			num += insulation;
			Logger.Log($"Ins[{insulation:F2}] ");

			// Corpse.
			num = ApparelScoreNeeds.ModifiedWornByCorpse(pawn, ap, policy, num);
			Logger.Log($"Final[{num:F2}] ");


			Logger.LogNL("");
		}

		private static void ShowApparelStatScores(Apparel ap)
		{
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				float CE_Bulk = ap.GetStatValue(StatDefOf_CE.Bulk);
				float CE_WornBulk = ap.GetStatValue(StatDefOf_CE.WornBulk);
				Logger.LogNL($"Ap[{ap.def.defName}] CE_Bulk[{CE_Bulk}] Worn[{CE_WornBulk}] Out[{_outfit.label}] Pawn[{_selectedPawn.Name}]");
			}
			else
				Logger.LogNL($"Ap[{ap.def.defName}] Out[{_outfit.label}] Pawn[{_selectedPawn.Name}]");

			foreach (var stat in _outfit.StatPriorities)
			{
				var baseStatQ = ap.GetStatValue(stat.Stat);
				var baseStat = ap.def.GetStatValueAbstract(stat.Stat, null);
				var wearStat = ap.def.equippedStatOffsets.GetStatOffsetFromList(stat.Stat);
				float wearStatQ = StatWorker.StatOffsetFromGear(ap, stat.Stat);


				Logger.LogNL($"\t-{stat.Stat.defName} " +
					$"Def[{stat.Stat.defaultBaseValue}] " +
					$"Pure[{baseStat}] " +
					$"Q[{baseStatQ}] " +
					$"Eq[{wearStat}] " +
					$"EqQ[{wearStatQ}] " +
					$"Sum[{baseStat + wearStat}] " +
					$"SumQ[{baseStatQ + wearStat}] " +
					$"Sum_Q[{baseStat + wearStatQ}] "
				);
				Logger.LogNL($"\tScore: {ApparelScorePriorities.ApparelScore(ap, stat.Stat)}");
			}
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

			if (obj is ISelectable s) ScoreDebug.ClickedOn(s);
		}
	}

	[StaticConstructorOnStartup]
	public static class LogSomeStuff
	{
		static LogSomeStuff()
		{
			// StatDefs
			//var defs = DefDatabase<StatDef>.AllDefsListForReading
			//	.Where(def => def.minValue > def.defaultBaseValue && def.minValue >= 0f && def.defaultBaseValue >= 0f);
			//Logger.LogNL("StatDefs: (minValue > defaultBaseValue) >= 0");
			//foreach (var def in defs)
			//{
			//	var dd = def.defaultBaseValue;
			//	var min = def.minValue;
			//	Logger.LogNL($"[{def.defName}] [{def.category}] Base[{dd}] Min[{min}] Select[{Math.Max(dd, min)}]");
			//}
			//Logger.LogNL("");

			//defs = DefDatabase<StatDef>.AllDefsListForReading
			//	.Where(def => def.minValue > def.defaultBaseValue && (def.minValue < 0f || def.defaultBaseValue < 0f));
			//Logger.LogNL("StatDefs: (minValue > defaultBaseValue) Any < 0");
			//foreach (var def in defs)
			//{
			//	var dd = def.defaultBaseValue;
			//	var min = def.minValue;
			//	Logger.LogNL($"[{def.defName}] [{def.category}] Base[{dd}] Min[{min}] Select[{Math.Max(dd, min)}]");
			//}
			//Logger.LogNL("");

			//defs = DefDatabase<StatDef>.AllDefsListForReading
			//	.Where(def => def.minValue != def.defaultBaseValue && def.defaultBaseValue < 0f);
			//Logger.LogNL("StatDefs: (minValue != defaultBaseValue) defaultBaseValue < 0");
			//foreach (var def in defs)
			//{
			//	var dd = def.defaultBaseValue;
			//	var min = def.minValue;
			//	Logger.LogNL($"[{def.defName}] [{def.category}] Base[{dd}] Min[{min}] Select[{Math.Max(dd, min)}]");
			//}
			//Logger.LogNL("");

			//defs = DefDatabase<StatDef>.AllDefsListForReading.
			//	Where(def => def.category?.ToString().ContainsIgnoreCase("Apparel") ?? false);
			//Logger.LogNL("StatDefs: category contains 'Apparel'");
			//foreach (var def in defs)
			//{
			//	var dd = def.defaultBaseValue;
			//	var min = def.minValue;
			//	Logger.LogNL($"[{def.defName}] [{def.category}] Base[{dd}] Min[{min}] Select[{Math.Max(dd, min)}]");
			//}
			//Logger.LogNL("");

			//LogStatDefs();

			//var defs = DefDatabase<ThingDef>.AllDefsListForReading
			//	.Where(def => def.equippedStatOffsets != null && def.equippedStatOffsets.Count > 0);

			//Logger.LogNL("Defs with equipped offset");
			//Dictionary<string, int> qty = new Dictionary<string, int>();
			//foreach (var def in defs)
			//{
			//	Logger.LogNL($"Def[{def.defName}]");
			//	foreach (var stat in def.equippedStatOffsets)
			//	{
			//		var st = def.GetStatValueDef(stat.stat);
			//		Logger.LogNL($"\tStat[{stat.stat.defName}] Cat[{stat.stat.category}] Default[{stat.stat.defaultBaseValue}] This[{st}] Equipped[{stat.value}]");

			//	}
			//	Logger.LogNL("");
			//}



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

			Logger.LogNL($"New StatDefs:");
			foreach (var group in stats)
			{
				if (!list.Contains(group.Key))
				{
					Logger.LogNL($"// {group.Key}");
					foreach (var stat in group)
					{
						Logger.LogNL($"[MayRequire(\"{group.Key}\")] public static StatDef {stat.defName};");
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
					Logger.LogNL($"Stat [{stat.defName}] Cat[{stat.category}] Default[{stat.defaultBaseValue}] Mod[{stat.modContentPack.PackageId}] ");
				}
			}
		}
	}
}
#endif