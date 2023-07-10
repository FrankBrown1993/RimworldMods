using Verse;
using HarmonyLib;
using rjw;
using System;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using rjw.MainTab.DefModExtensions;
using System.Linq;
using System.Reflection;

namespace rjwwhoring
{
	[HarmonyPatch(typeof(rjw.MainTab.MainTabWindow))]
	[HarmonyPatch(nameof(rjw.MainTab.MainTabWindow.MakeOptions))]
	static class RJWTab_Brothel
	{
		[HarmonyPostfix]
		private static void MakeOptionsPatch(rjw.MainTab.MainTabWindow __instance, ref List<FloatMenuOption> __result)
		{
			try
			{
				RJWTab_Brothel_Patch.MakeOptionsPatch(__instance, ref __result);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}

	static class RJWTab_Brothel_Patch
	{
		public static List<FloatMenuOption> MakeOptionsPatch(rjw.MainTab.MainTabWindow __instance, ref List<FloatMenuOption> __result)
		{
			PawnTableDef RJW_Brothel = DefDatabase<PawnTableDef>.GetNamed("RJW_Brothel");
			ModLog.Message("0");
			__result.Add(new FloatMenuOption(RJW_Brothel.GetModExtension<RJW_PawnTable>().label, () =>
			{
				ModLog.Message("1");
				__instance.pawnTableDef = RJW_Brothel;
				ModLog.Message("2");
				__instance.pawns = Find.CurrentMap.mapPawns.AllPawns.Where(p => xxx.is_human(p) && (p.IsColonist || p.IsPrisonerOfColony));
				ModLog.Message("3");
				__instance.Notify_ResolutionChanged();
				ModLog.Message("4");
				rjw.MainTab.MainTabWindow.Reloadtab();
				ModLog.Message("5");
			}, MenuOptionPriority.Default));
			return __result;
		}
	}
}
