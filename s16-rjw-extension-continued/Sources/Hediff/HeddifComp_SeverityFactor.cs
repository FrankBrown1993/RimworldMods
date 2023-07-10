using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HeddifComp_SeverityFactor : HediffComp
    {
        private float factor = 1f;

        public HediffCompProperties_SeverityFactor Props
        {
            get
            {
                return (HediffCompProperties_SeverityFactor)this.props;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.factor = this.Props.factorRange.RandomInRange;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.factor, "factor", 1f, false);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            severityAdjustment *= this.factor;
        }

        public override string CompDebugString()
        {
            return string.Format("severity factor: {0}", (object)this.factor);
        }
    }
}
