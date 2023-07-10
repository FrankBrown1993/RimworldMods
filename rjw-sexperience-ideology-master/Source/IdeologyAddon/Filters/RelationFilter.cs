using RimWorld;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Filters
{
	/// <summary>
	/// Filter to describe how one pawn sees another
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class RelationFilter
	{
		public bool? isVeneratedAnimal;
		public List<PawnRelationDef> hasOneOfRelations;
		public List<PawnRelationDef> hasNoneOfRelations;
		public List<BloodRelationDegree> hasOneOfRelationDegrees;

		private bool initialized = false;
		private HashSet<PawnRelationDef> hasOneOfRelationsHashed;
		private HashSet<PawnRelationDef> hasNoneOfRelationsHashed;
		private HashSet<BloodRelationDegree> hasOneOfRelationDegreesHashed;

		/// <summary>
		/// Check if the pair of pawns fits filter conditions
		/// </summary>
		public bool Applies(Pawn pawn, Pawn partner)
		{
			// Fail if any single condition fails
			if (isVeneratedAnimal != null && isVeneratedAnimal != pawn.Ideo.IsVeneratedAnimal(partner))
				return false;

			if (!CheckRelations(pawn, partner))
				return false;

			return true;
		}

		private bool CheckRelations(Pawn pawn, Pawn partner)
		{
			if (!initialized)
				Initialize();

			if (hasNoneOfRelationsHashed == null && hasOneOfRelationsHashed == null && hasOneOfRelationDegreesHashed == null)
				return true;

			if (hasOneOfRelationDegreesHashed != null && !hasOneOfRelationDegreesHashed.Contains(RelationHelpers.GetBloodRelationDegree(pawn, partner)))
			{
				return false;
			}

			IEnumerable<PawnRelationDef> relations = pawn.GetRelations(partner);

			if (hasOneOfRelationsHashed != null)
			{
				if (relations.EnumerableNullOrEmpty())
					return false;

				if (!hasOneOfRelationsHashed.Overlaps(relations))
					return false;
			}

			if (hasNoneOfRelationsHashed != null && !relations.EnumerableNullOrEmpty() && hasNoneOfRelationsHashed.Overlaps(relations))
			{
				return false;
			}

			return true;
		}

		private void Initialize()
		{
			if (!hasNoneOfRelations.NullOrEmpty())
				hasNoneOfRelationsHashed = new HashSet<PawnRelationDef>(hasNoneOfRelations);

			if (!hasOneOfRelations.NullOrEmpty())
				hasOneOfRelationsHashed = new HashSet<PawnRelationDef>(hasOneOfRelations);

			if (!hasOneOfRelationDegrees.NullOrEmpty())
				hasOneOfRelationDegreesHashed = new HashSet<BloodRelationDegree>(hasOneOfRelationDegrees);

			initialized = true;
		}
	}
}
