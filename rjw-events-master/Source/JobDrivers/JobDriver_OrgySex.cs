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
    class JobDriver_OrgySex : JobDriver_SexQuick
    {

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
			return true;
        }

		protected override IEnumerable<Toil> MakeNewToils()
		{
			base.setup_ticks();
			JobDef PartnerJob = DefDatabase<JobDef>.GetNamed("GettinOrgySex", true);
			this.FailOnDespawnedOrNull(this.iTarget);
			this.FailOn(() => this.Partner == null);
			this.FailOn(() => !this.Partner.health.capacities.CanBeAwake);
			this.FailOn(() => this.pawn.IsFighting());
			this.FailOn(() => this.Partner.IsFighting());
			this.FailOn(() => this.pawn.Drafted);
			this.FailOn(() => this.Partner.Drafted);

			yield return Toils_Goto.GotoThing(this.iTarget, PathEndMode.OnCell);
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Delay,
				initAction = delegate ()
				{
					this.ticksLeftThisToil = 5000;
				},
				tickAction = delegate ()
				{
					this.pawn.GainComfortFromCellIfPossible(false);
					if (this.pawn.Position.DistanceTo(this.Partner.Position) <= 3f)
					{
						this.ReadyForNextToil();
					}
				}
			};
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				socialMode = RandomSocialMode.Off,
				initAction = delegate ()
				{
					
					if(Partner?.jobs != null && Partner?.CurJob?.def != null && Partner.CurJob.def != PartnerJob)
                    {
						Job newJob = JobMaker.MakeJob(PartnerJob, this.pawn, this.Partner);
						this.Partner.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false, false);
					}
					
				}
			};
			Toil toil = new Toil();
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.socialMode = RandomSocialMode.Normal;
			toil.defaultDuration = this.duration;
			toil.handlingFacing = true;
			toil.FailOn(() => this.Partner.CurJob.def != PartnerJob);
			toil.initAction = delegate ()
			{
				this.Partner.pather.StopDead();
				this.Partner.jobs.curDriver.asleep = false;
				this.Start();
				this.Sexprops.usedCondom = (CondomUtility.TryUseCondom(this.pawn) || CondomUtility.TryUseCondom(this.Partner));
				
			};
			toil.AddPreTickAction(delegate
			{
				if (this.pawn.IsHashIntervalTick(this.ticks_between_hearts))
				{
					FleckMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, FleckDefOf.Heart);
				}
				this.SexTick(this.pawn, this.Partner, true, true);
				SexUtility.reduce_rest(this.Partner, 1f);
				SexUtility.reduce_rest(this.pawn, 1f);
				if (this.ticks_left <= 0)
				{
					this.ReadyForNextToil();
				}
			});
			toil.AddFinishAction(delegate
			{
				this.End();
				if(LordUtility.GetLord(pawn)?.LordJob != null && LordUtility.GetLord(pawn).LordJob is LordJob_Joinable_Orgy && !(LordUtility.GetLord(pawn).CurLordToil is LordToil_End))
                {
					SexUtility.DrawNude(pawn);
					SexUtility.DrawNude(Partner);
				}
					
			});
			yield return toil;
			yield return new Toil
			{
				initAction = delegate ()
				{
					SexUtility.ProcessSex(this.Sexprops);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}


	}
}
