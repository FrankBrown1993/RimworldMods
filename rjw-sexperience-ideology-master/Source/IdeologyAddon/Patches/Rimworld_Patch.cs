using HarmonyLib;
using Mono.Cecil.Cil;
using RimWorld;
using rjw;
using RJWSexperience.Ideology.HistoryEvents;
using RJWSexperience.Ideology.Precepts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace RJWSexperience.Ideology.Patches
{
	[HarmonyPatch(typeof(MarriageCeremonyUtility), nameof(MarriageCeremonyUtility.Married))]
	public static class Rimworld_Patch_Marriage
	{
		public static void Postfix(Pawn firstPawn, Pawn secondPawn)
		{
			RsiDefOf.HistoryEvent.RSI_NonIncestuosMarriage.RecordEventWithPartner(firstPawn, secondPawn);
			RsiDefOf.HistoryEvent.RSI_NonIncestuosMarriage.RecordEventWithPartner(secondPawn, firstPawn);
		}
	}

	[HarmonyPatch(typeof(RitualOutcomeEffectWorker_FromQuality), "GiveMemoryToPawn")]
	public static class Rimworld_Patch_RitualOutcome_DontGiveMemoryToAnimals
	{
		public static bool Prefix(Pawn pawn)
		{
			return !pawn.IsAnimal();
		}
	}

	[HarmonyPatch(typeof(IdeoFoundation), nameof(IdeoFoundation.CanAdd))]
	public static class Rimworld_Patch_IdeoFoundation
	{
		public static void Postfix(PreceptDef precept, ref IdeoFoundation __instance, ref AcceptanceReport __result)
		{
			DefExtension_MultipleMemesRequired extension = precept.GetModExtension<DefExtension_MultipleMemesRequired>();

			if (extension == null)
				return;

			if (extension.requiredAllMemes.NullOrEmpty())
				return;

			for (int i = 0; i < extension.requiredAllMemes.Count; i++)
			{
				if (!__instance.ideo.memes.Contains(extension.requiredAllMemes[i]))
				{
					List<string> report = new List<string>();
					foreach (MemeDef meme in extension.requiredAllMemes) report.Add(meme.LabelCap);

					__result = new AcceptanceReport("RequiresMeme".Translate() + ": " + report.ToCommaList());
					return;
				}
			}
		}
	}

	[HarmonyPatch(typeof(RelationsUtility), "Incestuous")]
	public static class Rimworld_Patch_IncestuousManualRomance
	{
		public static bool Prepare() => RsiMod.Prefs.patchIncestuousManualRomance;

		/// <summary>
		/// Override incestuous check in the manual romance
		/// </summary>
		/// <param name="one">Pawn to try do romance</param>
		/// <param name="two">Target for romance</param>
		/// <param name="__result">Result of the original method</param>
		/// <returns>Run original method implementation</returns>
		public static bool Prefix(Pawn one, Pawn two, ref bool __result)
		{
			__result = RsiIncestuous(one, two);
			return false;
		}

		/// <summary>
		/// Check if Ideology allows romance attempt
		/// </summary>
		/// <param name="one">Pawn to try do romance</param>
		/// <param name="two">Target for romance</param>
		/// <returns>Forbid romance option</returns>
		public static bool RsiIncestuous(Pawn one, Pawn two)
		{
			PreceptDef incestuousPrecept = one.Ideo?.PreceptsListForReading.Select(precept => precept.def).FirstOrFallback(def => def.issue == RsiDefOf.Issue.Incestuos);
			var allowManualRomanceOnlyFor = incestuousPrecept?.GetModExtension<DefExtension_Incest>()?.allowManualRomanceOnlyFor;
			BloodRelationDegree relationDegree = RelationHelpers.GetBloodRelationDegree(one, two);

			if (allowManualRomanceOnlyFor == null)
			{
				return relationDegree < BloodRelationDegree.NotRelated;
			}

			return !allowManualRomanceOnlyFor.Contains(relationDegree);
		}
	}

	[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryRomanceChanceFactor))]
	public static class Rimworld_Patch_SecondaryRomanceChanceFactor
	{
		public static bool Prepare() => RsiMod.Prefs.patchRomanceChanceFactor;

		/// <summary>
		/// <para>
		/// Replace
		/// float num = 1f;
		/// foreach (PawnRelationDef pawnRelationDef in this.pawn.GetRelations(otherPawn))
		/// {
		/// 	num *= pawnRelationDef.romanceChanceFactor;
		/// }
		/// </para>
		/// <para>with</para>
		/// <para>
		/// float num = 1f;
		/// num = RomanceChanceFactorHelpers.GetRomanceChanceFactor(this.pawn, otherPawn)
		/// </para>
		/// </summary>
		/// <param name="instructions">Original method body</param>
		/// <returns>Modified method body</returns>
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getRelationsInfo = AccessTools.Method(
				typeof(PawnRelationUtility),
				nameof(PawnRelationUtility.GetRelations),
				new[] { typeof(Pawn), typeof(Pawn) });
			MethodInfo helperInfo = AccessTools.Method(
				typeof(RomanceChanceFactorHelpers),
				nameof(RomanceChanceFactorHelpers.GetRomanceChanceFactor),
				new[] { typeof(Pawn), typeof(Pawn) });

			// Original IL looks something like this:
			// Load this.pawn
			// Load otherPawn
			// Call GetRelations
			// Start loop
			// ...
			// Mul num and romanceChanceFactor
			// Store result
			// ...
			// Endfinaly

			// Idea is to substitute GetRelations call with a method with the same signature,
			// store results in the same place original loop does and remove everything else until Endfinaly

			IEnumerator<CodeInstruction> enumerator = instructions.GetEnumerator();
			bool endOfInstructions = !enumerator.MoveNext();

			// Find and replace GetRelations
			while (!endOfInstructions)
			{
				var instruction = enumerator.Current;

				if (instruction.Calls(getRelationsInfo))
				{
					yield return new CodeInstruction(OpCodes.Call, helperInfo);
					enumerator.MoveNext(); // skip original method call
					break;
				}

				yield return instruction;
				endOfInstructions = !enumerator.MoveNext();
			}

			if (endOfInstructions)
			{
				Log.Error("[RSI] Failed to transpile Pawn_RelationsTracker.SecondaryRomanceChanceFactor: PawnRelationUtility.GetRelations call not found");
				yield break;
			}

			// Skip everything until Mul
			while (!endOfInstructions)
			{
				if (enumerator.Current.opcode == OpCodes.Mul)
				{
					enumerator.MoveNext(); // skip Mul
					yield return enumerator.Current; // return next op, it should be "store result"
					break;
				}

				endOfInstructions = !enumerator.MoveNext();
			}

			if (endOfInstructions)
			{
				Log.Error("[RSI] Failed to transpile Pawn_RelationsTracker.SecondaryRomanceChanceFactor: Mul not found. This error means half of SecondaryRomanceChanceFactor was erased. Very not good");
				yield break;
			}

			// Skip the rest of the loop
			while (!endOfInstructions)
			{
				if (enumerator.Current.opcode == OpCodes.Endfinally)
				{
					// Endfinally will be skipped by the next MoveNext()
					break;
				}

				endOfInstructions = !enumerator.MoveNext();
			}

			if (endOfInstructions)
			{
				Log.Error("[RSI] Failed to transpile Pawn_RelationsTracker.SecondaryRomanceChanceFactor: Endfinally not found. This error means half of SecondaryRomanceChanceFactor was erased. Very not good");
				yield break;
			}

			// Return the rest of the method
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}

			if (Prefs.DevMode) Log.Message("[RSI] Successfully transpiled Pawn_RelationsTracker.SecondaryRomanceChanceFactor");
		}

		[HarmonyPatch(typeof(Precept), nameof(Precept.GetTip))]
		public static class Rimworld_Patch_PreceptTip
		{
			public static void Postfix(ref string __result, Precept __instance)
			{
				if (__instance.def.modExtensions.NullOrEmpty())
				{
					return;
				}

				bool tipChanged = false;
				StringBuilder tipBuilder = new StringBuilder(__result);
				tipBuilder.AppendLine();
				tipBuilder.AppendInNewLine((Keyed.ModTitle + ":").Colorize(ColoredText.TipSectionTitleColor));

				for (int i = 0; i < __instance.def.modExtensions.Count; i++)
				{
					if (__instance.def.modExtensions[i] is IPreceptTipPostfix tipPostfix)
					{
						tipBuilder.AppendInNewLine("  - " + tipPostfix.GetTip());
						tipChanged = true;
					}
				}

				if (tipChanged)
				{
					__result = tipBuilder.ToString();
				}
			}
		}
	}
}
