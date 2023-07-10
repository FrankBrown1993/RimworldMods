using rjw;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Filters
{
	/// <summary>
	/// Filter to describe one pawn
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class SinglePawnFilter
	{
		public bool? isAnimal;
		public bool? isSlave;
		public bool? isPrisoner;

		/// <summary>
		/// Check if pawn fits filter conditions
		/// </summary>
		public bool Applies(Pawn pawn)
		{
			// Fail if any single condition fails
			if (isAnimal != null && isAnimal != pawn.IsAnimal())
				return false;

			if (isSlave != null && isSlave != pawn.IsSlave)
				return false;

			if (isPrisoner != null && isPrisoner != pawn.IsPrisoner)
				return false;

			return true;
		}
	}
}
