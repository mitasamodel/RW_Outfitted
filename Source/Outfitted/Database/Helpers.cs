using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted.Database
{
	internal class Helpers
	{
		internal static void AddBasicsStats(ExtendedOutfit outfit)
		{
			// CE
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				outfit.AddRange(new List<StatPriority>()
				{
					new StatPriority(StatDefOf_CE.CarryBulk, OutfittedMod.Settings.CECurryBulk),
					new StatPriority(StatDefOf_CE.CarryWeight, OutfittedMod.Settings.CECarryWeight)
				});
			}

			// Vanilla
			// Only if no CE presented. For CE there are better stats to be used.
			if (!ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				if (StatDefOf_Rimworld.Mass != null)
					outfit.AddStatPriority(StatDefOf_Rimworld.Mass, OutfittedMod.Settings.mass);
			}
		}
	}
}
