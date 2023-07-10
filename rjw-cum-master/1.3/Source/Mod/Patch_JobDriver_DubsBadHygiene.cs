using System;
using HarmonyLib;
using Verse;
using Verse.AI;
using rjw;

namespace rjwcum
{
	[HarmonyPatch(typeof(JobDriver), "Cleanup")]
	internal static class Patch_JobDriver_DubsBadHygiene
	{
		//not very good solution, some other mod can have same named jobdriver but w/e

		//Dubs Bad Hygiene washing
		private readonly static Type JobDriver_useWashBucket = AccessTools.TypeByName("JobDriver_useWashBucket");
		//private readonly static Type JobDriver_washAtCell = AccessTools.TypeByName("JobDriver_washAtCell");

		private readonly static Type JobDriver_UseHotTub = AccessTools.TypeByName("JobDriver_UseHotTub");
		private readonly static Type JobDriver_takeShower = AccessTools.TypeByName("JobDriver_takeShower");
		private readonly static Type JobDriver_takeBath = AccessTools.TypeByName("JobDriver_takeBath");

		[HarmonyPostfix]
		private static void Cleanup_cum(JobDriver __instance, JobCondition condition)
		{
			try
			{
				if (__instance == null)
					return;

				if (condition == JobCondition.Succeeded)
				{
					Do_cleanup_cum(__instance);
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public static void Do_cleanup_cum(JobDriver __instance)
		{
			Pawn pawn = __instance.pawn;

			//ModLog.Message("patches_DubsBadHygiene::on_cleanup_driver" + xxx.get_pawnname(pawn));

			if (xxx.DubsBadHygieneIsActive)
				//clear one instance of cum
				if (
					__instance.GetType() == JobDriver_useWashBucket// ||
																   //__instance.GetType() == JobDriver_washAtCell
					)
				{
					Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => (x.def == HediffDefOf.Hediff_Cum
																			|| x.def == HediffDefOf.Hediff_InsectSpunk
																			|| x.def == HediffDefOf.Hediff_MechaFluids
																			));
					if (hediff != null)
					{
						//ModLog.Message("patches_DubsBadHygiene::" + __instance.GetType()  + " clear => " + hediff.Label);
						hediff.Severity -= 1f;
					}
				}
				//clear all instance of cum
				else if (
						__instance.GetType() == JobDriver_UseHotTub ||
						__instance.GetType() == JobDriver_takeShower ||
						__instance.GetType() == JobDriver_takeBath
						)
				{
					foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
					{
						if (hediff.def == HediffDefOf.Hediff_Cum ||
							hediff.def == HediffDefOf.Hediff_InsectSpunk ||
							hediff.def == HediffDefOf.Hediff_MechaFluids
							)
						{
							//ModLog.Message("patches_DubsBadHygiene::" + __instance.GetType() + " clear => " + hediff.Label);
							hediff.Severity -= 1f;
						}
					}
				}
		}
	}
}