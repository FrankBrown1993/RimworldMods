﻿using RimWorld;
using rjw;
using RJWSexperience.SexHistory;
using Verse;

namespace RJWSexperience
{
	public static class DebugToolsSexperience
	{
		[DebugAction("RJW Sexperience", "Reset pawn's record", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void ResetRecords(Pawn p)
		{
			Trait virgin = p.story?.traits?.GetTrait(RsDefOf.Trait.Virgin);
			if (virgin != null) p.story.traits.RemoveTrait(virgin);
			ResetRecord(p, true);
			if (ResetRecord(p, false))
				Virginity.TraitHandler.GenerateVirginTrait(p);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Records resetted!");
		}

		[DebugAction("RJW Sexperience", "Reset pawn's record(virgin)", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void ResetRecordsZero(Pawn p)
		{
			ResetRecord(p, true);
			Virginity.TraitHandler.AddVirginTrait(p);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Records resetted!\nVirginified!");
		}

		[DebugAction("RJW Sexperience", "Reset lust", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void ResetLust(Pawn p)
		{
			float lust = RecordRandomizer.RandomizeLust(p);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Lust: " + lust);
		}

		[DebugAction("RJW Sexperience", "Set lust to 0", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void SetLust(Pawn p)
		{
			p.records.SetTo(RsDefOf.Record.Lust, 0);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Lust: 0");
		}

		[DebugAction("RJW Sexperience", "Add 10 to lust", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void AddLust(Pawn p)
		{
			p.records.AddTo(RsDefOf.Record.Lust, 10);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Lust: " + p.records.GetValue(RsDefOf.Record.Lust));
		}

		[DebugAction("RJW Sexperience", "Subtract 10 to lust", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void SubtractLust(Pawn p)
		{
			p.records.AddTo(RsDefOf.Record.Lust, -10);
			MoteMaker.ThrowText(p.TrueCenter(), p.Map, "Lust: " + p.records.GetValue(RsDefOf.Record.Lust));
		}

		private static bool ResetRecord(Pawn pawn, bool allzero)
		{
			if (!allzero)
			{
				if (SexperienceMod.Settings.EnableRecordRandomizer && xxx.is_human(pawn))
				{
					return RecordRandomizer.Randomize(pawn);
				}
			}
			else
			{
				pawn.records.SetTo(RsDefOf.Record.Lust, 0);
				pawn.records.SetTo(RsDefOf.Record.NumofEatenCum, 0);
				pawn.records.SetTo(RsDefOf.Record.AmountofEatenCum, 0);
				pawn.records.SetTo(RsDefOf.Record.VaginalSexCount, 0);
				pawn.records.SetTo(RsDefOf.Record.AnalSexCount, 0);
				pawn.records.SetTo(RsDefOf.Record.OralSexCount, 0);
				pawn.records.SetTo(RsDefOf.Record.BlowjobCount, 0);
				pawn.records.SetTo(RsDefOf.Record.CunnilingusCount, 0);
				pawn.records.SetTo(RsDefOf.Record.GenitalCaressCount, 0);
				pawn.records.SetTo(RsDefOf.Record.HandjobCount, 0);
				pawn.records.SetTo(RsDefOf.Record.FingeringCount, 0);
				pawn.records.SetTo(RsDefOf.Record.FootjobCount, 0);
				pawn.records.SetTo(RsDefOf.Record.MiscSexualBehaviorCount, 0);
				pawn.records.SetTo(RsDefOf.Record.SexPartnerCount, 0);
				pawn.records.SetTo(RsDefOf.Record.OrgasmCount, 0);
				pawn.records.SetTo(xxx.CountOfBeenRapedByAnimals, 0);
				pawn.records.SetTo(xxx.CountOfBeenRapedByHumanlikes, 0);
				pawn.records.SetTo(xxx.CountOfBeenRapedByInsects, 0);
				pawn.records.SetTo(xxx.CountOfBeenRapedByOthers, 0);
				pawn.records.SetTo(xxx.CountOfBirthAnimal, 0);
				pawn.records.SetTo(xxx.CountOfBirthEgg, 0);
				pawn.records.SetTo(xxx.CountOfBirthHuman, 0);
				pawn.records.SetTo(xxx.CountOfFappin, 0);
				pawn.records.SetTo(xxx.CountOfRapedAnimals, 0);
				pawn.records.SetTo(xxx.CountOfRapedHumanlikes, 0);
				pawn.records.SetTo(xxx.CountOfRapedInsects, 0);
				pawn.records.SetTo(xxx.CountOfRapedOthers, 0);
				pawn.records.SetTo(xxx.CountOfSex, 0);
				pawn.records.SetTo(xxx.CountOfSexWithAnimals, 0);
				pawn.records.SetTo(xxx.CountOfSexWithCorpse, 0);
				pawn.records.SetTo(xxx.CountOfSexWithHumanlikes, 0);
				pawn.records.SetTo(xxx.CountOfSexWithInsects, 0);
				pawn.records.SetTo(xxx.CountOfSexWithOthers, 0);
			}

			return true;
		}
	}
}
