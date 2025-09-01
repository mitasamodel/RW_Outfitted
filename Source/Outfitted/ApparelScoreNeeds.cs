using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	internal static class ApparelScoreNeeds
	{
		private const float nakedOffset = 1f;

		/// <summary>
		/// Return additional score if apparel needed due to nudity 
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="apparel"></param>
		/// <param name="whatIfNotWorn">Run the scoring like if this apparel will be removed and will be worn again</param>
		/// <returns></returns>
		internal static float PawnNeedThis(Pawn pawn, Apparel apparel, bool whatIfNotWorn = false)
		{
			if (pawn == null || apparel?.def?.apparel == null) return 0f;

			// Exclude this apparel if it is a run to get real _worn_ score.
			Apparel exclude = whatIfNotWorn ? apparel : null;

			// Pawn wants pants
			if (IsDefPants(apparel.def))
			{
				if (PawnCareAboutNaked(pawn) && !PawnWearPants(pawn, exclude))
				{
					return nakedOffset;
				}
			}
			// Pawn wants shirt
			else if (CoversTorso(apparel.def))
			{
				if (PawnCareAboutNaked(pawn) && PawnCareAboutTorso(pawn) && !PawnTorsoCovered(pawn, exclude))
				{
					return nakedOffset;
				}
			}

			return 0f;

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
		private static bool PawnTorsoCovered(Pawn pawn, Apparel exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => ap != exclude && CoversTorso(ap.def));
		}

		// Pawn wear pants or smth, what covers same area.
		private static bool PawnWearPants(Pawn pawn, Apparel exclude)
		{
			var worn = pawn.apparel?.WornApparel;
			if (worn == null) return false;

			return worn.Any(ap => ap != exclude && IsDefPants(ap.def));
		}

		// Can be used as pants?
		private static bool IsDefPants(ThingDef def)
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
	}
}
