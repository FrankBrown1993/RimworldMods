using Verse;
using HarmonyLib;
using System;
using rjw;
using RimWorld;

namespace rjwstd
{
	/// <summary>
	/// add some infections
	/// </summary>
	public static class std_Immunodeficiency
	{
		public static std_def hiv = DefDatabase<std_def>.GetNamed("HIV");
		//vanilla rimworld
		public static readonly HediffDef immunodeficiency = DefDatabase<HediffDef>.GetNamed("Immunodeficiency");
		public static bool is_wasting_away(Pawn p)
		{
			Hediff id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
			return id != null && id.CurStageIndex > 0;
		}

		public static void update(Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(immunodeficiency))
				update_immunodeficiency(pawn);
		}

		//[SyncMethod]
		public static void update_immunodeficiency(Pawn p)
		{
			float min_bf_for_id = 1.0f - immunodeficiency.minSeverity;
			Hediff id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
			float bf = p.health.capacities.GetLevel(PawnCapacityDefOf.BloodFiltration);
			bool has = id != null;
			bool should_have = bf <= min_bf_for_id;

			if (has && !should_have)
			{
				p.health.RemoveHediff(id);
				id = null;
			}
			else if (!has && should_have)
			{
				p.health.AddHediff(immunodeficiency);
				id = p.health.hediffSet.GetFirstHediffOfDef(immunodeficiency);
			}

			if (id == null) return;

			id.Severity = 1.0f - bf;

			// Roll for and apply opportunistic infections:
			// Pawns will have a 90% chance for at least one infection each year at 0% filtration, and a 0%
			// chance at 40% filtration, scaling linearly.
			// Let x = chance infected per roll
			// Then chance not infected per roll = 1 - x
			// And chance not infected on any roll in one day = (1 - x) ^ (60000 / 150) = (1 - x) ^ 400
			// And chance not infected on any roll in one year = (1 - x) ^ (400 * 60) = (1 - x) ^ 24000
			// So 0.10 = (1 - x) ^ 24000
			//	log (0.10) = 24000 log (1 - x)
			//	x = 0.00009593644334648975435114691213 = ~96 in 1 million
			// Important Note:
			// this function is called from Need_Sex::NeedInterval(), where it involves a needsex_tick and a std_tick to actually trigger this update_immunodeficiency.
			// j(this is not exactly the same as the value in Need_Sex, that value is 0, but here j should be 1) std_ticks per this function called, k needsex_ticks per std_tick, 150 ticks per needsex_tick, and x is the chance per 150 ticks,
			// The new equation should be .1 = (1-x)^(24000/kj)
			// log(.1) = (24000/kj) log(1-x),  so log(1-x)= (kj/24000) log(.1), 1-x = .1^(kj/24000), x= 1-.1^(kj/24000)
			// Since k=10,j=1, so kj=10, new x is 1-.1^(10/24000)=0.0009589504, let it be 959/1000000
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			if (Rand.RangeInclusive(1, 1000000) <= 959 && Rand.Value < bf / min_bf_for_id)
			{
				BodyPartRecord part;
				{
					float rv = Rand.Value;
					var parts = p.RaceProps.body.AllParts;
					if (rv < 0.25f)
						part = parts.Find(bpr => string.Equals(bpr.def.defName, "Jaw"));
					else if (rv < 0.50f)
						part = parts.Find(bpr => string.Equals(bpr.def.defName, "Lung"));
					else if (rv < 0.75f)
						part = parts.FindLast(bpr => string.Equals(bpr.def.defName, "Lung"));
					else
						part = parts.RandomElement();
				}

				if (part != null &&
					!p.health.hediffSet.PartIsMissing(part) && !p.health.hediffSet.HasDirectlyAddedPartFor(part) &&
					p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WoundInfection) == null && // If the pawn already has a wound infection, we can't properly set the immunity for the new one
					p.health.immunity.GetImmunity(HediffDefOf.WoundInfection) <= 0.0f)
				{ // Dont spawn infection if pawn already has immunity
					p.health.AddHediff(HediffDefOf.WoundInfection, part);
					p.health.HealthTick(); // Creates the immunity record
					ImmunityRecord ir = p.health.immunity.GetImmunityRecord(HediffDefOf.WoundInfection);
					if (ir != null)
						ir.immunity = STDBase.opp_inf_initial_immunity;
					const string message_title = "Opportunistic Infection";
					string message_text = "RJW_Opportunistic_Infection_Message".Translate(xxx.get_pawnname(p)).CapitalizeFirst();
					Find.LetterStack.ReceiveLetter(message_title, message_text, LetterDefOf.ThreatSmall);
				}
			}
		}
	}
}
