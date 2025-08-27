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
		public const bool disableStartScore_default = false;

		public bool disableStartScore = disableStartScore_default;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref disableStartScore, "disableStartScore", disableStartScore_default);
		}

		public void ResetDefault()
		{
			disableStartScore = disableStartScore_default;
		}
	}
}
