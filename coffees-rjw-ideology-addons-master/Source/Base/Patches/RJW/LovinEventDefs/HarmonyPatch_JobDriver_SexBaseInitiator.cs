using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;


namespace C0ffee_s_RJW_Ideology_Addons
{
    [HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "End")]
    public static class HarmonyPatch_JobDriver_SexBaseInitiator_End
    {

        public static void Prefix(JobDriver_SexBaseInitiator __instance)
        {
            if (__instance is JobDriver_Masturbate || __instance.Partner == null
                || __instance.pawn?.relations == null || __instance.pawn.RaceProps.IsMechanoid) return;

            HistoryEventDef def = __instance.pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, __instance.Partner) ? HistoryEventDefOf.GotLovin_Spouse : HistoryEventDefOf.GotLovin_NonSpouse;

            if (!(IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_NonSpouse, __instance.Partner) && (__instance.Partner.IsSlaveOfColony || __instance.Partner.IsPrisonerOfColony))) //ensure raped pawns don't enjoy
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.GotLovin, __instance.Partner.Named(HistoryEventArgsNames.Doer)), true);
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(def, __instance.Partner.Named(HistoryEventArgsNames.Doer)), true);
            }

            if (IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_NonSpouse, __instance.pawn) || !(__instance.Partner.IsSlaveOfColony || __instance.Partner.IsPrisonerOfColony)) //ensure slaves are free game, but not otherwise
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.GotLovin, __instance.pawn.Named(HistoryEventArgsNames.Doer)), true);
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(def, __instance.pawn.Named(HistoryEventArgsNames.Doer)), true);
            }
            
            
        }
    }


    [HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "Start")]
    public static class HarmonyPatch_JobDriver_SexBaseInitiator_Start
    {

        public static void Prefix(JobDriver_SexBaseInitiator __instance)
        {

            if (__instance is JobDriver_Masturbate || __instance.Partner == null) return;


            if (IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_NonSpouse, __instance.Partner) || !(__instance is JobDriver_Rape)) //ensure raped pawns don't enjoy
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.InitiatedLovin, __instance.pawn.Named(HistoryEventArgsNames.Doer)), true);
            }

            

        }

    }

}
