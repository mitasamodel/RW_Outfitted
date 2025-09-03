using System.Collections.Generic;
using Verse;

namespace Outfitted
{
	public class WorktypePriorities : IExposable
	{
		public List<StatPriority> priorities = new List<StatPriority>();
		public WorkTypeDef worktype;
		internal string workTypeDefName;

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
			// Save/load the defName as a string.
			workTypeDefName = worktype?.defName;
			Scribe_Values.Look(ref workTypeDefName, "worktype");
			Scribe_Collections.Look(ref priorities, "statPriorities", LookMode.Deep);
			priorities ??= new List<StatPriority>();
		}
	}
}
