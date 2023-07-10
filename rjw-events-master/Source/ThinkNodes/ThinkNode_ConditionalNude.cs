using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RJW_Events
{

    public class ThinkNode_ConditionalNude : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            //if pawn is rendering apparel they shouldn't be,
            if (pawn.Drawer.renderer.graphics.apparelGraphics.Any((x) => {

                if (
                x.sourceApparel.def is bondage_gear_def ||
                x.sourceApparel.def.defName.ToLower().ContainsAny(new string[]
                {
                    "vibrator",
                    "piercing",
                    "strapon"
                }) || 
                (RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Headgear && !x.sourceApparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)))
                    return false;
                return true;
            
            }))
            {
                //they aren't nude
                return false;
            }

            return true;
        }
    }
}
