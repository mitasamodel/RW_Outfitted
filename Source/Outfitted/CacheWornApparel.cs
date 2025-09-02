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
		private static int _skipTicksOverlay = 12;
		private static readonly Dictionary<int, CacheWornApparelEntry> _cachedScores = new Dictionary<int, CacheWornApparelEntry>();

		public static List<float> GetScore(Pawn pawn)
		{
			if (pawn == null) throw new ArgumentNullException(nameof(pawn));

			int id = pawn.thingIDNumber;
			if (!_cachedScores.TryGetValue(id, out CacheWornApparelEntry entry))
			{
				entry = new CacheWornApparelEntry();
				_cachedScores[id] = entry;
			}

			if (entry.CachedTick < GenTicks.TicksGame)
			{
				entry.CachedTick = GenTicks.TicksGame + _skipTicksOverlay;
				entry.CachedScores = Outfitted.BuildWornScore(pawn);
			}

			return entry.CachedScores;
		}
	}
	internal sealed class CacheWornApparelEntry
	{
		internal int CachedTick { get; set; } = -1;
		internal List<float> CachedScores { get; set; } = new List<float>();
	}
}
