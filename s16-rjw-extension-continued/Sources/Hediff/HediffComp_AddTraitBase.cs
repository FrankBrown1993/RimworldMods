using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public abstract class HediffComp_AddTraitBase : HediffComp
    {
        protected bool isTriggered;

        private HediffCompProperties_AddTraitBase BaseProps
        {
            get
            {
                return (HediffCompProperties_AddTraitBase)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            HediffCompProperties_AddTraitBase baseProps = this.BaseProps;
            if (!this.isTriggered)
            {
                if (this.parent?.pawn?.story?.traits == null || !this.ValidateConfig() || !this.IsInSeverityRange())
                    return;
                this.UpdatePawnTraits(this.parent?.pawn);
                this.isTriggered = true;
            }
            else
            {
                if (!baseProps.canRetrigger || this.IsInSeverityRange())
                    return;
                this.isTriggered = false;
            }
        }

        protected abstract void UpdatePawnTraits(Pawn pawn);

        protected abstract bool ValidateConfig();

        protected void UpdateTrait(Pawn pawn, TraitDef traitDef, int degreeOffset)
        {
            TraitSet traits = pawn.story.traits;
            Trait trait = traits.GetTrait(traitDef);
            int newDegree = trait.Degree + degreeOffset;
            if (traitDef.degreeDatas.Any<TraitDegreeData>((Predicate<TraitDegreeData>)(x => x.degree == newDegree)))
            {
                traits.allTraits.Remove(trait);
                traits.GainTrait(new Trait(traitDef, newDegree, false));
            }
            else
            {
                if (newDegree != 0)
                    return;
                traits.allTraits.Remove(trait);
            }
        }

        public bool IsInSeverityRange()
        {
            HediffCompProperties_AddTraitBase baseProps = this.BaseProps;
            if (!baseProps.ifBelow && (double)this.parent.Severity >= (double)baseProps.onSeverity)
                return true;
            return baseProps.ifBelow && (double)this.parent.Severity <= (double)baseProps.onSeverity;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.isTriggered, "isTriggered", false, false);
        }

        public override bool CompShouldRemove
        {
            get
            {
                return this.BaseProps.removeHediff && this.isTriggered;
            }
        }
    }
}
