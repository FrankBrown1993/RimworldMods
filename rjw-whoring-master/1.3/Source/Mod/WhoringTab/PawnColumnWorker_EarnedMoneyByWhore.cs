using Verse;

namespace rjwwhoring.MainTab
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_EarnedMoneyByWhore : PawnColumnWorker_TextCenter
	{
		protected override string GetTextFor(Pawn pawn)
		{
			return GetValueToCompare(pawn).ToString();
		}

		public override int Compare(Pawn a, Pawn b)
		{
			return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
		}

		private int GetValueToCompare(Pawn pawn)
		{
			return pawn.records.GetAsInt(RecordDefOf.EarnedMoneyByWhore);
		}
	}
}
