using RimWorld;
using Verse;

namespace rjwstd
{
	public class ThoughtWorker_WastingAway : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!std_Immunodeficiency.is_wasting_away(p))
				return ThoughtState.Inactive;
			else
				return ThoughtState.ActiveAtStage(0);
		}
	}
}