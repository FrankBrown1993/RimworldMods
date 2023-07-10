using Verse;
using Verse.AI;
using RimWorld;
using rjw;
using Multiplayer.API;

namespace rjwex
{
	public class JobGiver_UseFM : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			return this.TryGiveJob(pawn, (Building_FuckMachine)null);
		}

		[SyncMethod]
		public Job TryGiveJob(Pawn pawn, Building_FuckMachine fm)
		{
			if (pawn.Drafted) return null;

			if (!RJWSettings.WildMode)
			{
				if (!xxx.is_healthy(pawn) || (!SexUtility.ReadyForLovin(pawn) && !xxx.is_frustrated(pawn))) return null;
			}
			if (!(pawn.jobs.curJob == null || pawn.jobs.curJob.def == JobDefOf.LayDown)) return null;

			if (!(pawn.Faction?.IsPlayer ?? false) && !pawn.IsPrisonerOfColony) return null;

			if (!(Genital_Helper.has_vagina(pawn) && !Genital_Helper.genitals_blocked(pawn))
				&& !(Genital_Helper.has_anus(pawn) && !Genital_Helper.anus_blocked(pawn)))
				return null;

			if (pawn.gender == Gender.Male)
			{
				Orientation ori = CompRJW.Comp(pawn).orientation;
				if (ori == Orientation.Asexual)
					return null;

				switch (ori)
				{
					case Orientation.Heterosexual:
						if (Rand.Chance(0.97f)) return null;
						break;
					case Orientation.MostlyHeterosexual:
						if (Rand.Chance(0.85f)) return null;
						break;
					case Orientation.LeaningHeterosexual:
						if (Rand.Chance(0.65f)) return null;
						break;
					case Orientation.LeaningHomosexual:
						if (Rand.Chance(0.35f)) return null;
						break;
					case Orientation.MostlyHomosexual:
						if (Rand.Chance(0.15f)) return null;
						break;
					case Orientation.Homosexual:
						break;
				}
			}

			Building_FuckMachine bestFM = (Building_FuckMachine)null;
			if (fm != null)
			{
				bestFM = fm;
			}
			if (fm == null)
			{
				bestFM = FMHelper.GetFMForPawn(pawn);
			}
			if (bestFM != null)
			{
				return new Job(DefDatabase<JobDef>.GetNamed("UseFM"), bestFM);
			}
			return (Job)null;
		}
	}
}
