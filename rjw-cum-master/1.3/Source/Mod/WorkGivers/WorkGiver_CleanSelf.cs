using RimWorld;
using Verse;
using Verse.AI;
using rjw;

namespace rjwcum
{
	public class WorkGiver_CleanSelf : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		//conditions for self-cleaning job to be available
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (xxx.DubsBadHygieneIsActive && CumBase.dubsDBH_block_CleanSelf) // selfclean only in shower/bath etc
				return false;

			if (pawn != t)
				return false;

			if (!pawn.CanReserve(t, 1, -1, null, forced))
				return false;

			if (pawn.IsDesignatedHero())
			{
				if (!forced && CumBase.manual_hero_CleanSelf)
				{
					//ModLog.Message("WorkGiver_CleanSelf::not player interaction for hero, exit");
					return false;
				}
				if (!pawn.IsHeroOwner())
				{
					//ModLog.Message("WorkGiver_CleanSelf::player interaction for not owned hero, exit");
					return false;
				}
			}

			Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => (x.def == HediffDefOf.Hediff_CumController));
			if (hediff == null)
				return false;

			int minAge = 3 * 2500;//3 hours in-game must have passed
			if (!forced)
				if (!(hediff.ageTicks > minAge))
				{
					//ModLog.Message("WorkGiver_CleanSelf:: 3 hours in-game must pass to self-clean, exit");
					return false;
				}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(JobDefOf.CleanSelf);
		}
	}
}
