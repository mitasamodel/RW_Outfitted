using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
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

			if (!entry.CachedScores.TryGetValue(apId, out _) || entry.CachedTick < GenTicks.TicksGame)
			{
				entry.CachedTick = GenTicks.TicksGame + _skipTicks;
				entry.CachedScores = Outfitted.BuildWornScoreToDict(pawn);
			}

			return entry.CachedScores.TryGetValue(apId, out var score) ? score : 0.1f;
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
	}

	internal sealed class CacheWornApparelEntry
	{
		internal int CachedTick { get; set; } = -1;
		internal Dictionary<int, float> CachedScores { get; set; } = new Dictionary<int, float>();
	}
}
