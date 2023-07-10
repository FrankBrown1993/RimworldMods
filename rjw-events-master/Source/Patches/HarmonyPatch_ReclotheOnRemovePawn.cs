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
    [HarmonyPatch(typeof(Lord), "RemovePawn")]
    public static class HarmonyPatch_ReclotheOnRemovePawn
    {
        public static void Postfix(Lord __instance, Pawn p)
        {
            if(__instance?.LordJob != null && __instance.LordJob is LordJob_Joinable_Orgy)
            {
                p.Drawer.renderer.graphics.ResolveApparelGraphics();
            }
        }
    }
}
