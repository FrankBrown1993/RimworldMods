using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RJW_Events
{
    class JobDriver_GetNaked : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil t = new Toil();
            t.AddFinishAction(() => {
                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
                SexUtility.DrawNude(pawn);
            
            });
            yield return t;
        }
    }
}
