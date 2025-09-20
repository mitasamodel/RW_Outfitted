using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Outfitted.RW_JustUtils;
using System.Linq.Expressions;

namespace Outfitted
{
	/// <summary>
	/// Replace for original StatWorker.
	/// Reasons:
	/// 1. Original worker assigns stat.defaultBaseValue for everything even if apparel doesn't have this stat.
	/// 2. Some apparel has real stat smaller, than defaultBaseValue (e.g. shield recharge rate) and therefore
	///		simple reduction by defaultBaseValue will not work.
	///	3. stat.postProcessCurve applied to apparels stat, which should not be. It is for final effect calculation.
	///		Example: cooking speed. If raw stat is 0, then postProcessCurve results in 0.4. That means all apparels
	///		have at least 0.4 cooking speed stat, which is absurd.
	///	Check out a diagram for the original in the file "<...>\Ideas\diagrams\StatWorker.GetValue.drawio".
	/// </summary>
	public static class StatWorkerOutfitted
	{
		// Cache for stat, which will never change. Example: DoorOpenSpeed.
		private static readonly Dictionary<(int ThingID, StatDef StatDef), float> _immutableStatCache =
			new Dictionary<(int ThingID, StatDef StatDef), float>();

		// Cache for all other stats. Recache in some intervals.
		private const int TicksUpdateInterval = 120;
		private static readonly Dictionary<int, StatCacheEntry> _statCache = new Dictionary<int, StatCacheEntry>();

		/// <summary>
		/// Outfitted implementation.
		/// Checks caches and return either cached value or newly got.
		/// Vanilla caches stats only if stat.cacheable is true.
		/// For our purposes (score) we can cache all stats and recache them in some intervals if needed.
		/// </summary>
		public static float GetStatValue(this Apparel apparel, StatDef stat)
		{
#if DEBUG
			if (apparel == null) throw new ArgumentNullException(nameof(apparel));
			if (stat == null) throw new ArgumentNullException(nameof(stat));
#else
			if (apparel == null) { Logger.Log_ErrorOnce("Null apparel passed to GetStatValue", 0x0F00D201); return 0f; }
			if (stat == null) { Logger.Log_ErrorOnce("Null stat passed to GetStatValue", 0x0F00D202); return 0f; }
#endif

			float value;

			// Immutable stats cache.
			if (stat.immutable)
			{
				var key = (apparel.thingIDNumber, stat);
				if (!_immutableStatCache.TryGetValue(key, out value))
				{
					value = GetValue(apparel, stat);
					_immutableStatCache[key] = value;
#if DEBUG
					//if (MyDebug.ApparelStatsCache)
					//	Logger.LogNL($"New immutable cache entry [{apparel.def.defName} - {stat.defName}][{value}]");
#endif
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
				entry.UpdateTick = GenTicks.TicksGame + TicksUpdateInterval + Verse.Rand.Range(0, TicksUpdateInterval / 3);
				entry.StatCache[stat] = value;
#if DEBUG
				//if (MyDebug.ApparelStatsCache)
				//	Logger.LogNL($"New cache entry [{apparel.def.defName} - {stat.defName}][{value}]");
#endif
			}
			return value;
		}

		/// <summary>
		/// Calculate stat value of the apparel.
		/// </summary>
		private static float GetValue(Apparel apparel, StatDef stat)
		{
			float value = GetBaseValue(apparel, stat);
			value = ApplyComps(apparel, stat, value);
			value = ApplyParts(apparel, stat, value);
			return value;
		}

		/// <summary>
		/// Different to vanilla: do not use stat.defaultBaseValue, return 0 instead.
		/// </summary>
		private static float GetBaseValue(Apparel apparel, StatDef stat)
		{
			if (apparel.def.statBases != null)
			{
				for (int i = 0; i < apparel.def.statBases.Count; i++)
				{
					var sb = apparel.def.statBases[i];
					if (sb.stat == stat)
					{
						return sb.value;
					}
				}
			}

			return 0f;
		}

		/// <summary>
		/// If comps modify stat, use it. Default offset is 0 and factor is 1.
		/// </summary>
		private static float ApplyComps(Apparel apparel, StatDef stat, float value)
		{
			var comps = apparel.AllComps;
			if (comps == null) return value;

			foreach (var comp in comps)
				value += comp.GetStatOffset(stat);

			foreach (var comp in comps)
				value *= comp.GetStatFactor(stat);

			return value;
		}

		/// <summary>
		/// Apply parts. Example: stuff.
		/// </summary>
		private static float ApplyParts(Apparel apparel, StatDef stat, float value)
		{
			if (stat.parts != null)
			{
				foreach ( var part in stat.parts)
				{
					part.TransformValue(StatRequest.For(apparel), ref value);
				}
			}
			return value;
		}

		/// <summary>
		/// Called when an apparel is destroyed.
		/// TODO: this must be called on Thing.Destroy / PostDestroy -> Harmony patch needed. 
		/// </summary>
		public static void ClearCacheFor(Apparel apparel)
		{
#if DEBUG
			if (MyDebug.ApparelStatsCache)
				Logger.LogNL($"Clearing cache for [{apparel.def.defName}]");
			using var _ = Logger.Scope();
#endif
			// Immutable stats.
			var keys = _immutableStatCache.Keys.Where(key => key.ThingID == apparel.thingIDNumber).ToList();
			for (int i = 0; i < keys.Count; i++)
			{
#if DEBUG
				if (MyDebug.ApparelStatsCache)
					Logger.LogNL($"Immutable cache [{keys[i]}]");
#endif
				_immutableStatCache.Remove(keys[i]);
			}

			// Normal stats.
			_statCache.Remove(apparel.thingIDNumber);
		}

		public static void ClearAllCaches()
		{
#if DEBUG
			if (MyDebug.ApparelStatsCache)
				Logger.LogNL($"Clear all stat caches");
#endif
			_immutableStatCache.Clear();
			_statCache.Clear();
		}
	}

	internal sealed class StatCacheEntry
	{
		internal int UpdateTick { get; set; } = -1;
		internal Dictionary<StatDef, float> StatCache { get; set; } = new Dictionary<StatDef, float>();
	}
}
