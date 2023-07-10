using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using rjw;
using Rimworld_Animations;

namespace Privacy_Please
{
	public class JobDriver_WatchSex : JobDriver_SexBaseInitiator
	{
		private int ticks_between_eyes = 120;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);

			Toil WatchToil = new Toil();
			WatchToil.defaultCompleteMode = ToilCompleteMode.Never;
			WatchToil.socialMode = RandomSocialMode.Off;
			WatchToil.FailOn(() => (Partner.jobs.curDriver is JobDriver_Sex) == false);
			WatchToil.initAction = delegate
			{
				pawn.pather.StopDead();
			};
			WatchToil.AddPreTickAction(delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_eyes))
				{ ThrowMetaIconF(pawn.Position, pawn.Map, ModFleckDefOf.EyeHeart); }

				if (pawn?.needs?.TryGetNeed<Need_Sex>() != null)
				{ pawn.needs.TryGetNeed<Need_Sex>().CurLevel += 0.5f / 2500f; }
			});

			yield return WatchToil;
		}
	}
}
