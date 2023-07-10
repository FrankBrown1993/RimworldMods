using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Rimworld_Animations
{
    [HarmonyPatch(typeof(Thing), "DrawAt")]
    public static class HarmonyPatch_Thing
    {

        public static bool Prefix(Thing __instance)
        {
            CompThingAnimator thingAnimator = __instance.TryGetComp<CompThingAnimator>();
            if (thingAnimator != null && thingAnimator.isAnimating)
            {
                thingAnimator.AnimateThing(__instance);
                return false;
                
            }

            return true;

        }

    }
}
