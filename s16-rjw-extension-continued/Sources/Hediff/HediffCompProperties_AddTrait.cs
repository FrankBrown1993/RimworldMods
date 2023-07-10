using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s16_extension
{
    public class HediffCompProperties_AddTrait : HediffCompProperties_AddTraitBase
    {
        public TraitDef trait;
        public int degreeOffset;

        public HediffCompProperties_AddTrait()
        {
            this.compClass = typeof(HediffComp_AddTrait);
        }
    }
}
