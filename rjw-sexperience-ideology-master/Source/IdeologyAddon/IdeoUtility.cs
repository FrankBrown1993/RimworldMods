using RimWorld;
using rjw;
using Verse;

namespace RJWSexperience.Ideology
{
	public static class IdeoUtility
	{
		public static bool IsSubmissive(this Pawn pawn)
		{
			Ideo ideo = pawn.Ideo;
			if (ideo == null)
				return false;

			if (ideo.HasPrecept(RsiDefOf.Precept.Submissive_Female) && pawn.gender == Gender.Female)
				return true;
			else if (ideo.HasPrecept(RsiDefOf.Precept.Submissive_Male) && pawn.gender == Gender.Male)
				return true;

			return false;
		}

		public static float GetPreceptsMtbMultiplier<T>(Ideo ideo) where T : Precepts.DefExtension_ModifyMtb
		{
			float finalMultiplier = 1f;
			for (int i = 0; i < ideo.PreceptsListForReading.Count; i++)
			{
				T defExtension = ideo.PreceptsListForReading[i].def.GetModExtension<T>();

				if (defExtension == null)
					continue;

				finalMultiplier *= defExtension.multiplier;
			}
			return finalMultiplier;
		}

		internal static void ConvertPawnBySex(Pawn pawn, Pawn partner, float severity = 0.01f)
		{
			// Important Note: This is called on "orgasm" - hence when a pawn has the orgasm he is the "pawn" here.
			// If Bob fucks Alice, Alice has the orgasm and Alice is the Pawn while Bob is the Partner.
			// Hence, the Conversion happens from Partner -> Pawn and not the other way round!

			// Short Circuit: Either pawn is null, exit early and do nothing
			if (pawn == null || partner == null)
				return;

			bool sameIdeo = pawn.Ideo == partner.Ideo;
			// Option A: Partner has same Ideo as Pawn, increase certainty
			if (sameIdeo)
			{
				if (partner.ideo.Certainty < 1f)
				{
					partner.ideo.OffsetCertainty(severity);
				}
			}
			// Option B: Partner as different Ideo, try to convert
			else
			{
				pawn.ideo.IdeoConversionAttempt(severity, partner.Ideo);
			}
		}

		public static float GetGenitalSize(Pawn p)
		{
			if (p == null)
				return 0f;

			// Iff the pawn has multiple genitalia, the "best" is picked (the biggest penis or tightest vagina)
			float bestSeenSize = 0f;
			foreach (Hediff part in Genital_Helper.get_AllPartsHediffList(p))
			{
				// Only check for Vaginas and Penises, not for Anus or for things not categorized as primary sexual parts
				if (Genital_Helper.is_penis(part) || Genital_Helper.is_vagina(part))
				{
					bestSeenSize = part.Severity > bestSeenSize ? part.Severity : bestSeenSize;
				}
			}

			// For Women, the scale is inverted.
			if (p.gender == Gender.Female)
				return 1 - bestSeenSize;

			return bestSeenSize;
		}

		public static bool IsVisiblyPregnant(Pawn pawn)
		{
			Hediff pregnancy = PregnancyHelper.GetPregnancy(pawn);

			// Currently RJW does not check Biotech pregnancy
			if (pregnancy == null && RsiDefOf.Hediff.PregnantHuman != null)
			{
				pregnancy = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PregnantHuman);
			}

			return pregnancy?.Visible == true;
		}
	}
}
