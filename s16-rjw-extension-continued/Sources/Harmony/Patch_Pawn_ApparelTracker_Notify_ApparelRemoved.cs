using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s16_extension
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    internal static class Patch_Pawn_ApparelTracker_Notify_ApparelRemoved
    {
        [HarmonyPriority(800)]
        public static void Postfix(Apparel apparel)
        {
            if (apparel.Wearer != null)
                return;
            apparel.BroadcastCompSignal("HediffApparel_RemoveHediffsFromPawnSignal");
        }
    }
}
