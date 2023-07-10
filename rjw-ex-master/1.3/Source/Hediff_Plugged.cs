using System.Linq;
using Verse;
using rjw;

namespace rjwex
{
	public class Plugged : HediffWithComps
	{
		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(6000))
			{
				//var apparel = pawn.apparel.WornApparel.Where(x => x.def.defName.Contains("Plug"));
				//var isExpandable = apparel.Any(x => x.def.defName.Contains("Expandable"));
				//var isLarge = apparel.Any(x => x.def.defName.Contains("Large"));
				//var isMedium = apparel.Any(x => x.def.defName.Contains("Medium"));
				//var isSmall = apparel.Any(x => x.def.defName.Contains("Small"));
				//Log.Message("Plug CurStageIndex:" + this.CurStageIndex + " CurStage.label: " + this.CurStage.label);
				int index = this.CurStageIndex;
				if (index > 1) //1 - comfortable
				{
					Hediff anusHD = getAnus();
					if (anusHD != null)
					{
						var diff = 0.01f * index;
						anusHD.Severity += diff;
						this.Severity -= diff;
					}
				}
			}
		}

		public Hediff getAnus()
		{
			Hediff anusHD = null;
			var bpr = pawn.GetAnusList();
			if (!bpr.NullOrEmpty())
			{
				anusHD = bpr.FirstOrDefault((Hediff hed) =>
							(hed is Hediff_PartBaseNatural));
				//mult anuses?
				//anusHD = bpr.Where((Hediff hed) =>
				//			(hed is Hediff_PartBaseNatural)).RandomElement();
			}
			return anusHD;
		}
	}
}
