// Decompiled with JetBrains decompiler
// Type: Outfitted.StatPriority
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using RimWorld;
using Verse;

namespace Outfitted
{
	public class StatPriority : IExposable
	{
		private StatDef stat;
		public float Weight;
		public float Default;

		public StatPriority(StatDef stat, float weight, float defaultWeight = float.NaN)
		{
			this.stat = stat;
			this.Weight = weight;
			this.Default = defaultWeight;
		}

		public StatPriority()
		{
		}

		public StatDef Stat => this.stat;

		public bool IsDefault => (double)this.Default == (double)this.Weight;

		public bool IsManual => float.IsNaN(this.Default);

		public bool IsOverride => !this.IsManual && !this.IsDefault;

		public void ExposeData()
		{
			Scribe_Defs.Look<StatDef>(ref this.stat, "Stat");
			Scribe_Values.Look<float>(ref this.Weight, "Weight");
			Scribe_Values.Look<float>(ref this.Default, "Default", float.NaN);
		}
	}
}
