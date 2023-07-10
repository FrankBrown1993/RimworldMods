using Verse;

namespace RJWSexperience.Ideology
{
	public class RsiSettings : ModSettings
	{
		public bool patchRomanceChanceFactor;
		public bool patchIncestuousManualRomance;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref patchRomanceChanceFactor, "patchSecondaryRomanceChanceFactor", true);
			Scribe_Values.Look(ref patchIncestuousManualRomance, "patchIncestuousManualRomance", true);
		}
	}
}
