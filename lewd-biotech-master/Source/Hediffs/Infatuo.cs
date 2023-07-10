// FOR ANYONE (THAT ISN"T ME (BOEUF)) THAT CARES
// =====================
// This is an idea for an implant that I had that basically forces a pawn to romantically obsess about another pawn, but I'm taking a break from working on it
// because I'm too smoothbrain to work out relation defs and making custom relations, and the multibirth stuff was taking up my time as is
// If you want to mess with it, this code is here for shits and gigs - modify it, throw it out, make something new, idgaf

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LewdBiotech.Hediffs
{
    class Infatuo
    {
		public class PawnRelationWorker_InfatuoInfatuated : PawnRelationWorker
		{
			// BaseGenerationChanceFactor - should NEVER autogenerate on pawns (for now, anyway)
			new public float BaseGenerationChanceFactor(Pawn generated, Pawn other, PawnGenerationRequest request)
			{
				return 0.0f;
			}

			// CreateRelation - shouldn't be needed for now since we're not autogenerating infatuo relations, but may be used in future
			public override void CreateRelation(Pawn generated, Pawn other, ref PawnGenerationRequest request)
			{
				return;
			}

			// GenerationChance - should NEVER autogenerate on pawns (for now, anyway)
			public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
			{
				return 0.0f;
			}

			public override bool InRelation(Pawn me, Pawn other)
			{
				return (me.health.hediffSet.GetFirstHediff<Hediff_Infatuo>().target == other);
			}
		}

		public class Hediff_Infatuo : Hediff
		{
			public Hediff_Infatuo(Pawn intTarget)
			{
				target = intTarget;
			}
			public Pawn target;
		}

		public class Hediff_InfatuoCalibrating : Hediff
		{

		}
	}
}*/
