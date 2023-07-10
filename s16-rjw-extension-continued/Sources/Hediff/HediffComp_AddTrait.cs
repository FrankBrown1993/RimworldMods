using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffComp_AddTrait : HediffComp_AddTraitBase
    {
        public HediffCompProperties_AddTrait Props
        {
            get
            {
                return (HediffCompProperties_AddTrait)this.props;
            }
        }

        public override string CompDebugString()
        {
            return this.Props.trait != null ? string.Format("is triggered: {0}", (object)this.isTriggered) : "configuration error: trait is 'null'";
        }

        protected override void UpdatePawnTraits(Pawn pawn)
        {
            TraitSet traits = pawn.story.traits;
            if (!traits.HasTrait(this.Props.trait))
            {
                traits.GainTrait(new Trait(this.Props.trait, this.Props.degreeOffset, false));
            }
            else
            {
                if (this.Props.degreeOffset == 0)
                    return;
                this.UpdateTrait(pawn, this.Props.trait, this.Props.degreeOffset);
            }
        }

        protected override bool ValidateConfig()
        {
            return this.Props.trait?.degreeDatas != null;
        }
    }
}
