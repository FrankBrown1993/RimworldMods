using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s16_extension
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    internal static class Patch_Pawn_ApparelTracker_Notify_ApparelAdded
    {
        [HarmonyPriority(800)]
        public static void Postfix(Apparel apparel)
        {
            if (apparel.Wearer == null)
                return;
            apparel.BroadcastCompSignal("HediffApparel_AddHediffsToPawnSignal");
        }
    }
}
