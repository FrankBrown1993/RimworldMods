using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CRIALactation
{
    [HarmonyPatch]
    public static class HarmonyPatch_FoodUtility
    {

        [HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
        [HarmonyPatch(typeof(FoodUtility), "AddThoughtsFromIdeo")]
        public static void AddThoughtsFromIdeo_Patch(HistoryEventDef eventDef, Pawn ingester, ThingDef foodDef, MeatSourceCategory meatSourceCategory)
        {
            //throw new NotImplementedException("thoughts from ideo wasn't implemented!");
        }

        [HarmonyPatch(typeof(FoodUtility), "ThoughtsFromIngesting")]
        public static void Postfix(ref List<FoodUtility.ThoughtFromIngesting> __result, ref List<FoodUtility.ThoughtFromIngesting> ___ingestThoughts,  Pawn ingester, Thing foodSource, ThingDef foodDef) 
        {
            /**
             * checks if food has milk or not
             */

            if (ingester.Ideo != null)
            {

                CompIngredients ingredients = foodSource.TryGetComp<CompIngredients>();
                if (foodDef == ThingDefOf_Milk.HumanMilk || foodDef == ThingDefOf_Milk.HumanoidMilk || foodDef == ThingDefOf_Milk.HumanMilkBulk || foodDef == ThingDefOf_Milk.HumanoidMilkBulk || foodDef.defName == "VCE_HumanoidCheese" || foodDef.defName == "cheese")
                {
                    AddThoughtsFromIdeo_Patch(HistoryEventDefOf_Milk.DrankMilkRaw, ingester, foodDef, FoodUtility.GetMeatSourceCategory(foodDef));

                    __result = ___ingestThoughts;

                }
                else if (ingredients == null
                    || !(ingredients.ingredients.Contains(ThingDefOf_Milk.HumanMilk) || (ingredients.ingredients.Contains(ThingDefOf_Milk.HumanoidMilk)) || (ingredients.ingredients.Contains(ThingDefOf_Milk.HumanMilkBulk)) || (ingredients.ingredients.Contains(ThingDefOf_Milk.HumanoidMilkBulk)))
                    && !LactationUtility.IsHucow(ingester)) {
                    AddThoughtsFromIdeo_Patch(HistoryEventDefOf_Milk.DrankNonMilkMeal, ingester, foodDef, FoodUtility.GetMeatSourceCategory(foodDef));
                    __result = ___ingestThoughts;
                }

                 
            }

        }

        [HarmonyPatch(typeof(FoodUtility), "AddIngestThoughtsFromIngredient")]
        public static void Postfix(ThingDef ingredient, Pawn ingester) {
           
            MeatSourceCategory meatSourceCategory = FoodUtility.GetMeatSourceCategory(ingredient);

            if (ingester.Ideo != null)
            {

                if (ingredient == ThingDefOf_Milk.HumanoidMilk || ingredient == ThingDefOf_Milk.HumanMilk || ingredient == ThingDefOf_Milk.HumanoidMilkBulk || ingredient == ThingDefOf_Milk.HumanMilkBulk || ingredient.defName == "VCE_HumanoidCheese" || ingredient.defName == "cheese")
                {
                    AddThoughtsFromIdeo_Patch(HistoryEventDefOf_Milk.DrankMilkMeal, ingester, ingredient, meatSourceCategory);
                }

            }

        }

        [HarmonyPatch(typeof(FoodUtility), "GenerateGoodIngredients")]
        public static void Postfix(Thing meal, Ideo ideo)
        {
            CompIngredients compIngredients = meal.TryGetComp<CompIngredients>();

            if(ideo.HasPrecept(PreceptDefOf_Lactation.Lactating_Essential) ||
                ideo.HasPrecept(PreceptDefOf_Lactation.Lactating_MandatoryHucow))
            {
                compIngredients.ingredients.Add(ThingDefOf_Milk.HumanMilk);
                compIngredients.ingredients.Add(ThingDefOf_Milk.HumanoidMilk);
                compIngredients.ingredients.Add(ThingDefOf_Milk.HumanMilkBulk);
                compIngredients.ingredients.Add(ThingDefOf_Milk.HumanoidMilkBulk);
            }
        }

    }
}