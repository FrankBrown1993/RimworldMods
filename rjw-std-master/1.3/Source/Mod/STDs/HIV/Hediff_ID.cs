using Verse;

namespace rjwstd
{
	public class Hediff_ID : Hediff
	{
		/// <summary>
		/// rename HIVs to AIDS???
		/// </summary>
		public override string LabelBase
		{
			get
			{
				if (!(pawn.health.hediffSet.HasHediff(std_Immunodeficiency.hiv.hediff_def)))
					return base.LabelBase;
				else
					return "AIDS";
			}
		}
	}
}