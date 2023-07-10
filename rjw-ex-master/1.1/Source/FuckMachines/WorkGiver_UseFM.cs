using RimWorld;
using Verse;
using Verse.AI;
using rjw;

namespace rjwex
{
	public class WorkGiver_UseFM : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Building_FuckMachine target = t as Building_FuckMachine;
			if (target == null)
			{
				if (RJWSettings.DevMode) JobFailReason.Is("not a fuck machine");
				return false;
			}

			if (target.owners.Count>0&&!target.IsOwner(pawn))
			{
				if (RJWSettings.DevMode) JobFailReason.Is("not pawn's machine");
				return false;
			}

			CompPowerTrader comp = t.TryGetComp<CompPowerTrader>();
			if (comp != null && !comp.PowerOn)
			{
				if (RJWSettings.DevMode) JobFailReason.Is("no power");
				return false;
			}

			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("UseFM"), t);
		}
	}
}