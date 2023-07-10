/*using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace rjwex
{
	public class SexCardUtility
	{
		public static Vector2 CardSize = new Vector2(395f, 536f); // set the card's size

		public static void DrawCard(Rect rect, Pawn pawn)
		{
			CompRJW comp = pawn.TryGetComp<CompRJW>();
			if (pawn == null || comp == null) return;

			Text.Font = GameFont.Medium;
			Rect rect1 = new Rect(8f, 4f, rect.width - 8f, rect.height - 20f);
			Widgets.Label(rect1, "RJW");
			Text.Font = GameFont.Tiny;
			float num = rect1.y + 40f;
			Rect row1 = new Rect(10f, num, rect.width - 8f, 24f);
			Rect row2 = new Rect(10f, num + 24, rect.width - 8f, 24f);
			Rect row3 = new Rect(10f, num + 48, rect.width - 8f, 24f);
			Rect button1 = new Rect(10f, rect1.height - 10f, rect.width - 8f, 24f);
			Rect button2 = new Rect(10f, rect1.height - 34f, rect.width - 8f, 24f);

			string price;
			string sexuality;

			// Check for Rational Romance consistency, in case the player adds it mid-game or adds traits (such as with Prepare Carefully)
			if (xxx.RomanceDiversifiedIsActive || pawn.story.traits.HasTrait(TraitDefOf.Gay))
				CompRJW.RRTraitCheck(pawn);

			switch (CompRJW.Comp(pawn).orientation)
			{
				case Orientation.Asexual:
					sexuality = "Asexual";
					break;
				case Orientation.Bisexual:
					sexuality = "Bisexual";
					break;
				case Orientation.Heterosexual:
					sexuality = "Hetero";
					break;
				case Orientation.Homosexual:
					sexuality = "Gay";
					break;
				case Orientation.LeaningHeterosexual:
					sexuality = "Bisexual, leaning hetero";
					break;
				case Orientation.LeaningHomosexual:
					sexuality = "Bisexual, leaning gay";
					break;
				case Orientation.MostlyHeterosexual:
					sexuality = "Mostly hetero";
					break;
				case Orientation.MostlyHomosexual:
					sexuality = "Mostly gay";
					break;
				case Orientation.Pansexual:
					sexuality = "Pansexual";
					break;
				default:
					sexuality = "None";
					break;
			}


			Widgets.Label(row1, "Sexuality: " + sexuality);
			if (Mouse.IsOver(row1))
				Widgets.DrawHighlight(row1);

			string quirklist = CompRJW.Comp(pawn).quirks.ToString();
			Widgets.Label(row2, "Quirks".Translate() + quirklist);
			if (Mouse.IsOver(row2))
			{
				Widgets.DrawHighlight(row2);
				if (quirklist == "None")
					TooltipHandler.TipRegion(row2, "NoQuirks".Translate());
				else
				{
					StringBuilder tooltip = new StringBuilder();

					if (quirklist.Contains("Breeder"))
						tooltip.AppendLine("BreederQuirk".Translate());

					if (quirklist.Contains("Endytophile"))
						tooltip.AppendLine("EndytophileQuirk".Translate());

					if (quirklist.Contains("Exhibitionist"))
						tooltip.AppendLine("ExhibitionistQuirk".Translate());

					if (quirklist.Contains("Fertile"))
						tooltip.AppendLine("FertileQuirk".Translate());

					if (quirklist.Contains("Gerontophile"))
						tooltip.AppendLine("GerontophileQuirk".Translate());

					if (quirklist.Contains("Impregnation fetish"))
						tooltip.AppendLine("ImpregnationFetishQuirk".Translate());

					if (quirklist.Contains("Incubator"))
						tooltip.AppendLine("IncubatorQuirk".Translate());

					if (quirklist.Contains("Infertile"))
						tooltip.AppendLine("InfertileQuirk".Translate());

					if (quirklist.Contains("Messy"))
						tooltip.AppendLine("MessyQuirk".Translate());

					if (quirklist.Contains("Podophile"))
						tooltip.AppendLine("PodophileQuirk".Translate());

					if (quirklist.Contains("Pregnancy fetish"))
						tooltip.AppendLine("PregnancyFetishQuirk".Translate());

					if (quirklist.Contains("Sapiosexual"))
						tooltip.AppendLine("SapiosexualQuirk".Translate());

					if (quirklist.Contains("Somnophile"))
						tooltip.AppendLine("SomnophileQuirk".Translate());

					if (quirklist.Contains("Teratophile"))
						tooltip.AppendLine("TeratophileQuirk".Translate());

					if (quirklist.Contains("Vigorous"))
						tooltip.AppendLine("VigorousQuirk".Translate());

					TooltipHandler.TipRegion(row2, tooltip.ToString());
				}
			}

			if (RJWSettings.sex_minimum_age > pawn.ageTracker.AgeBiologicalYears)
				price = "Inapplicable (too young)";
			else if (pawn.ownership.OwnedRoom == null)
			{
				if (Current.ProgramState == ProgramState.Playing)
					price = "TODO";//WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn) + " (base, needs suitable bedroom)";
				else
					price = "TODO";//WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn) + " (base, modified by bedroom quality)";
			}
			else if (xxx.is_animal(pawn))
				price = "Incapable of whoring";
			else
				price = "TODO";//WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn);

			Widgets.Label(row3, "WhorePrice".Translate() + price);
			if (Mouse.IsOver(row3))
				Widgets.DrawHighlight(row3);

			// TODO: Add translation.
			if (Prefs.DevMode || Current.ProgramState != ProgramState.Playing)
			{
				if (Widgets.ButtonText(button1, Current.ProgramState != ProgramState.Playing ? "Reroll sexuality" : "[DEV] Reroll sexuality"))
				{
					CompRJW.Comp(pawn).Sexualize(pawn, true);
				}
			}
			if (pawn.health.hediffSet.HasHediff(Genital_Helper.archotech_penis) || pawn.health.hediffSet.HasHediff(Genital_Helper.archotech_vagina))
			{
				BodyPartRecord genitalia = Genital_Helper.get_genitals(pawn);
				if (pawn.health.hediffSet.HasHediff(HediffDef.Named("ImpregnationBlocker")))
				{
					if (Widgets.ButtonText(button2, "[Archotech genitalia] Enable reproduction"))
					{
						Hediff blocker = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("ImpregnationBlocker"));
						if (blocker != null)
							pawn.health.RemoveHediff(blocker);
						pawn.health.AddHediff(HediffDef.Named("FertilityEnhancer"), genitalia);
					}
				}
				else if (Widgets.ButtonText(button2, "[Archotech genitalia] Disable reproduction"))
				{
					Hediff enhancer = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("FertilityEnhancer"));
					if (enhancer != null)
						pawn.health.RemoveHediff(enhancer);
					pawn.health.AddHediff(HediffDef.Named("ImpregnationBlocker"), genitalia);
				}
			}
		}
	}
}*/