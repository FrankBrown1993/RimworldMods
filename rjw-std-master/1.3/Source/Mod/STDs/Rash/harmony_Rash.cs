using Verse;
using HarmonyLib;
using rjw;
using System;
using RimWorld;

namespace rjwstd
{
	[HarmonyPatch(typeof(AfterSexUtility), "think_about_sex", new Type[] {typeof(Pawn), typeof(Pawn), typeof(bool), typeof(SexProps), typeof(bool)})]
	[StaticConstructorOnStartup]
	static class Aftersex_STDThoughtApply
	{
		[HarmonyPostfix]
		private static void ThinkAboutDiseasesStdPatch(Pawn pawn, Pawn partner, bool isReceiving, SexProps props, bool whoring = false)
		{
			try
			{
				std_Rash.ThinkAboutDiseases(pawn, partner);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		} 
	}
}
