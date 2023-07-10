using rjw;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjwex
{
	class ThinkNode_ChancePerHour_UsingFM : ThinkNode_ChancePerHour
	{
		public static float get_fappin_mtb_hours(Pawn p)
		{
			if (DebugSettings.alwaysDoLovin)
				return 0.1f;

			return (xxx.is_nympho(p) ? 0.5f : 1.0f) * rjw_CORE_EXPOSED.LovePartnerRelationUtility.LovinMtbSinglePawnFactor(p); //todo look into this
		}

		protected override float MtbHours(Pawn p)
		{
			if (xxx.is_animal(p))
				return -1.0f;

			bool is_horny = xxx.need_some_sex(p) > 1f;
			if (is_horny)
			{
				bool isAlone = !p.Map.mapPawns.AllPawnsSpawned.Any(x => p.CanSee(x) && xxx.is_human(x));
				float clothingFactor = p.apparel.PsychologicallyNude ? 0.8f : 1.0f;

				float SexNeedFactor = (4 - xxx.need_some_sex(p)) / 2f;
				return get_fappin_mtb_hours(p) * SexNeedFactor * clothingFactor;
			}

			return -1.0f;
		}
	}
}
