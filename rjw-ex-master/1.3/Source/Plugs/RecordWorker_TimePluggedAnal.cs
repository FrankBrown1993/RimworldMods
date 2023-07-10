using RimWorld;
using Verse;

namespace rjwex
{
	public class RecordWorker_TimePluggedAnal : RecordWorker
	{
		public override bool ShouldMeasureTimeNow(Pawn pawn)
		{
			return pawn.health.hediffSet.HasHediff(HediffDef.Named("Plugged"));
		}
	}
}
