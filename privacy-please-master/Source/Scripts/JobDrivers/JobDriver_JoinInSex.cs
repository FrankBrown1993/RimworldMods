using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using rjw;

namespace Privacy_Please
{
	public class JobDriver_JoinInSex : JobDriver_SexBaseInitiator
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);

			Toil FollowToil = new Toil();
			FollowToil.defaultCompleteMode = ToilCompleteMode.Delay;
			FollowToil.socialMode = RandomSocialMode.Off;
			FollowToil.defaultDuration = 1200;
			FollowToil.tickAction = delegate
			{
				pawn.pather.StartPath(Partner, PathEndMode.Touch);
				
				if (pawn.pather.Moving == false && Partner.pather.Moving == false && Partner.jobs.curDriver is JobDriver_SexBaseReciever)
				{ ReadyForNextToil(); }
			};
			yield return FollowToil;

			Toil SexToil = new Toil();			
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.socialMode = RandomSocialMode.Off;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => (Partner.jobs.curDriver is JobDriver_SexBaseReciever) == false);
			SexToil.initAction = delegate
			{
				Start();
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);
			};
			SexToil.AddPreTickAction(delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(pawn, 1);
				if (ticks_left <= 0)
					ReadyForNextToil();
			});
			SexToil.AddFinishAction(delegate
			{
				End();
			});
			yield return SexToil;

			yield return new Toil
			{
				initAction = delegate { SexUtility.ProcessSex(Sexprops); },
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
