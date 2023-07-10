using RimWorld;
using Verse;

namespace RJWSexperience.Ideology
{
	public class Thought_Opinionbased : Thought_Memory
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

		/// <summary>
		/// This method is called for every thought right after the pawn is assigned
		/// </summary>
		public override bool TryMergeWithExistingMemory(out bool showBubble)
		{
			UpdateCurStage();
			return base.TryMergeWithExistingMemory(out showBubble);
		}

		/// <summary>
		/// Called every 150 ticks
		/// </summary>
		public override void ThoughtInterval()
		{
			UpdateCurStage();
			base.ThoughtInterval();
		}

		protected void UpdateCurStage()
		{
			if (otherPawn == null)
			{
				Log.Warning($"[RSI] Thought_Opinionbased {def.defName} for pawn {pawn.NameShortColored} lacks otherPawn");
				SetForcedStage(0);
			}

			float value = pawn.relations?.OpinionOf(otherPawn) ?? 0f;
			SetForcedStage(StageFromValue.GetStageIndex(value));
		}
	}
}
