using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RJW_Events
{
    public class IncidentWorker_PsychicArouse : IncidentWorker_PsychicEmanation
    {
        protected override void DoConditionAndLetter(IncidentParms parms, Map map, int duration, Gender gender, float points)
        {
			PsychicDroneLevel level;
			if (points < 800f)
			{
				level = PsychicDroneLevel.BadLow;
			}
			else if (points < 2000f)
			{
				level = PsychicDroneLevel.BadMedium;
			}
			else
			{
				level = PsychicDroneLevel.BadHigh;
			}

			GameCondition_PsychicArouse gameCondition_PsychicEmanation = (GameCondition_PsychicArouse)GameConditionMaker.MakeCondition(GameConditionDefOf.PsychicArouse, duration);

			gameCondition_PsychicEmanation.gender = gender;
			gameCondition_PsychicEmanation.level = level;

			map.gameConditionManager.RegisterCondition(gameCondition_PsychicEmanation);
			base.SendStandardLetter(gameCondition_PsychicEmanation.LabelCap, gameCondition_PsychicEmanation.LetterText, gameCondition_PsychicEmanation.def.letterDef, parms, LookTargets.Invalid, Array.Empty<NamedArgument>());
		}
    }
}
