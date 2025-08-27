// Decompiled with JetBrains decompiler
// Type: Outfitted.WorktypePriorities
// Assembly: Outfitted, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7FA0F5BF-790B-428D-866C-5D33983FFC76
// Assembly location: D:\SteamLibrary\steamapps\workshop\content\294100\3454809174\1.5\Assemblies\Outfitted.dll

using System.Collections.Generic;
using Verse;

#nullable disable
namespace Outfitted
{
	public class WorktypePriorities : IExposable
	{
		public List<StatPriority> priorities = new List<StatPriority>();
		public WorkTypeDef worktype;

		public WorktypePriorities()
		{
		}

		public WorktypePriorities(WorkTypeDef worktype, List<StatPriority> priorities)
		{
			this.worktype = worktype;
			this.priorities = priorities;
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<WorkTypeDef>(ref this.worktype, "worktype");
			Scribe_Collections.Look<StatPriority>(ref this.priorities, "statPriorities", LookMode.Deep);
		}
	}
}
