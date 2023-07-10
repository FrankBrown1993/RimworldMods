using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Milk;

namespace CRIALactation
{
    public abstract class SpecialThingFilterWorker_MilkBase : SpecialThingFilterWorker
    {

        protected bool IsHumanMilk(ThingDef t) => t == ThingDefOf_Milk.HumanMilk || t == ThingDefOf_Milk.HumanoidMilk || t == ThingDefOf_Milk.HumanoidMilkBulk || t == ThingDefOf_Milk.HumanMilkBulk;

		protected bool IsHumanMilk(Thing t) => IsHumanMilk(t.def);

        public override bool CanEverMatch(ThingDef def)
        {
            return def.IsIngestible && def.IsProcessedFood;
        }

		protected bool IsFoodWithMilk(Thing food)
		{
			CompIngredients compIngredients = food.TryGetComp<CompIngredients>();

			if (compIngredients == null)
				return false;

			foreach (ThingDef ingredient in compIngredients.ingredients)
			{
				if (IsHumanMilk(ingredient))
					return true;
			}

			return false;
		}


	}
}
