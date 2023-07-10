using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJW_Events
{
    public static class GameConditionManagerExtension
    {

        public static PsychicDroneLevel GetHighestPsychicArouseLevelFor(this GameConditionManager g, Gender gender)
        {
			PsychicDroneLevel psychicDroneLevel = PsychicDroneLevel.None;
			for (int i = 0; i < g.ActiveConditions.Count; i++)
			{
				GameCondition_PsychicArouse gameCondition_PsychicEmanation = g.ActiveConditions[i] as GameCondition_PsychicArouse;
				if (gameCondition_PsychicEmanation != null && gameCondition_PsychicEmanation.gender == gender && gameCondition_PsychicEmanation.level > psychicDroneLevel)
				{
					psychicDroneLevel = gameCondition_PsychicEmanation.level;
				}
			}
			return psychicDroneLevel;
		}

    }
}
