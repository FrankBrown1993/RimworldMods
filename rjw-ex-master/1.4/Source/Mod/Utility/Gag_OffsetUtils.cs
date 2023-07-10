using System.Linq;
using RimWorld;
using Verse;
using rjw;

namespace rjwex
{
	/// <summary>Extension to rjw.xxx functionality</summary>
	public static class xxxEx
	{
		public static BodyPartGroupDef GroupDefTeeth = DefDatabase<BodyPartGroupDef>.GetNamed("Teeth");
		public static void DrawNudeWithBondage(Pawn pawn, bool keep_hat_on = false)
		{
			if (!rjw.xxx.is_human(pawn)) return;
			if (pawn.Map != Find.CurrentMap) return;
			if (RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Clothed) return;

			pawn.Drawer.renderer.graphics.ClearCache();
			pawn.Drawer.renderer.graphics.apparelGraphics.Clear();

			bool hgEnabled = RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Headgear || keep_hat_on;

			foreach (Apparel current in pawn.apparel.WornApparel.Where(x =>
			hgEnabled && (x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead) || x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead)) //is apparel  headgear
			|| x is bondage_gear
			|| x.def.thingCategories.Any(y => y.defName.ToLower().ContainsAny("vibrator", "piercing", "strapon"))
			))
			{
				ApparelGraphicRecord item;
				//ApparelGraphicRecordGetter.TryGetGraphicApparel(current, pawn.story.bodyType, out item) by default, corrected to visible gags handler
				if (ResolveApparelGraphics_Patch.RJWExTryGetGraphicApparel(current, pawn.story.bodyType, out item))
				{
					pawn.Drawer.renderer.graphics.apparelGraphics.Add(item);
				}
			}
			GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
		}
	}
}
