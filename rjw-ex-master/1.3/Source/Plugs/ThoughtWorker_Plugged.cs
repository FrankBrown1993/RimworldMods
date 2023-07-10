using RimWorld;
using Verse;

namespace rjwex
{
	public class ThoughtWorker_Plugged : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn pawn)
		{
			if (pawn.apparel != null)
			{
				var hed = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Plugged"));
				if (hed != null)
				{
					if (hed.Severity < 0.2f)
					{
						return ThoughtState.ActiveAtStage(0);
					}
					else if (hed.Severity < 0.4f)
					{
						return ThoughtState.ActiveAtStage(1);
					}
					else if (hed.Severity < 0.6f)
					{
						return ThoughtState.ActiveAtStage(2);
					}
					else if (hed.Severity < 0.8f)
					{
						return ThoughtState.ActiveAtStage(3);
					}
					else
					{
						return ThoughtState.ActiveAtStage(4);
					}
				}
			}
			return ThoughtState.Inactive;
		}
	}
}