using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class CompProperties_HediffApparel : CompProperties
    {
        public HediffDef hediffDef;
        public List<BodyPartDef> partsToAffect;
        public List<BodyPartGroupDef> groupsToAffect;
        public List<string> filterTerms;
        public FilterMode filterMode;
        public SeverityMode severityMode;
        public bool global;

        public CompProperties_HediffApparel()
        {
            this.compClass = typeof(CompHediffApparel);
        }
    }
}
