using System.Collections.Generic;
using Verse;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace rjwex
{
	//implemented in PawnRendererRenderInternal_Pathch
	//[HarmonyPatch(typeof(Pawn_DrawTracker))]
	//[HarmonyPatch("DrawPos", MethodType.Getter)]
	class Pawn_DrawTracker_DrawPos_Patch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			//HarmonyInstance.DEBUG = true;
			CodeInstruction newLine;
			var codes = new List<CodeInstruction>(instructions);
			int insertIndex = codes.Count - 2;// insertion before final "ldloc.0" "ret"

			/* inserted code
			ldloc.0      // vector3
			ldarg.0      // this
			ldfld        class Verse.Pawn Verse.Pawn_DrawTracker::pawn
			call         valuetype [UnityEngine]UnityEngine.Vector3 rjwex.FMHelper::FMOffset(class ['Assembly-CSharp']Verse.Pawn)
			call         valuetype [UnityEngine]UnityEngine.Vector3 [UnityEngine]UnityEngine.Vector3::op_Addition(valuetype [UnityEngine]UnityEngine.Vector3, valuetype [UnityEngine]UnityEngine.Vector3)
			stloc.0      // vector3
			*/

			newLine = new CodeInstruction(OpCodes.Ldloc_0);
			codes.Insert(insertIndex, newLine);
			insertIndex++;

			newLine = new CodeInstruction(OpCodes.Ldarg_0);
			codes.Insert(insertIndex, newLine);
			insertIndex++;

			newLine = new CodeInstruction(OpCodes.Ldfld) { operand = AccessTools.Field(typeof(Pawn_DrawTracker), "pawn") };
			codes.Insert(insertIndex, newLine);
			insertIndex++;

			newLine = new CodeInstruction(OpCodes.Call) { operand = typeof(DrawUtils).GetMethod("GetDrawOffset") };
			codes.Insert(insertIndex, newLine);
			insertIndex++;

			newLine = new CodeInstruction(OpCodes.Call) { operand = typeof(Vector3).GetMethod("op_Addition", new Type[] { typeof(Vector3), typeof(Vector3) }) };
			codes.Insert(insertIndex, newLine);
			insertIndex++;

			newLine = new CodeInstruction(OpCodes.Stloc_0);
			codes.Insert(insertIndex, newLine);


			Logger.ListLine("Pawn draw FMOffset addition applied @ Verse.Pawn_DrawTracker.get_DrawPos as Transpiler");
			return codes.AsEnumerable();
		}
	}

	[HarmonyPatch]
	static class PawnRendererRenderInternal_Pathch
	{
		static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4),
																								typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool)});
		}

		static bool Prefix(ref Vector3 rootLoc, ref float angle, PawnRenderer __instance)
		{
			Pawn pawn = (AccessTools.Field(typeof(PawnRenderer), "pawn")).GetValue(__instance) as Pawn;

			rootLoc += pawn.GetDrawOffset();
			angle += pawn.GetAngleOffset();

			return true;
		}
	}
}
