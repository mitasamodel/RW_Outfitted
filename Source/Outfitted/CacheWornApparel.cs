using HarmonyLib;
using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Outfitted.RW_JustUtils;

namespace Outfitted
{
	internal sealed class CacheWornApparelEntry
	{
		internal int TickToUpdate { get; set; } = -1;
		internal Dictionary<int, float> CachedScores { get; set; } = new Dictionary<int, float>();
	}

	internal static class CacheWornApparel
	{
		[TweakValue("Outfitted", 0, 60)]
		private static int _skipTicks = 12;
		private static readonly Dictionary<int, CacheWornApparelEntry> _cachedScores = new Dictionary<int, CacheWornApparelEntry>();

		/// <summary>
		/// Return the score of specific apparel worn by pawn.
		/// Recalculate the cache if apparel not found or if 200ms passed.
		/// Score is stored per Pawn (id) per Apparel (id).
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="ap"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static float GetScore(Pawn pawn, Apparel ap)
		{
			if (pawn == null) throw new ArgumentNullException(nameof(pawn));
			if (ap == null) throw new ArgumentNullException(nameof(ap));

			int pawnId = pawn.thingIDNumber;
			int apId = ap.thingIDNumber;

			if (!_cachedScores.TryGetValue(pawnId, out CacheWornApparelEntry entry))
			{
				entry = new CacheWornApparelEntry();
				_cachedScores[pawnId] = entry;
			}

			if (!entry.CachedScores.TryGetValue(apId, out _) || entry.TickToUpdate < GenTicks.TicksGame)
			{
				entry.TickToUpdate = GenTicks.TicksGame + _skipTicks;
				entry.CachedScores = BuildWornScoreToDict(pawn);
			}

			return entry.CachedScores.TryGetValue(apId, out var score) ? score : 0f;
		}

		/// <summary>
		/// Legacy method. Return the list of score for worn apparels.
		/// Caller must ensure that the corresponding worn list is up-to-date.
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static List<float> GetScoreList(Pawn pawn)
		{
			if (pawn == null) throw new ArgumentNullException(nameof(pawn));
			var worn = pawn.apparel?.WornApparel ?? new List<Apparel>();
			var listScores = new List<float>(worn.Count);
			foreach (var ap in worn)
				listScores.Add(GetScore(pawn, ap));
			return listScores;
		}

		public static Dictionary<int, float> BuildWornScoreToDict(Pawn pawn)
		{
			var worn = pawn?.apparel?.WornApparel;
			worn ??= new List<Apparel>();
			Dictionary<int, float> scoresDict = new Dictionary<int, float>(worn.Count);

			foreach (var ap in worn)
			{
				if (ap == null)
				{
					LoggerMy.Log_Warning("BuildWornScoreToDict: Unexpected Apparel-null in worn list.");
					continue;
				}

				if (IsForbiddenApparel(pawn, ap))
					scoresDict[ap.thingIDNumber] = -1000f;
				else
					scoresDict[ap.thingIDNumber] = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			}
			return scoresDict;
		}

		/// <summary>
		/// This is used to correctly score already worn apparel.
		/// The original "ApparelScoreGain" doesn't apply to it, so this is how to do it.
		/// </summary>
		private static readonly Type ShieldType = AccessTools.TypeByName("CombatExtended.Apparel_Shield");
		private static bool IsForbiddenApparel(Pawn pawn, Apparel ap)
		{
			// Vanilla check for shield belt.
			if (ap.def == ThingDefOf.Apparel_ShieldBelt && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsWeaponUsingProjectiles)
				return true;

			// CE check for worn shields.
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (ShieldType != null && ShieldType.IsInstanceOfType(ap))
				{
					bool hasOneHandedWeapon = pawn?.equipment?.Primary?.def?.weaponTags?.Contains("CE_OneHandedWeapon") ?? false;
					if (pawn?.equipment?.Primary != null && !hasOneHandedWeapon)
						return true;
				}
			}

			return false;
		}
	}
}
