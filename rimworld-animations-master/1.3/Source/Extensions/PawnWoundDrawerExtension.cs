using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Rimworld_Animations;

namespace Rimworld_Animations
{
	[StaticConstructorOnStartup]
	public static class PawnWoundDrawerExtension
	{
		public static void RenderOverBody(this PawnWoundDrawer pawnWoundDrawer, Vector3 drawLoc, Mesh bodyMesh, Quaternion quat, bool drawNow, BodyTypeDef.WoundLayer layer, Rot4 pawnRot, bool? overApparel = null, Pawn pawn = null, PawnRenderFlags flags = new PawnRenderFlags())
		{
			if (pawn == null)
			{ return; }

			if (!flags.FlagSet(PawnRenderFlags.Portrait) && layer == BodyTypeDef.WoundLayer.Head)
			{
				CompBodyAnimator pawnAnimator = pawn.TryGetComp<CompBodyAnimator>();
				if (pawnAnimator != null && pawnAnimator.isAnimating && pawn.Drawer.renderer.graphics.headGraphic != null)
				{
					pawnRot = pawnAnimator.headFacing;
					quat = Quaternion.AngleAxis(angle: pawnAnimator.headAngle, axis: Vector3.up);
					float y = drawLoc.y;
					drawLoc = pawnAnimator.getPawnHeadPosition() - Quaternion.AngleAxis(pawnAnimator.headAngle, Vector3.up) * pawn.Drawer.renderer.BaseHeadOffsetAt(pawnAnimator.headFacing);
					drawLoc.y = y;
				}
			}

			pawnWoundDrawer.RenderOverBody(drawLoc, bodyMesh, quat, drawNow, layer, pawnRot, overApparel);
		}
	}
}
