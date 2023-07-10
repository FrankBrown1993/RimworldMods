using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using rjw;

namespace Privacy_Please
{
    public static class PrivacyUtility
    {
		public static void PrivacyCheckForPawn(Pawn pawn, float radius)
		{
			if (pawn.IsHavingSex() == false && pawn.IsMasturbating() == false) return;
			if (pawn.IsUnableToSenseSex() || pawn.RaceProps.Animal || pawn.RaceProps.IsMechanoid) return;
			if (BasicSettings.ignoreRitualAndPartySex && pawn.IsPartOfRitualOrGathering()) return;

			// Local variables
			JobDriver_Sex jobDriver = pawn.jobs.curDriver as JobDriver_Sex;
			if (jobDriver == null) return;

			pawn.IsInBed(out Building bed);

			foreach (Thing thing in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, radius, true))
			{
				Pawn witness = thing as Pawn;
				if (witness == null) continue;

				if (SexInteractionUtility.PawnCaughtLovinByWitness(pawn, witness))
				{
					// Get the pawn's and witness' reaction to the discovery
					SexInteractionUtility.GetReactionsToSexAct(jobDriver, witness, out ReactionToSexAct reactionOfPawn, out ReactionToSexAct reactionOfWitness, true);

					bool tryToPropositionTheWitness = Random.value < BasicSettings.chanceForOtherToJoinInSex && 						
						SexInteractionUtility.PasserbyCanBePropositionedForSex(witness, pawn.GetAllSexParticipants()) &&
						pawn.TryGetComp<CompPawnThoughtData>()?.CanSendAnInvitionForSex() == true;

					// Try to proposition the witness
					if ((int)reactionOfPawn >= (int)ReactionToSexAct.Acceptance && (int)reactionOfWitness >= (int)ReactionToSexAct.Acceptance && tryToPropositionTheWitness)
					{
						// Voyeurism
						if (witness.IsVoyeur() || (witness.IsCuckold() && SexInteractionUtility.SexParticipantsIncludesACheatingPartner(witness, pawn.GetAllSexParticipants())))
						{
							pawn.interactions.TryInteractWith(witness, ModInteractionDefOf.InviteVoyeurism);

							Job job = new Job(ModJobDefOf.WatchSex, pawn);
							witness.jobs.TryTakeOrderedJob(job);							
						}

						// Consensual sex
						else if (pawn.IsMasturbating())
						{
							pawn.interactions.TryInteractWith(witness, ModInteractionDefOf.InviteToHaveSex);

							if (bed == null)
							{
								Job job = new Job(xxx.quick_sex, pawn);
								witness.jobs.TryTakeOrderedJob(job);
							}

							else
							{
								Job job = new Job(xxx.casual_sex, pawn, bed);
								witness.jobs.TryTakeOrderedJob(job);
							}
						}

						// Group sex
						else
						{
							pawn.interactions.TryInteractWith(witness, ModInteractionDefOf.InviteToHaveGroupSex);

							Job job = new Job(DefDatabase<JobDef>.GetNamed("JoinInSex", false), pawn.GetSexReceiver(), bed);
							witness.jobs.TryTakeOrderedJob(job);
						}
					}

					// The proposition failed. Is this awkward for those having sex?
					else if ((int)reactionOfPawn < (int)ReactionToSexAct.Uncaring)
					{
						Find.PlayLog.Add(new PlayLogEntry_Interaction(ModInteractionDefOf.InterruptedSex, witness, pawn, new List<RulePackDef>()));

						// The pawn is uncomfortable and is stopping sex
						foreach (Pawn participant in pawn.GetAllSexParticipants())
						{
							JobDriver_Sex participantJobDriver = participant.jobs.curDriver as JobDriver_Sex;	
		
							if (participantJobDriver?.ticks_left > 60)
							{ participantJobDriver.ticks_left = 60; }
						}
					}			
				}
			}
		}
	}
}
