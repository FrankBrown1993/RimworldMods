using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;


namespace C0ffee_s_RJW_Ideology_Addons
{
    public static class CRIAUtility
    {

        public static bool TwoPawnsWillingToHaveSex(Pawn fucker, Pawn fucked)
        {
            
            if (fucker == fucked) return true; //pawns are always allowed to masturbate

            if (DesignatorsData.rjwComfort.Contains(fucked)) return true; //designated comfort pawns allowed to have sex with

            if (fucked.IsSlaveOfColony || fucked.IsPrisonerOfColony) return true; //prisoners and slaves are free game

            if (PawnsAreDesperate(fucker, fucked)) return true;

            if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed, fucker) || !IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed, fucked))
            {
                return false;
            }

            if (fucker.relations.GetDirectRelation(PawnRelationDefOf.Spouse, fucked) != null)
            {

                if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_Spouse, fucker) || !IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_Spouse, fucked))
                {
                    return false;
                }

            }
            else if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_NonSpouse, fucker) || !IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed_NonSpouse, fucked))
            {
                return false;
            }

            return true;
        }

        public static bool PawnsAreDesperate(Pawn fucker, Pawn fucked)
        {
            if ((IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed, fucker) || xxx.is_frustrated(fucker) && (IdeoUtility.DoerWillingToDo(HistoryEventDefOf.SharedBed, fucked) || xxx.is_frustrated(fucked)))) return true; //frustrated pawns have to have sex

            return false;
        }

    }
}
