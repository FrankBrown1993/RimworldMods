using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using rjw;

namespace Privacy_Please
{
	[HarmonyPatch(typeof(JobDriver_Sex), "setup_ticks")]
	public static class HarmonyPatch_JobDriver_Sex_setup_ticks
	{
		public static void Postfix(ref JobDriver_Sex __instance)
		{
			Pawn pawn = __instance.pawn;

			// Invite another for a threesome?
			if (RJWHookupSettings.QuickHookupsEnabled &&
				__instance is JobDriver_SexBaseInitiator &&
				pawn.GetAllSexParticipants().Count == 2 &&
				(__instance is JobDriver_JoinInSex) == false &&
				Random.value < BasicSettings.chanceForOtherToJoinInSex &&
				pawn.TryGetComp<CompPawnThoughtData>()?.CanSendAnInvitionForSex() == true)
			{
				DebugMode.Message("Find another to join in sex?");

				List<Pawn> candidates = new List<Pawn>();
				float radius = 4f;

				foreach (Thing thing in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, radius, true))
				{
					Pawn other = thing as Pawn;
					if (other == null || pawn == other) continue;

					SexInteractionUtility.GetReactionsToSexAct(pawn.jobs.curDriver as JobDriver_Sex, other, out ReactionToSexAct reactionOfPawn, out ReactionToSexAct reactionOfOther, false);

					// Find candidates to invite
					if ((int)reactionOfOther >= (int)ReactionToSexAct.Acceptance && SexInteractionUtility.PasserbyCanBePropositionedForSex(other, pawn.GetAllSexParticipants()))
					{
						DebugMode.Message(other.NameShortColored + " is a potential candidate");
						candidates.Add(other);
					}
				}

				// Invite a random candidate (weighted by attraction)
				if (candidates.Count > 0)
				{
					Pawn invitedPawn = candidates.RandomElementByWeight(x => SexAppraiser.would_fuck(pawn, x, false, false, true) + SexAppraiser.would_fuck(pawn.GetSexPartner(), x, false, false, true));
					pawn.GetSexInitiator().IsInBed(out Building bed);

					DebugMode.Message(invitedPawn.NameShortColored + " was invited to join in sex");
					pawn.interactions.TryInteractWith(invitedPawn, ModInteractionDefOf.InviteToHaveGroupSex);

					Job job = new Job(DefDatabase<JobDef>.GetNamed("JoinInSex", false), pawn.GetSexPartner(), bed);
					invitedPawn.jobs.TryTakeOrderedJob(job);
				}
			}
		}
	}

	[HarmonyPatch(typeof(JobDriver_Sex), "SexTick")]
	public static class HarmonyPatch_JobDriver_Sex_SexTick
	{
		// When pawns are having sex, intermittently check their surrounds for privacy
		public static void Postfix(ref JobDriver_Sex __instance)
		{
			Pawn pawn = __instance.pawn;
			Pawn partner = pawn.GetSexPartner();

			if (pawn.IsHashIntervalTick(90))
			{
				if (pawn.IsMasturbating() || pawn.IsHavingSex())
				{ 
					PrivacyUtility.PrivacyCheckForPawn(pawn, 8f);

					if (partner != null)
					{ PrivacyUtility.PrivacyCheckForPawn(partner, 8f); }
				}
			}
		}
	}
}
