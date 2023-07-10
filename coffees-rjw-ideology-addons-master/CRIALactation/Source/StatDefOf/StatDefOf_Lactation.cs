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
    public static class StatDefOf_Lactation
    {
        static StatDefOf_Lactation()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOf_Lactation));
        }

        public static StatDef MilkProductionSpeed;
        public static StatDef MilkProductionYield;

    }
}
