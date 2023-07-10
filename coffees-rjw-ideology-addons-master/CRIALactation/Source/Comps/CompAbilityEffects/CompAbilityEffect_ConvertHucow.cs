using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using Milk;
using UnityEngine;

namespace CRIALactation
{
    public class CompAbilityEffect_ConvertHucow : CompAbilityEffect
    {

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            return pawn != null && AbilityUtility.ValidateMustBeHuman(pawn, throwMessages, this.parent) &&
                AbilityUtility.ValidateNoMentalState(pawn, throwMessages, this.parent) &&
                AbilityUtility.ValidateSameIdeo(this.parent.pawn, pawn, throwMessages, this.parent) &&
                LactationUtility.IsLactating(pawn) &&
                !LactationUtility.IsHucow(pawn);

        }


        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            LactationUtility.ExtendLactationDuration(pawn);

            //let's expand the breasts too!
            var breastList = pawn.GetBreastList();
            if (!breastList.NullOrEmpty())
                foreach (var breasts in breastList.Where(x => !x.TryGetComp<CompHediffBodyPart>().FluidType.NullOrEmpty()))
                {
                    breasts.Severity += LactationSettings.hucowBreastSizeBonus; //this could make some big ones rediculous.
                    if (breasts.Severity < LactationSettings.hucowBreastSizeMinimum) breasts.Severity = LactationSettings.hucowBreastSizeMinimum; 
                }
        }
    }
}
