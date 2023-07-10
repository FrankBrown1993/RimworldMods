using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using Milk;
using UnityEngine;


namespace CRIALactation
{
    public class CompProperties_AbilityBeginInducingLactation : CompProperties_AbilityEffect
    {

        public CompProperties_AbilityBeginInducingLactation()
        {
            this.compClass = typeof(CompAbilityEffect_BeginInducingLactation);
        }

    }
}
