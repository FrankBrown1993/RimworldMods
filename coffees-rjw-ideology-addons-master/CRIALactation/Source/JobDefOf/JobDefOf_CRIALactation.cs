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
    public static class JobDefOf_CRIALactation
    {
        static JobDefOf_CRIALactation()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf_Milk));
        }

        public static JobDef MassageBreasts;
    }
}
