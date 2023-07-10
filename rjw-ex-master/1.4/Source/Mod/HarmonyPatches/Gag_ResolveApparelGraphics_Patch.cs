using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using rjw;

namespace rjwex
{
	[HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
	public static class ResolveApparelGraphics_Patch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int Index = -1;
			string targetCall = "TryGetGraphicApparel";

			var codes = new List<CodeInstruction>(instructions);

			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains(targetCall))
				{
					Index = i;
					break;
				}
			}
			if (Index > -1)
			{
				codes[Index].operand = typeof(ResolveApparelGraphics_Patch).GetMethod(
					nameof(ResolveApparelGraphics_Patch.RJWExTryGetGraphicApparel), BindingFlags.Static | BindingFlags.Public);
				Logger.ListLine("Gag graphics patch applied @ Verse.PawnGraphicSet.ResolveApparelGraphics as Transpiler");
			}
			else
			{
				Log.Error("[RJWEx] Verse.PawnGraphicSet.ResolveApparelGraphics not patched!");
			}

			return codes.AsEnumerable();
		}

		public static bool RJWExTryGetGraphicApparel(Apparel apparel, BodyTypeDef bodyType, out ApparelGraphicRecord rec)
		{
			if (!(apparel is bondage_gear && apparel.def.apparel.bodyPartGroups.Contains(xxxEx.GroupDefTeeth))) //Not RJW Gag? TODO better item detection
				return ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, bodyType, out rec);  //return default TryGetGraphicApparel()
			else
			{
				Pawn pawn = apparel.Wearer;
				if (pawn == null)
				{                                                   //handle as null wornGraphicPath
					rec = new ApparelGraphicRecord(null, null);
					return false;
				}

				string headGraphicPath = pawn.story.headType.graphicPath; //path containing Normal/Pointy/Wide
				string gender = pawn.gender.ToString(); //Female/Male/None
				string crownType = pawn.story.headType.narrow.ToString(); //Narrow/Average  TODO check
				if (pawn.story.headType.narrow)
					crownType = "Narrow";
				else 
					crownType = "Average";

				//checks====
				if (headGraphicPath.NullOrEmpty() || apparel.def.apparel.wornGraphicPath.NullOrEmpty())
					return ApparelGraphicRecordGetter.TryGetGraphicApparel(apparel, bodyType, out rec);  //something wrong go default
				if (pawn.gender == Gender.None)
					gender = "Female";
				//==========

				string faceType; //Normal/Pointy/Wide, from headGraphicPath;
				if (headGraphicPath.Contains("Pointy")) { faceType = "Pointy"; }
				else if (headGraphicPath.Contains("Wide")) { faceType = "Wide"; }
				else { faceType = "Normal"; }

				string path = apparel.def.apparel.wornGraphicPath + "_" + gender + "_" + crownType + "_" + faceType;

				Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
				rec = new ApparelGraphicRecord(graphic, apparel);

				return true;
			}
		}
	}
}
