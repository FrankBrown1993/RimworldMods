using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RJW_Events
{
    class ThoughtWorker_PsychicArouse : ThoughtWorker
    {

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
			PsychicDroneLevel psychicDroneLevel = PsychicDroneLevel.None;
			Map mapHeld = p.MapHeld;
			if (mapHeld != null)
			{
				PsychicDroneLevel highestPsychicDroneLevelFor = mapHeld.gameConditionManager.GetHighestPsychicArouseLevelFor(p.gender);
				if (highestPsychicDroneLevelFor > psychicDroneLevel)
				{
					psychicDroneLevel = highestPsychicDroneLevelFor;
				}
			}

			switch (psychicDroneLevel)
			{
				case PsychicDroneLevel.None:
					return false;
				case PsychicDroneLevel.GoodMedium:
					return ThoughtState.ActiveAtStage(0);
				case PsychicDroneLevel.BadLow:
					return ThoughtState.ActiveAtStage(0);
				case PsychicDroneLevel.BadMedium:
					return ThoughtState.ActiveAtStage(1);
				case PsychicDroneLevel.BadHigh:
					return ThoughtState.ActiveAtStage(2);
				case PsychicDroneLevel.BadExtreme:
					return ThoughtState.ActiveAtStage(2);
				default:
					throw new NotImplementedException();
			}
        }
    }
}
