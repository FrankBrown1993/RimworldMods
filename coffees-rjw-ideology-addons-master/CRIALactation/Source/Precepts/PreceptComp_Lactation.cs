using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using Milk;
using UnityEngine;

namespace CRIALactation
{
    public class PreceptComp_Lactation : PreceptComp
    {

        public override void Notify_MemberGenerated(Pawn pawn, Precept precept, bool newborn)
        {

            if (newborn) return;

            if((precept.def == PreceptDefOf_Lactation.Lactating_Essential
                || precept.def == PreceptDefOf_Lactation.Lactating_MandatoryHucow)
                && LactationUtility.HasMilkableBreasts(pawn))
            {

                if (!LactationUtility.IsLactating(pawn))
                {
                    LactationUtility.StartLactating(pawn, pawn.relations.ChildrenCount > 0);
                    Log.Message("Creating pawn with lact" + pawn.Name);
                }
            }
        }

        
    }
}
