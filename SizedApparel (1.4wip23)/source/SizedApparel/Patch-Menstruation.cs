using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using RJW_Menstruation;

namespace SizedApparel
{
    public class Patch_Menstruation
    {

        public static Color? GetNippleColor(Hediff breastHediff)
        {
            var breastcomp = breastHediff.TryGetComp<HediffComp_Breast>();
            if (breastcomp == null)
                return null;
            return breastcomp.NippleColor;        
        }

        public static float? GetNippleSize(Hediff breastHediff)
        {
            var breastcomp = breastHediff.TryGetComp<HediffComp_Breast>();
            if (breastcomp == null)
                return null;
            return breastcomp.NippleSize;
        }

        public static float? GetAreolaSize(Hediff breastHediff)
        {
            var breastcomp = breastHediff.TryGetComp<HediffComp_Breast>();
            if (breastcomp == null)
                return null;
            return breastcomp.AreolaSize;
        }
    }
}
