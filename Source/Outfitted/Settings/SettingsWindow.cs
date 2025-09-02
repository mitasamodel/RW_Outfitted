using Outfitted.RW_JustUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Outfitted
{
	public class OutfittedMod : Mod
	{
		public static OutfittedSettigs Settings;

		public OutfittedMod(ModContentPack content) : base(content)
		{
			Settings = GetSettings<OutfittedSettigs>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Rect mainArea = inRect;
			mainArea.height -= Utils_GUI.resetButtonAreaHeight;

			var listing = new Listing_Standard();
			listing.Begin(inRect);
			listing.Label("UI options");
			listing.CheckboxLabeled("Draggable outfit window", ref Settings.draggableWindow, "Allow to drag the outfit manager window.\n\n" +
				"When it is not a good option: if you intend to use \"drag\" across selections. Obviously, instead of dragging the setting, the window will be dragged."
				);
			listing.CheckboxLabeled("Window does not block iteractions", ref Settings.nonBlockingWindow, "Allow to iteract with objects in background.\n\n"+
				"Allows: click on objects, move camera, don't force pause.");
			listing.CheckboxLabeled("Mod names in stat selection", ref Settings.displayModName, "Display mod name on top of each stat group.");
			listing.CheckboxLabeled("Category names in stat selection", ref Settings.displayCatName, "Display category of group of stats.");
			listing.CheckboxLabeled("Use description for filter too", ref Settings.includeDescrForStatSearch, "Filter stat will include description text.\n\n" +
				"It will increase probability to find a stat, but it will also increase a number of outputted stats, which don't fit to filter directly.");

			listing.GapLine();
			listing.Label("Score options");
			listing.CheckboxLabeled("Disable base score", ref Settings.disableStartScore,
				"Disable base 1.0 score\n\n" +
				"By default all Apparels gets 1.0 starting score."
				);
			listing.CheckboxLabeled("Disable score offset", ref Settings.disableScoreOffset,
				"Disable score offset defined for Apparel\n\n" +
				"Some Apparel has constant positive offset.\n\n" +
				"Example: Apparel_PackJump has +4 constant offset. All shields - too."
				);
			listing.CheckboxLabeled("Sum stats scores instead of averaging", ref Settings.sumScoresInsteadOfAverage, 
				"All previous versions of the mod used an average value of all scores of stats for the selected outfit.\n"+
				"It is a good approach to hold the total score in some manageble range.\n"+
				"However, it creates a possibility for better apparel be scored lower than sligltly worse apparel nearby.\n\n"+
				"Example: as priority stats the \"Move speed\" and \"Armor\" are selected. In stockpile there are 2 apparels options:\n" +
				"1 - armor#1, which adds only armor. Value = 10\n"+
				"2 - armor#2, which adds armor (value=10) and move speed (value=+0.4)\n"+
				"Armor#1 will have a raw score of +10. Armor#2 will have a raw score of (10+0.4)/2 = +5.2. Armor#1 wins.\n\n"+
				"This option replaces the average calculation by simple sum of all values. Better armor should win.");
			listing.CheckboxLabeled("Temperature-relevant incidents affect apparel score", ref Settings.insScoreBasedOnConditions,
				"This setting relevant only if auto-temperature is set for an apparel policy.\n\n"+
				"By default only the seasonal temperature on the current map affects the score for an apparel selection.\n"+
				"This setting enable the temperature-relevant incidents (heat wave, cold snap, etc.) to be considered for scoring.");

			listing.End();

			if (Utils_GUI.ResetButton(inRect, "Reset to defaults")) Settings.ResetDefault();
		}

		public override string SettingsCategory() => "Outfitted";
	}
}
