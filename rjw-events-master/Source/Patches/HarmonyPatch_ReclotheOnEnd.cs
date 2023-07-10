using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RJW_Events
{
    [HarmonyPatch(typeof(LordJob_Joinable_Party), "ApplyOutcome")]
    public static class HarmonyPatch_ReclotheOnEnd
    {
        public static void Postfix(LordToil_Party toil)
        {
            List<Pawn> ownedPawns = toil.lord.ownedPawns;
            for (int i = 0; i < ownedPawns.Count; i++)
            {
                ownedPawns[i].Drawer.renderer.graphics.ResolveApparelGraphics();
            }
        }
    }
}
