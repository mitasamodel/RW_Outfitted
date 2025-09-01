using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	/// <summary>
	/// Stores extra per-Pawn context used during job processing. This exists to work
	/// around Harmony's limitation: you cannot add parameters to existing method
	/// signatures. Instead, call sites set a flag here, and patched methods read it.
	/// 
	/// Example:
	/// Vanilla calls a helper twice (e.g., ApparelScoreRaw).
	/// Our transpiler injects a call in the helper and needs to behave differently depending on which call site invoked it.
	///	Harmony cannot modify the original method parameters, it is a compile-time thing.
	/// Workaround: patch one of the calls site in the main method. It enables a per-Pawn flag before calling helper.
	/// When our method gets executed, it checkes the per-Pawn flag.
	/// 
	/// This class helps to work with Pawns. Therefore, the data here is stored in Dictionaries per Pawn ID to distinguish threads.
	/// </summary>
	public static class PawnContext
	{
		/// <summary>
		/// Set and return the WhatIfNotWorn flag, which is then used in:
		///		JobGiver_OptimizeApparel_ApparelScoreRaw_Patch.ApparelScoreExtra
		///		
		/// Use case: in original JobGiver_OptimizeApparel class the helper method ApparelScoreRaw get called twise.
		/// Our ApparelScoreExtra method is inserted into the middle of ApparelScoreRaw method.
		/// For one of the call the flag WhatIfNotWorn is required.
		/// 
		/// Key is pawn.thingIDNumber.
		/// </summary>

		// Each thread holds its own dictionary of PawnID -> flag.
		private static readonly ThreadLocal<Dictionary<int, bool>> WhatIfNotWornByThread
			= new ThreadLocal<Dictionary<int, bool>>(() => new Dictionary<int, bool>());

		// Returns true if the current thread has the WhatIfNotWorn flag set for this pawn.
		public static bool GetWhatIfNotWorn(Pawn pawn)
		{
			if (pawn == null) return false;

			var map = WhatIfNotWornByThread.Value;
			return map.TryGetValue(pawn.thingIDNumber, out var result) && result;
		}

		// Sets or clears the WhatIfNotWorn flag for this pawn on the current thread.
		public static void SetWhatIfNotWorn(Pawn pawn, bool flag)
		{
			if (pawn == null) return;

			var map = WhatIfNotWornByThread.Value;
			if (flag)
			{
				map[pawn.thingIDNumber] = true;
			}
			else
			{
				map.Remove(pawn.thingIDNumber);
			}
		}
		/// <summary>
		/// Scope helper: sets the flag to true for the duration of the scope,
		/// then restores the previous value (on the same thread).
		/// Usage: using (PawnContext.WhatIfNotWornScope(pawn)) { /* internal logic */ }
		/// </summary>
		public static IDisposable WhatIfNotWornScope(Pawn pawn) => new WhatIfNotWornScopeImpl(pawn);
		private sealed class WhatIfNotWornScopeImpl : IDisposable
		{
			private readonly Pawn _pawn;
			private readonly bool _prev;

			public WhatIfNotWornScopeImpl(Pawn pawn)
			{
				_pawn = pawn;
				_prev = GetWhatIfNotWorn(pawn);
				SetWhatIfNotWorn(pawn, true);
			}

			public void Dispose()
			{
				// Restore previous value (removes entry if it was false).
				SetWhatIfNotWorn(_pawn, _prev);
			}
		}
	}
}
