using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIALactation
{
    [DefOf]
    public static class HistoryEventDefOf_Milk
    {
        static HistoryEventDefOf_Milk()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HistoryEventDefOf_Milk));
        }

        public static HistoryEventDef DrankMilkRaw;

        public static HistoryEventDef DrankMilkMeal;

        public static HistoryEventDef DrankNonMilkMeal;
    }
}
