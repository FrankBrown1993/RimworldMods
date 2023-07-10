using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace SizedApparel
{
    public static class SizedApparelDubsApparelPatch
    {
        public static void indoorPostFixPatch(Pawn pawn)
        {
            PawnGraphicSet graphicSet = pawn.Drawer?.renderer?.graphics;

            if (graphicSet == null)
                return;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.needToCheckApparelGraphicRecords = true;
        }

    }
}
