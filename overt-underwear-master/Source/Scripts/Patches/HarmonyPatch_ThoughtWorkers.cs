using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Overt_Underwear
{	
	[HarmonyPatch(typeof(ThoughtWorker_Precept_GroinChestHairOrFaceUncovered), "HasUncoveredGroinChestHairOrFace")]
	public static class HarmonyPatch_ThoughtWorker_Precept_GroinChestHairOrFaceUncovered
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			if (__result == false) return;

			Pawn pawn = p;

			if (pawn?.apparel == null)
			{ __result = false; return; }

			if (pawn.apparel.WornApparel.NullOrEmpty())
			{ __result = true; return; }

			bool fullHeadCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead));
			bool groinCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.GenitalsBPG));
			bool chestCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.ChestBPG));
			bool faceCovered = fullHeadCovered || pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Eyes));
			bool hairCovered = fullHeadCovered || pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead));

			__result = !(groinCovered && chestCovered && faceCovered && hairCovered);
		}
	}

	[HarmonyPatch(typeof(ThoughtWorker_Precept_GroinChestOrHairUncovered), "HasUncoveredGroinChestOrHair")]
	public static class HarmonyPatch_ThoughtWorker_Precept_GroinChestOrHairUncovered
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			if (__result == false) return;

			Pawn pawn = p;

			if (pawn?.apparel == null)
			{ __result = false; return; }

			if (pawn.apparel.WornApparel.NullOrEmpty())
			{ __result = true; return; }

			bool fullHeadCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead));
			bool groinCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.GenitalsBPG));
			bool chestCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.ChestBPG));
			bool hairCovered = fullHeadCovered || pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead));

			__result = !(groinCovered && chestCovered && hairCovered);
		}
	}

	[HarmonyPatch(typeof(ThoughtWorker_Precept_GroinOrChestUncovered), "HasUncoveredGroinOrChest")]
	public static class HarmonyPatch_ThoughtWorker_Precept_HasUncoveredGroinOrChest
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			if (__result == false) return;

			Pawn pawn = p;

			if (pawn?.apparel == null)
			{ __result = false; return; }

			if (pawn.apparel.WornApparel.NullOrEmpty())
			{ __result = true; return; }

			bool groinCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.GenitalsBPG));
			bool chestCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.ChestBPG));

			__result = !(groinCovered && chestCovered);
		}
	}

	[HarmonyPatch(typeof(ThoughtWorker_Precept_GroinUncovered), "HasUncoveredGroin")]
	public static class HarmonyPatch_ThoughtWorker_Precept_GroinUncovered
	{
		public static void Postfix(ref bool __result, Pawn p)
		{
			if (__result == false) return;

			Pawn pawn = p;

			if (pawn?.apparel == null)
			{ __result = false; return; }

			if (pawn.apparel.WornApparel.NullOrEmpty())
			{ __result = true; return; }

			bool groinCovered = pawn.apparel.WornApparel.Any(x => x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) || x.def.apparel.bodyPartGroups.Contains(ModBodyPartGroupDefOf.GenitalsBPG));

			__result = !groinCovered;
		}
	}
}
