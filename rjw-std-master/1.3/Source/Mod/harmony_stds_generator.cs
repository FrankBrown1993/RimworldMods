using Verse;
using HarmonyLib;
using rjw;
using System;
using System.Reflection;

namespace rjwstd
{
	///<summary>
	///roll for STD update every 10 sex need ticks
	///</summary>
	[HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest)})]
	[StaticConstructorOnStartup]
	static class PawnGenerator_STD_spreader
	{
		[HarmonyPostfix]
		private static void PawnGenerator_STD_spreader_Patch(ref PawnGenerationRequest request, ref Pawn __result)
		{
			try
			{
				if (!request.Newborn)
					if (__result != null)
						std_spreader.generate_on(__result);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
