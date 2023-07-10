using System.Linq;
using Verse;
using rjw;

namespace rjwex
{
	public class ChastityCage : HediffWithComps
	{
		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(60000))
			{
				var apparel = pawn.apparel.WornApparel.Where(x => x.def.defName.Contains("ChastityCage"));
				var isSmall = apparel.Any(x => x.def.defName.Contains("Small"));
				var isMicro = apparel.Any(x => x.def.defName.Contains("Micro"));
				if (isSmall || isMicro)
				{
					Hediff penisHD = getPenis();
					if (penisHD != null)
					{
						if (isMicro && penisHD.Severity > 0.05f)
							penisHD.Severity -= 0.01f;
						else if (isSmall && penisHD.Severity > 0.25f)
							penisHD.Severity -= 0.01f;
					}
				}
			}
		}

		public Hediff getPenis()
		{
			Hediff penisHD = null;
			var bpr = pawn.GetGenitalsList();
			if (!bpr.NullOrEmpty())
			{
				penisHD = bpr.FirstOrDefault((Hediff hed) =>
							(hed is Hediff_PartBaseNatural) &&
							(hed.def.defName.ToLower().Contains("penis")));
			}
			return penisHD;
		}
	}
}
