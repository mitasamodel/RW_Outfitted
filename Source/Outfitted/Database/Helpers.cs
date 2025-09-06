using RimWorld;
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
		internal static void AddBasicStats(ExtendedOutfit outfit)
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

		internal static ApparelPolicy GenerateBasedOnKnownVanillaOutfits(ApparelPolicy outfit)
		{
			ExtendedOutfit extendedOutfit = new ExtendedOutfit(outfit);
			switch (extendedOutfit.label)
			{
				case "Worker":
					extendedOutfit.AddRange(new List<StatPriority>()
					{
						new StatPriority(StatDefOf.MoveSpeed, 0.0f),
						new StatPriority(StatDefOf.WorkSpeedGlobal, 1f)
					});
					break;
				case "Soldier":
					extendedOutfit.AddRange(new List<StatPriority>()
					{
						new StatPriority(StatDefOf.ShootingAccuracyPawn, 2f),
						new StatPriority(StatDefOf.AccuracyShort, 1f),
						new StatPriority(StatDefOf.AccuracyMedium, 1f),
						new StatPriority(StatDefOf.AccuracyLong, 1f),
						new StatPriority(StatDefOf.MoveSpeed, 1f),
						new StatPriority(StatDefOf.ArmorRating_Blunt, 0.0f),
						new StatPriority(StatDefOf.ArmorRating_Sharp, 1f),
						new StatPriority(StatDefOf.MeleeDodgeChance, 0.0f),
						new StatPriority(StatDefOf.AimingDelayFactor, -2f),
						new StatPriority(StatDefOf.RangedWeapon_Cooldown, -2f),
						new StatPriority(StatDefOf.PainShockThreshold, 2f)
					});
					break;
				case "Nudist":
					extendedOutfit.AddRange(new List<StatPriority>()
					{
						new StatPriority(StatDefOf.MoveSpeed, 1f),
						new StatPriority(StatDefOf.WorkSpeedGlobal, 2f)
					});
					break;
				default:
					extendedOutfit.AddRange(new List<StatPriority>()
					{
						new StatPriority(StatDefOf.MoveSpeed, 1f),
						new StatPriority(StatDefOf.WorkSpeedGlobal, 2f),
						new StatPriority(StatDefOf.ArmorRating_Blunt, 1f),
						new StatPriority(StatDefOf.ArmorRating_Sharp, 1f)
					});
					break;
			}
			return extendedOutfit;
		}
	}
}
