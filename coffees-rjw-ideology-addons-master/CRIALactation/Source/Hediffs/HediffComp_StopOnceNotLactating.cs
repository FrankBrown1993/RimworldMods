using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    public class HediffComp_StopOnceNotLactating : HediffComp
    {

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if(!LactationUtility.IsLactating(Pawn))
            {
                LactationUtility.StopBeingHucow(Pawn);

            }
        }

    }
}
