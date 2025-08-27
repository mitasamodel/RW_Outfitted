using RW_Utils;
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
			listing.CheckboxLabeled("Disable base score", ref Settings.disableStartScore, 
				"Disable base 1.0 score\n\n" +
				"By default each Apparel gets 1.0 starting score."
				);
			listing.CheckboxLabeled("Disable score offset", ref Settings.disableScoreOffset,
				"Disable score offset defined for Apparel\n\n" +
				"Some Apparel (e.g. Apparel_PackJump) has constant positive offset."
				);


			listing.End();

			if (Utils_GUI.ResetButton(inRect, "Reset to defaults")) Settings.ResetDefault();
		}

		public override string SettingsCategory() => "Outfitted";
	}
}
