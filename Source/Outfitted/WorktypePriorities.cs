using System.Collections.Generic;
using Verse;

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
			Scribe_Defs.Look(ref worktype, "worktype");
			Scribe_Collections.Look(ref priorities, "statPriorities", LookMode.Deep);
		}
	}
}
