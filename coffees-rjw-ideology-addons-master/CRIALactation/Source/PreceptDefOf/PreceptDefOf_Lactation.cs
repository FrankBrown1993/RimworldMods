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
    public static class PreceptDefOf_Lactation
    {
        static PreceptDefOf_Lactation()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PreceptDefOf_Lactation));
        }

        public static PreceptDef Lactating_Essential;
        public static PreceptDef Lactating_MandatoryHucow;
        //public static PreceptDef IdeoRole_Hucow;

    }
}