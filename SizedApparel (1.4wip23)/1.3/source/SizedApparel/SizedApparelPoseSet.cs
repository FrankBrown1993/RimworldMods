using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;
using UnityEngine;

namespace SizedApparel
{
    /*
    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt")]
    [HarmonyAfter(new string[]
    {
        "rimworld.erdelf.alien_race.main"
    })]
    */
    //TODO
    public static class PawnRenderer_BaseHeadOffsetAt_Patch_For_Pose
    {
        [HarmonyPostfix]
        public static void BaseHeadOffsetAtPostfix_Post(PawnRenderer __instance, Rot4 rotation, ref Vector3 __result, ref Pawn ___pawn)
        {
            return;

            /*
            var comp = ___pawn.GetComp<ApparelRecorderComp>();

            if (comp == null)
                return;
            if (comp.customPose == null)
                return;
            
            SizedApparelPoseDef poseDef = DefDatabase<SizedApparelPoseDef>.GetNamed(comp.customPose);
            if (poseDef == null)
                return;

            Rot4Offsets? headOffset = new Rot4Offsets();

            string bodyType = ___pawn.story?.bodyType?.defName;
            if (bodyType == null)
            {
                bodyType = "default";
                headOffset = poseDef.headOffset.FirstOrDefault(b => b.bodyType.ToLower() == "default" | b.bodyType.ToLower() == "null").offsets;
            }
            else
            {
                headOffset = poseDef.headOffset.FirstOrDefault(b => b.bodyType.ToLower() == bodyType.ToLower()).offsets;
            }

            if (headOffset == null)
                return;
            */


            //headOffset.

            //TODO: Mesh Based Scaled Offset
            //__result = __result + headOffset;



            return;
        }
    }
    public class SizedApparelPoseSetDef : Def
    {

        public List<SizedApparelPose> poses;

    }

    public class PoseDef : Def
    {

    }


    public class SizedApparelPose
    {
        //public string poseName; use defName as PoseName
        public SizedApparelBodyPartOf targetBodyPart = SizedApparelBodyPartOf.Torso;
        //public List<BodyTypeAndOffset> headOffset;

    }

}
