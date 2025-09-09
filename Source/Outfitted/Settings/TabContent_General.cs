using CombatExtended;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using Outfitted.RW_JustUtils;

namespace Outfitted
{
	public class TabContent_General : ITabView
	{
		private readonly ITabHost _host;

		public TabContent_General(ITabHost host)
		{
			_host = host;
		}

		public void Draw(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			Rect listingRect = inRect;
			listingRect.height -= Utils_GUI.resetButtonAreaHeight;

			listing.Begin(listingRect);
			listing.Label("UI options");
			listing.CheckboxLabeled("Draggable outfit window", ref OutfittedMod.Settings.draggableWindow, "Allow to drag the outfit manager window.\n\n" +
				"When it is not a good option: if you intend to use \"drag\" across selections. Obviously, instead of dragging the setting, the window will be dragged."
				);
			listing.CheckboxLabeled("Window does not block iteractions", ref OutfittedMod.Settings.nonBlockingWindow, "Allow to iteract with objects in background.\n\n" +
				"Allows: click on objects, move camera, don't force pause.");
			listing.CheckboxLabeled("Mod names in stat selection", ref OutfittedMod.Settings.displayModName, "Display mod name on top of each stat group.");
			listing.CheckboxLabeled("Category names in stat selection", ref OutfittedMod.Settings.displayCatName, "Display category of group of stats.");
			listing.CheckboxLabeled("Use description for filter too", ref OutfittedMod.Settings.includeDescrForStatSearch, "Filter stat will include description text.\n\n" +
				"It will increase probability to find a stat, but it will also increase a number of outputted stats, which don't fit to filter directly.");

			listing.GapLine();
			listing.Label("Score options");
			listing.CheckboxLabeled("Disable base score", ref OutfittedMod.Settings.disableStartScore,
				"Disable base 1.0 score\n\n" +
				"By default all Apparels gets 1.0 starting score."
				);
			listing.CheckboxLabeled("Disable vanilla score offset", ref OutfittedMod.Settings.disableScoreOffset,
				"Disable vanilla score offset defined for specific Apparel.\n\n" +
				"Some Apparel has constant positive offset.\n\n" +
				"Example: Apparel_PackJump has +4 constant offset. All shields - too."
				);
			listing.CheckboxLabeled("Sum stats scores instead of averaging", ref OutfittedMod.Settings.sumScoresInsteadOfAverage,
				"All previous versions of the mod used an average value of all scores of stats for the selected outfit.\n" +
				"It is a good approach to hold the total score in some manageble range.\n" +
				"However, it creates a possibility for better apparel be scored lower than sligltly worse apparel nearby.\n\n" +
				"Example: as priority stats the \"Move speed\" and \"Armor\" are selected. In stockpile there are 2 apparels options:\n" +
				"1 - armor#1, which adds only armor. Value = 10\n" +
				"2 - armor#2, which adds armor (value=10) and move speed (value=+0.4)\n" +
				"Armor#1 will have a raw score of +10. Armor#2 will have a raw score of (10+0.4)/2 = +5.2. Armor#1 wins.\n\n" +
				"This option replaces the average calculation by simple sum of all values. Better armor should win.");
			listing.CheckboxLabeled("Temperature-relevant incidents affect apparel score", ref OutfittedMod.Settings.insScoreBasedOnConditions,
				"This setting relevant only if auto-temperature is set for an apparel policy.\n\n" +
				"By default only the seasonal temperature on the current map affects the score for an apparel selection.\n" +
				"This setting enable the temperature-relevant incidents (heat wave, cold snap, etc.) to be considered for scoring.");

			listing.GapLine();
			listing.CheckboxLabeled("[Outdated] Generate additional starting outfits", ref OutfittedMod.Settings.generateStartingOutfits,
				"Generate additional outfits like 'Doctor', 'Cook', 'Builder' with typical priority stats.\n\n" +
				"These policies are generated on new game start or on game's load if there are vanilla outfits saved " +
				"(convert vanilla outfits).\n\n" +
				"[Outdated] Current set of stats need to be re-checked.");

			listing.End();

			if (Utils_GUI.ResetButton(inRect, "Reset to defaults")) OutfittedMod.Settings.ResetGeneralDefault();
		}

		public bool Enabled() => true;
		public string GetLabel() => "General";
	}
}
