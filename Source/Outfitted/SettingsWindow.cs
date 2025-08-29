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

			listing.End();

			if (Utils_GUI.ResetButton(inRect, "Reset to defaults")) Settings.ResetDefault();
		}

		public override string SettingsCategory() => "Outfitted";
	}
}
