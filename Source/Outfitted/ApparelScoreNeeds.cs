using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace Outfitted
{
	internal static class ApparelScoreNeeds
	{
		private const float nakedOffset = 2f;
		private const float ideologyOffset = 2f;

		// Return additional score if apparel needed due to nudity 
		internal static float PawnNeedThis(Pawn pawn, Apparel apparel)
		{
			if (pawn == null || apparel?.def?.apparel == null) return 0f;
			float result = 0f;

			List<ThingDef> conflicts = new List<ThingDef>();
			foreach(var ap in pawn.apparel.WornApparel)
			{
				if ( !ApparelUtility.CanWearTogether(ap.def, apparel.def, pawn.RaceProps.body))
				{
					conflicts.Add(ap.def);
				}
			}

			// Pants.
			if (CoversLegSkin(apparel.def))
			{
				// TODO: need to exclude other items, which this item will force to not wear.
				// VAE_Apparel_MilitaryUniform covers both - legs and torso.


				if (PawnCareAboutNaked(pawn) && !PawnLegsCovered(pawn, conflicts))
				{
					result += nakedOffset;
				}
			}

			// Pawn wants shirt
			if (CoversTorso(apparel.def))
			{
				// TODO: the same

				if (PawnCareAboutNaked(pawn) && PawnCareAboutTorso(pawn) && !PawnTorsoCovered(pawn, conflicts))
				{
					result += nakedOffset;
				}
			}

			return result;

			//var skinLayer = ApparelLayerDefOf.OnSkin;
			//var legs = BodyPartGroupDefOf.Legs;
		}

		private static bool PawnCareAboutTorso(Pawn pawn)
		{
			if (pawn == null) return false;
			//if (!ModsConfig.IdeologyActive) return pawn.gender == Gender.Female;

			// For now.
			// TODO: How to check Thought "naked" for female if Ideology is active and it is acceptable to not wear a shirt.
			return pawn.gender == Gender.Female;
		}

		private static bool PawnCareAboutNaked(Pawn pawn)
		{
			return ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.Naked, true);
		}

		// Pawn wear shirt or smth, what covers torso.
		private static bool PawnTorsoCovered(Pawn pawn, List<ThingDef> exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => !exclude.Contains(ap.def) && CoversTorso(ap.def));
		}

		// Pawn wear other item, which covers same area.
		private static bool PawnLegsCovered(Pawn pawn, List<ThingDef> exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => !exclude.Contains(ap.def) && CoversLegSkin(ap.def));
		}

		// Can be used as pants?
		private static bool CoversLegSkin(ThingDef def)
		{
			var apparel = def?.apparel;
			if (apparel == null) return false;

			return apparel.layers.Contains(ApparelLayerDefOf.OnSkin) && apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs);
		}

		// Can be used as shirt (or anything what covers torso)?
		private static bool CoversTorso(ThingDef def)
		{
			var apparel = def?.apparel;
			if (apparel == null) return false;

			return apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso);
		}

		internal static float ModifiedWornByCorpse(Pawn pawn, Apparel apparel, ExtendedOutfit currentApparelPolicy, float num)
		{
			if (currentApparelPolicy.PenaltyWornByCorpse && apparel.WornByCorpse && ThoughtUtility.CanGetThought(pawn, ThoughtDefOf.DeadMansApparel, true))
			{
				num -= 0.5f;
				if (num > 0.0f)
					num *= 0.1f;
			}

			return num;
		}

		internal static float PawnNeedIdeology(Pawn pawn, Apparel ap)
		{
			if (pawn?.apparel?.AllRequirements == null || ap == null) return 0f;

			foreach (ApparelRequirementWithSource req in pawn.apparel.AllRequirements)
			{
				if (req.requirement.RequiredForPawn(pawn, ap.def))
				{
					return ideologyOffset;
				}
			}
			return 0f;
		}
	}
}
