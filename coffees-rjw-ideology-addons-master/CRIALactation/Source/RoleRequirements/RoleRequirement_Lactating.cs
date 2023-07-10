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
    public class RoleRequirement_Lactating : RoleRequirement
    {

        public override string GetLabel(Precept_Role role)
        {
            return "must be lactating";
        }

        public override bool Met(Pawn p, Precept_Role role)
        {
            if(LactationUtility.IsLactating(p))
            {
                return true;
            }
            return false;
        }
    }
}
