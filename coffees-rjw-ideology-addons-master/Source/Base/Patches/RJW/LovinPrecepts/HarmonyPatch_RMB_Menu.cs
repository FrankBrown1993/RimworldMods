using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using System.Reflection.Emit;

namespace C0ffee_s_RJW_Ideology_Addons
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatch_RMB_Menu
    {
        static HarmonyPatch_RMB_Menu()
        {
            (new Harmony("C0ffeeRIA")).Patch(AccessTools.Method(AccessTools.TypeByName("rjw.RMB_Menu"), "GenerateRMBOptions"),
                            postfix: new HarmonyMethod(AccessTools.Method(typeof(HarmonyPatch_RMB_Menu), "Postfix")));


        }

        public static void Postfix(ref List<FloatMenuOption> __result, Pawn pawn, LocalTargetInfo target)
        {

            if (target.Pawn != null)
            {

                if (!CRIAUtility.TwoPawnsWillingToHaveSex(pawn, target.Pawn))
                {
                    for(int i = 0; i < __result.Count; i++)
                    {

                        if(__result[i].Label.StartsWith("RJW_RMB_Sex".Translate()))
                        {
                            __result[i].Label += " (sinful)";
                        }

                    }

                    return;
                }
            }

            return;
            
        }
    }
}
