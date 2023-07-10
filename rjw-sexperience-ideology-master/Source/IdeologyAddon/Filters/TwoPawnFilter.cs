using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Filters
{
	/// <summary>
	/// Filter to describe two pawns and relations between them
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class TwoPawnFilter
	{
		public SinglePawnFilter doer;
		public SinglePawnFilter partner;
		public RelationFilter relations;

		/// <summary>
		/// Check if the pair of pawns fits filter conditions
		/// </summary>
		public bool Applies(Pawn pawn, Pawn partner)
		{
			// Fail if any single condition fails
			if (doer?.Applies(pawn) == false)
				return false;

			if (this.partner?.Applies(partner) == false)
				return false;

			if (relations?.Applies(pawn, partner) == false)
				return false;

			return true;
		}
	}
}
