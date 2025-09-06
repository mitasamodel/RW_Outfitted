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
	public class TabContent_Tune : ITabView
	{
		private readonly ITabHost _host;

		public TabContent_Tune(ITabHost host)
		{
			_host = host;
		}

		public void Draw(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			Rect listingRect = inRect;
			listingRect.height -= Utils_GUI.resetButtonAreaHeight;

			float labelWidth = 150f;
			listing.Begin(listingRect);

			listing.Label("Additional score");
			OutfittedMod.Settings.nakedOffset =
					listing.LabelTextFieldSlider("Covers naked", OutfittedMod.Settings.nakedOffset, 0f, 10f,
					"Additional score for apparel for helping Pawn to cover naked body parts.\n\n" +
					"If Pawn don't care about being naked, then this offset is skipped.", labelWidth);

			if (ModsConfig.IsActive("Ludeon.RimWorld.Ideology"))
				OutfittedMod.Settings.ideologyOffset =
					listing.LabelTextFieldSlider("Ideology apparel", OutfittedMod.Settings.ideologyOffset, 0f, 10f,
					"Additional score for apparel required by Ideology (apparel for role).", labelWidth);

			listing.GapLine();

			listing.Label("New policy defaults");
			// CE. Bulk and weight.
			if (ModsConfig.IsActive("CETeam.CombatExtended"))
			{
				OutfittedMod.Settings.CECurryBulk =
					listing.LabelTextFieldSlider("CE Curry bulk", OutfittedMod.Settings.CECurryBulk, -2.5f, 2.5f,
					"Default priority for Curry bulk.\n\n" +
					"If this stat appllied to Outfit policy, the Pawn's curry bulk will be " +
					"reduced by apparel's worn bulk (then it will affect score).", labelWidth);
				OutfittedMod.Settings.CECarryWeight =
					listing.LabelTextFieldSlider("CE Weight capacity", OutfittedMod.Settings.CECarryWeight, -2.5f, 2.5f,
					"Default priority for Weight capacity.\n\n" +
					"If this stat appllied to Outfit policy, the Pawn's weight capacity will be " +
					"reduced by apparel's mass (then it will affect score).", labelWidth);
			}
			// Vanilla. Mass.
			else
			{
				OutfittedMod.Settings.mass =
					listing.LabelTextFieldSlider("Mass", OutfittedMod.Settings.mass, -2.5f, 2.5f,
					"Default priority for Mass.\n\n" +
					"Set a negative number to provides negative score for adding mass.\n" +
					"Encourage Pawns to use lighter apparels with same stats.", labelWidth);
			}

			listing.End();

			if (Utils_GUI.ResetButton(inRect, "Reset tune values to defaults")) OutfittedMod.Settings.ResetTuneDefault();
		}

		public bool Enabled() => true;
		public string GetLabel() => "Fine tuning";
	}
}
