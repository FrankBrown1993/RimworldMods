using RimWorld;

namespace RJWSexperience.Ideology
{
	/// <summary>
	/// Increments record every time the thought is added
	/// The increment is permanent
	/// </summary>
	public class Thought_IncreaseRecord : Thought_Memory
	{
		/// <summary>
		/// This method is called for every thought right after the pawn is assigned
		/// </summary>
		public override bool TryMergeWithExistingMemory(out bool showBubble)
		{
			var defExtension = def.GetModExtension<ThoughtDefExtension_IncreaseRecord>();

			if (defExtension != null)
			{
				pawn.records.AddTo(defExtension.recordDef, defExtension.increment);
			}
			return base.TryMergeWithExistingMemory(out showBubble);
		}
	}
}
