using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffComp_ResetSeverity : HediffComp
    {
        public HediffCompProperties_ResetSeverity Props
        {
            get
            {
                return (HediffCompProperties_ResetSeverity)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            HediffCompProperties_ResetSeverity props = this.Props;
            if (props.resetIfBelow)
            {
                if ((double)this.parent.Severity > (double)props.resetsUponReaching)
                    return;
                Log.Message("Changing severity");
                this.parent.Severity = props.resetsTo;
            }
            else
            {
                if ((double)this.parent.Severity < (double)props.resetsUponReaching)
                    return;
                Log.Message("Changing severity");
                this.parent.Severity = props.resetsTo;
            }
        }
    }
}
