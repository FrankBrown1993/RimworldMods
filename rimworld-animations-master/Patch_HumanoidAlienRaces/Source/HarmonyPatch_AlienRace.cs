﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using AlienRace;

namespace Rimworld_Animations {

	
	[StaticConstructorOnStartup]
	public static class HarmonyPatch_AlienRace
	{
		static HarmonyPatch_AlienRace()
		{
			(new Harmony("rjwanim")).Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.HarmonyPatches"), "DrawAddons"),
							prefix: new HarmonyMethod(AccessTools.Method(typeof(HarmonyPatch_AlienRace), "Prefix_AnimateHeadAddons")));
		}

		/* todo: replace jank prefix with this
		public static void Prefix_DrawAddonsFinalHook(ref Pawn pawn, ref AlienPartGenerator.BodyAddon addon, ref Rot4 rot, ref Graphic graphic, ref Vector3 offsetVector, ref float angle, ref Material mat)
        {
			CompBodyAnimator animator = pawn.TryGetComp<CompBodyAnimator>();

			if (animator == null || !animator.isAnimating)
            {
				return;
            }

			if(addon.alignWithHead || addon.drawnInBed)
            {
				rot = animator.headFacing;
				angle = animator.headAngle;
				offsetVector += animator.deltaPos + animator.bodyAngle * animator.headBob;

			}
			else
            {
				rot = animator.bodyFacing;
				angle = animator.bodyAngle;
				offsetVector += animator.deltaPos;
            }
			
        }
		*/
		public static bool Prefix_AnimateHeadAddons(PawnRenderFlags renderFlags, Vector3 vector, Vector3 headOffset, Pawn pawn, Quaternion quat, Rot4 rotation)
		{

			if (renderFlags.FlagSet(PawnRenderFlags.Portrait) || pawn.TryGetComp<CompBodyAnimator>() == null || !pawn.TryGetComp<CompBodyAnimator>().isAnimating) return true;
			if (!(pawn.def is ThingDef_AlienRace alienProps) || renderFlags.FlagSet(PawnRenderFlags.Invisible)) return false;

			List<AlienPartGenerator.BodyAddon> addons = alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons;
			AlienPartGenerator.AlienComp comp = pawn.GetComp<AlienPartGenerator.AlienComp>();
			CompBodyAnimator pawnAnimator = pawn.TryGetComp<CompBodyAnimator>();

			bool flag = renderFlags.FlagSet(PawnRenderFlags.Portrait);
			bool flag2 = renderFlags.FlagSet(PawnRenderFlags.Invisible);

			for (int i = 0; i < addons.Count; i++)
			{
				AlienPartGenerator.BodyAddon ba = addons[index: i];
				
				if (!ba.CanDrawAddon(pawn: pawn)) continue;

				bool forceDrawForBody = false;
				if (alienProps.defName.Contains("Orassan") && ba.path.ToLower().Contains("tail"))
                {
					forceDrawForBody = true;
                }
				AlienPartGenerator.RotationOffset offset = ba.defaultOffsets.GetOffset((ba.drawnInBed && !forceDrawForBody) || ba.alignWithHead ? pawnAnimator.headFacing : pawnAnimator.bodyFacing);
				Vector3 a = (offset != null) ? offset.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, pawn.story.headType) : Vector3.zero;
				AlienPartGenerator.RotationOffset offset2 = ba.offsets.GetOffset((ba.drawnInBed && !forceDrawForBody) || ba.alignWithHead ? pawnAnimator.headFacing : pawnAnimator.bodyFacing);
				Vector3 vector2 = a + ((offset2 != null) ? offset2.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, pawn.story.headType) : Vector3.zero);
				vector2.y = (ba.inFrontOfBody ? (0.3f + vector2.y) : (-0.3f - vector2.y));
				float num = ba.angle;
				if (((ba.drawnInBed && !forceDrawForBody) || ba.alignWithHead ? pawnAnimator.headFacing : pawnAnimator.bodyFacing) == Rot4.North)
				{
					if (ba.layerInvert)
					{
						vector2.y = 0f - vector2.y;
					}
					num = 0f;
				}
				if (((ba.drawnInBed && !forceDrawForBody) || ba.alignWithHead ? pawnAnimator.headFacing : pawnAnimator.bodyFacing) == Rot4.East)
				{
					num = 0f - num;
					vector2.x = 0f - vector2.x;
				}
				Graphic addonGraphic = comp.addonGraphics[i];

				addonGraphic.drawSize = ((flag && ba.drawSizePortrait != Vector2.zero) ? ba.drawSizePortrait : ba.drawSize) * (ba.scaleWithPawnDrawsize ? (ba.alignWithHead ? ((flag ? comp.customPortraitHeadDrawSize : comp.customHeadDrawSize) * (ModsConfig.BiotechActive ? (pawn.ageTracker.CurLifeStage.headSizeFactor ?? 1.5f) : 1.5f)) : ((flag ? comp.customPortraitDrawSize : comp.customDrawSize) * (ModsConfig.BiotechActive ? pawn.ageTracker.CurLifeStage.bodySizeFactor : 1f) * 1.5f)) : (Vector2.one * 1.5f));

				if ((ba.drawnInBed && !forceDrawForBody) || ba.alignWithHead)
				{
					
					Quaternion addonRotation = Quaternion.AngleAxis(pawnAnimator.headAngle < 0 ? 360 - (360 % pawnAnimator.headAngle) : pawnAnimator.headAngle, Vector3.up);

					GenDraw.DrawMeshNowOrLater(mesh: addonGraphic.MeshAt(rot: pawnAnimator.headFacing), loc: vector + (ba.alignWithHead ? headOffset : headOffset - addonRotation * pawn.Drawer.renderer.BaseHeadOffsetAt(pawnAnimator.headFacing)) + vector2.RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: addonRotation)) * 2f * 57.29578f),
					quat: Quaternion.AngleAxis(angle: num, axis: Vector3.up) * addonRotation, mat: addonGraphic.MatAt(rot: pawnAnimator.headFacing), renderFlags.FlagSet(PawnRenderFlags.DrawNow));

					
				}

				else
				{
					Quaternion addonRotation;
					if (AnimationSettings.controlGenitalRotation && ba.path.ToLower().Contains("penis"))
                    {
						addonRotation = Quaternion.AngleAxis(pawnAnimator.genitalAngle, Vector3.up);
					}
					else
                    {
						addonRotation = Quaternion.AngleAxis(pawnAnimator.bodyAngle, Vector3.up);
					}
					
					if (AnimationSettings.controlGenitalRotation && pawnAnimator.controlGenitalAngle && ba?.hediffGraphics != null && ba.hediffGraphics.Count != 0 && ba.hediffGraphics[0]?.path != null && (ba.hediffGraphics[0].path.Contains("Penis") || ba.hediffGraphics[0].path.Contains("penis")))
					{
						GenDraw.DrawMeshNowOrLater(mesh: addonGraphic.MeshAt(rot: rotation), loc: vector + (ba.alignWithHead ? headOffset : Vector3.zero) + vector2.RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: addonRotation)) * 2f * 57.29578f),
						quat: Quaternion.AngleAxis(angle: pawnAnimator.genitalAngle, axis: Vector3.up), mat: addonGraphic.MatAt(rot: rotation), renderFlags.FlagSet(PawnRenderFlags.DrawNow));
					}

					else
                    {
						GenDraw.DrawMeshNowOrLater(mesh: addonGraphic.MeshAt(rot: rotation), loc: vector + (ba.alignWithHead ? headOffset : Vector3.zero) + vector2.RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: addonRotation)) * 2f * 57.29578f),
						quat: Quaternion.AngleAxis(angle: num, axis: Vector3.up) * addonRotation, mat: addonGraphic.MatAt(rot: rotation), renderFlags.FlagSet(PawnRenderFlags.DrawNow));
					}

				}


			}

			return false;
		}
	}



	/*

    [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "DrawAddons")]
    public static class HarmonyPatch_AlienRace {

		public static void RenderHeadAddonInAnimation(Mesh mesh, Vector3 loc, Quaternion quat, Material mat, bool drawNow, Graphic graphic, AlienPartGenerator.BodyAddon bodyAddon, Vector3 v, Vector3 headOffset, Pawn pawn, PawnRenderFlags renderFlags, Vector3 vector, Rot4 rotation)
        {

			CompBodyAnimator pawnAnimator = pawn.TryGetComp<CompBodyAnimator>();
			AlienPartGenerator.AlienComp comp = pawn.GetComp<AlienPartGenerator.AlienComp>();

			if (pawnAnimator != null && pawnAnimator.isAnimating)
            {

				if((bodyAddon.drawnInBed || bodyAddon.alignWithHead))
                {
					
					AlienPartGenerator.RotationOffset offset = bodyAddon.defaultOffsets.GetOffset(rotation);
					Vector3 a = (offset != null) ? offset.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, comp.crownType) : Vector3.zero;
					AlienPartGenerator.RotationOffset offset2 = bodyAddon.offsets.GetOffset(rotation);
					Vector3 vector2 = a + ((offset2 != null) ? offset2.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, comp.crownType) : Vector3.zero);
					vector2.y = (bodyAddon.inFrontOfBody ? (0.3f + vector2.y) : (-0.3f - vector2.y));
					float num = bodyAddon.angle;
					if (rotation == Rot4.North)
					{
						if (bodyAddon.layerInvert)
						{
							vector2.y = -vector2.y;
						}
						num = 0f;
					}
					if (rotation == Rot4.East)
					{
						num = -num;
						vector2.x = -vector2.x;
					}

					vector = vector + Quaternion.AngleAxis(pawnAnimator.bodyAngle, Vector3.up) * pawn.Drawer.renderer.BaseHeadOffsetAt(pawnAnimator.bodyFacing) - pawnAnimator.getPawnHeadOffset(); //convert vector into pseudo body pos for head
					quat = Quaternion.AngleAxis(pawnAnimator.headAngle, Vector3.up);
					loc = vector + (bodyAddon.alignWithHead ? headOffset : Vector3.zero) + vector2.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 2f * 57.29578f);
					mat = graphic.MatAt(rot: pawnAnimator.headFacing);
				}
				else
                {

					AlienPartGenerator.RotationOffset offset = bodyAddon.defaultOffsets.GetOffset(rotation);
					Vector3 a = (offset != null) ? offset.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, comp.crownType) : Vector3.zero;
					AlienPartGenerator.RotationOffset offset2 = bodyAddon.offsets.GetOffset(rotation);
					Vector3 vector2 = a + ((offset2 != null) ? offset2.GetOffset(renderFlags.FlagSet(PawnRenderFlags.Portrait), pawn.story.bodyType, comp.crownType) : Vector3.zero);
					vector2.y = (bodyAddon.inFrontOfBody ? (0.3f + vector2.y) : (-0.3f - vector2.y));
					float num = bodyAddon.angle;
					if (rotation == Rot4.North)
					{
						if (bodyAddon.layerInvert)
						{
							vector2.y = -vector2.y;
						}
						num = 0f;
					}
					if (rotation == Rot4.East)
					{
						num = -num;
						vector2.x = -vector2.x;
					}
					quat = Quaternion.AngleAxis(pawnAnimator.bodyAngle, Vector3.up);
					loc = vector + (bodyAddon.alignWithHead ? headOffset : Vector3.zero) + vector2.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 2f * 57.29578f);
					
				}

			}
			GenDraw.DrawMeshNowOrLater(mesh, loc, quat, mat, drawNow);

			/*
			if (pawnAnimator != null && !renderFlags.FlagSet(PawnRenderFlags.Portrait) && pawnAnimator.isAnimating && (bodyAddon.drawnInBed || bodyAddon.alignWithHead))
            {

				
				if ((pawn.def as ThingDef_AlienRace).defName == "Alien_Orassan")
                {
					orassan = true;

					if(bodyAddon.path.Contains("closed"))
                    {
						return;
                    }

					if (bodyAddon.bodyPart.Contains("ear"))

					{
						orassan = true;

						orassanv = new Vector3(0, 0, 0.23f);
						if (pawnAnimator.headFacing == Rot4.North)
						{
							orassanv.z -= 0.1f;
							orassanv.y += 1f;

							if(bodyAddon.bodyPart.Contains("left"))
                            {
								orassanv.x += 0.03f;
                            } else
                            {
								orassanv.x -= 0.03f;
							}

						}
						else if (pawnAnimator.headFacing == Rot4.East)
						{
							orassanv.x -= 0.1f;
						}
						else if (pawnAnimator.headFacing == Rot4.West)
						{
							orassanv.x = 0.1f;
						}
						else
                        {
							orassanv.z -= 0.1f;
							orassanv.y += 1f;

							if (bodyAddon.bodyPart.Contains("right"))
							{
								orassanv.x += 0.05f;
							}
							else
							{
								orassanv.x -= 0.05f;
							}
						}
						orassanv = orassanv.RotatedBy(pawnAnimator.headAngle);
					}
				}
					
					



			GenDraw.DrawMeshNowOrLater(mesh: graphic.MeshAt(rot: headRotInAnimation), loc: loc + orassanv + (bodyAddon.alignWithHead ? headOffset : Vector3.zero),// + v.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 2f * 57.29578f),
					quat: Quaternion.AngleAxis(angle: num, axis: Vector3.up) * headQuatInAnimation, mat: graphic.MatAt(rot: pawnAnimator.headFacing), drawNow: drawNow);;
			}

			else
            {
				
			}

			
		}


		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			List<CodeInstruction> ins = instructions.ToList();
			for (int i = 0; i < ins.Count; i++)
            {

				Type[] type = new Type[] { typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(bool) };


				if (ins[i].OperandIs(AccessTools.Method(typeof(GenDraw), "DrawMeshNowOrLater", type)))
                {
					
					yield return new CodeInstruction(OpCodes.Ldloc, (object)7); //graphic
					yield return new CodeInstruction(OpCodes.Ldloc, (object)4); //bodyAddon
					yield return new CodeInstruction(OpCodes.Ldloc, (object)5); //offsetVector/AddonOffset (v)
					yield return new CodeInstruction(OpCodes.Ldarg, (object)2); //headOffset
					yield return new CodeInstruction(OpCodes.Ldarg, (object)3); //pawn
					yield return new CodeInstruction(OpCodes.Ldarg, (object)0); //renderflags
					yield return new CodeInstruction(OpCodes.Ldarg, (object)1); //vector
					yield return new CodeInstruction(OpCodes.Ldarg, (object)5); //rotation

					yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(HarmonyPatch_AlienRace), "RenderHeadAddonInAnimation"));

                }
				
				else
                {
					yield return ins[i];
				}
            }
        }

		public static bool Prefix(PawnRenderFlags renderFlags, ref Vector3 vector, ref Vector3 headOffset, Pawn pawn, ref Quaternion quat, ref Rot4 rotation)
		{
			if(pawn == null)
            {
				return true;
			}
				
			CompBodyAnimator anim = pawn.TryGetComp<CompBodyAnimator>();

			if(anim == null)
            {
				return true;
            }

			if (anim != null && !renderFlags.FlagSet(PawnRenderFlags.Portrait) && anim.isAnimating)
			{
				//quat = Quaternion.AngleAxis(anim.bodyAngle, Vector3.up);
			}

			return true;

		}
	}

	
	*/

}


