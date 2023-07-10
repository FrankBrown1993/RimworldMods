using Verse;
using HarmonyLib;
using System;
using rjw;
using RimWorld;

namespace rjwstd
{
	/// <summary>
	/// do damage to pawn health
	/// </summary>
	public static class std_Syphilis
	{
		public static std_def syphilis = DefDatabase<std_def>.GetNamed("Syphilis");
		public static void update(Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(syphilis.hediff_def))
				roll_for_syphilis_damage(pawn);
		}

		//[SyncMethod]
		public static void roll_for_syphilis_damage(Pawn p)
		{
			Hediff syp = p.health.hediffSet.GetFirstHediffOfDef(syphilis.hediff_def);
			if (syp == null || !(syp.Severity >= 0.60f) || syp.FullyImmune()) return;

			// A 30% chance per day of getting any permanent damage works out to ~891 in 1 million for each roll
			// The equation is (1-x)^(60000/150)=.7
			// Important Note:
			// this function is called from Need_Sex::NeedInterval(), where it involves a needsex_tick and a std_tick to actually trigger this roll_for_syphilis_damage.
			// j(this is not exactly the same as the value in Need_Sex, that value is 0, but here j should be 1) std_ticks per this function called, k needsex_ticks per std_tick, 150 ticks per needsex_tick, and x is the chance per 150 ticks,
			// The new equation should be .7 = (1-x)^(400/kj)
			// 1-x = .7^(kj/400), x =1-.7^(kj/400)
			// Since k=10,j=1, so kj=10, new x is 1-.7^(10/400)=0.0088772362, let it be 888/100000
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			if (Rand.RangeInclusive(1, 100000) <= 888)
			{
				BodyPartRecord part;
				float sev;
				var parts = p.RaceProps.body.AllParts;

				float rv = Rand.Value;
				if (rv < 0.10f)
				{
					part = parts.Find(bpr => string.Equals(bpr.def.defName, "Brain"));
					sev = 1.0f;
				}
				else if (rv < 0.50f)
				{
					part = parts.Find(bpr => string.Equals(bpr.def.defName, "Liver"));
					sev = Rand.RangeInclusive(1, 3);
				}
				else if (rv < 0.75f)
				{
					//LeftKidney, probably
					part = parts.Find(bpr => string.Equals(bpr.def.defName, "Kidney"));
					sev = Rand.RangeInclusive(1, 2);
				}
				else
				{
					//RightKidney, probably
					part = parts.FindLast(bpr => string.Equals(bpr.def.defName, "Kidney"));
					sev = Rand.RangeInclusive(1, 2);
				}

				if (part != null && !p.health.hediffSet.PartIsMissing(part) && !p.health.hediffSet.HasDirectlyAddedPartFor(part))
				{
					DamageDef vir_dam = DefDatabase<DamageDef>.GetNamed("ViralDamage");
					HediffDef dam_def = HealthUtility.GetHediffDefFromDamage(vir_dam, p, part);
					Hediff_Injury inj = (Hediff_Injury)HediffMaker.MakeHediff(dam_def, p, null);
					inj.Severity = sev;
					inj.TryGetComp<HediffComp_GetsPermanent>().IsPermanent = true;
					p.health.AddHediff(inj, part, null);
					string message_title = syphilis.label + " Damage";
					string baby_pronoun = p.gender == Gender.Male ? "his" : "her";
					string message_text = "RJW_Syphilis_Damage_Message".Translate(xxx.get_pawnname(p), baby_pronoun, part.def.label, syphilis.label).CapitalizeFirst();
					Find.LetterStack.ReceiveLetter(message_title, message_text, LetterDefOf.ThreatSmall, p);
				}
			}
		}
	}
}
