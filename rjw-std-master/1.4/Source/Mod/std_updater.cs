//using Multiplayer.API;
using RimWorld;
using Verse;
using rjw;

namespace rjwstd
{
	/// <summary>
	/// Responsible for handling the periodic effects of having an STD hediff.
	/// Not technically tied to the infection vector itself,
	/// but some of the STD effects are weird and complicated.
	/// </summary>
	public static class std_updater
	{
		public const float UpdatesPerDay = GenDate.TicksPerDay / 150f / 10f;

		public static void update(Pawn p)
		{
			// Check if any infections are below the autocure threshold and cure them if so
			foreach (std_def sd in std.all)
			{
				Hediff inf = std.get_infection(p, sd);
				if (inf != null && (inf.Severity < sd.autocure_below_severity || std.IsImmune(p)))
				{
					p.health.RemoveHediff(inf);
					if (sd.cohediff_def != null)
					{
						Hediff coinf = p.health.hediffSet.GetFirstHediffOfDef(sd.cohediff_def);
						if (coinf != null)
							p.health.RemoveHediff(coinf);
					}
				}
			}
		}

		/// <summary>
		/// For meanDays = 1.0, will return true on average once per day. For 2.0, will return true on average once every two days.
		/// </summary>
		//[SyncMethod]
		static bool RollFor(float meanDays)
		{
			return Rand.Chance(1.0f / meanDays / UpdatesPerDay);
		}
	}
}
