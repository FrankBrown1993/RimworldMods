using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using Milk;
using UnityEngine;
using HarmonyLib;
using System.Reflection.Emit;

namespace CRIALactation

{
    /*
    [HarmonyPatch(typeof(HumanCompHasGatherableBodyResource), "CompTick")]
    public static class HarmonyPatch_Milk_HumanCompHasGatherableBodyResource
    {
        
        public static void Prefix(HumanCompHasGatherableBodyResource __instance)
        {
            if (!__instance.parent.IsHashIntervalTick(100))
            {
                return;
            }
            if (!(__instance.parent is Pawn)) return;

            Pawn p = __instance.parent as Pawn;
            if (p?.health?.hediffSet == null) return;

            if (p.Ideo?.GetRole(p) != null && p.Ideo.GetRole(p).def == PreceptDefOf_Lactation.IdeoRole_Hucow)
            {

                if(p.health.hediffSet.HasHediff(HediffDef.Named("Lactating_Natural"), false))
                {
                    p.health.RemoveHediff(p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Lactating_Natural"), false));
                }
                if (p.health.hediffSet.HasHediff(HediffDef.Named("Lactating_Drug"), false))
                {
                    p.health.RemoveHediff(p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Lactating_Drug"), false));
                }
                if(!(p.health.hediffSet.HasHediff(HediffDef.Named("Lactating_Permanent"), false) ||
                    p.health.hediffSet.HasHediff(HediffDef.Named("Heavy_Lactating_Permanent"), false)))
                {
                    p.health.AddHediff(HediffDef.Named("Lactating_Permanent"));
                }
            }
        }

        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {

            var ins = codeInstructions.ToList();

            for(int i = 0; i < ins.Count; i++)
            {

                if (ins[i].opcode == OpCodes.Callvirt && ins.Count > i + 1 && ins[i + 1].OperandIs(60000))
                {

                    yield return ins[i];
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(HarmonyPatch_Milk_HumanCompHasGatherableBodyResource), "AdjustGatherResourceDaysForPrecept"));


                }

                else
                {
                    
                    yield return ins[i];
                }

            }

        }

        
        public static float AdjustGatherResourceDaysForPrecept(float resourcesIntervalDays, HumanCompHasGatherableBodyResource __instance)
        {

            Pawn pawn = __instance.parent as Pawn;
            if(pawn.Ideo.HasPrecept(PreceptDefOf_Lactation.Lactating_Essential))
            {
                

                return resourcesIntervalDays * 0.666f * pawn.GetStatValue(StatDefOf_Lactation.MilkProductionSpeed); //1.5x normal rate
            }

            return resourcesIntervalDays;

        }

    }
    */

    /*
    [HarmonyPatch(typeof(CompHyperMilkableHuman), "ResourceAmount", MethodType.Getter)]
    public static class HarmonyPatch_IncreaseYieldForHucowHyperMilkable
    {
        public static void Postfix(CompHyperMilkableHuman __instance, ref float __result)
        {
            Pawn p = __instance.parent as Pawn;
            __result *= p.GetStatValue(StatDefOf_Lactation.MilkProductionYield);
        }
    }

    [HarmonyPatch(typeof(CompMilkableHuman), "ResourceAmount", MethodType.Getter)]
    public static class HarmonyPatch_IncreaseYieldForHucowMilkable
    {
        public static void Postfix(CompHyperMilkableHuman __instance, ref float __result)
        {
            Pawn p = __instance.parent as Pawn;
            __result *= p.GetStatValue(StatDefOf_Lactation.MilkProductionYield);
        }
    }*/
}
