using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using AlienRace;
using UnityEngine;
using rjw;
using HarmonyLib;

namespace Privacy_Please
{
    public class CompPawnThoughtData : ThingComp
    {
        private Pawn pawn;
        private int lastExclaimationTick = -1;
        private int exclaimationCoolDown = 90;
        private int lastInvitationTick = -1;
        private int invitationCoolDown = 600;

        public override void Initialize(CompProperties props)
        {
            if (pawn == null)
            { pawn = parent as Pawn; }
        }

        // Delay the cleaning of filth under the sexing pawns
        public override void CompTickRare()
        {
            base.CompTickRare();

            if (pawn?.jobs?.curDriver != null && pawn.jobs.curDriver is JobDriver_Sex)
            {
                IEnumerable<Thing> filthPile = pawn.PositionHeld.GetThingList(pawn.Map);

                if (filthPile == null) return;

                foreach(Thing thing in filthPile)
                {
                    if ((thing is Filth) == false) continue;

                    AccessTools.Field(typeof(Filth), "growTick").SetValue(thing as Filth, Find.TickManager.TicksGame); 
                }
            }
        }

        public void TryToExclaim()
        {
            if (Find.TickManager.TicksGame > exclaimationCoolDown + lastExclaimationTick)
            {
                lastExclaimationTick = Find.TickManager.TicksGame;
                FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.IncapIcon);
            }
        }

        public bool CanSendAnInvitionForSex()
        {
            if (Find.TickManager.TicksGame > invitationCoolDown + lastInvitationTick)
            {
                lastInvitationTick = Find.TickManager.TicksGame;
                return true;
            }

            return false;
        }
    }
}
