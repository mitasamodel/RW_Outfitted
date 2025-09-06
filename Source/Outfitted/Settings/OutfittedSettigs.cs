using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	public class OutfittedSettigs : ModSettings
	{
		/// General.
		// UI options.
		public const bool draggableWindow_default = true;
		public const bool nonBlockingWindow_default = true;
		public const bool displayModName_default = true;
		public const bool displayCatName_default = true;
		public const bool includeDescrForStatSearch_default = true;

		public bool draggableWindow = draggableWindow_default;
		public bool nonBlockingWindow = nonBlockingWindow_default;
		public bool displayModName = displayModName_default;
		public bool displayCatName = displayCatName_default;
		public bool includeDescrForStatSearch = includeDescrForStatSearch_default;

		// Score options.
		public const bool disableStartScore_default = true;
		public const bool disableScoreOffset_default = true;
		public const bool sumScoresInsteadOfAverage_default = true;
		public const bool insScoreBasedOnConditions_default = true;

		public bool disableStartScore = disableStartScore_default;
		public bool disableScoreOffset = disableScoreOffset_default;
		public bool sumScoresInsteadOfAverage = sumScoresInsteadOfAverage_default;
		public bool insScoreBasedOnConditions = insScoreBasedOnConditions_default;

		/// Tune values.
		// Offsets.
		public const float nakedOffset_default = 2f;
		public const float ideologyOffset_default = 2f;

		public float nakedOffset = nakedOffset_default;
		public float ideologyOffset = nakedOffset_default;

		// CE.
		public const float CECurryBulk_default = 1f;
		public const float CECarryWeight_default = 1f;

		public float CECurryBulk = CECurryBulk_default;
		public float CECarryWeight = CECarryWeight_default;

		// Vanilla.
		public const float mass_default = -0.5f;

		public float mass = mass_default;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref draggableWindow, "draggableWindow", draggableWindow_default);
			Scribe_Values.Look(ref nonBlockingWindow, "draggableWindow", nonBlockingWindow_default);
			Scribe_Values.Look(ref displayModName, "draggableWindow", displayModName_default);
			Scribe_Values.Look(ref displayCatName, "draggableWindow", displayCatName_default);
			Scribe_Values.Look(ref includeDescrForStatSearch, "includeDescrForStatSearch", includeDescrForStatSearch_default);
			Scribe_Values.Look(ref disableStartScore, "disableStartScore", disableStartScore_default);
			Scribe_Values.Look(ref disableScoreOffset, "disableScoreOffset", disableScoreOffset_default);
			Scribe_Values.Look(ref sumScoresInsteadOfAverage, "sumScoresInsteadOfAverage", sumScoresInsteadOfAverage_default);
			Scribe_Values.Look(ref insScoreBasedOnConditions, "insScoreBasedOnConditions", insScoreBasedOnConditions_default);

			Scribe_Values.Look(ref nakedOffset, "nakedOffset", nakedOffset_default);
			Scribe_Values.Look(ref ideologyOffset, "ideologyOffset", nakedOffset_default);

			Scribe_Values.Look(ref CECurryBulk, "CECurryBulk", CECurryBulk_default);
			Scribe_Values.Look(ref CECarryWeight, "CECarryWeight", CECarryWeight_default);

			Scribe_Values.Look(ref mass, "mass", mass_default);
		}

		public void ResetGeneralDefault()
		{
			draggableWindow = draggableWindow_default;
			nonBlockingWindow = nonBlockingWindow_default;
			displayModName = displayModName_default;
			displayCatName = displayCatName_default;
			includeDescrForStatSearch = includeDescrForStatSearch_default;

			disableStartScore = disableStartScore_default;
			disableScoreOffset = disableScoreOffset_default;
			sumScoresInsteadOfAverage = sumScoresInsteadOfAverage_default;
			insScoreBasedOnConditions = insScoreBasedOnConditions_default;
		}

		public void ResetTuneDefault()
		{
			nakedOffset = nakedOffset_default;
			ideologyOffset = nakedOffset_default;

			CECurryBulk = CECurryBulk_default;
			CECarryWeight = CECarryWeight_default;

			mass = mass_default;
		}
	}
}
