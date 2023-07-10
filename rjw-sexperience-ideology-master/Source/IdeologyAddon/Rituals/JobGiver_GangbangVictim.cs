using RimWorld;
using rjw;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RJWSexperience.Ideology
{
	public class JobGiver_GangbangVictim : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.Drafted || pawn.mindState == null)
			{
				return null;
			}

			PawnDuty duty = pawn.mindState.duty;

			if (duty.def == DutyDefOf.TravelOrLeave || !xxx.can_do_loving(pawn))
			{
				return null;
			}

			Pawn target = duty.focusSecond.Pawn;

			if (!pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.None)) return null;

			return JobMaker.MakeJob(RsiDefOf.Job.RapeVictim, target);
		}
	}

	/// <summary>
	/// copied from rjw
	/// </summary>
	public class JobDriver_RapeVictim : JobDriver_Rape
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (RJWSettings.DebugRape) ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();
			var PartnerJob = xxx.gettin_raped;

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => Partner == null);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			SexUtility.RapeTargetAlert(pawn, Partner);

			Toil StartPartnerJob = new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				socialMode = RandomSocialMode.Off,
				initAction = delegate
				{
					if (!(Partner.jobs.curDriver is JobDriver_SexBaseRecieverRaped))
					{
						Job gettin_raped = JobMaker.MakeJob(PartnerJob, pawn);

						Partner.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					}
				}
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Never,
				defaultDuration = duration,
				handlingFacing = true
			};
			SexToil.FailOn(() => Partner.CurJob.def != PartnerJob);
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;

				if (RJWSettings.DebugRape) ModLog.Message("JobDriver_RapeComfortPawn::MakeNewToils() - reserving prisoner");
				Start();
			};
			SexToil.tickAction = delegate
			{
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			};
			SexToil.AddFinishAction(delegate
			{
				End();
			});
			yield return SexToil;

			yield return new Toil
			{
				initAction = delegate
				{
					// Trying to add some interactions and social logs
					SexUtility.ProcessSex(Sexprops);
					Partner.records.Increment(xxx.GetRapedAsComfortPawn);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
