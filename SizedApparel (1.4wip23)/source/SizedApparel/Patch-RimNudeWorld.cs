using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
//using AlienRace;
using UnityEngine;
using rjw;




namespace SizedApparel
{

    //[HarmonyPatch(typeof(RimNudeWorld.GenitalPatch), "Postfix")]
    [StaticConstructorOnStartup]
    public class SizedApparelRNWPatch
    {
        static bool Prefix(Pawn pawn)
        {
            if (pawn == null)
                return false;

            return true;
        }

    }


    public class RevealingApparelPatch
    {
        static void Postfix( Pawn pawn, ref bool __result)
        {
            if (__result == false)
                return;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            if (comp.hasUnsupportedApparel)
                return;
            if(pawn.apparel.WornApparel != null)
            {
                if(pawn.apparel.WornApparel.Any((Apparel ap) =>( ap.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_IgnorBreastSize".ToLower()))))
                __result = false;
            }
            return;
        }
    }


}
