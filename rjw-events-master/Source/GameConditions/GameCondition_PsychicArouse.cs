using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.Grammar;

namespace RJW_Events
{
    public class GameCondition_PsychicArouse : GameCondition
    {
		// Token: 0x17000EA2 RID: 3746
		// (get) Token: 0x06005437 RID: 21559 RVA: 0x001C44F0 File Offset: 0x001C26F0
		public override string Label
		{
			get
			{
				if (this.level == PsychicDroneLevel.GoodMedium)
				{
					return this.def.label + ": " + this.gender.GetLabel(false).CapitalizeFirst();
				}
				if (this.gender != Gender.None)
				{
					return string.Concat(new string[]
					{
						this.def.label,
						": ",
						this.level.GetLabel().CapitalizeFirst(),
						" (",
						this.gender.GetLabel(false).ToLower(),
						")"
					});
				}
				return this.def.label + ": " + this.level.GetLabel().CapitalizeFirst();
			}
		}

		// Token: 0x17000EA3 RID: 3747
		// (get) Token: 0x06005438 RID: 21560 RVA: 0x001C45B4 File Offset: 0x001C27B4
		public override string LetterText
		{
			get
			{
				if (this.level == PsychicDroneLevel.GoodMedium)
				{
					return this.def.letterText.Formatted(this.gender.GetLabel(false).ToLower());
				}
				return this.def.letterText.Formatted(this.gender.GetLabel(false).ToLower(), this.level.GetLabel());
			}
		}

		// Token: 0x17000EA4 RID: 3748
		// (get) Token: 0x06005439 RID: 21561 RVA: 0x001C4631 File Offset: 0x001C2831
		public override string Description
		{
			get
			{
				return base.Description.Formatted(this.gender.GetLabel(false).ToLower());
			}
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x001C4659 File Offset: 0x001C2859
		public override void PostMake()
		{
			base.PostMake();
			this.level = this.def.defaultDroneLevel;
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x001C4674 File Offset: 0x001C2874
		public override void RandomizeSettings(float points, Map map, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
		{
			if (this.def.defaultDroneLevel == PsychicDroneLevel.GoodMedium)
			{
				this.level = PsychicDroneLevel.GoodMedium;
			}
			else if (points < 800f)
			{
				this.level = PsychicDroneLevel.BadLow;
			}
			else if (points < 2000f)
			{
				this.level = PsychicDroneLevel.BadMedium;
			}
			else
			{
				this.level = PsychicDroneLevel.BadHigh;
			}
			if (map.mapPawns.FreeColonistsCount > 0)
			{
				this.gender = map.mapPawns.FreeColonists.RandomElement<Pawn>().gender;
			}
			else
			{
				this.gender = Rand.Element<Gender>(Gender.Male, Gender.Female);
			}
			outExtraDescriptionRules.Add(new Rule_String("psychicArouseLevel", this.level.GetLabel()));
			outExtraDescriptionRules.Add(new Rule_String("psychicArouseGender", this.gender.GetLabel(false)));
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x001C4730 File Offset: 0x001C2930
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Gender>(ref this.gender, "psychicArouseGender", Gender.None, false);
			Scribe_Values.Look<PsychicDroneLevel>(ref this.level, "psychicArouseLevel", PsychicDroneLevel.None, false);
		}

		// Token: 0x04003496 RID: 13462
		public Gender gender;

		// Token: 0x04003497 RID: 13463
		public PsychicDroneLevel level = PsychicDroneLevel.BadMedium;

		// Token: 0x04003498 RID: 13464
		public const float MaxPointsDroneLow = 800f;

		// Token: 0x04003499 RID: 13465
		public const float MaxPointsDroneMedium = 2000f;
	}
}
