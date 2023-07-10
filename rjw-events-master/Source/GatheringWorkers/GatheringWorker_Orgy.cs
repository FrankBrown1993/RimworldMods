using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace RJW_Events
{
    class GatheringWorker_Orgy : GatheringWorker
    {

        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
        {
            return new LordJob_Joinable_Orgy(spot, organizer, this.def);
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {

            return RCellFinder.TryFindGatheringSpot(organizer, this.def, false, out spot);
        }

        public override bool CanExecute(Map map, Pawn organizer = null)
        {

            if(ModLister.IdeologyInstalled)
            {
                var ideo = Faction.OfPlayer.ideos.PrimaryIdeo;
                if (!ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("Lovin_FreeApproved", true)) &&
                    !ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("Lovin_Free", true)))
                {
                    return false;
                }

            }

            return base.CanExecute(map, organizer);

        }

    }
}
