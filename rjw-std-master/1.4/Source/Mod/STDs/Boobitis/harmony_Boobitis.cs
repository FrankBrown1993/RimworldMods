using Verse;
using HarmonyLib;
using System;
using rjw;

namespace rjwstd
{
	///<summary>
	///boobitis increase sex need/make pawn horny
	///</summary>
	[HarmonyPatch(typeof(Need_Sex), "diseasefactor")]
	[StaticConstructorOnStartup]
	static class SexNeed_diseasefactorSTD_Boobitis
	{
		[HarmonyPostfix]
		private static void diseasefactor_StdPatch(Pawn pawn, ref float __result)
		{
			try
			{
				if (pawn.health.hediffSet.HasHediff(std_Boobitis.boobitis.hediff_def))
				{
					__result *= 3f;
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}

	/// <summary>
	/// boobitis increase breast size
	/// this is probably outdated, but keep it for now atleast
	/// </summary>
	[HarmonyPatch(typeof(std_updater), "update")]
	[StaticConstructorOnStartup]
	static class std_updater_Boobitis
	{
		[HarmonyPostfix]
		private static void updateSTD(Pawn p)
		{
			try
			{
				std_Boobitis.update(p);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
