using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    class SpecialThingFilterWorker_NoMilk : SpecialThingFilterWorker_MilkBase   
    {
        public override bool Matches(Thing t)
        {
            return !(IsHumanMilk(t) || IsFoodWithMilk(t));
        }
    }
}
