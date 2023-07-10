using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RJW_Events
{
    public class JobDriver_OrgySexReceiver : JobDriver_SexBaseReciever
    {

        protected override IEnumerable<Toil> MakeNewToils()
        {
			base.setup_ticks();
			this.parteners.Add(base.Partner);
			if (this.pawn.relations.OpinionOf(base.Partner) < 0)
			{
				this.ticks_between_hearts += 50;
			}
			else if (this.pawn.relations.OpinionOf(base.Partner) > 60)
			{
				this.ticks_between_hearts -= 25;
			}
			this.FailOnDespawnedOrNull(this.iTarget);
			this.FailOn(() => this.Partner == null);
			this.FailOn(() => !(Partner.jobs.curDriver is JobDriver_Sex));
			this.FailOn(() => !base.Partner.health.capacities.CanBeAwake);
			this.FailOn(() => this.pawn.Drafted);
			this.FailOn(() => base.Partner.Drafted);


			yield return Toils_Reserve.Reserve(this.iTarget, 1, 0, null);
			Toil toil2 = this.MakeSexToil();
			toil2.handlingFacing = false;
			yield return toil2;


			yield break;
		}

		private Toil MakeSexToil()
		{
			Toil toil = new Toil();
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.socialMode = RandomSocialMode.Normal;
			toil.handlingFacing = true;
			toil.tickAction = delegate ()
			{
				if (this.pawn.IsHashIntervalTick(this.ticks_between_hearts))
				{
					FleckMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, FleckDefOf.Heart);
				}
			};
			toil.AddEndCondition(delegate
			{
				if (this.parteners.Count <= 0)
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			}); 
			toil.socialMode = RandomSocialMode.Off;
			return toil;
		}


	}
}
