using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
//using Rimworld_Animations;

namespace SizedApparel
{

    public class RimworldAnimationPatch
    {
        //since instance (CompBodyAnimator class) must be soft dependency, Get as System.Object.
        public static void TickClipPostfix(System.Object __instance) //CompBodyAnimator __instance,  AnimationDef ___anim, int ___curStage, int ___actor, int ___clipTicks, float ___clipPercent
        {
            Rimworld_Animations.CompBodyAnimator instance = __instance as Rimworld_Animations.CompBodyAnimator;

            if (instance == null)
                return;

            if (!instance.controlGenitalAngle)
                return;

            var comp = instance.parent.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;


            comp.SetPenisAngle(instance.genitalAngle - instance.bodyAngle); //genitalAngle is global Angle value in rjwanimation... fix with body Angle;


            if (!SizedApparelSettings.AnimationPatch)//Rotating Penis Setting(avobe) is set from RimworldAnimation Setting, not in SizedApparel.
                return;

            foreach (var actor in instance.actorsInCurrentAnimation)
            {
                //comp.ForceUpdateTickAnimation = true;
                var actorcomp = actor.GetComp<ApparelRecorderComp>();
                if (actorcomp == null)
                    continue;
                //actorcomp.SetBreastJiggle(true);
                actorcomp.ForceUpdateTickAnimation = true;

            }
            return;
            bool isFuckTick = false;
            /*
            var soundEffects = ((PawnAnimationClip)___anim.animationStages[___curStage].animationClips[___actor]).SoundEffects;
            if (soundEffects.ContainsKey(___clipTicks) && (soundEffects[___clipTicks].Contains("Fuck") || soundEffects[___clipTicks].Contains("Suck")))
            {
                isFuckTick = true;
            }
            else
            {
                const int jiggleTime = 3;
                for(int i = 0; i < jiggleTime; i++)
                {
                    if (soundEffects.ContainsKey(___clipTicks - i) && soundEffects[___clipTicks - i].Contains("Fuck"))
                    {
                        isFuckTick = true;
                        break;
                    }
                }
            }*/
            //need to find partner's breasts.
            /*
            if (isFuckTick)
            {
                foreach (var actor in __instance.actorsInCurrentAnimation)
                {
                    actor.GetComp<ApparelRecorderComp>().SetBreastJiggle(true);
                }
            }
            else
            {
                foreach (var actor in __instance.actorsInCurrentAnimation)
                {
                    actor.GetComp<ApparelRecorderComp>().SetBreastJiggle(false);
                }
            }*/
            //may have some tick issue? too fast jiggle?
        }

        //rjw's JobDriver_SexBaseInitiator end patch
        public static void EndClipPostfix(System.Object __instance)//CompBodyAnimator __instance
        {
            Rimworld_Animations.CompBodyAnimator instance = __instance as Rimworld_Animations.CompBodyAnimator;
            if (instance == null)
                return;

            if (!instance.controlGenitalAngle)
                return;
            var comp = instance.parent.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.SetBreastJiggle(false, -1);
            comp.ForceUpdateTickAnimation = false;
            comp.SetPenisAngle(0);
            
        }
    }

}
