using Verse;
using HarmonyLib;
using rjw;
using System;
using RimWorld;

namespace rjwstd
{
	public static class std_Rash
	{
		public static std_def herpes = DefDatabase<std_def>.GetNamed("Herpes");
		public static std_def warts = DefDatabase<std_def>.GetNamed("Warts");

		public static readonly ThoughtDef saw_rash_1 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates1");
		public static readonly ThoughtDef saw_rash_2 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates2");
		public static readonly ThoughtDef saw_rash_3 = DefDatabase<ThoughtDef>.GetNamed("SawDiseasedPrivates3");

		/// <summary>
		/// Returns how severely affected this pawn's crotch is by rashes and warts, on a scale from 0 to 3.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static int genital_rash_severity(Pawn p)
		{
			int tr = 0;

			Hediff her = p.health.hediffSet.GetFirstHediffOfDef(std_Rash.herpes.hediff_def);
			if (her != null && her.Severity >= 0.25f)
				++tr;

			Hediff war = p.health.hediffSet.GetFirstHediffOfDef(std_Rash.warts.hediff_def);
			if (war != null)
				tr += war.Severity < 0.40f ? 1 : 2;

			return tr;
		}

		///<summary>
		///add aftersex thoughts about herpes, warts
		///</summary>
		public static void ThinkAboutDiseases(Pawn pawn, Pawn partner)
		{
			if (pawn == null)
				return;
			if (partner == null)
				return;

			if (!(xxx.is_human(pawn) && xxx.is_human(partner)))
				return;

			// Dead and non-humans have no diseases (yet?).
			if (partner.Dead || !xxx.is_human(partner))
				return;

			// check for visible diseases
			// Add negative relation for visible diseases on the genitals
			int pawn_rash_severity = genital_rash_severity(pawn) - genital_rash_severity(partner);
			ThoughtDef pawn_thought_about_rash;
			if (pawn_rash_severity == 1)
				pawn_thought_about_rash = saw_rash_1;
			else if (pawn_rash_severity == 2)
				pawn_thought_about_rash = saw_rash_2;
			else if (pawn_rash_severity >= 3)
				pawn_thought_about_rash = saw_rash_3;
			else
				return;
			Thought_Memory memory = (Thought_Memory)ThoughtMaker.MakeThought(pawn_thought_about_rash);
			partner.needs.mood.thoughts.memories.TryGainMemory(memory, pawn);
		}
	}
}
