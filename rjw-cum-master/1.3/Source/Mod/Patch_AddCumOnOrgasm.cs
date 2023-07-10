using Verse;
using HarmonyLib;
using rjw;
using System;
//using Multiplayer.API;

namespace rjwcum
{
	///<summary>
	///apply cum to pawn after vanilla sex
	///</summary>
	[HarmonyPatch(typeof(SexUtility), "Aftersex")]
	[StaticConstructorOnStartup]
	static class Aftersex_Cum_Apply
	{
		[HarmonyPostfix]
		private static void Aftersex_Cum_Patch(SexProps props)
		{
			try
			{
				if (props.isCoreLovin)
					if (!props.usedCondom)
					{
						CumHelper.calculateAndApplyCum(props);
						//SexUtility.CumFilthGenerator(props.pawn);
						//SexUtility.CumFilthGenerator(props.partner);
					}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}

	///<summary>
	///apply cum to pawn after rjw orgasm
	///</summary>
	[HarmonyPatch(typeof(JobDriver_Sex), "Orgasm")]
	[StaticConstructorOnStartup]
	static class Orgasm_Cum_Apply
	{
		[HarmonyPrefix]
		private static bool Orgasm_Cum_Patch(JobDriver_Sex __instance)
		{
			try
			{
				if (__instance.sex_ticks > __instance.orgasmstick) //~3s at speed 1
				{
					return true;
				}
				var props = __instance.Sexprops;
				if (!props.usedCondom)
				{
					CumHelper.calculateAndApplyCum(props);
					//SexUtility.CumFilthGenerator(props.pawn);
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
			return true;
		}
	}
}
