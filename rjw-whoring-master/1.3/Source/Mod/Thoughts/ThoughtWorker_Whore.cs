using RimWorld;
using Verse;
using System.Collections.Generic;

namespace rjwwhoring
{
	/// <summary>
	/// Extends the standard thought to add a counter for the whore stages
	/// </summary>
	public class ThoughtDef_Whore : ThoughtDef
	{
		public List<int> stageCounts = new List<int>();
		public int storyOffset = 0;
	}

	public class ThoughtWorker_Whore : Thought_Memory
	{
		public static readonly HashSet<string> backstories = new HashSet<string>(DefDatabase<StringListDef>.GetNamed("WhoreBackstories").strings);

		protected List<int> Stages => ((ThoughtDef_Whore) def).stageCounts;
		protected int StoryOffset => ((ThoughtDef_Whore) def).storyOffset;

		public override int CurStageIndex
		{
			get
			{
				int timesWhored = pawn.records.GetAsInt(RecordDefOf.CountOfWhore);

				if (backstories.Contains(pawn.story?.adulthood?.titleShort))
				{
					timesWhored += StoryOffset;
				}

				if (timesWhored > Stages[Stages.Count - 1])
				{
					return Stages.Count - 1;
				}

				return Stages.FindLastIndex(v => timesWhored > v) + 1;
			}
		}
	}
}
