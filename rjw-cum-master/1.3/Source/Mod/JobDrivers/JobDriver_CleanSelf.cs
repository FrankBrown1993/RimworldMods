using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace rjwcum
{
	class JobDriver_CleanSelf : JobDriver
	{
		float cleanAmount = 1f;//severity of a single cumHediff removed per cleaning-round; 1f = remove entirely
		int cleaningTime = 120;//ticks - 120 = 2 real seconds, 3 in-game minutes

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(pawn, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(delegate
			{
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				return !hediffs.Exists(x => x.def == HediffDefOf.Hediff_CumController);//fail if cum disappears - means that also all the cum is gone
			});
			Toil cleaning = Toils_General.Wait(cleaningTime, TargetIndex.None);//duration of 
			cleaning.WithProgressBarToilDelay(TargetIndex.A);

			yield return cleaning;
			yield return new Toil()
			{
				initAction = delegate ()
				{
					//get one of the cum hediffs, reduce its severity
					Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => (x.def == HediffDefOf.Hediff_Cum || x.def == HediffDefOf.Hediff_InsectSpunk || x.def == HediffDefOf.Hediff_MechaFluids));
					if (hediff != null)
					{
						if (hediff.Severity >= 0.5)
						{
							if (hediff.def == HediffDefOf.Hediff_InsectSpunk)
							{
								Thing jelly = ThingMaker.MakeThing(ThingDefOf.InsectJelly);
								jelly.SetForbidden(true, false);
								GenPlace.TryPlaceThing(jelly, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Near);
							}
						}
						hediff.Severity -= cleanAmount;
					}
				}
			};
			yield break;
		}
	}
}
