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
    public static class HediffDefOf_Milk { 
        static HediffDefOf_Milk()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf_Milk));
        }

        public static HediffDef Lactating_Drug;
        public static HediffDef Lactating_Permanent;
        public static HediffDef Heavy_Lactating_Permanent;
        public static HediffDef Lactating_Natural;


        public static HediffDef InducingLactation;
        public static HediffDef Hucow;
        public static HediffDef InducingLactationCooldown;



    }
}
