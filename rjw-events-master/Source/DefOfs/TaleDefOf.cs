using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJW_Events
{
    [DefOf]
    public static class TaleDefOf
    {
        static TaleDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(TaleDefOf));
        }

        public static TaleDef AttendedOrgy;
    }
}
