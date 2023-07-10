using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rimworld_Animations {
    public class CompThingAnimator : ThingComp
    {
        Vector3 anchor;

        Pawn pawn;

        public bool isAnimating = false, mirror;

        int animTicks = 0, stageTicks = 0, clipTicks = 0, curStage = 0;
        float rotation = 0;
        float clipPercent = 0;

        public Vector3 deltaPos;

        AnimationDef anim;
        private ThingAnimationClip clip => (ThingAnimationClip)stage.animationClips[1];
        private AnimationStage stage
        {
            get
            {
                return anim.animationStages[curStage];
            }

        }

        public void StartAnimation(AnimationDef anim, Pawn pawn, bool mirror = false)
        {
            isAnimating = true;

            this.anim = anim;
            this.pawn = pawn;
            this.mirror = mirror;

            animTicks = 0;
            stageTicks = 0;
            clipTicks = 0;

            curStage = 0;
            clipPercent = 0;

            tickAnim();

        }

        public void setAnchor(IntVec3 position)
        {
            anchor = position.ToVector3();
        }

        public override void CompTick()
        {
            base.CompTick();

            if(isAnimating)
            {
                if (pawn.Dead || pawn?.jobs?.curDriver == null || (pawn?.jobs?.curDriver != null && !(pawn?.jobs?.curDriver is rjw.JobDriver_Sex)))
                {
                    isAnimating = false;
                }
                else
                {
                    tickAnim();
                }
            }

            
        }

        public void tickAnim()
        {
            if (!isAnimating) return;
            animTicks++;

            if (animTicks < anim.animationTimeTicks)
            {
                tickStage();
            }
            else
            {
                if (LoopNeverending())
                {
                    ResetOnLoop();
                }
                else
                {
                    isAnimating = false;
                }


            }

        }

        public void tickStage()
        {
            if (stage == null)
            {
                isAnimating = false;
                return;
            }

            stageTicks++;

            if (stageTicks >= stage.playTimeTicks)
            {

                curStage++;

                stageTicks = 0;
                clipTicks = 0;
                clipPercent = 0;
            }

            if (curStage >= anim.animationStages.Count)
            {
                if (LoopNeverending())
                {
                    ResetOnLoop();
                }
                else
                {
                    isAnimating = false;
                }

            }
            else
            {
                tickClip();
            }
        }

        public void tickClip()
        {
            clipTicks++;

            if (clipPercent >= 1 && stage.isLooping)
            {
                clipTicks = 1;//warning: don't set to zero or else calculations go wrong
            }
            clipPercent = (float)clipTicks / (float)clip.duration;

            calculateDrawValues();
        }

        public void setAnchor(Thing thing)
        {

            //center on bed
            if (thing is Building_Bed)
            {
                anchor = thing.Position.ToVector3();
                if (((Building_Bed)thing).SleepingSlotsCount == 2)
                {
                    if (thing.Rotation.AsInt == 0)
                    {
                        anchor.x += 1;
                        anchor.z += 1;
                    }
                    else if (thing.Rotation.AsInt == 1)
                    {
                        anchor.x += 1;
                    }
                    else if (thing.Rotation.AsInt == 3)
                    {
                        anchor.z += 1;
                    }

                }
                else
                {
                    if (thing.Rotation.AsInt == 0)
                    {
                        anchor.x += 0.5f;
                        anchor.z += 1f;
                    }
                    else if (thing.Rotation.AsInt == 1)
                    {
                        anchor.x += 1f;
                        anchor.z += 0.5f;
                    }
                    else if (thing.Rotation.AsInt == 2)
                    {
                        anchor.x += 0.5f;
                    }
                    else
                    {
                        anchor.z += 0.5f;
                    }
                }
            }
            else
            {
                anchor = thing.Position.ToVector3Shifted();
            }

            anchor -= new Vector3(0.5f, 0, 0.5f);
        }
        private void calculateDrawValues()
        {
            
            //shift up and right 0.5f to align center
            deltaPos = new Vector3((clip.PositionX.Evaluate(clipPercent)) * (mirror ? -1 : 1) + 0.5f, AltitudeLayer.Item.AltitudeFor(), clip.PositionZ.Evaluate(clipPercent) + 0.5f);
            //Log.Message("Clip percent: " + clipPercent + " deltaPos: " + deltaPos);
            rotation = clip.Rotation.Evaluate(clipPercent) * (mirror ? -1 : 1);
        }

        public void AnimateThing(Thing thing) 
        {
            thing.Graphic.Draw(deltaPos + anchor, mirror ? Rot4.West : Rot4.East, thing, rotation);
        }

        public bool LoopNeverending()
        {
            if (pawn?.jobs?.curDriver != null &&
                (pawn.jobs.curDriver is JobDriver_Sex) && (pawn.jobs.curDriver as JobDriver_Sex).neverendingsex)
            {
                return true;
            }

            return false;
        }

        public void ResetOnLoop()
        {
            curStage = 1;
            animTicks = 0;
            stageTicks = 0;
            clipTicks = 0;

            tickAnim();
        }

    }
}
