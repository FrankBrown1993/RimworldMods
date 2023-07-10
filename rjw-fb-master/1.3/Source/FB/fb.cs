using HarmonyLib;
using RimWorld;
using rjw;
using Verse;
using System;
using System.Reflection;

namespace rjwfb
{
	[StaticConstructorOnStartup]
	internal static class InitHarmonyPatches
	{
		static InitHarmonyPatches()
		{
			var har = new Harmony("rjw.FB");
			har.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[HarmonyPatch(typeof(AfterSexUtility), "think_about_sex", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(SexProps), typeof(bool) })]
	[StaticConstructorOnStartup]
	static class Beautyfuck_AfterSexUtility_think_about_sex
	{
		public static readonly ThoughtDef RJW_Beautyfuck = DefDatabase<ThoughtDef>.GetNamed("RJW_Beautyfuck");

		[HarmonyPostfix]
		public static void think_about_sex_Patch(Pawn pawn, Pawn partner, bool isReceiving, SexProps props, bool whoring = false)
		{
			try
			{
				if (pawn == null)
				{
					return;
				}
				if (partner == null)
				{
					return;
				}
				var p1 = pawn;
				var p2 = partner;
				var p2beauty = p2.GetStatValue(StatDefOf.PawnBeauty, true);
				var beautystage = -1;

				if (p2beauty != 0)
				{
					if (p2beauty <= -2)
						beautystage = 0;
					else if (p2beauty <= -1)
						beautystage = 1;
					else if (p2beauty < 2)
						beautystage = 2;
					else
						beautystage = 3;

					if (beautystage != -1)
						p1?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtMaker.MakeThought(RJW_Beautyfuck, beautystage), null);
				}
			}
			catch(Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
