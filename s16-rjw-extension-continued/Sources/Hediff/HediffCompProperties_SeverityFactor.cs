using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffCompProperties_SeverityFactor : HediffCompProperties
    {
        public FloatRange factorRange = FloatRange.One;

        public HediffCompProperties_SeverityFactor()
        {
            this.compClass = typeof(HeddifComp_SeverityFactor);
        }
    }
}
