using Verse;
using HarmonyLib;
using rjw;
using System;
using RimWorld;

namespace rjwwhoring
{
	[HarmonyPatch(typeof(AfterSexUtility), "think_about_sex", new Type[] {typeof(Pawn), typeof(Pawn), typeof(bool), typeof(SexProps), typeof(bool)})]
	[StaticConstructorOnStartup]
	static class Aftersex_WhoringhoughtApply
	{
		[HarmonyPostfix]
		private static void ThinkAboutWhoringPatch(Pawn pawn, Pawn partner, bool isReceiving, SexProps props, bool whoring = false)
		{
			try
			{
				AfterSexUtilityPatch.ThinkAboutWhoring(pawn, partner, isReceiving, props, whoring);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		} 
	}

	static class AfterSexUtilityPatch
	{
		///<summary>
		///add aftersex thoughts for whoring
		///</summary>
		public static void ThinkAboutWhoring(Pawn pawn, Pawn partner, bool isReceiving, SexProps props, bool whoring = false)
		{
			//no whoring in vanilla sex
			if (props.isCoreLovin)
				return;

			//masturbation?
			if (pawn == null || partner == null)
				return;

			//necro
			if (pawn.Dead || partner.Dead)
				return;

			//bestiality
			if (!(xxx.is_human(pawn) && xxx.is_human(partner)))
				return;

			//whoring, initiator is whore
			if (whoring && !props.isReceiver)
			{
				ThoughtDef memory;

				if (pawn.IsPrisoner || xxx.is_slave(pawn))
					memory = ThoughtDefOf.Whorish_Thoughts_Captive;
				else
					memory = ThoughtDefOf.Whorish_Thoughts;

				pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
			}
		}
	}
}
