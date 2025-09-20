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
	/// <summary>
	/// Replace for original StatWorker.
	/// Reasons:
	/// 1. Original worker assigns stat.defaultBaseValue for everything even if apparel doesn't have this stat.
	/// 2. Some apparel has real stat lower, than defaultBaseValue (e.g. shield recharge rate) and therefore
	///		simple reduction by defaultBaseValue will not work.
	///	3. stat.postProcessCurve applied to apparels stat, which should not be. It is for final effect calculation.
	///		Example: cooking speed. If raw stat is 0, then postProcessCurve results in 0.4. That means all apparel
	///		have at least 0.4 cooking speed stat, which is absurd.
	///	Check out a diagram for the riginal in the file "<...>\Ideas\diagrams\StatWorker.GetValue.drawio".
	/// </summary>
	public static class StatWorkerOutfitted
	{
		// Cache for stat, which will never change. Example: DoorOpenSpeed.
		private static readonly Dictionary<(int ThingID, StatDef StatDef), float> _immutableStatCache =
			new Dictionary<(int ThingID, StatDef StatDef), float>();

		// Cache for all other stats. Will update in some intervals.
		private const int TicksUpdateInterval = 120;
		private static readonly Dictionary<int, StatCacheEntry> _statCache = new Dictionary<int, StatCacheEntry>();


		public static float GetStatValue(this Apparel apparel, StatDef stat)
		{
			float value;

			// Immutable stats cache.
			if (stat.immutable)
			{
				var key = (apparel.thingIDNumber, stat);
				if (!_immutableStatCache.TryGetValue(key, out value))
				{
					value = GetValue(apparel, stat);
					_immutableStatCache[key] = value;
				}

				return value;
			}

			// Normal cache.
			if (!_statCache.TryGetValue(apparel.thingIDNumber, out StatCacheEntry entry))
			{
				entry = new StatCacheEntry();
				_statCache[apparel.thingIDNumber] = entry;
			}
			if (!entry.StatCache.TryGetValue(stat, out value) || entry.UpdateTick <= GenTicks.TicksGame)
			{
				value = GetValue(apparel, stat);
				entry.UpdateTick = GenTicks.TicksGame + TicksUpdateInterval;
				entry.StatCache[stat] = value;
			}
			return value;
		}

		private static float GetValue(Apparel thing, StatDef stat)
		{
			// TODO
			return 0f;
		}

		/// <summary>
		/// Called when an apparel is destroyed.
		/// TODO: this must be called on Thing.Destroy / PostDestroy -> Harmony patch needed. 
		/// </summary>
		public static void ClearCacheFor(Apparel apparel)
		{
#if DEBUG
			if (MyDebug.ApparelStatsCache)
				LoggerMy.LogNL($"Clearing cache for [{apparel.def.defName}]");
			using var _ = LoggerMy.Scope();
#endif
			// Immutable stats.
			var keys = _immutableStatCache.Keys.Where(key => key.ThingID == apparel.thingIDNumber).ToList();
			for (int i = 0; i < keys.Count; i++)
			{
#if DEBUG
				if (MyDebug.ApparelStatsCache)
					LoggerMy.LogNL($"Immutable cache [{keys[i]}]");
#endif
				_immutableStatCache.Remove(keys[i]);
			}

			// Normal stats.
			_statCache.Remove(apparel.thingIDNumber);
		}
	}

	internal sealed class StatCacheEntry
	{
		internal int UpdateTick { get; set; } = -1;
		internal Dictionary<StatDef, float> StatCache { get; set; } = new Dictionary<StatDef, float>();
	}
}
