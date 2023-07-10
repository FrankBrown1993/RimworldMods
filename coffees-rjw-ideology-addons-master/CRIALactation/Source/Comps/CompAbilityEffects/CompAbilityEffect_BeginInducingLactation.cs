using RimWorld;
using RimWorld.Planet;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    public class CompAbilityEffect_BeginInducingLactation : CompAbilityEffect
    {

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;


            if (pawn == null) return false;
            if (!(pawn.IsColonistPlayerControlled || pawn.IsSlaveOfColony || pawn.IsPrisonerOfColony)) return false;
            if (!Genital_Helper.has_breasts(pawn) || Genital_Helper.has_male_breasts(pawn)) return false;
            if (LactationUtility.IsLactating(pawn)) return false;

            return true;

        }

    }
}
