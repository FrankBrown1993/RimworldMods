using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s16_extension
{
    public class HediffCompProperties_AddRandomTrait : HediffCompProperties_AddTraitBase
    {
        public List<TraitInfo> traits;

        public HediffCompProperties_AddRandomTrait()
        {
            this.compClass = typeof(HediffComp_AddRandomTrait);
        }
    }
}
