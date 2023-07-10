using RimWorld;
using RJWSexperience.Ideology.Precepts;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RJWSexperience.Ideology
{
	public static class RomanceChanceFactorHelpers
	{
		/// <summary>
		/// Default value for parent relation
		/// </summary>
		private const float parentRomanceChanceFactor = 0.03f;

		/// <summary>
		/// Get ideology adjusted romanceChanceFactor
		/// </summary>
		public static float GetRomanceChanceFactor(Pawn pawn, Pawn partner)
		{
			float romanceChanceFactor = 1f;

			if (!pawn.relations.FamilyByBlood.Contains(partner))
			{
				if (pawn.Ideo?.HasPrecept(RsiDefOf.Precept.Incestuos_IncestOnly) == true)
				{
					return parentRomanceChanceFactor;
				}
				else
				{
					return romanceChanceFactor;
				}
			}

			PreceptDef incestuousPrecept = pawn.Ideo?.PreceptsListForReading.Select(precept => precept.def).FirstOrFallback(def => def.issue == RsiDefOf.Issue.Incestuos);
			IEnumerable<PawnRelationDef> relations = pawn.GetRelations(partner).Where(def => def.familyByBloodRelation);
			foreach (PawnRelationDef relationDef in relations)
			{
				romanceChanceFactor *= GetRomanceChanceFactor(relationDef, incestuousPrecept);
			}

			return romanceChanceFactor;
		}

		/// <summary>
		/// Get ideology adjusted romanceChanceFactor for the relation
		/// </summary>
		public static float GetRomanceChanceFactor(PawnRelationDef relationDef, PreceptDef incestuousPrecept)
		{
			if (incestuousPrecept == null)
			{
				return relationDef.romanceChanceFactor;
			}

			var incestDefExt = incestuousPrecept.GetModExtension<DefExtension_Incest>();

			if (incestDefExt == null)
			{
				return relationDef.romanceChanceFactor;
			}

			BloodRelationDegree relationDegree = RelationHelpers.GetBloodRelationDegree(relationDef);

			if (incestDefExt.TryGetRomanceChanceFactor(relationDegree, out var romanceChanceOverride))
			{
				return romanceChanceOverride;
			}

			return relationDef.romanceChanceFactor;
		}

		[DebugAction("RJW Sexperience Ideology", "Show romanceChanceFactors", false, true, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Entry)]
		public static void DisplayDebugTable()
		{
			IEnumerable<PreceptDef> incestuousPrecepts = DefDatabase<PreceptDef>
				.AllDefsListForReading
				.Where(def => def.issue == RsiDefOf.Issue.Incestuos);

			IEnumerable<TableDataGetter<PawnRelationDef>> preceptGetters = incestuousPrecepts
				.Select(precept => new TableDataGetter<PawnRelationDef>(precept.defName,(PawnRelationDef rel) => GetRomanceChanceFactor(rel, precept)));

			var relName = new TableDataGetter<PawnRelationDef>("Relation Def", (PawnRelationDef rel) => rel.defName);

			TableDataGetter<PawnRelationDef>[] getters = (new List<TableDataGetter<PawnRelationDef>>() { relName }).Concat(preceptGetters).ToArray();

			DebugTables.MakeTablesDialog(DefDatabase<PawnRelationDef>.AllDefsListForReading, getters);
		}
	}
}