using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    public class HediffCompProperties_AddTraitBase : HediffCompProperties
    {
        public float onSeverity = 1f;
        public bool ifBelow;
        public bool canRetrigger;
        public bool removeHediff;
    }
}
