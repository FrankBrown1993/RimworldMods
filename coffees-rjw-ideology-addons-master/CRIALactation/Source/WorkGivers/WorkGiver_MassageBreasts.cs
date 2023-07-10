using Milk;
using RimWorld;
using RimWorld.Planet;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace CRIALactation
{
    public class WorkGiver_MassageBreasts : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
             return pawn.Map.mapPawns.AllPawnsSpawned;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Pawn> list = pawn.Map.mapPawns.AllPawnsSpawned;
            for(int i = 0; i < list.Count; i++)
            {
                if(LactationUtility.isMassageable(list[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool HasJobOnThing(Pawn p, Thing t, bool forced = false)
        {
            Pawn pawn2 = t as Pawn;
            if(pawn2?.health?.hediffSet.GetFirstHediffOfDef(HediffDefOf_Milk.InducingLactation) == null)
            {
                return false;
            }
            Hediff lactationInductionHediff = pawn2?.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf_Milk.InducingLactation);
            if (lactationInductionHediff == null) return false;

            //we have the cooldown
            Hediff lactationInductionCooldownHediff = pawn2?.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf_Milk.InducingLactationCooldown);
            if (lactationInductionCooldownHediff != null) return false;

            //HediffComp_LactationInduction lactInductComp = lactationInductionHediff.TryGetComp<HediffComp_LactationInduction>();
            return p != pawn2 && !pawn2.Downed && !pawn2.Drafted && !pawn2.InAggroMentalState && !pawn2.IsFormingCaravan() && pawn2.CanCasuallyInteractNow(false, true, false) && p.CanReserve(pawn2, 1, -1, null, forced);

        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(JobDefOf_CRIALactation.MassageBreasts, t);
        }


    }
}
