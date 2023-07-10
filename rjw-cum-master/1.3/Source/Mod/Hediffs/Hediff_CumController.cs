using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;
//using Multiplayer.API;

namespace rjwcum
{
	class Hediff_CumController : HediffWithComps
	{
		/*
		Whenever cum is applied, this hediff is also added to the pawn.
		Since there is always only a single hediff of this type on the pawn, it serves as master controller, adding up the individual cum hediffs, applying debuffs and drawing overlays
		*/

		private static readonly float cumWeight = 0.2f;//how much individual cum_hediffs contribute to the overall bukkake severity
		List<Hediff> hediffs_cum;
		Dictionary<string, CumSplatch> splatches;

		public override void ExposeData()
		{
			base.ExposeData();
			//Scribe_Values.Look<Dictionary<string, CumSplatch>>(ref splatches, "splatches", new Dictionary<string, CumSplatch>()); - causes errors when loading. for now, just make a new dictionary
			splatches = new Dictionary<string, CumSplatch>();//instead of loading, just recreate anew
			hediffs_cum = new List<Hediff>();
		}

		public override void PostMake()
		{
			base.PostMake();
			splatches = new Dictionary<string, CumSplatch>();
		}

		public override void PostTick()
		{
			if (pawn.RaceProps.Humanlike)//for now, only humans are supported 
			{
				hediffs_cum = this.pawn.health.hediffSet.hediffs.FindAll(x => (x.def == HediffDefOf.Hediff_Cum || x.def == HediffDefOf.Hediff_InsectSpunk || x.def == HediffDefOf.Hediff_MechaFluids));
				float Level = CalculateLevel();//sum of severity of all the cum hediffs x cumWeight
				this.Severity = Level;
				bool updatePortrait = false;

				//loop through all cum hediffs, add missing ones to dictionary
				for (int i = 0; i < hediffs_cum.Count(); i++)
				{
					Hediff_Cum h = (Hediff_Cum)hediffs_cum[i];
					string ID = h.GetUniqueLoadID();//unique ID for each hediff
					if (!splatches.ContainsKey(ID))//if it isn't here yet, make new object
					{
						updatePortrait = true;
						bool leftSide = h.Part.Label.Contains("left") ? true : false;//depending on whether the body part is left or right, drawing-offset on x-aixs may be inverted

						splatches[ID] = new CumSplatch(h, pawn.story.bodyType, h.Part.def, leftSide, h.cumType);
					}
				}



				//remove splatch objects once their respective cum hediff is gone
				List<string> removeKeys = new List<string>();
				foreach (string key in splatches.Keys)
				{
					CumSplatch s = splatches[key];

					if (!hediffs_cum.Contains(s.hediff_cum))
					{
						removeKeys.Add(key);
						updatePortrait = true;
					}
				}
				//loop over and remove elements that should be destroyed:
				foreach (string key in removeKeys)
				{
					CumSplatch s = splatches[key];
					splatches.Remove(key);
				}

				if (updatePortrait)//right now, portraits are only updated when a completely new cum hediff is added or an old one removed - maybe that should be done more frequently
				{
					PortraitsCache.SetDirty(pawn);
				}
			}
		}

		//called from the PawnWoundDrawer (see HarmonyPatches)
		public void DrawCum(Vector3 drawLoc, Quaternion quat, bool forPortrait, float angle, bool inBed = false)
		{
			Rot4 bodyFacing = pawn.Rotation;
			int facingDir = bodyFacing.AsInt;//0: north, 1:east, 2:south, 3:west
			foreach (string key in splatches.Keys)
			{
				CumSplatch s = splatches[key];
				s.Draw(drawLoc, quat, forPortrait, facingDir, angle, inBed);
			}
		}

		//new Hediff_CumController added to pawn -> just combine the two
		public override bool TryMergeWith(Hediff other)
		{
			if (other == null || other.def != this.def)
			{
				return false;
			}
			return true;
		}

		private float CalculateLevel()
		{
			float num = 0f;
			for (int i = 0; i < hediffs_cum.Count; i++)
			{
				num += hediffs_cum[i].Severity * cumWeight;
			}
			return num;
		}

		//class for handling drawing of the individual splatches
		private class CumSplatch
		{
			public readonly Hediff_Cum hediff_cum;
			public readonly Material cumMaterial;
			public readonly BodyPartDef bodyPart;
			private bool mirrorMesh;

			private const float maxSize = 0.20f;//1.0 = 1 tile
			private const float minSize = 0.05f;

			//data taken from CumHelper.cs:
			private readonly float[] xAdjust;
			private readonly float[] zAdjust;
			private readonly float[] yAdjust;

			public CumSplatch(Hediff_Cum hediff, BodyTypeDef bodyType, BodyPartDef bodyPart, bool leftSide = false, int cumType = CumHelper.CUM_NORMAL)
			{
				hediff_cum = hediff;
				cumMaterial = new Material(CumTextures.pickRandomSplatch());//needs to create new material in order to allow for different colors
				cumMaterial.SetTextureScale("_MainTex", new Vector2(-1, 1));
				this.bodyPart = bodyPart;
				//Rand.PopState();
				//Rand.PushState(RJW_Multiplayer.PredictableSeed());

				//set color:
				switch (cumType)
				{
					case CumHelper.CUM_NORMAL:
						cumMaterial.color = CumHelper.color_normal;
						break;
					case CumHelper.CUM_INSECT:
						cumMaterial.color = CumHelper.color_insect;
						break;
					case CumHelper.CUM_MECHA:
						cumMaterial.color = CumHelper.color_mecha;
						break;
				}

				//if (!MP.enabled)
					mirrorMesh = (Rand.Value > 0.5f);//in 50% of the cases, flip mesh horizontally for more variance

				//x,y,z adjustments to draw splatches over the approximately correct locations; values stored in Cum helper - accessed by unique combinations of bodyTypes and bodyParts
				CumHelper.key k = new CumHelper.key(bodyType, bodyPart);
				if (CumHelper.splatchAdjust.Keys.Contains(k))
				{
					CumHelper.values helperValues = CumHelper.splatchAdjust[k];

					//invert, x-adjust (horizontal) depending on left/right body side:
					if (!leftSide)
					{
						float[] xAdjTemp = new float[4];
						for (int i = 0; i < xAdjTemp.Length; i++)
						{
							xAdjTemp[i] = helperValues.x[i] * -1f;
						}
						xAdjust = xAdjTemp;
					}
					else
					{
						xAdjust = helperValues.x;
					}

					zAdjust = helperValues.z;//vertical adjustment

				}
				else//fallback in the the key can't be found
				{
					//if (RJWSettings.DevMode)
					//{
					//	ModLog.Message("created cum splatch for undefined body type or part. BodyType: " + bodyType + " , BodyPart: " + bodyPart);
					//}
					xAdjust = new float[] { 0f, 0f, 0f, 0f };
					zAdjust = new float[] { 0f, 0f, 0f, 0f };
				}


				//y adjustments: plane/layer of drawing, > 0 -> above certain objects, < 0 -> below
				CumHelper.key_layer k2 = new CumHelper.key_layer(leftSide, bodyPart);

				if (CumHelper.layerAdjust.Keys.Contains(k2))
				{
					CumHelper.values_layer helperValues_layer = CumHelper.layerAdjust[k2];
					yAdjust = helperValues_layer.y;
				}
				else
				{
					yAdjust = new float[] { 0.02f, 0.02f, 0.02f, 0.02f };//over body in every direction
				}

			}

			public void Draw(Vector3 drawPos, Quaternion quat, bool forPortrait, int facingDir = 0, float angle = 0,bool inBed=false)
			{
				if (inBed)
				{
					if (this.bodyPart != BodyPartDefOf.Jaw && this.bodyPart != BodyPartDefOf.Head)//when pawn is in bed (=bed with sheets), only draw cum on head
					{
						return;
					}
				}

				//these two create new mesh instance and never destroying it, filling ram and crashing
				//float size = minSize+((maxSize-minSize)*hediff_cum.Severity);
				//mesh = MeshMakerPlanes.NewPlaneMesh(size);

				//use core MeshPool.plane025 instead

				//if (mirrorMesh)//horizontal flip
				//{
					//mesh = flipMesh(mesh);
				//}

				//rotation:
				if (angle == 0)//normal situation (pawn standing upright)
				{
					drawPos.x += xAdjust[facingDir];
					drawPos.z += zAdjust[facingDir];
				}
				else//if downed etc, more complex calculation becomes necessary
				{
					float radian = angle / 180 * (float)Math.PI;
					radian = -radian;
					drawPos.x += Mathf.Cos(radian) * xAdjust[hediff_cum.pawn.Rotation.AsInt] - Mathf.Sin(radian) * zAdjust[facingDir];//facingDir doesn't appear to be chosen correctly in all cases
					drawPos.z += Mathf.Cos(radian) * zAdjust[hediff_cum.pawn.Rotation.AsInt] + Mathf.Sin(radian) * xAdjust[facingDir];
				}

				//drawPos.y += yAdjust[facingDir];// 0.00: over body; 0.01: over body but under face, 0.02: over face, but under hair, -99 = "never" visible
				drawPos.y += yAdjust[facingDir];

				GenDraw.DrawMeshNowOrLater(MeshPool.plane025, drawPos, quat, cumMaterial, forPortrait);
			}

			//flips mesh UV horizontally, thereby mirroring the texture
			private Mesh flipMesh(Mesh meshToFlip)
			{
				var uvs = meshToFlip.uv;
				if (uvs.Length != 4)
				{
					return (meshToFlip);
				}
				for (var i = 0; i < uvs.Length; i++)
				{
					if (Mathf.Approximately(uvs[i].x, 1.0f))
						uvs[i].x = 0.0f;
					else
						uvs[i].x = 1.0f;
				}
				meshToFlip.uv = uvs;
				return (meshToFlip);
			}
		}
	}
}
