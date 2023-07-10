using Verse;

namespace rjwwhoring.MainTab
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_AverageMoneyByWhore : PawnColumnWorker_TextCenter
	{
		protected override string GetTextFor(Pawn pawn)
		{
			return ((int)GetValueToCompare(pawn)).ToString();
		}

		public override int Compare(Pawn a, Pawn b)
		{
			return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
		}

		private float GetValueToCompare(Pawn pawn)
		{
			float total = pawn.records.GetValue(RecordDefOf.EarnedMoneyByWhore);
			float count = pawn.records.GetValue(RecordDefOf.CountOfWhore);
			if ((int)count == 0)
			{
				return 0;
			}
			return (total / count);
		}
	}
}
