using System.Collections.Generic;
using Verse;

namespace rjwwhoring.MainTab
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_WhoreExperience : PawnColumnWorker_TextCenter
	{
		public static readonly HashSet<string> backstories = new HashSet<string>(DefDatabase<StringListDef>.GetNamed("WhoreBackstories").strings);

		protected override string GetTextFor(Pawn pawn)
		{

			int b = backstories.Contains(pawn.story?.Adulthood?.titleShort) ? 30 : 0;
			int score = pawn.records.GetAsInt(RecordDefOf.CountOfWhore);
			return (score + b).ToString();
		}
	}
}