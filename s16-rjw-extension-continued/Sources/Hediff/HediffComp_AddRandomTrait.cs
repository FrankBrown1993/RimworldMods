using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffComp_AddRandomTrait : HediffComp_AddTraitBase
    {
        public HediffCompProperties_AddRandomTrait Props
        {
            get
            {
                return (HediffCompProperties_AddRandomTrait)this.props;
            }
        }

        protected override void UpdatePawnTraits(Pawn pawn)
        {
            TraitSet traits = pawn.story.traits;
            TraitInfo[] array1 = this.GetMatchingTraits(traits, (IEnumerable<TraitInfo>)this.Props.traits).ToArray<TraitInfo>();
            if (array1.Length == 0)
            {
                TraitInfo traitInfo = this.Props.traits.RandomElementByWeight<TraitInfo>((Func<TraitInfo, float>)(x => x.weight));
                traits.GainTrait(new Trait(traitInfo.trait, traitInfo.degreeOffset, false));
            }
            else
            {
                TraitInfo[] array2 = ((IEnumerable<TraitInfo>)array1).Where<TraitInfo>((Func<TraitInfo, bool>)(x => (uint)x.degreeOffset > 0U)).ToArray<TraitInfo>();
                if (array2.Length == 0)
                    return;
                TraitInfo traitInfo = ((IEnumerable<TraitInfo>)array2).RandomElementByWeight<TraitInfo>((Func<TraitInfo, float>)(x => x.weight));
                this.UpdateTrait(pawn, traitInfo.trait, traitInfo.degreeOffset);
            }
        }

        private IEnumerable<TraitInfo> GetMatchingTraits(
          TraitSet pawnTraits,
          IEnumerable<TraitInfo> compTraits)
        {
            Dictionary<TraitDef, TraitInfo> compTraitsMap = compTraits.ToDictionary<TraitInfo, TraitDef>((Func<TraitInfo, TraitDef>)(x => x.trait));
            foreach (Trait allTrait in pawnTraits.allTraits)
            {
                if (compTraitsMap.ContainsKey(allTrait.def))
                    yield return compTraitsMap[allTrait.def];
            }
        }

        protected override bool ValidateConfig()
        {
            List<TraitInfo> traits = this.Props.traits;
            return traits != null && traits.All<TraitInfo>((Func<TraitInfo, bool>)(x => x.trait != null));
        }

        public override string CompDebugString()
        {
            List<TraitInfo> traits = this.Props.traits;
            return (traits != null ? traits.Count<TraitInfo>() : 0) != 0 ? string.Format("is triggered: {0}", (object)this.isTriggered) : "configuration error: no traits defined";
        }
    }
}
