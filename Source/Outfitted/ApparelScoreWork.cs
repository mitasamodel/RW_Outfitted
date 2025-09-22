using Outfitted.RW_JustUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted
{
	internal class ApparelScoreWork
	{
		internal static float ApparelScoreAutoWorkPriorities(Pawn pawn, Apparel apparel)
		{
			// Priorities for this set of jobs.
			List<StatPriority> spList = WorkPriorities.WorktypeStatPriorities(pawn);

#if DEBUG
			MyDebug.debugDeepScoreWork.Start(apparel.def.defName);
			MyDebug.debugDeepScoreWork.AddToLog($"Work priotities for pawn[{pawn}][{apparel.def.defName}]\n");
			foreach(StatPriority sp in spList)
			{
				MyDebug.debugDeepScoreWork.AddToLog($"\tPrio[{sp.Stat.defName}] Wei[{sp.Weight}]\n");
			}
#endif
			bool isWorn = apparel.Wearer == pawn;
			float sum = 0f;
			int count = 0;
			foreach (StatPriority sp in spList)
			{
				float scaledDelta = ApparelScore.GetFinalDelta(pawn, apparel, sp, isWorn);
				float score = scaledDelta * sp.Weight;
				sum += score;
				count++;
#if DEBUG
				MyDebug.debugDeepScorePriorities.AddToLog(
					$"Score[{score:F2}] " +
					$"SUM[{sum:F2}] " +
					$"COUNT[{count}]\n");
#endif
			}
			//float result = WorkPriorities.WorktypeStatPriorities(pawn)
			//	.Select(sp => (apparel.def.equippedStatOffsets.GetStatOffsetFromList(sp.Stat) + apparel.GetOutfittedStatValue(sp.Stat) - sp.Stat.defaultBaseValue) * sp.Weight).Sum();

			return sum;
		}
	}
}
