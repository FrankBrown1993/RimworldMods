using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace RJW_Events
{
    
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class HarmonyPatch_StayNudeForOrgy
    {
        public static bool Prefix(PawnGraphicSet __instance)
        {
            if(LordUtility.GetLord(__instance.pawn)?.LordJob != null && 
                LordUtility.GetLord(__instance.pawn).LordJob is LordJob_Joinable_Orgy &&
                !(LordUtility.GetLord(__instance.pawn).LordJob as LordJob_Joinable_Orgy).IsGatheringAboutToEnd())
            {
                return false;
            }
            return true;
        }
    }
}
