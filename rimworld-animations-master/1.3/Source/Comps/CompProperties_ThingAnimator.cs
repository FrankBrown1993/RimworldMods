using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rimworld_Animations {
    public class CompProperties_ThingAnimator : CompProperties
    {

        public CompProperties_ThingAnimator()
        {
            base.compClass = typeof(CompThingAnimator);
        }
    }
}
