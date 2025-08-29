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
		// UI options.
		public const bool draggableWindow_default = true;
		public const bool displayModName_default = true;
		public const bool displayCatName_default = true;

		public bool draggableWindow = draggableWindow_default;
		public bool displayModName = displayModName_default;
		public bool displayCatName = displayCatName_default;

		// Score options.
		public const bool disableStartScore_default = false;
		public const bool disableScoreOffset_default = false;

		public bool disableStartScore = disableStartScore_default;
		public bool disableScoreOffset = disableScoreOffset_default;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref draggableWindow, "draggableWindow", draggableWindow_default);
			Scribe_Values.Look(ref displayModName, "draggableWindow", displayModName_default);
			Scribe_Values.Look(ref displayCatName, "draggableWindow", displayCatName_default);
			Scribe_Values.Look(ref disableStartScore, "disableStartScore", disableStartScore_default);
			Scribe_Values.Look(ref disableScoreOffset, "disableScoreOffset", disableScoreOffset_default);
		}

		public void ResetDefault()
		{
			draggableWindow = draggableWindow_default;
			displayModName = displayModName_default;
			displayCatName = displayCatName_default;

			disableStartScore = disableStartScore_default;
			disableScoreOffset = disableScoreOffset_default;
		}
	}
}
