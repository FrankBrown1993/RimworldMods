using System.Collections.Generic;
using Verse;
using HarmonyLib;
using UnityEngine;
using System;
using RimWorld;
//using Multiplayer.API;

namespace rjwcum
{

	[HarmonyPatch(typeof(RimWorld.PawnOverlayDrawer))]
	[HarmonyPatch("RenderPawnOverlay")]
	[HarmonyPatch(new Type[] { typeof(Vector3), typeof(Mesh), typeof(Quaternion), typeof(bool), typeof(PawnOverlayDrawer.OverlayLayer), typeof(Rot4), typeof(bool) })]
	class Patch_RenderPawnOverlay
	{
		[HarmonyPostfix]
		static void DrawCum(RimWorld.PawnOverlayDrawer __instance, Vector3 drawLoc, Mesh bodyMesh, Quaternion quat, bool drawNow, PawnOverlayDrawer.OverlayLayer layer, Rot4 pawnRot, bool? overApparel = null)
		{
			if (!CumBase.cum_overlays) return;

			Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();//get local variable

			//TODO add support for animals? unlikely as they has weird meshes
			//for now, only draw humans
			if (pawn.RaceProps.Humanlike) //&& CumOverlayBase.cum_overlays)
			{
				//find cum hediff. if it exists, use its draw function
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				if (hediffs.Exists(x => x.def == HediffDefOf.Hediff_CumController))
				{
					Hediff_CumController h = hediffs.Find(x => x.def == HediffDefOf.Hediff_CumController) as Hediff_CumController;

					quat.ToAngleAxis(out float angle, out Vector3 axis);//angle changes when pawn is e.g. downed

					//adjustments if the pawn is sleeping in a bed:
					bool inBed = false;
					Building_Bed building_Bed = pawn.CurrentBed();
					if (building_Bed != null)
					{
						inBed = !building_Bed.def.building.bed_showSleeperBody;
						AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 15);
						Vector3 vector2 = pawn.Position.ToVector3ShiftedWithAltitude(altLayer);
						vector2.y += 0.02734375f+0.01f;//just copied from rimworld code+0.01f
						drawLoc.y = vector2.y;
					}

					h.DrawCum(drawLoc, quat, layer == PawnOverlayDrawer.OverlayLayer.Head, angle);
				}
			}

		}
	}
}
