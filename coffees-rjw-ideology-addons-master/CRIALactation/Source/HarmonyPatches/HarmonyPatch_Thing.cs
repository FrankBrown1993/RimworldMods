using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace CRIALactation
{
    [HarmonyPatch(typeof(Thing), "Ingested")]
    public static class HarmonyPatch_Thing
    {
        public static void Prefix(Thing __instance, Pawn ingester)
        {

            if(__instance?.def == ThingDefOf_Milk.HumanMilk || __instance?.def == ThingDefOf_Milk.HumanoidMilk || __instance?.def == ThingDefOf_Milk.HumanMilkBulk || __instance?.def == ThingDefOf_Milk.HumanoidMilkBulk)
            {
                if (ingester.TryGetComp<CompLactation>() == null) return;
                ingester.TryGetComp<CompLactation>().lastHumanLactationIngestedTick = Find.TickManager.TicksGame;
            }

        }
    }
}
