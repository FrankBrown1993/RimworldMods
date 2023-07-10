using HarmonyLib;
using Rimworld_Animations;
using rjw;
using RJW_ToysAndMasturbation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Patch_SexToysMasturbation
{
    [HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "Start")]
    public class HarmonyPatch_JobDriver_SexBaseInitiator
    {

        public static void Postfix(ref JobDriver_SexBaseInitiator __instance)
        {

            if(__instance is JobDriver_MasturbateWithToy masturbateJobDriver)
            {

                Log.Message("Rerolling animations...");
                Pawn pawn = masturbateJobDriver.pawn;
                Thing sexToy = masturbateJobDriver.dildo;

                RerollAnimationsForSexToy(pawn, sexToy, masturbateJobDriver.Bed);
            }

            
        }

        public static void RerollAnimationsForSexToy(Pawn pawn, Thing thing, Thing bed)
        {
            CompSexToy sextoy = thing.TryGetComp<CompSexToy>();

            SexToyAnimationDef anim = AnimSexToyUtility.tryFindAnimation(sextoy, pawn);
            
            if (anim != null)
            {
                Log.Message("Playing anim " + anim.defName);

                if(bed != null)
                {
                    pawn.TryGetComp<CompBodyAnimator>().setAnchor(bed);
                    thing.TryGetComp<CompThingAnimator>().setAnchor(bed);
                }
                else
                {
                    pawn.TryGetComp<CompBodyAnimator>().setAnchor(pawn.Position);
                    thing.TryGetComp<CompThingAnimator>().setAnchor(pawn.Position);
                }

                bool mirror = GenTicks.TicksGame % 2 == 0;

                pawn.TryGetComp<CompBodyAnimator>().StartAnimation(anim, new List<Pawn> { pawn }, 0, mirror);
                thing.TryGetComp<CompThingAnimator>().StartAnimation(anim, pawn, mirror);

                (pawn.jobs.curDriver as JobDriver_Sex).ticks_left = anim.animationTimeTicks;
                (pawn.jobs.curDriver as JobDriver_Sex).sex_ticks = anim.animationTimeTicks;
                (pawn.jobs.curDriver as JobDriver_Sex).duration = anim.animationTimeTicks;
            }
            else
            {
                Log.Message("No animation found");
            }
            

        }

    }

    
}
