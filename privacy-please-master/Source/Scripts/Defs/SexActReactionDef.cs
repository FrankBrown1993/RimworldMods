using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;

namespace Privacy_Please
{
    public class SexActReactionDef : Def
    {
        public string issueDefName;
        public string sexActCheck;
        public SubSexActReactionDef pawnReaction;
        public SubSexActReactionDef witnessReaction;

        private IssueDef _issueDef;
        private bool _checkedForIssueDef;

        public IssueDef issueDef
        {
            get
            {
                if (_checkedForIssueDef == false)
                { _issueDef = DefDatabase<IssueDef>.GetNamedSilentFail(issueDefName); _checkedForIssueDef = true; }

                return _issueDef;
            }
        }

        public class SubSexActReactionDef
        {
            public SexActThoughtDef defaultThoughtDef;
            public List<SexActThoughtDef> preceptThoughtDefs;
            public List<ReplacementThought> replacementThoughts;
        }

        public class ReplacementThought
        {
            public List<TraitRequirement> requiredTraits;
            public string requiredQuirk;
            public PreceptDef requiredPreceptDef;
            public SexActThoughtDef replacementThoughtDef;
        }
   
        public void DetermineReactionOfPawns(Pawn pawn, Pawn witness, out ReactionToSexAct reactionOfPawn, out ReactionToSexAct reactionOfWitness, bool applyThoughtDefs = false)
        {
            reactionOfPawn = DetermineReaction(pawn, witness, pawnReaction, applyThoughtDefs);
            reactionOfWitness = DetermineReaction(witness, pawn, witnessReaction, applyThoughtDefs);
        }

        public ReactionToSexAct DetermineReaction(Pawn reactor, Pawn otherPawn, SubSexActReactionDef reaction, bool applyThoughtDef)
        {
            JobDriver_Sex jobDriver = reactor.jobs.curDriver as JobDriver_Sex;

            // Reactors who do not have thoughts applied to them
            if (reactor.IsUnableToSenseSex()) return ReactionToSexAct.Ignored;
            if (reactor.HostileTo(otherPawn)) return ReactionToSexAct.Ignored;
            if (reactor.RaceProps.Animal || reactor.RaceProps.IsMechanoid) return ReactionToSexAct.Ignored;
            if (otherPawn.RaceProps.Animal || otherPawn.RaceProps.IsMechanoid) return ReactionToSexAct.Uncaring;       
            if (BasicSettings.slavesIgnoreSex && (reactor.IsPrisoner || reactor.IsSlave)) return ReactionToSexAct.Uncaring;
            if (BasicSettings.otherFactionsIgnoreSex && reactor.Faction.IsPlayer == false) return ReactionToSexAct.Uncaring;
            if (BasicSettings.colonistsIgnoreSlaves && (otherPawn.IsPrisoner || otherPawn.IsSlave)) return ReactionToSexAct.Uncaring;
            if (BasicSettings.colonistsIgnoreOtherFactions && otherPawn.Faction.IsPlayer == false) return ReactionToSexAct.Uncaring;
            
            // Apply thoughtDef
            SexActThoughtDef thoughtDef = GetThoughtDefForReactor(reactor, reaction, out Precept precept);
            
            if (thoughtDef == null) return ReactionToSexAct.Uncaring;
            if (applyThoughtDef) reactor.needs.mood.thoughts.memories.TryGainMemory(thoughtDef, otherPawn, precept);
            
            var nullifyingTraits = ThoughtUtility.GetNullifyingTraits(thoughtDef)?.ToList();       
            if (applyThoughtDef && thoughtDef.stages[0].baseMoodEffect < 0 && nullifyingTraits?.Any(x => x.HasTrait(reactor)) != true) reactor.TryGetComp<CompPawnThoughtData>()?.TryToExclaim();

            // Reactors who have their reactions changed after applying thoughtDefs
            if ((otherPawn.jobs.curDriver as JobDriver_Sex)?.Sexprops?.isWhoring == true) return ReactionToSexAct.Ignored;
            if (BasicSettings.whoringIsUninteruptable && jobDriver?.Sexprops?.isWhoring == true) return ReactionToSexAct.Uncaring;
            if (BasicSettings.rapeIsUninteruptable && jobDriver?.Sexprops?.isRape == true) return ReactionToSexAct.Uncaring;          
            
            return thoughtDef.reactionToSexDiscovery;
        }

        private SexActThoughtDef GetThoughtDefForReactor(Pawn reactor, SubSexActReactionDef reaction, out Precept precept)
        {
            precept = null;

            if (reactor == null || reaction == null) return null;

            if (reaction.replacementThoughts.NullOrEmpty() == false)
            {
                foreach (ReplacementThought replacementThought in reaction.replacementThoughts)
                {
                    if (replacementThought?.requiredTraits?.Any(x => x.HasTrait(reactor)) == true)
                    { return replacementThought.replacementThoughtDef; }

                    if (replacementThought.requiredQuirk != null && xxx.has_quirk(reactor, replacementThought.requiredQuirk))
                    { return replacementThought.replacementThoughtDef; }

                    if (replacementThought.requiredPreceptDef != null && reactor.ideo?.Ideo.HasPrecept(replacementThought.requiredPreceptDef) == true)
                    { 
                        precept = reactor.ideo.Ideo.GetPrecept(replacementThought.requiredPreceptDef);
                        return replacementThought.replacementThoughtDef; 
                    }
                }
            }

            precept = reactor.GetPreceptForIssue(issueDef);

            if (precept != null && reaction.preceptThoughtDefs.NullOrEmpty() == false)
            {
                string thoughtDefName = precept.def.defName;

                foreach (SexActThoughtDef associatedThoughtDef in reaction.preceptThoughtDefs)
                {
                    if (associatedThoughtDef.defName.Contains(thoughtDefName))
                    { return associatedThoughtDef; }
                }
            }

            return reaction.defaultThoughtDef;
        }
    }
}
