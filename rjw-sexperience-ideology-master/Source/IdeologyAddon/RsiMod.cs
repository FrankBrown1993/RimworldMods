using UnityEngine;
using Verse;

namespace RJWSexperience.Ideology
{
	public class RsiMod : Mod
	{
		public static RsiSettings Prefs { get; private set; }

		public RsiMod(ModContentPack content) : base(content)
		{
			Prefs = GetSettings<RsiSettings>();
		}

		public override string SettingsCategory() => Keyed.ModTitle;

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listmain = new Listing_Standard();
			listmain.Begin(inRect);
			listmain.CheckboxLabeled(Keyed.PatchRomanceChanceFactor, ref Prefs.patchRomanceChanceFactor, Keyed.PatchRomanceChanceFactorTip);
			listmain.CheckboxLabeled(Keyed.PatchIncestuousManualRomance, ref Prefs.patchIncestuousManualRomance, Keyed.PatchIncestuousManualRomanceTip);
			listmain.End();
		}
	}
}
