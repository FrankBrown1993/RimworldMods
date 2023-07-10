using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    class HediffComp_ItemSpawner : HediffComp
    {
        private int SpawningTicker = 0;

        public HediffCompProperties_ItemSpawner Props
        {
            get
            {
                return (HediffCompProperties_ItemSpawner)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            this.SpawnItem();
        }

        public void SpawnItem()
        {
            if ((double)this.SpawningTicker < (double)this.Props.DaysForItemSpawning * 60000.0)
            {
                ++this.SpawningTicker;
            }
            else
            {
                if (this.parent.pawn.Map != null && !this.parent.pawn.Downed && (this.parent.pawn.Faction == Faction.OfPlayer || this.parent.pawn.IsPrisoner && this.parent.pawn.Map.IsPlayerHome))
                {
                    if (this.parent.pawn.health.hediffSet.hediffs.Find((Predicate<Hediff>)(x => x.def == this.Props.PreventedByHediff)) == null)
                    {
                        Thing thing = ThingMaker.MakeThing(this.Props.thingToSpawn, (ThingDef)null);
                        thing.stackCount = this.Props.spawnCount;
                        GenPlace.TryPlaceThing(thing, this.parent.pawn.Position, this.parent.pawn.Map, ThingPlaceMode.Near, out Thing _, (Action<Thing, int>)null, (Predicate<IntVec3>)null, new Rot4());
                    }
                    else
                        this.SpawningTicker = 0;
                }
                this.SpawningTicker = 0;
            }
        }
    }
}
