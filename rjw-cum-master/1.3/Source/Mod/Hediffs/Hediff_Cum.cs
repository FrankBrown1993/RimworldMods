using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
//using Multiplayer.API;

namespace rjwcum
{
	public class Hediff_Cum : HediffWithComps
	{
		public int cumType = CumHelper.CUM_NORMAL;//-> different colors

		public string giverName = null;//not utilized right now, maybe in the future save origin of the cum

		public override string LabelInBrackets
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.LabelInBrackets);
				if (this.sourceHediffDef != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(this.sourceHediffDef.label);
				}
				else if (this.source != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(this.source.label);
					if (this.sourceBodyPartGroup != null)
					{
						stringBuilder.Append(" ");
						stringBuilder.Append(this.sourceBodyPartGroup.LabelShort);
					}
				}
				return stringBuilder.ToString();
			}
		}


		public override string SeverityLabel
		{
			get
			{
				if (this.Severity == 0f)
				{
					return null;
				}
				return this.Severity.ToString("F1");
			}
		}

		//[SyncMethod]
		public override bool TryMergeWith(Hediff other)
		{
			//if a new cum hediff is added to the same body part, they are combined. if severity reaches more than 1, spillover to other body parts occurs

			Hediff_Cum hediff_cum = other as Hediff_Cum;
			if (hediff_cum != null && hediff_cum.def == this.def && hediff_cum.Part == base.Part && this.def.injuryProps.canMerge)
			{
				cumType = hediff_cum.cumType;//take over new creature color

				float totalAmount = hediff_cum.Severity + this.Severity;
				if (totalAmount > 1.0f)
				{
					BodyPartDef spillOverTo = CumHelper.spillover(this.Part.def);//cumHelper saves valid other body parts for spillover
					if (spillOverTo != null)
					{
						//Rand.PopState();
						//Rand.PushState(RJW_Multiplayer.PredictableSeed());
						IEnumerable<BodyPartRecord> availableParts = CumHelper.getAvailableBodyParts(pawn);//gets all non missing, valid body parts
						IEnumerable<BodyPartRecord> filteredParts = availableParts.Where(x => x.def == spillOverTo);//filters again for valid spill target
						if (!filteredParts.EnumerableNullOrEmpty())
						{
							BodyPartRecord spillPart = null;
							spillPart = filteredParts.RandomElement<BodyPartRecord>();//then pick one
							if (spillPart != null)
							{
								CumHelper.cumOn(pawn, spillPart, totalAmount - this.Severity, null, cumType);
							}
						}
					}
				}

				return (base.TryMergeWith(other));

			}
			return (false);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref cumType, "cumType", CumHelper.CUM_NORMAL);

			if (Scribe.mode == LoadSaveMode.PostLoadInit && base.Part == null)
			{
				//Log.Error("Hediff_cum has null part after loading.", false);
				this.pawn.health.hediffSet.hediffs.Remove(this);
				return;
			}
		}

		//handles the icon in the health tab and its color
		public override TextureAndColor StateIcon
		{
			get
			{
				TextureAndColor tex = TextureAndColor.None;
				Color color = Color.white;
				switch (cumType)
				{
					case CumHelper.CUM_NORMAL:
						color = CumHelper.color_normal;
						break;
					case CumHelper.CUM_INSECT:
						color = CumHelper.color_insect;
						break;
					case CumHelper.CUM_MECHA:
						color = CumHelper.color_mecha;
						break;
				}

				Texture2D tex2d = CumTextures.CumIcon_little;
				switch (this.CurStageIndex)
				{
					case 0:
						tex2d = CumTextures.CumIcon_little;
						break;
					case 1:
						tex2d = CumTextures.CumIcon_some;
						break;
					case 2:
						tex2d = CumTextures.CumIcon_dripping;
						break;
					case 3:
						tex2d = CumTextures.CumIcon_drenched;
						break;
				}

				tex = new TextureAndColor(tex2d, color);

				return tex;
			}
		}

	}
}
