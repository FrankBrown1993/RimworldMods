using RimWorld;
using System.Linq;
using Verse;

namespace RJWSexperience.Ideology
{
	public static class RelationHelpers
	{
		/// <summary>
		/// Get degree of blood relation between two pawns
		/// </summary>
		public static BloodRelationDegree GetBloodRelationDegree(Pawn pawn, Pawn partner)
		{
			if (!pawn.relations.FamilyByBlood.Contains(partner))
			{
				return BloodRelationDegree.NotRelated;
			}

			PawnRelationDef closestBloodRelation = pawn
				.GetRelations(partner)
				?.Where(def => def.familyByBloodRelation)
				?.OrderByDescending(def => def.importance)
				?.FirstOrFallback();

			if (closestBloodRelation == null)
			{
				return BloodRelationDegree.NotRelated;
			}

			return GetBloodRelationDegree(closestBloodRelation);
		}

		/// <summary>
		/// Get degree of blood relation for a relationDef
		/// </summary>
		public static BloodRelationDegree GetBloodRelationDegree(PawnRelationDef relationDef)
		{
			if (!relationDef.familyByBloodRelation)
			{
				return BloodRelationDegree.NotRelated;
			}
			else if (relationDef.importance <= PawnRelationDefOf.Cousin.importance)
			{
				return BloodRelationDegree.FarRelative;
			}
			else
			{
				return BloodRelationDegree.CloseRelative;
			}
		}
	}
}
