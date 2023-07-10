using RimWorld;
using rjw;
using Verse;

namespace RJWSexperience.Ideology.PreceptWorkers
{
	public class ThoughtWorker_Precept_GenitalSize : ThoughtWorker_Precept
	{
		private ThoughtDefExtension_StageFromValue stageFromValue;

		protected ThoughtDefExtension_StageFromValue StageFromValue
		{
			get
			{
				if (stageFromValue == null)
				{
					stageFromValue = def.GetModExtension<ThoughtDefExtension_StageFromValue>();
				}
				return stageFromValue;
			}
		}

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p?.DevelopmentalStage == DevelopmentalStage.Adult && Genital_Helper.get_AllPartsHediffList(p).Count > 0)
			{
				float bestSize = IdeoUtility.GetGenitalSize(p);
				return ThoughtState.ActiveAtStage(StageFromValue.GetStageIndex(bestSize));
			}
			// This might can happen if the pawn has no genitalia ... maybe?
			return ThoughtState.Inactive;
		}
	}
}
