using Milk;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    public static class LactationUtility
    {

        public static bool IsLactating(Pawn p)
        {

            return
                p.health.hediffSet.HasHediff(HediffDefOf_Milk.Lactating_Natural, false) ||
                p.health.hediffSet.HasHediff(HediffDefOf_Milk.Lactating_Drug, false) ||
                p.health.hediffSet.HasHediff(HediffDefOf_Milk.Lactating_Permanent, false) ||
                p.health.hediffSet.HasHediff(HediffDefOf_Milk.Heavy_Lactating_Permanent, false);
                
        }

        public static bool HasMilkableBreasts(Pawn p)
        {
            if (p.TryGetComp<CompMilkableHuman>() == null) return false;

            if (Genital_Helper.has_breasts(p) && (MilkSettings.flatChestGivesMilk || !Genital_Helper.has_male_breasts(p)))
            {
                return true;
            }

            return false;
        }

        public static void StartLactating(Pawn p, bool natural)
        {
            Hediff lactating = HediffMaker.MakeHediff(HediffDefOf_Milk.Lactating_Natural, p, null);
            lactating.Severity = 1;// Rand.Value;
            p.health.AddHediff(lactating, null, null, null);
            //just adding to base body, not to breasts. Just in case the target has multiple breasts so to not confuse
            //p.health.AddHediff(lactating, Genital_Helper.get_breastsBPR(p));
        }

        public static void StopBeingHucow(Pawn p)
        {
            p.health.RemoveHediff(p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Milk.Hucow, false));
        }

        public static bool IsHucow(Pawn p)
        {
            return p.health.hediffSet.HasHediff(HediffDefOf_Milk.Hucow);
        }

        public static bool isMassageable(Pawn p)
        {
            Hediff lactationInductionHediff = p.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf_Milk.InducingLactation);
            Hediff lactationInductionCooldownHediff = p.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf_Milk.InducingLactationCooldown);
            if (lactationInductionHediff == null) return false;
            if (lactationInductionCooldownHediff != null) return false;

            return true;

        }

        public static void ExtendLactationDuration(Pawn p)
        {

            var drugLact = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Milk.Lactating_Drug);
            if(drugLact != null)
            {
                drugLact.Severity = 1;
                //drugLact.TryGetComp<HediffComp_Disappears>().ticksToDisappear = 1800000;
            }

            var naturalLact = p.health.hediffSet.GetFirstHediffOfDef(HediffDefOf_Milk.Lactating_Natural);
            if (naturalLact != null)
            {
                naturalLact.Severity = 1;
                //naturalLact.TryGetComp<HediffComp_Disappears>().ticksToDisappear = 1800000;
            }

            if (ModsConfig.BiotechActive)
            {
                var hediffLactBT = p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Lactating"));
                if (hediffLactBT != null)
                {
                    hediffLactBT.Severity = 1;
                }
            }


        }
    }
}
