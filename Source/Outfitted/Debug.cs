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

namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class LogSomeStuff
	{
		static LogSomeStuff()
		{
			var stats = DefDatabase<StatDef>.AllDefsListForReading;
			foreach (var stat in stats)
			{
				//if (stat.defaultBaseValue != 0)
				{
					Logger.LogNL($"Stat [{stat.defName}] Cat[{stat.category}] Def[{stat.defaultBaseValue}] ");
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

			float CE_Bulk = _apparel.GetStatValue(CE_StatDefOf.Bulk);
			Logger.LogNL($"Ap[{_apparel.def.defName}] CE_Bulk[{CE_Bulk}] Out[{_outfit.label}] Pawn[{_pawn.Name}]");
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
				if (stat.Stat.defName == "CarryWeight")
					Logger.LogNL($"Score: {Outfitted.ApparelScore(_apparel, stat.Stat)}");
			}
		}
	}

	public sealed class DebugSelectionMod : Mod
	{
		public DebugSelectionMod(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("yourname.rw.debugselection");
			harmony.PatchAll();
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
