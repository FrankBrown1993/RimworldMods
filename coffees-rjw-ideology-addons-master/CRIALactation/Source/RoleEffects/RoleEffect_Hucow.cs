using Milk;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CRIALactation
{
    public class RoleEffect_Hucow : RoleEffect
    {
        public override string Label(Pawn pawn, Precept_Role role)
        {
            return "Greatly increased milk yield, permenant lactation, slower speed";
        }

        public RoleEffect_Hucow()
        {

        }
    }
}
