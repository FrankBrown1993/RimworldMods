using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


namespace CRIALactation
{
    public class HediffCompProperties_StopOnceNotLactating : HediffCompProperties
    {
        public HediffCompProperties_StopOnceNotLactating()
        {
            this.compClass = typeof(HediffComp_StopOnceNotLactating);
        }
    }
}
