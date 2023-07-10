using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using rjw;
using Rimworld_Animations;
using HarmonyLib;

namespace Privacy_Please
{
	public static class PawnExtension
	{
		public static bool IsInBed(this Pawn pawn, out Building bed)
		{
			bed = pawn.Position.GetThingList(pawn.Map).FirstOrDefault(x => x is Building_Bed) as Building;
			return bed != null;
		}

		public static bool IsHavingSex(this Pawn pawn)
		{
			return GetAllSexParticipants(pawn).Count > 1;
		}

		public static bool IsMasturbating(this Pawn pawn)
		{
			return GetAllSexParticipants(pawn).Count == 1;
		}

		public static Pawn GetSexInitiator(this Pawn pawn)
		{
			if (pawn?.jobs?.curDriver != null && pawn.jobs.curDriver is JobDriver_SexBaseInitiator)
			{ return pawn; }

			JobDriver_SexBaseReciever jobDriver = pawn.jobs.curDriver as JobDriver_SexBaseReciever;

			if (jobDriver?.Partner?.jobs?.curDriver != null && jobDriver.Partner.jobs.curDriver is JobDriver_SexBaseInitiator)
			{ return jobDriver.Partner; }

			return null;
		}

		public static Pawn GetSexReceiver(this Pawn pawn)
		{
			if (pawn.jobs.curDriver is JobDriver_SexBaseReciever)
			{ return pawn; }

			JobDriver_SexBaseInitiator jobDriver = pawn.jobs.curDriver as JobDriver_SexBaseInitiator;

			if (jobDriver?.Partner?.jobs?.curDriver != null && jobDriver.Partner.jobs.curDriver is JobDriver_SexBaseReciever)
			{ return jobDriver.Partner; }

			return null;
		}

		public static Pawn GetSexPartner(this Pawn pawn)
		{
			return (pawn.jobs.curDriver as JobDriver_Sex)?.Partner;
		}

		public static List<Pawn> GetAllSexParticipants(this Pawn pawn)
		{
			List<Pawn> participants = new List<Pawn>();

			if (pawn?.jobs?.curDriver == null || (pawn.jobs.curDriver is JobDriver_Sex) == false)
			{ return participants; }

			if (pawn.GetSexReceiver() != null)
			{
				List<Pawn> partners = (pawn.GetSexReceiver().jobs.curDriver as JobDriver_SexBaseReciever).parteners.ToList();

				if (partners != null)
				{
					foreach (Pawn partner in partners)
					{
						if (partner != null)
						{ participants = partners; }
					}
				}
			}

			if (pawn.GetSexInitiator() != null)
			{
				Pawn partner = (pawn.GetSexInitiator().jobs.curDriver as JobDriver_SexBaseInitiator).Partner;

				if (partner != null && partner.Dead == false)
				{ participants.AddDistinct(partner); }
			}

			participants.AddDistinct(pawn);

			return participants;
		}

		public static bool IsLoverOfOther(this Pawn pawn, Pawn other)
		{
			if (pawn?.relations?.DirectRelations == null || other?.relations?.DirectRelations == null)
			{ return false; }

			List<DirectPawnRelation> lovers = SpouseRelationUtility.GetLoveRelations(pawn, false);
			return lovers.Any(x => x.otherPawn == other);
		}

		public static Precept GetPreceptForIssue(this Pawn pawn, IssueDef issueDef)
		{
			if (issueDef == null || pawn?.Ideo == null)
			{ return null; }

			foreach (Precept precept in pawn.Ideo.PreceptsListForReading)
			{
				if (precept.def.issue == issueDef)
				{ return precept; }
			}

			return null;
		}

		public static bool HasTrait(this Pawn pawn, string trait)
		{
			if (pawn?.story?.traits?.allTraits == null || pawn.story.traits.allTraits.NullOrEmpty())
			{ return false; }

			TraitDef traitDef = DefDatabase<TraitDef>.GetNamedSilentFail(trait);

			if (traitDef == null)
			{ traitDef = DefDatabase<TraitDef>.GetNamedSilentFail(trait.ToLower()); }

			return HasTrait(pawn, traitDef);
		}

		public static bool HasTrait(this Pawn pawn, TraitDef traitDef)
		{
			if (pawn?.story?.traits?.allTraits == null || pawn.story.traits.allTraits.NullOrEmpty())
			{ return false; }

			if (traitDef == null)
			{ return false; }

			return pawn.story.traits.HasTrait(traitDef);
		}

		public static bool IsUnableToSenseSex(this Pawn pawn)
		{
			if (pawn.Dead ||
				pawn.Awake() == false ||
				pawn.Suspended)
			{ return true; }

			return false;
		}

		public static bool IsPartOfRitualOrGathering(this Pawn pawn)
		{
			return pawn.GetLord() != null && (pawn.GetLord().LordJob is LordJob_Ritual || pawn.GetLord().LordJob is LordJob_Joinable_Party);
		}

		public static bool IsVictim(this Pawn pawn)
		{
			return pawn.CurJob.def == xxx.gettin_raped || pawn.Dead;
		}

		public static bool IsExhibitionist(this Pawn pawn)
		{
			return xxx.has_quirk(pawn, "Exhibitionist") || pawn?.ideo?.Ideo.HasPrecept(ModPreceptDefOf.Exhibitionism_Approved) == true;
		}

		public static bool IsVoyeur(this Pawn pawn)
		{
			return xxx.has_quirk(pawn, "Voyeur");
		}

		public static bool IsCuckold(this Pawn pawn)
		{
			return xxx.has_quirk(pawn, "Cuckold");
		}
	}
}
