using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;

namespace RJW_Events
{
    [DefOf]
    public static class OrgyDutyDefOf
    {
        static OrgyDutyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
        }

        public static DutyDef Orgy;

    }
}
