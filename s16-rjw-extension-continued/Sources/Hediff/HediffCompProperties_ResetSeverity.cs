using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffCompProperties_ResetSeverity : HediffCompProperties
    {
        public float resetsUponReaching = 1f;
        public float resetsTo = 0.01f;
        public bool resetIfBelow;

        public HediffCompProperties_ResetSeverity()
        {
            this.compClass = typeof(HediffComp_ResetSeverity);
        }
    }
}
