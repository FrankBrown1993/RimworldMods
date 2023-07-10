using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
//using Multiplayer.API;
using rjw;

namespace rjwwhoring
{
	public class JobDriver_WhoreIsServingVisitors : JobDriver_SexBaseInitiator
	{
		public IntVec3 SleepSpot => Bed.SleepPosOfAssignedPawn(pawn);

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, 0, null, errorOnFailed);
		}

		//[SyncMethod]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":MakeNewToils() - making toils");
			setup_ticks();
			var PartnerJob = xxx.gettin_loved;

			this.FailOnDespawnedOrNull(iTarget);
			this.FailOnDespawnedNullOrForbidden(iBed);

			if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":fail conditions check " + !WhoreBed_Utility.CanUseForWhoring(pawn, Bed) + " " + !pawn.CanReserve(Partner));
			this.FailOn(() => !WhoreBed_Utility.CanUseForWhoring(pawn, Bed) || !pawn.CanReserve(Partner));
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.IsFighting());

			yield return Toils_Reserve.Reserve(iTarget, 1, 0);
			int basePrice = WhoringHelper.PriceOfWhore(pawn);
			float bedMult = WhoreBed_Utility.CalculatePriceFactor(Bed);
			//yield return Toils_Reserve.Reserve(BedInd, Bed.SleepingSlotsCount, 0);

			if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":generate job toils");
			Toil gotoBed = new Toil();
			gotoBed.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			gotoBed.FailOnBedNoLongerUsable(iBed, Bed);
			gotoBed.AddFailCondition(() => Partner.Downed);
			gotoBed.FailOn(() => !Partner.CanReach(Bed, PathEndMode.Touch, Danger.Deadly));
			gotoBed.initAction = delegate
			{
				if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":gotoWhoreBed");
				pawn.pather.StartPath(SleepSpot, PathEndMode.OnCell);
				Partner.jobs.StopAll();
				Job job = JobMaker.MakeJob(JobDefOf.GotoMindControlled, SleepSpot);
				Partner.jobs.StartJob(job, JobCondition.InterruptForced);
			};
			yield return gotoBed;

			ticks_left = (int)(2000.0f * Rand.Range(0.30f, 1.30f));

			Toil waitInBed = new Toil();
			waitInBed.initAction = delegate
			{
				ticksLeftThisToil = 5000;
			};
			waitInBed.tickAction = delegate
			{
				pawn.GainComfortFromCellIfPossible();
				if (IsInOrByBed(Bed, Partner) && pawn.PositionHeld == Partner.PositionHeld)
				{
					ReadyForNextToil();
				}
			};
			waitInBed.defaultCompleteMode = ToilCompleteMode.Delay;
			yield return waitInBed;

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":StartPartnerJob");
				var gettin_loved = JobMaker.MakeJob(PartnerJob, pawn, Bed);
				Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced);
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.socialMode = RandomSocialMode.Off;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.Dead);
			SexToil.FailOn(() => Partner.CurJob.def != PartnerJob);
			SexToil.initAction = delegate
			{
				if (WhoringBase.DebugWhoring) ModLog.Message("" + this.GetType().ToString() + ":SexToil start");

				// refresh bed reservation
				Bed.ReserveForWhoring(pawn, ticks_left+100);

				Start();

				// TODO: replace this quick n dirty way
				CondomUtility.GetCondomFromRoom(pawn);
				// Try to use whore's condom first, then client's
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);

				if (!RJWSettings.HippieMode && xxx.HasNonPolyPartner(Partner, true))
				{
					Pawn lover = LovePartnerRelationUtility.ExistingLovePartner(Partner);
					// We have to do a few other checks because the pawn might have multiple lovers and ExistingLovePartner() might return the wrong one
					if (lover != null && pawn != lover && !lover.Dead && (lover.Map == Partner.Map || Rand.Value < 0.25) && GenSight.LineOfSight(lover.Position, Partner.Position, lover.Map))
					{
						lover.needs.mood.thoughts.memories.TryGainMemory(RimWorld.ThoughtDefOf.CheatedOnMe, Partner);
					}
				}
			};
			SexToil.AddPreTickAction(delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					if (xxx.is_nympho(pawn))
						FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
					else
						FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, xxx.mote_noheart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left % 100 == 0)
					Bed.ReserveForWhoring(pawn, ticks_left + 100); // without this, reservation sometimes expires before sex is finished
				if (ticks_left <= 0)
					ReadyForNextToil();
			});
			SexToil.AddFinishAction(delegate
			{
				End();
			});
			yield return SexToil;

			Toil afterSex = new Toil
			{
				initAction = delegate
				{
					// Adding interactions, social logs, etc
					SexUtility.ProcessSex(Sexprops);

					Bed.UnreserveForWhoring();

					if (!(Partner.IsColonist && (pawn.IsPrisonerOfColony || pawn.IsColonist)))
					{
						int netPrice = (int) (basePrice * bedMult);
						if (netPrice == 0)
							netPrice += 1;
						
						int bedTip = netPrice - basePrice;
						int defect = WhoringHelper.PayPriceToWhore(Partner, netPrice, pawn);
						
						if (WhoringBase.DebugWhoring)
						{
							ModLog.Message($"{GetType()}:afterSex toil - {Partner} tried to pay {basePrice}(whore price) + {bedTip}(room modifier) silver to {pawn}");

							if (defect <= 0)
								ModLog.Message(" Paid full price");
							else if (defect <= bedTip)
								ModLog.Message(" Could not pay full tip");
							else
								ModLog.Message(" Failed to pay base price");
						}

						WhoringHelper.UpdateRecords(pawn, netPrice - defect);
					}

					if (SexUtility.ConsiderCleaning(pawn))
					{
						LocalTargetInfo cum = pawn.PositionHeld.GetFirstThing<Filth>(pawn.Map);

						Job clean = JobMaker.MakeJob(JobDefOf.Clean);
						clean.AddQueuedTarget(TargetIndex.A, cum);

						pawn.jobs.jobQueue.EnqueueFirst(clean);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield return afterSex;
		}
	}
}
