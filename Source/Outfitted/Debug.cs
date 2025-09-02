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

#if DEBUG
namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class LogSomeStuff
	{
		static LogSomeStuff()
		{
			LogStatDefs();

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

	public static class ScoreDebug
	{
		private static Apparel _apparel;
		private static ExtendedOutfit _outfit;
		private static Pawn _pawn;
		private static List<float> _wornScore = new List<float>();

		public static void ClickedOn(ISelectable selectable)
		{
			if (selectable is Pawn pawn)
			{
				_pawn = pawn;
				if (pawn.outfits.CurrentApparelPolicy is ExtendedOutfit outfit)
					_outfit = outfit;

				_wornScore = Outfitted.BuildWornScore(pawn);

				ShowScoreWornApparel(pawn);
			}
			else if (selectable is Apparel apparel)
			{
				_apparel = apparel;
				ShowScoreApp(_apparel, _pawn, _outfit);
				ShowIfWorn(_apparel, _pawn, _outfit);
			}

			if (_apparel == null || _outfit == null || _pawn == null) return;

			//ShowApparelStatScores();
		}

		private static void ShowIfWorn(Apparel ap, Pawn pawn, ExtendedOutfit outfit)
		{

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

			List<float> gameScore = new List<float>();
			gameScore = Outfitted.BuildWornScore(pawn);
			ExtendedOutfit policy = pawn.outfits.CurrentApparelPolicy as ExtendedOutfit;
			Map map = pawn.MapHeld ?? pawn.Map;
			var seasonTemp = map.mapTemperature.SeasonalTemp;
			var tempOffset = GetTempOffset(map);
			Logger.LogNL($"SeasonalTemp [{seasonTemp}] Offset [{tempOffset}] Final [{seasonTemp + tempOffset}]");
			foreach (var ap in wornApparel)
			{
				//ShowScoreApp(ap, pawn, policy);
				ShowScoreApp(ap, pawn, policy, whatIfNotWorn: true);
			}
			Logger.LogNL($"\tMin[{pawn.ComfortableTemperatureRange().min}] Max[{pawn.ComfortableTemperatureRange().max}]");
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
				Logger.Log($"\tDefNormScore[{ap.def?.defName}] ");
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

			if (!whatIfNotWorn)
				Logger.Log($"RWGain[{JobGiver_OptimizeApparel.ApparelScoreGain(pawn, ap, _wornScore):F2}] ");

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

		private static void ShowApparelStatScores()
		{
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				float CE_Bulk = _apparel.GetStatValue(StatDefOf_CE.Bulk);
				float CE_WornBulk = _apparel.GetStatValue(StatDefOf_CE.WornBulk);
				Logger.LogNL($"Ap[{_apparel.def.defName}] CE_Bulk[{CE_Bulk}] Worn[{CE_WornBulk}] Out[{_outfit.label}] Pawn[{_pawn.Name}]");
			}
			else
				Logger.LogNL($"Ap[{_apparel.def.defName}] Out[{_outfit.label}] Pawn[{_pawn.Name}]");

			foreach (var stat in _outfit.StatPriorities)
			{
				var baseStatQ = _apparel.GetStatValue(stat.Stat);
				var baseStat = _apparel.def.GetStatValueAbstract(stat.Stat, null);
				var wearStat = _apparel.def.equippedStatOffsets.GetStatOffsetFromList(stat.Stat);
				float wearStatQ = StatWorker.StatOffsetFromGear(_apparel, stat.Stat);


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
				Logger.LogNL($"\tScore: {ApparelScorePriorities.ApparelScore(_apparel, stat.Stat)}");
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
}
#endif