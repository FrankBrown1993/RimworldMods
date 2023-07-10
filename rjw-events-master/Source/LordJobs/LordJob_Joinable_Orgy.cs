using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RJW_Events
{
    class LordJob_Joinable_Orgy : LordJob_Joinable_Party
    {
        List<Pawn> participants;

        protected override ThoughtDef AttendeeThought
        {
            get
            {
                return ThoughtDefOf.AttendedOrgy;
            }
        }

        protected override TaleDef AttendeeTale
        {
            get
            {
                return TaleDefOf.AttendedOrgy;
            }
        }
        protected override ThoughtDef OrganizerThought
        {
            get
            {
                return ThoughtDefOf.AttendedOrgy;
            }
        }

        protected override TaleDef OrganizerTale
        {
            get
            {
                return TaleDefOf.AttendedOrgy;
            }
        }

        public LordJob_Joinable_Orgy(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef) : base(spot, organizer, gatheringDef)
        {

        }
        public override void ExposeData()
        {
            Scribe_Collections.Look<Pawn>(ref this.participants, "orgyParticipants", LookMode.Reference, Array.Empty<object>());
        }


        public override float VoluntaryJoinPriorityFor(Pawn p)
        {
            if (!CasualSex_Helper.CanHaveSex(p)) return 0;

            if (ModLister.IdeologyInstalled)
            {
                var ideo = p.ideo.Ideo;

                if (!ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("Lovin_FreeApproved", true)) &&
                    !ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("Lovin_Free", true)))
                {
                    return 0;
                }

            }

            return base.VoluntaryJoinPriorityFor(p);
        }

        public new bool IsGatheringAboutToEnd()
        {
            return timeoutTrigger.TicksLeft < 300;
        }

        public override void Notify_PawnAdded(Pawn p)
        {
            if(participants == null)
            {
                participants = new List<Pawn>();
            }
            participants.Add(p);
            base.Notify_PawnAdded(p);
        }

        public override void Cleanup()
        {

            foreach(Pawn participant in participants)
            {
                if (participant != null)
                {
                    participant.mindState.Notify_OutfitChanged();

                    GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(participant);
                    if (xxx.is_human(participant))
                        participant.Drawer.renderer.graphics.ResolveAllGraphics();
                }
            }

            participants = null;
            
            base.Cleanup();
        }

    }
}
