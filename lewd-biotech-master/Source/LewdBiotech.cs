using HarmonyLib;
using RimWorld;
using rjw;
using LewdBiotech.Helpers;
using System.Collections.Generic;
using Verse;

namespace LewdBiotech
{
    [StaticConstructorOnStartup]
    public class LewdBiotechMod
    {
		public static readonly ThoughtDef regretsStealingLovin = DefDatabase<ThoughtDef>.GetNamed("RegretsStealingLovin");
		public static readonly ThoughtDef stoleSomeLovin = DefDatabase<ThoughtDef>.GetNamed("StoleSomeLovin");
		public static readonly ThoughtDef bloodlustStoleSomeLovin = DefDatabase<ThoughtDef>.GetNamed("BloodlustStoleSomeLovin");
		public static readonly TraitDef rapist = DefDatabase<TraitDef>.GetNamed("Rapist");
		static Dictionary<string, LaborState> laborStateMap = new Dictionary<string, LaborState>();

		static LewdBiotechMod()
        {
            Harmony harmony = new Harmony(id: "rimworld.leboeuf.lewdbiotech");

			// Non-rapist would_rape bypass for limbic stimulator - may be unused
			harmony.Patch(AccessTools.Method(typeof(SexAppraiser), nameof(SexAppraiser.would_rape)), 
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(would_rape_PostFix)));

			// Non-rapist is_rapist bypass for limbic stimulator
			harmony.Patch(AccessTools.Method(typeof(xxx), nameof(xxx.is_rapist)),
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(is_rapist_PostFix)));

			// Non-Rapist trait rape thoughts
			harmony.Patch(AccessTools.Method(typeof(AfterSexUtility), nameof(AfterSexUtility.think_about_sex_Rapist)),
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(think_about_sex_Rapist_PostFix)));

			// Bioscaffold double gestation speed tick
			harmony.Patch(AccessTools.Method(typeof(Hediff_Pregnant), nameof(Hediff_Pregnant.Tick)),
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(Hediff_Pregnant_TickPostFix)));

			// Hediff_Labor state capture
			harmony.Patch(AccessTools.Method(typeof(Hediff_Labor), nameof(Hediff_Labor.PostRemoved)),
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(Hediff_Labor_PostRemovedPostFix)));

			// OvaryAgitator/Gene_LitteredBirths multibirth logic
			harmony.Patch(AccessTools.Method(typeof(Hediff_LaborPushing), nameof(Hediff_LaborPushing.PostRemoved)),
				postfix: new HarmonyMethod(typeof(LewdBiotechMod), nameof(Hediff_LaborPushing_PostRemovedPostFix)));

			// =================================================================================================
			// Future content - Infatuo implant (possibly - don't know if I'm going to finish it; mixed feelings
			/*			harmony.Patch(AccessTools.Method(typeof(InteractionUtility), nameof(InteractionWorker.Interacted)),
							postfix: new HarmonyMethod(typeof(LewdBiotech), nameof(InteractionWorker_InteractedPostfix)));*/
			// =================================================================================================

			LBTLogger.Message("Lewd Biotech started successfully.");
			
			if (LBTSettings.devMode)
            {
				LBTLogger.Message("Notice: Developer logging for Lewd Biotech is currently active - it can be disabled in the mod settings.");
            }
		}

		// Is this even used??? I haven't seen it be hit in debug logs yet ...
		// Keeping it here for now, I suppose.
		static void would_rape_PostFix(ref bool __result, Pawn rapist)
		{
			if (rapist.health.hediffSet.HasHediff(HediffDef.Named("LimbicStimulator")))
			{
				if (LBTSettings.devMode)
                {
					LBTLogger.MessageGroupHead("Found LimbicStimulator hediff during xxx.would_rape check");
					LBTLogger.MessageGroupBody("Pawn: " + rapist.NameShortColored + " (" + rapist.ThingID + ")");
					LBTLogger.MessageGroupBody("__result (Before roll): " + __result);
                }
				__result = Rand.Chance(0.95f);
				if (LBTSettings.devMode)
                {
					LBTLogger.MessageGroupFoot("__result (After roll): " + __result);
                }
			}
		}

		static void is_rapist_PostFix(ref bool __result, Pawn pawn)
        {
			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("LimbicStimulator")))
			{
				if (LBTSettings.devMode)
				{
					LBTLogger.Message("Found LimbicStimulator hediff during xxx.is_rapist check for " + pawn.NameShortColored + " (" + pawn.ThingID + ")" + " with __result = " + __result + " - forcing to true");
					__result = true;
				}
			}
		}

		static void think_about_sex_Rapist_PostFix(ref ThoughtDef __result, Pawn pawn)
        {
			if (LBTSettings.regretStealingLovinThoughtDisabled) return;

			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("LimbicStimulator")) && (__result == stoleSomeLovin || __result == bloodlustStoleSomeLovin) && !pawn.story.traits.HasTrait(rapist))
			{
				__result = regretsStealingLovin;
			}
		}

		static void Hediff_Pregnant_TickPostFix(ref Hediff_Pregnant __instance)
        {
			// 60000 = 1 day ticks
			// 0.055 = severity/gestation per day
			// 0.000000916 (repeating) = target for "2x pregnancy speed"-ish
			// Note for posterity: my original calucation ousted me as COMICALLY bad at math
			// I also need to do a performance analysis on this - having this run every pregnant tick with multiple pawns on a map may be super expensive
			if (__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("Bioscaffold")) && __instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("PregnantHuman")))
			{
				__instance.Severity = __instance.Severity + 0.000000916f;
			}
		}

		// Notes about Hediff_Labor_PostRemovedPostFix
		// ===========================================
		// Alright, didn't really want to make a patch for Labor as well as LaborPushing, but I have to because otherwise there's
		// no way to assign a doctor on second/third/etc births if I only use LaborPushing, which can disproportionately cause
		// more stillbirths than you might normally have
		//
		// I TRIED to modify the ChildBirth lord job and ritual code to allow the first doctor that was assigned and any childbirth
		// attendees to just autoreassign themselves to the next births, but it wasn't working - for now, I'm not happy with how I'm
		// doing this because multiple births now require:
		// 1. (potentially several) forced pauses with letters telling the player to reassign a doctor to the mother who's giving birth to another child
		// 2. A LOT of state carrying (that I likely overengineered :v) ) between hediff removes and adds
		//
		// I've got a dictionary now for storing state across hediffs and on multiple pawns, instead of what I was originally doing, which was using
		// severity as a way of tracking state. LaborState class should help keep things a little more organized.
		//
		// I'll revisit this in the future (probably). Thanks for coming to my TED talk
		static void Hediff_Labor_PostRemovedPostFix(ref Hediff_Labor __instance)
        {
			bool randomTwinsRoll;
			int totalBirths;
			bool laborStateIsNull = !laborStateMap.ContainsKey(__instance.pawn.ThingID);
			bool hasLitteredBirthsGene = __instance.pawn.genes.HasGene(LewdBiotech.GeneDefOf.LitteredBirths);

			// we'll never do additional processing if this is the guaranteed last birth (eg birth #4)
			if (!laborStateIsNull && laborStateMap.TryGetValue(__instance.pawn.ThingID).birthCount == 4) 
			{
				return;
            }
			
			// For now, littered birth overrides ovary agitator and twin calculations, so if a LaborState already exists
			// with littered births gene, move on
			if (!laborStateIsNull && hasLitteredBirthsGene)
            {
				if (LBTSettings.devMode)
				{
					LBTLogger.MessageGroupHead("Found active LaborState and LitteredBirths gene - skipping additional Hediff_Labor_PostRemovedPostFix work");
					LBTLogger.MessageGroupBody("Pawn: " + __instance.pawn.NameShortColored + " (" + __instance.pawn.ThingID + ")");
					LBTLogger.MessageGroupFoot("birthCount: " + laborStateMap.TryGetValue(__instance.pawn.ThingID).birthCount);
				}

				return;
            }
			
			// Make a new LaborState for the null case with littered births
			if (laborStateIsNull && hasLitteredBirthsGene)
            {
				LBTLogger.Message("Found littered births gene");
				int litteredBirthsTotalRoll = Rand.RangeInclusive(2, 4);
				laborStateMap.SetOrAdd(__instance.pawn.ThingID, new LaborState(__instance.pawn, litteredBirthsTotalRoll));
				return;
            }
			
			// Finally, regardless of littered births gene, we only want new state creation on
			// pawns that don't already have state, so return if state is !null (STATE SHOULD ALWAYS BE CLEANED IN LABORPUSHING POSTFIX)
			if (!laborStateIsNull)
            {
				if (LBTSettings.devMode)
                {
					LBTLogger.Warning("Labor state for pawn " + __instance.pawn.NameShortColored + " (" + __instance.pawn.ThingID + ") is not null despite all checks passing for determining first instance of Hediff_Labor - this warning should never occur, and may indicate a bug in Hediff_LaborPushing of lingering labor state from a previous pregnancy");
                }
				return;
            }

			// For everything else, we do random twin and OvaryAgitator handling
			// -------
			// If we fail a base chance twins roll, return without any additional processing and proceed with vanilla childbirth
			// Notes on rolls:
			// -> Chance without OvaryAgitator to have twins: 1%
			// -> Chance with OvaryAgitator to have twins: Guaranteed
			// ---> Chance with OvaryAgitator to have triplets (MUST HAVE SUCCEEDED TWINS ROLL): 50%
			// ---> Chance with OvaryAgitator to have quadruplets (MUST HAVE SUCCEEDED TRIPLETS ROLL): 10%
			// -> Chance with Littered Births gene: random between 2 and 4 (inclusive)
			randomTwinsRoll = Rand.Chance(0.01f);
			bool hasAgitator = __instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("OvaryAgitator"));
			if (!randomTwinsRoll && !hasAgitator)
			{
				// We failed rolls, and we don't have an agitator - no additional processing, do vanilla single baby birth
				if (LBTSettings.devMode)
				{
					LBTLogger.MessageGroupHead("Inside Hediff_Labor_PostRemovedPostFix random twins check fail");
					LBTLogger.MessageGroupBody("Pawn: " + __instance.pawn.NameShortColored);
					LBTLogger.MessageGroupBody("Random twins roll outcome: " + randomTwinsRoll);
					LBTLogger.MessageGroupFoot("Has OvaryAgitator: " + hasAgitator);
				}
				return;
			}
			
			// Beyond this point, we can assume the pawn has an agitator
			totalBirths = 2;
			bool agitatorTriplets = Rand.Chance(0.5f);
			bool agitatorQuadruplets = Rand.Chance(0.1f);
			if (hasAgitator)
            {
				if (agitatorTriplets) totalBirths = 3;
				if (agitatorTriplets && agitatorQuadruplets) totalBirths = 4;
            }

			// Set new LaborState
			laborStateMap.Add(__instance.pawn.ThingID, new LaborState(__instance.pawn, totalBirths));
		}

		static void Hediff_LaborPushing_PostRemovedPostFix(ref Hediff_LaborPushing __instance)
        {
			bool hasAgitator = __instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("OvaryAgitator"));
			bool hasLitteredBirthsGene = __instance.pawn.genes.HasGene(LewdBiotech.GeneDefOf.LitteredBirths);
			bool laborStateIsNull = !laborStateMap.ContainsKey(__instance.pawn.ThingID);
			LaborState currentLaborState;
			laborStateMap.TryGetValue(__instance.pawn.ThingID, out currentLaborState);

			if (laborStateIsNull)
            {
				if (__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("Bioscaffold")))
				{
					__instance.pawn.health.RemoveHediff(__instance.pawn.health.hediffSet.GetFirstHediffOfDef(LewdBiotech.HediffDefOf.Bioscaffold));
				}
				return;
            }

			if (currentLaborState.birthTotal == currentLaborState.birthCount)
			{
				laborStateMap.Remove(__instance.pawn.ThingID);
				if (__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("Bioscaffold"))) {

					__instance.pawn.health.RemoveHediff(__instance.pawn.health.hediffSet.GetFirstHediffOfDef(LewdBiotech.HediffDefOf.Bioscaffold));
                }
				return;
			}

			((Hediff_Labor)__instance.pawn.health.AddHediff(RimWorld.HediffDefOf.PregnancyLabor)).SetParents(__instance.pawn, __instance.Father, PregnancyUtility.GetInheritedGeneSet(__instance.Father, __instance.pawn));
			currentLaborState.birthCount++;

			if (!hasAgitator && !hasLitteredBirthsGene)
			{
				if (LBTSettings.devMode)
                {
					LBTLogger.Message("Pawn " + __instance.pawn.NameShortColored + " (" + __instance.pawn.ThingID + ") is having random twins");
                }
				Find.LetterStack.ReceiveLetter("Twins!", __instance.pawn.NameShortColored + " is still in labor and is having twins!\n\nBe sure to gather your doctor and additional friends and family to ensure the other baby is also born healthy!", LewdBiotech.LetterDefOf.AnotherBaby, __instance.pawn);
				return;
			}

			Find.LetterStack.ReceiveLetter("Another baby!", __instance.pawn.NameShortColored + " is still in labor and is having another baby!\n\nBe sure to gather your doctor and additional friends and family to ensure the next baby is also born healthy!", LewdBiotech.LetterDefOf.AnotherBaby, __instance.pawn);
        }
	}
}