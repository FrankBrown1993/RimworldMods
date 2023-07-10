using System;
using RimWorld;
using Verse;
using rjw;

namespace Overt_Underwear
{
	public class ThoughtWorker_Underwear : ThoughtWorker
	{
		public static ThoughtState CurrentThoughtState(Pawn pawn)
		{
			if (xxx.has_quirk(pawn, "Exhibitionist") || pawn?.ideo?.Ideo.HasPrecept(ModPreceptDefOf.Exhibitionism_Approved) == true) return ThoughtState.ActiveAtStage(3); // Joy
			if (pawn?.ideo?.Ideo.HasPrecept(ModPreceptDefOf.Underwear_CanBeVisible) == true) return ThoughtState.ActiveAtStage(2); // Indifference		

			return ThoughtState.ActiveAtStage(1); // Shame
		}

		protected override ThoughtState CurrentStateInternal(Pawn pawn)
		{
			if (pawn?.apparel?.WornApparel == null || pawn.apparel.WornApparel.NullOrEmpty()) return false;

			bool wearingUnderwear = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.GenitalsBPG) && x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) == false);
			bool wearingUnderwearTop = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.ChestBPG) && x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) == false);

			if (pawn?.ideo?.Ideo.HasPrecept(ModPreceptDefOf.Underwear_Disapproved) == true && (wearingUnderwear || wearingUnderwearTop)) return ThoughtState.ActiveAtStage(0);
			if (wearingUnderwear && pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs)) == false) return CurrentThoughtState(pawn);
			if (wearingUnderwearTop && pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso)) == false) return CurrentThoughtState(pawn); 

			return ThoughtState.Inactive;
		}
	}
}
