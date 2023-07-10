using Verse;
using HarmonyLib;
using System;
using rjw;
using RimWorld;

namespace rjwstd
{
	public static class std_Boobitis
	{
		public static std_def boobitis = DefDatabase<std_def>.GetNamed("Boobitis");
		public static void update(Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(boobitis.hediff_def))
				UpdateBoobitis(pawn);

		}

		public static void UpdateBoobitis(Pawn pawn)
		{
			var Parts = pawn.GetBreastList();

			if (!Parts.NullOrEmpty())
			{
				foreach (Hediff hed in Parts)
				{
					if (!(hed is Hediff_PartBaseNatural))
						continue;

					var boob = pawn.health.hediffSet.GetFirstHediffOfDef(boobitis.hediff_def).Severity;
					if (boob >= 1.0)
					{
						//re-add boob dmg someday?
						continue;
					}

					var hedstage = hed.CurStageIndex;
					//GenderHelper.ChangeSex(pawn, () =>
					//{
						hed.Severity += boob * 0.01f; // ~0.7 beast grow
					//});

					if (hedstage < hed.CurStageIndex)
					{
						PartSizeExtension.TryGetCupSize(hed, out float size);
						var cupSize = (int)size;
						var cup = PartStagesDef.GetCupSizeLabel(cupSize);

						string message_title = boobitis.LabelCap;
						string message_text = "RJW_BreastsHaveGrownFromBoobitis".Translate(xxx.get_pawnname(pawn), pawn.Possessive(), hed.def.label.ToLower(), cup, boobitis.LabelCap).CapitalizeFirst();
						Find.LetterStack.ReceiveLetter(message_title, message_text, LetterDefOf.NeutralEvent, pawn);

						//var message = "RJW_BreastsHaveGrownFromBoobitis".Translate(xxx.get_pawnname(pawn));
						//Messages.Message(message, pawn, MessageTypeDefOf.SilentInput);
					}
				}
			}
		}
	}
}
