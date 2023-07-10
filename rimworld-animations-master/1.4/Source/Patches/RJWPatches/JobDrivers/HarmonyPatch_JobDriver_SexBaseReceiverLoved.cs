using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using rjw;
using System.Reflection.Emit;
using Verse.AI;

namespace Rimworld_Animations
{

    [HarmonyPatch(typeof(JobDriver_SexBaseRecieverLoved), "MakeSexToil")]
    public static class HarmonyPatch_JobDriver_SexBaseReceiverLoved
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {

            var ins = codeInstructions.ToList();
            for(int i = 0; i < ins.Count; i++)
            {
                if(i < ins.Count && ins[i].opcode == OpCodes.Call && ins[i].OperandIs(AccessTools.DeclaredMethod(typeof(Toils_LayDown), "LayDown"))) {

                    ins[i].operand = AccessTools.DeclaredMethod(typeof(HarmonyPatch_JobDriver_SexBaseReceiverLoved), "DoNotLayDown");
                    yield return ins[i];

                }

                else
                {
                    yield return ins[i];
                }
            }

        }

        public static Toil DoNotLayDown(TargetIndex bedOrRestSpotIndex, bool hasBed, bool lookForOtherJobs, bool canSleep = true, bool gainRestAndHealth = true, PawnPosture noBedLayingPosture = PawnPosture.LayingMask, bool deathrest = false)
        {
            return new Toil();
        }

    }
}
