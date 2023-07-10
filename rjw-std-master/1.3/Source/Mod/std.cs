using System.Collections.Generic;
using System.Linq;
using Verse;
using rjw;

namespace rjwstd
{
	/// <summary>
	/// Common functions and constants relevant to STDs.
	/// </summary>
	public static class std
	{
		//all STDs
		public static List<std_def> all => DefDatabase<std_def>.AllDefsListForReading;

		public static Hediff get_infection(Pawn p, std_def sd)
		{
			return p.health.hediffSet.GetFirstHediffOfDef(sd.hediff_def);
		}

		public static BodyPartRecord GetRelevantBodyPartRecord(Pawn pawn, std_def std)
		{
			if (std.appliedOnFixedBodyParts == null)
			{
				return null;
			}

			BodyPartDef target = std.appliedOnFixedBodyParts.Single();
			return pawn?.RaceProps.body.GetPartsWithDef(target).Single();
			//return pawn?.RaceProps.body.GetPartsWithDef(std.appliedOnFixedBodyParts.Single()).Single();
		}

		public static bool IsImmune(Pawn pawn)
		{
			// Archotech genitalia automagically purge STDs.
			return pawn.health.hediffSet.HasHediff(Genital_Helper.archotech_vagina)
				|| pawn.health.hediffSet.HasHediff(Genital_Helper.archotech_penis)
				|| xxx.is_demon(pawn)
				|| xxx.is_slime(pawn)
				|| xxx.is_mechanoid(pawn);
		}
		public static bool PartsImmune(Pawn pawn, List<Hediff> list = null)
		{
			List<string> propslist;
			if (!list.NullOrEmpty())
				if (list.Any())
					foreach (var y in list)
					{
						PartProps.TryGetProps(y, out propslist);
						if (!propslist.NullOrEmpty())
							if (propslist.Contains("STDImmune"))
							{
								return true;
							}
					}
			return false;
		}
	}
}
