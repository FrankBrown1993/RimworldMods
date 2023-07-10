using Verse;
using HarmonyLib;
using System;
using rjw;
using RimWorld;

namespace rjwstd
{
	[HarmonyPatch(typeof(std_updater), "update")]
	[StaticConstructorOnStartup]
	static class std_updater_Syphilis
	{
		[HarmonyPostfix]
		private static void updateSTD(Pawn p)
		{
			try
			{
				std_Syphilis.update(p);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
