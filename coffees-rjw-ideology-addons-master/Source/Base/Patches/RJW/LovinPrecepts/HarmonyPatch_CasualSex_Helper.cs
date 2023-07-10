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
    [HarmonyPatch(typeof(CasualSex_Helper), "FindBestPartner")]
    public static class HarmonyPatch_CasualSex_Helper
    {

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {

            var ins = codeInstructions.ToList();
            for(int i = 0; i < ins.Count; i++)
            {

                if(ins[i].OperandIs(AccessTools.DeclaredMethod(typeof(CasualSex_Helper), "HookupAllowedViaSettings")))
                {

                    yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(HarmonyPatch_CasualSex_Helper), "HookupAllowed"));

                }
                else
                {
                    yield return ins[i];
                }
            }

        }

        public static bool HookupAllowed(Pawn pawn1, Pawn pawn2)
        {
            if(CRIAUtility.TwoPawnsWillingToHaveSex(pawn1, pawn2))
            {
                return CasualSex_Helper.HookupAllowedViaSettings(pawn1, pawn2);
            }
            return false;
        }

    }
}
