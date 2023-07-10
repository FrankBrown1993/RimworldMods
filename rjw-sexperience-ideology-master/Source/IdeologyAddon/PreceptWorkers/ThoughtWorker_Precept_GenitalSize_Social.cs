using RimWorld;
using rjw;
using Verse;

namespace RJWSexperience.Ideology.PreceptWorkers
{
	public class ThoughtWorker_Precept_GenitalSize_Social : ThoughtWorker_Precept_Social
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

		// Important Note: For the Social Worker, we measure otherPawns genitalia 
		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (otherPawn?.DevelopmentalStage == DevelopmentalStage.Adult &&
				p?.DevelopmentalStage == DevelopmentalStage.Adult &&
				Genital_Helper.get_AllPartsHediffList(otherPawn).Count > 0)
			{
				float bestSize = IdeoUtility.GetGenitalSize(otherPawn);
				return ThoughtState.ActiveAtStage(StageFromValue.GetStageIndex(bestSize));
			}
			// This might can happen if the pawn has no genitalia ... maybe?
			return ThoughtState.Inactive;
		}
	}
}
