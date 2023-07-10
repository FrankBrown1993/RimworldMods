using RimWorld;
using RJWSexperience.Ideology.Filters;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.HistoryEvents
{
	/// <summary>
	/// Type to associate single HistoryEventDef with a TwoPawnFilter
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class TwoPawnEventRule
	{
		public HistoryEventDef historyEventDef;
		public TwoPawnFilter filter;

		/// <summary>
		/// Check if the pair of pawns fits filter conditions
		/// </summary>
		public bool Applies(Pawn pawn, Pawn partner) => filter?.Applies(pawn, partner) == true;
	}
}
