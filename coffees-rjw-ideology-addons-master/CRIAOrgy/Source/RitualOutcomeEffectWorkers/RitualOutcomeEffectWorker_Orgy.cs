using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using UnityEngine;

namespace CRIAOrgy
{
    public class RitualOutcomeEffectWorker_Orgy : RitualOutcomeEffectWorker_FromQuality
    {

        public RitualOutcomeEffectWorker_Orgy()
        {
             
        }

        public RitualOutcomeEffectWorker_Orgy(RitualOutcomeEffectDef def) : base(def)
        {
        }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
			foreach (KeyValuePair<Pawn, int> keyValuePair in totalPresence)
			{
                Pawn participant = keyValuePair.Key;

                participant.mindState.Notify_OutfitChanged();

                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(participant);
                if (xxx.is_human(participant))
                    participant.Drawer.renderer.graphics.ResolveAllGraphics();
            }

            base.Apply(progress, totalPresence, jobRitual);
		}
    }
}
