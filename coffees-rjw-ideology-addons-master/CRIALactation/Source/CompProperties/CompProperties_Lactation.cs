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
    public class CompProperties_Lactation : CompProperties
    {
        public CompProperties_Lactation()
        {
            this.compClass = typeof(CompLactation);
        }

        //public float DaysToLactating = 15;
        //public float TimesMassagedADay = 2.5f;
    }
}
