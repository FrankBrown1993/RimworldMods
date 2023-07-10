using RimWorld;
using Verse;

namespace RJWSexperience.Ideology.PreceptWorkers
{
	/// <summary>
	/// thought worker for a thought that is active when a certain hediff is present, and who's stage depends on the ether state of the pawn
	/// Shamelessly taken from: https://github.com/Tachyonite/Pawnmorpher/blob/master/Source/Pawnmorphs/Esoteria/Thoughts/ThoughtWorker_EtherHediff.cs
	/// </summary>
	public class ThoughtWorker_Precept_Pregnant : ThoughtWorker_Precept
	{
		/// <summary>Gets the current thought state of the given pawn.</summary>
		/// <param name="p">The pawn for whom the thoughts are generated.</param>
		/// <returns></returns>
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			return p?.DevelopmentalStage == DevelopmentalStage.Adult && IdeoUtility.IsVisiblyPregnant(p);
		}
	}
}
