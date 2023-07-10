using HarmonyLib;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RJW_Events
{

    [HarmonyPatch(typeof(Need_Sex), "fall_per_tick")]
    public static class HarmonyPatch_Need_Sex
    {

        public static void Postfix(Pawn pawn, ref float __result)
        {
			PsychicDroneLevel psychicDroneLevel = PsychicDroneLevel.None;
			Map mapHeld = pawn.MapHeld;
			if (mapHeld != null)
			{
				PsychicDroneLevel highestPsychicDroneLevelFor = mapHeld.gameConditionManager.GetHighestPsychicArouseLevelFor(pawn.gender);
				if (highestPsychicDroneLevelFor > psychicDroneLevel)
				{
					psychicDroneLevel = highestPsychicDroneLevelFor;
				}
			}

			switch (psychicDroneLevel)
			{
				case PsychicDroneLevel.None:
					break;
				case PsychicDroneLevel.GoodMedium:
					__result *= 3;
					break;
				case PsychicDroneLevel.BadLow:
					__result *= 3;
					break;
				case PsychicDroneLevel.BadMedium:
					__result *= 6;
					break;
				case PsychicDroneLevel.BadHigh:
					__result *= 9;
					break;
				case PsychicDroneLevel.BadExtreme:
					__result *= 9;
					break;
				default:
					throw new NotImplementedException();
			}
		}

    }
}
