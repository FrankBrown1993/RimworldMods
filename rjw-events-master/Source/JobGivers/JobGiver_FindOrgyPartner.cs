using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RJW_Events
{
	

	public class JobGiver_FindOrgyPartner : ThinkNode_JobGiver
    {

		protected override Job TryGiveJob(Pawn pawn)
        {
			if (!RJWHookupSettings.HookupsEnabled)
			{
				return null;
			}
			if (pawn.Drafted)
			{
				return null;
			}

			Pawn_MindState mindState = pawn.mindState;
			DutyDef dutyDef;
			if (mindState == null)
			{
				dutyDef = null;
			}
			else
			{
				PawnDuty duty = mindState.duty;
				dutyDef = ((duty != null) ? duty.def : null);
			}
			if (dutyDef == DutyDefOf.TravelOrLeave)
			{
				if (RJWSettings.DebugLogJoinInBed)
				{
					ModLog.Message("JoinInBed.TryGiveJob:(" + xxx.get_pawnname(pawn) + "): has TravelOrLeave, no time for lovin!");
				}
				return null;
			}

			if (!CasualSex_Helper.CanHaveSex(pawn))
			{
				return null;
			}

			List<Pawn> targets = LordUtility.GetLord(pawn).ownedPawns.Where((Pawn p) => {

				if(p?.mindState?.duty == null && p.mindState.duty.def != OrgyDutyDefOf.Orgy)
                {
					return false;
                }

				IntVec3 cell = p.mindState.duty.focus.Cell;
				if (GatheringsUtility.InGatheringArea(p.Position, cell, p.Map)) {
					return true;
				}
				return false;
			}).ToList();


            if (!BestPawnForOrgyExists(pawn, targets, out Pawn pawn2))
            {
                return null;
            }
            if (pawn2 == null)
			{
				return null;
			}

			if (pawn2.jobs.curDriver is JobDriver_SexBaseInitiator)
			{
				pawn2 = (Pawn)pawn2.CurJob.targetA;
			}
			else if (pawn.CurJob != null && pawn.jobs.curDriver != null)
			{
				pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
			}
			return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("OrgySex", true), pawn2);
		}

		public bool BestPawnForOrgyExists(Pawn pawn1, List<Pawn> targets, out Pawn result)
        {
			if(targets.TryRandomElementByWeight((Pawn p) => {

				if (p == pawn1) return 0;
				float chance = pawn1.relations.SecondaryRomanceChanceFactor(p);
				if(!(p.jobs.curDriver is JobDriver_Sex))
                {
					//higher chance if person is doing nothing
					chance *= 7f;
                }
				return chance;
			
			}, out Pawn option)) {
				result = option;
				return true;
            }
			else
            {
				result = null;
				return false;
			}
			
        }
    }
}
