using System;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LewdBiotech
{
    class LaborState
    {
        public Pawn pawn;
        public int birthTotal = 0;
        public int birthCount = 1;
        public bool hasOvaryAgitator = false;
        public bool hasBioscaffold = false;

        public LaborState(Pawn pawn, int birthTotal)
        {
            this.pawn = pawn;
            this.birthTotal = birthTotal;
            this.birthCount = 0;
            this.hasOvaryAgitator = pawn.health.hediffSet.HasHediff(HediffDef.Named("OvaryAgitator"));
            this.hasBioscaffold = pawn.health.hediffSet.HasHediff(HediffDef.Named("OvaryAgitator"));
        }
    }
}
