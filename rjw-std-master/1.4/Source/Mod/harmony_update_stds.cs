using Verse;
using HarmonyLib;
using rjw;
using System;
using System.Reflection;

namespace rjwstd
{
	///<summary>
	///roll for STD update
	///</summary>
	[HarmonyPatch(typeof(Need_Sex), "NeedInterval")]
	[StaticConstructorOnStartup]
	static class Need_Sex_STD_Update
	{
		[HarmonyPostfix]
		private static void Need_Sex_STD_Patch(Need_Sex __instance, Pawn ___pawn)
		{
			try
			{
				if (__instance.isInvisible)
					return; // no caravans

				std_updater.update(___pawn);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
