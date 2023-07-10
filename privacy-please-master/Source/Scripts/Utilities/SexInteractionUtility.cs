using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using rjw;
using UnityEngine;
using HarmonyLib;

namespace Privacy_Please
{
    public static class SexInteractionUtility
    {
		public static bool PawnCaughtLovinByWitness(Pawn pawn, Pawn witness)
		{
			if (witness == null || pawn == witness || witness.IsUnableToSenseSex() || witness.CanSee(pawn) == false) return false;

			List<Pawn> sexParticipants = pawn.GetAllSexParticipants();
			bool witnessIsJoiningSex = witness.jobs.curDriver is JobDriver_SexBaseInitiator && sexParticipants.Contains((witness.jobs.curDriver as JobDriver_SexBaseInitiator).Target);

			if (sexParticipants.Contains(witness) || witnessIsJoiningSex) return false;
	
			return true;
		}

		public static bool PawnIsCheatingOnPartner(Pawn cheater, Pawn victim)
		{
			List<Pawn> spouses = cheater.GetSpouses(false);
			
			if (BasicSettings.worryAboutInfidelity == false || 
				victim.IsLoverOfOther(cheater) == false ||
				cheater.HasTrait("Polygamous") ||
				victim.HasTrait("Polygamous") ||
				cheater.IsHavingSex() == false ||
				SexActIsNecrophilia(cheater.jobs.curDriver as JobDriver_Sex) ||
				SexActIsBestiality(cheater.jobs.curDriver as JobDriver_Sex) ||
				cheater.GetAllSexParticipants().Contains(victim) ||
				(spouses.NullOrEmpty() == false && cheater.GetAllSexParticipants().Any(x => spouses.Contains(x))))
			{ return false; }
			
			return true;
		}

		public static bool SexParticipantsIncludesACheatingPartner(Pawn pawn, List<Pawn> participants)
		{
			foreach (Pawn participant in participants)
			{
				if (PawnIsCheatingOnPartner(pawn, participant))
				{ return true; }
			}

			return false;
		}

		public static bool PasserbyCanBePropositionedForSex(Pawn passerby, List<Pawn> participants)
		{
			if (passerby == null || participants.Contains(passerby))
			{ DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": they are already involved in the activity"); return false; }

			if (participants.Any(x => x.CanSee(passerby)) == false)
			{ DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": no-one involved can see them"); return false; }

			if (participants.Count > 2)			
			{ DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": max participants has been reached"); return false; }

			if (participants.Any(x => x.IsForbidden(passerby) || x.HostileTo(passerby)))
			{ DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": someone is forbidden or hostile"); return false; }

			if (CasualSex_Helper.CanHaveSex(passerby) == false || xxx.IsTargetPawnOkay(passerby) == false)
			{ DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": they cannot have sex"); return false; }

			if (SexUtility.ReadyForHookup(passerby) &&
				(passerby?.jobs?.curJob == null || (passerby.jobs.curJob.playerForced == false && CasualSex_Helper.quickieAllowedJobs.Contains(passerby.jobs.curJob.def))) &&
				participants.Any(x => SexAppraiser.would_fuck(x, passerby) > 0.1f && SexAppraiser.would_fuck(passerby, x) > 0.1f) &&
				participants.All(x => SexAppraiser.would_fuck(x, passerby, false, false, true) > 0.1f && SexAppraiser.would_fuck(passerby, x, false, false, true) > 0.1f))
			{ return true; }

			DebugMode.Message("Cannot proposition " + passerby.NameShortColored + ": they are either too busy or not appealing");
			return false;
		}

		public static void GetReactionsToSexAct(JobDriver_Sex jobDriver, Pawn witness, out ReactionToSexAct reactionOfPawn, out ReactionToSexAct reactionOfWitness, bool applyThoughtDefs = false)
		{
			Pawn pawn = jobDriver.pawn;

			reactionOfPawn = ReactionToSexAct.Uncaring;
			reactionOfWitness = ReactionToSexAct.Uncaring;
			
			// Determine if there are any issues with the sex event and the witness' morals and apply thoughtDefs as required
			foreach (SexActReactionDef sexActReactionDef in DefDatabase<SexActReactionDef>.AllDefs)
			{
				var methodInfo = AccessTools.Method(typeof(SexInteractionUtility), sexActReactionDef.sexActCheck, null, null);

				if (methodInfo == null)
				{ DebugMode.Message("Method '" + sexActReactionDef.sexActCheck + "' was not found"); continue; }

				if ((bool)methodInfo.Invoke(null, new object[] { jobDriver, witness }))
				{ sexActReactionDef.DetermineReactionOfPawns(pawn, witness, out reactionOfPawn, out reactionOfWitness, applyThoughtDefs); break; }
			}

			DebugMode.Message("Reaction of " + pawn.NameShortColored + " to " + witness.NameShortColored + "'s presence: " + reactionOfPawn.ToString());
			DebugMode.Message("Reaction of " + witness.NameShortColored + " to " + pawn.NameShortColored + "'s activities: " + reactionOfWitness.ToString());

			// Exit here if thoughtDefs are not being applied			
			if (applyThoughtDefs == false || BasicSettings.majorTabooCanCausePanic == false) return;
			if (witness?.Drafted == true || witness?.mindState?.duty?.def.alwaysShowWeapon == true) return;
			
			// Panic reaction
			if (reactionOfWitness == ReactionToSexAct.Panic)
			{
				Job job = JobMaker.MakeJob(JobDefOf.FleeAndCower, CellFinderLoose.GetFleeDest(witness, new List<Thing>() { pawn }, 24f), pawn);

				witness.jobs.ClearQueuedJobs();
				witness.jobs.EndCurrentJob(JobCondition.InterruptForced, false, false);
				witness.jobs.StartJob(job);				
			}

			// Vomit reaction
			else if (reactionOfWitness == ReactionToSexAct.Nausea)
			{
				Job jobVomit = JobMaker.MakeJob(JobDefOf.Vomit);
				Job jobFlee = JobMaker.MakeJob(JobDefOf.FleeAndCower, CellFinderLoose.GetFleeDest(witness, new List<Thing>() { pawn }, 24f), pawn);

				witness.jobs.ClearQueuedJobs();
				witness.jobs.EndCurrentJob(JobCondition.InterruptForced, false, false);

				if (Random.value <= 0.25f)
				{
					witness.jobs.StartJob(jobVomit);
					witness.jobs.jobQueue.EnqueueFirst(jobFlee);
				}

				else
				{ witness.jobs.StartJob(jobFlee); }
			}
		}

		public static bool SexActIsNecrophilia(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutNecro && jobDriver.Partner != null && jobDriver.Partner.Dead;
		}

		public static bool SexActIsBestiality(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutBeastiality && jobDriver.Partner != null && jobDriver.Partner.RaceProps.Animal;
		}

		public static bool SexActIsBestialityWithOrdinaryAnimal(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			if (BasicSettings.worryAboutBeastiality == false) return false;

			if (jobDriver.Partner == null || jobDriver.Partner.RaceProps.Animal == false) return false;
			if (jobDriver.pawn.Ideo.PreceptsListForReading.Any(x => x.def.defName == "Bestiality_BondOnly") && jobDriver.Partner.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) != jobDriver.pawn) return true;
			if (jobDriver.pawn.Ideo.PreceptsListForReading.Any(x => x.def.defName == "Bestiality_OnlyVenerated") && jobDriver.pawn.Ideo.IsVeneratedAnimal(jobDriver.Partner) == false) return true;
			
			return false;
		}

		public static bool SexActIsBestialityWithSpecialAnimal(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			if (BasicSettings.worryAboutBeastiality == false) return false;
			
			if (jobDriver.Partner == null || jobDriver.Partner.RaceProps.Animal == false) return false;
			if (jobDriver.pawn.Ideo.PreceptsListForReading.Any(x => x.def.defName == "Bestiality_BondOnly") && jobDriver.Partner.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) == jobDriver.pawn) return true;
			if (jobDriver.pawn.Ideo.PreceptsListForReading.Any(x => x.def.defName == "Bestiality_OnlyVenerated") && jobDriver.pawn.Ideo.IsVeneratedAnimal(jobDriver.Partner) == true) return true;

			return false;
		}

		public static bool SexActIsRape(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutRape && (jobDriver is JobDriver_Rape || jobDriver is JobDriver_RapeEnemy || jobDriver is JobDriver_SexBaseRecieverRaped);
		}

		public static bool SexActIsXenophilia(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutXeno && jobDriver.Partner != null && jobDriver.Partner.def.defName != jobDriver.pawn.def.defName;
		}

		public static bool SexActIsExhibitionism(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutExhibitionism && (jobDriver.pawn.IsMasturbating() || jobDriver.pawn.IsHavingSex());
		}

		public static bool SexActIsInfidelity(JobDriver_Sex jobDriver, Pawn witness = null)
		{
			return BasicSettings.worryAboutInfidelity && PawnIsCheatingOnPartner(jobDriver.pawn, witness);
		}
	}
}
