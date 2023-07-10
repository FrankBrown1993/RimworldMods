using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RJW_Events
{
    public class JobGiver_GetNaked : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("GetNaked", true));
        }
    }
}
