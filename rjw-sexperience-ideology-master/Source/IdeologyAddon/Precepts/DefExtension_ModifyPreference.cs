using RJWSexperience.Ideology.Filters;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

namespace RJWSexperience.Ideology.Precepts
{
	/// <summary>
	/// Def extension to enable changing SexAppraiser results based on filters
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class DefExtension_ModifyPreference : DefModExtension
	{
		public List<Rule> rules;

		/// <summary>
		/// Apply SexAppraiser modifiers from rules with a satisfied filter
		/// </summary>
		public void Apply(Pawn pawn, Pawn partner, ref float preference)
		{
			foreach (Rule rule in rules.Where(rule => rule.Applies(pawn, partner)))
			{
				preference *= rule.multiplier;
			}
		}

		/// <summary>
		/// Type to associate SexAppraiser result modifier with a TwoPawnFilter
		/// </summary>
		public class Rule
		{
			public float multiplier = 1f;
			public TwoPawnFilter filter;

			/// <summary>
			/// Check if the pair of pawns fits filter conditions
			/// </summary>
			public bool Applies(Pawn pawn, Pawn partner)
			{
				if (filter == null)
					return true;

				return filter.Applies(pawn, partner);
			}
		}
	}
}
