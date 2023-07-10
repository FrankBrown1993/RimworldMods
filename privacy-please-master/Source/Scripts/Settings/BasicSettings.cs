using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace Privacy_Please
{
	public class BasicSettings : ModSettings
	{
		public static bool worryAboutExhibitionism = true;
		public static bool worryAboutMasturbation = true;
		public static bool worryAboutInfidelity = true;
		public static bool worryAboutBeastiality = true;
		public static bool worryAboutRape = true;
		public static bool worryAboutNecro = true;
		public static bool worryAboutXeno = true;
		
		public static bool ignoreRitualAndPartySex = true;
		public static bool slavesIgnoreSex = false;
		public static bool otherFactionsIgnoreSex = false;
		public static bool colonistsIgnoreSlaves = false;
		public static bool colonistsIgnoreOtherFactions = false;

		public static bool rapeIsUninteruptable = false;
		public static bool whoringIsUninteruptable = false;

		public static bool majorTabooCanCausePanic = true;
		public static float chanceForOtherToJoinInSex = 0.25f;
		public static bool debugMode = false;

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref worryAboutExhibitionism, "worryAboutExhibitionism", true);
			Scribe_Values.Look(ref worryAboutMasturbation, "worryAboutMasturbation", true);
			Scribe_Values.Look(ref worryAboutInfidelity, "worryAboutInfidelity", true);
			Scribe_Values.Look(ref worryAboutBeastiality, "worryAboutBeastiality", true);
			Scribe_Values.Look(ref worryAboutRape, "worryAboutRape", true);
			Scribe_Values.Look(ref worryAboutNecro, "worryAboutNecro", true);
			Scribe_Values.Look(ref worryAboutXeno, "worryAboutXeno", true);		
			
			Scribe_Values.Look(ref ignoreRitualAndPartySex, "ignoreRitualAndPartySex", true);
			Scribe_Values.Look(ref slavesIgnoreSex, "slavesIgnoreSex", false);
			Scribe_Values.Look(ref otherFactionsIgnoreSex, "otherFactionsIgnoreSex", false);
			Scribe_Values.Look(ref colonistsIgnoreSlaves, "colonistsIgnoreSlaves", false);
			Scribe_Values.Look(ref colonistsIgnoreOtherFactions, "colonistsIgnoreOtherFactions", false);

			Scribe_Values.Look(ref rapeIsUninteruptable, "rapeIsUninteruptable", false);
			Scribe_Values.Look(ref whoringIsUninteruptable, "whoringIsUninteruptable", false);

			Scribe_Values.Look(ref majorTabooCanCausePanic, "majorTabooCanCausePanic", true);
			Scribe_Values.Look(ref chanceForOtherToJoinInSex, "chanceForSexExtra", 0.25f);
			Scribe_Values.Look(ref debugMode, "debugMode", false);
		}
	}

	public class BasicSettingsDisplay : Mod
	{
		public BasicSettingsDisplay(ModContentPack content) : base(content)
		{
			GetSettings<BasicSettings>();
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			ApplySettings();
		}

		// Update all humanlike pawn graphics when settings window is closed
		public void ApplySettings()
		{
			if (Current.ProgramState == ProgramState.Playing)
			{
				foreach (Pawn pawn in Current.Game.CurrentMap.mapPawns.AllPawns)
				{
					if (pawn == null) continue;

					pawn.Drawer?.renderer?.graphics?.ResolveAllGraphics();

					PortraitsCache.SetDirty(pawn);
					GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
				}
			}
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listingStandard;
			listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);

			listingStandard.Gap(10f);
			listingStandard.Label("privacy_please_general".Translate());
			listingStandard.Gap(5f);

			listingStandard.CheckboxLabeled("worry_about_exhibitionism".Translate(), ref BasicSettings.worryAboutExhibitionism, "worry_about_exhibitionism_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_masturbation".Translate(), ref BasicSettings.worryAboutMasturbation, "worry_about_masturbation_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_infidelity".Translate(), ref BasicSettings.worryAboutInfidelity, "worry_about_infidelity_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_beastiality".Translate(), ref BasicSettings.worryAboutBeastiality, "worry_about_beastiality_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_rape".Translate(), ref BasicSettings.worryAboutRape, "worry_about_rape_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_necro".Translate(), ref BasicSettings.worryAboutNecro, "worry_about_necro_desc".Translate());
			listingStandard.CheckboxLabeled("worry_about_xeno".Translate(), ref BasicSettings.worryAboutXeno, "worry_about_xeno_desc".Translate());

			listingStandard.Gap(20f);

			listingStandard.CheckboxLabeled("ignore_ritual_and_party_sex".Translate(), ref BasicSettings.ignoreRitualAndPartySex, "ignore_ritual_and_party_sex_desc".Translate());
			listingStandard.CheckboxLabeled("slaves_ignore_sex".Translate(), ref BasicSettings.slavesIgnoreSex, "slaves_ignore_sex_desc".Translate());
			listingStandard.CheckboxLabeled("other_factions_ignore_sex".Translate(), ref BasicSettings.otherFactionsIgnoreSex, "other_factions_ignore_sex_desc".Translate());
			listingStandard.CheckboxLabeled("colonists_ignore_slaves".Translate(), ref BasicSettings.colonistsIgnoreSlaves, "colonists_ignore_slaves_desc".Translate());
			listingStandard.CheckboxLabeled("colonists_ignore_other_factions".Translate(), ref BasicSettings.colonistsIgnoreOtherFactions, "colonists_ignore_other_factions_desc".Translate());
			listingStandard.CheckboxLabeled("major_taboo_can_start_fights".Translate(), ref BasicSettings.majorTabooCanCausePanic, "major_taboo_can_start_fights_desc".Translate());
			listingStandard.CheckboxLabeled("rape_is_uninteruptable".Translate(), ref BasicSettings.rapeIsUninteruptable, "rape_is_uninteruptable_desc".Translate());
			listingStandard.CheckboxLabeled("whoring_is_uninteruptable".Translate(), ref BasicSettings.whoringIsUninteruptable, "whoring_is_uninteruptable_desc".Translate());

			listingStandard.Gap(20f);

			listingStandard.Label("chance_for_other_to_join_in_sex".Translate() + ": " + BasicSettings.chanceForOtherToJoinInSex.ToString("F"), -1f, "chance_for_other_to_join_in_sex_desc".Translate());
			BasicSettings.chanceForOtherToJoinInSex = listingStandard.Slider(BasicSettings.chanceForOtherToJoinInSex, 0f, 1f);

			listingStandard.Gap(15f);
			listingStandard.Label("privacy_please_debugging".Translate());
			listingStandard.Gap(5f);

			listingStandard.CheckboxLabeled("debug_mode".Translate(), ref BasicSettings.debugMode, "debug_mode_desc".Translate());

			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}

		public sealed override string SettingsCategory()
		{
			return "privacy_please_basicsettings".Translate();
		}
	}
}
