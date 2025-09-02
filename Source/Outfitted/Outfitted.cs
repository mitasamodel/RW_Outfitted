using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Outfitted
{
	[StaticConstructorOnStartup]
	public static class Outfitted
	{
		internal static bool showApparelScores;
		internal static bool isSaveStorageSettingsEnabled;
		internal static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve
		{
			{ new CurvePoint(0.0f, 0.0f), true },
			{ new CurvePoint(0.2f, 0.2f), true },
			{ new CurvePoint(0.22f, 0.6f), true },
			{ new CurvePoint(0.5f, 0.6f), true },
			{ new CurvePoint(0.52f, 1f), true }
		};

		static Outfitted()
		{
#if DEBUG
			Logger.Init();
#endif
			isSaveStorageSettingsEnabled = ModLister.GetActiveModWithIdentifier("savestoragesettings.kv.rw") != null;
			new Harmony("rimworld.outfitted").PatchAll();
			Log.Message("[Outfitted] loaded");
		}

		internal static void PruneNullStatPriorities(ExtendedOutfit outfit)
		{
			if (outfit?.StatPriorities == null) return;
			outfit.StatPriorities.RemoveAll(sp => sp == null || sp.Stat == null);
		}

		public static float ApparelScoreExtra(Pawn pawn, Apparel apparel, NeededWarmth neededWarmth)
		{
			if (!(pawn.outfits.CurrentApparelPolicy is ExtendedOutfit currentApparelPolicy))
			{
				Log.ErrorOnce("Outfitted :: Not an ExtendedOutfit, something went wrong.", 399441);
				return 0.0f;
			}

			// Starting score.
			float num = OutfittedMod.Settings.disableStartScore ? 0f : 0.1f;

			// Score offset.
			num += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.def.apparel.scoreOffset;

			// Score from appaerl itself.
			num += ApparelScorePriorities.RawPriorities(apparel, currentApparelPolicy);

			// If Pawn need pants / shirt.
			num += ApparelScoreNeeds.PawnNeedThis(pawn, apparel);

			// Ideology
			num += ApparelScoreNeeds.PawnNeedIdeology(pawn, apparel);

			if (currentApparelPolicy.AutoWorkPriorities)
				num += ApparelScoreAutoWorkPriorities(pawn, apparel);

			if (apparel.def.useHitPoints)
			{
				float hp = (float)apparel.HitPoints / apparel.MaxHitPoints;
				num *= HitPointsPercentScoreFactorCurve.Evaluate(Mathf.Clamp01(hp));
			}

			num += OutfittedMod.Settings.disableScoreOffset ? 0f : apparel.GetSpecialApparelScoreOffset();

			if (pawn != null && currentApparelPolicy != null)
				num += ApparelScoreInsulation.RawInsulation(pawn, apparel, currentApparelPolicy, neededWarmth);

			num = ApparelScoreNeeds.ModifiedWornByCorpse(pawn, apparel, currentApparelPolicy, num);
			return num;
		}

		internal static float ApparelScoreAutoWorkPriorities(Pawn pawn, Apparel apparel)
		{
			return WorkPriorities.WorktypeStatPriorities(pawn)
				.Select(sp => (apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetStatValue(sp.Stat) - sp.Stat.defaultBaseValue) * sp.Weight).Sum();
		}

		internal static void Notify_OutfitChanged(int id)
		{
			try
			{
				if (PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer) == null)
					return;
				foreach (Pawn pawn in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer).Where<Pawn>(i => i != null && i.outfits != null && i.outfits.CurrentApparelPolicy != null && i.outfits.CurrentApparelPolicy.id == id))
					pawn.mindState?.Notify_OutfitChanged();
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Outfitted.Notify_OutfitChanged: {0}", ex));
			}
		}

		public static void ReBuildWornScore(Pawn pawn, List<float> wornScores)
		{
			if (pawn == null || pawn.apparel == null || pawn.apparel.WornApparel == null) 
				throw new ArgumentNullException(nameof(pawn)); ;
			if (wornScores is null) 
				throw new ArgumentNullException(nameof(wornScores));

			wornScores.Clear();
			var worn = pawn.apparel.WornApparel;
			wornScores.Capacity = wornScores.Count + worn.Count;
			using (PawnContext.WhatIfNotWornScope(pawn))
			{
				foreach (var ap in worn)
				{
					if (ap == null)
					{
						Logger.Log_Warning("BuildWornScore: Unexpected Apparel-null in worn list.");
						wornScores.Add(0f);
					}
					else
						wornScores.Add(JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap));
				}
			}
		}
	}
}
