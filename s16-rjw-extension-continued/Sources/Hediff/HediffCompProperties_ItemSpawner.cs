using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    class HediffCompProperties_ItemSpawner : HediffCompProperties
    {
        public float DaysForItemSpawning = 1f;
        public int spawnCount = 1;
        public ThingDef thingToSpawn;
        public HediffDef PreventedByHediff;

        public HediffCompProperties_ItemSpawner()
        {
            this.compClass = typeof(HediffComp_ItemSpawner);
        }
    }
}
