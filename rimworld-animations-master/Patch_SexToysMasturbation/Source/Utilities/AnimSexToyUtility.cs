using rjw;
using RJW_ToysAndMasturbation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Patch_SexToysMasturbation
{
    public class AnimSexToyUtility
    {

        public static SexToyAnimationDef tryFindAnimation(CompSexToy sexToy, Pawn pawn)
        {
            
            IEnumerable<SexToyAnimationDef> options = DefDatabase<SexToyAnimationDef>.AllDefs.Where((SexToyAnimationDef x) =>
            {

                if(!sexToy.Props.requiredBodyParts.Contains(x.requiredBodyPart))
                {
                    return false;
                }

                if(x.requiredBodyPart == "vagina" && !Genital_Helper.has_vagina(pawn))
                {
                    return false;
                }

                return true;

            });

            if(options != null && options.Any())
            {
                return options.RandomElement();
            } 
            else
            {
                return null;
            }
            
        }

    }
}
