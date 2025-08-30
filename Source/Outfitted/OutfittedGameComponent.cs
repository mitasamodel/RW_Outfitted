using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	/// <summary>
	/// Executes @ game start & load.
	/// Checks if there are some null-stats in outfit policies.
	/// null can come from other mods, which allow to restore outfits or exchange them between saves (e.g. Export Agency).
	/// </summary>
	public sealed class OutfittedGameComponent : GameComponent
	{
		public OutfittedGameComponent(Game game) { }

		public override void StartedNewGame()
		{
			CleanAllPolicies();
		}

		public override void LoadedGame()
		{
			CleanAllPolicies();
		}

		private static void CleanAllPolicies()
		{
			try
			{
				var db = Current.Game?.outfitDatabase;
				if (db == null) return;

				foreach (var outfit in db.AllOutfits)
				{
					if (outfit is ExtendedOutfit eo)
						Outfitted.PruneNullStatPriorities(eo);
				}
			}
			catch (System.Exception ex)
			{
				Log.Warning($"[Outfitted] Cleaning policies failed: {ex}");
			}
		}
	}
}
