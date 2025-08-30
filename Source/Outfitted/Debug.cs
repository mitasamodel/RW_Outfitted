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

#if DEBUG
namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class LogSomeStuff
	{
		static LogSomeStuff()
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

			//ListAllStats();
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

		public static void ClickedOn(ISelectable selectable)
		{
			if (selectable is Pawn pawn)
			{
				_pawn = pawn;
				if (_pawn.outfits.CurrentApparelPolicy is ExtendedOutfit outfit)
					_outfit = outfit;
			}
			else if (selectable is Apparel apparel)
			{
				_apparel = apparel;
			}

			if (_apparel == null || _outfit == null || _pawn == null) return;

			//ShowApparelStatScores();
		}

		private static void ShowApparelStatScores()
		{
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				float CE_Bulk = _apparel.GetStatValue(StatDefOf_CE.Bulk);
				Logger.LogNL($"Ap[{_apparel.def.defName}] CE_Bulk[{CE_Bulk}] Out[{_outfit.label}] Pawn[{_pawn.Name}]");
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
				Logger.LogNL($"\tScore: {Outfitted.ApparelScore(_apparel, stat.Stat)}");
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