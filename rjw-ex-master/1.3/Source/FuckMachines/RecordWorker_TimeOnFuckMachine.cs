using RimWorld;
using Verse;

namespace rjwex
{
	public class RecordWorker_TimeOnFuckMachine : RecordWorker
	{
		public override bool ShouldMeasureTimeNow(Pawn pawn)
		{
			return pawn.health.hediffSet.HasHediff(HediffDef.Named("Plugged"));
		}
	}
}
