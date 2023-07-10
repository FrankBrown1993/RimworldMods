using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    [DefOf]
    public static class ThingDefOf_Milk
    {
        static ThingDefOf_Milk()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf_Milk));
        }

        public static ThingDef HumanMilk;

        public static ThingDef HumanoidMilk;

        public static ThingDef HumanMilkBulk;

        public static ThingDef HumanoidMilkBulk;

    }
}
