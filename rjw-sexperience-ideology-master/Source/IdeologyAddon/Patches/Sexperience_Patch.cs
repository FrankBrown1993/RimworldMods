using HarmonyLib;
using rjw;
using RJWSexperience.Ideology.HistoryEvents;
using Verse;

namespace RJWSexperience.Ideology.Patches
{
	[HarmonyPatch("RJWSexperience.RJWUtility", "ThrowVirginHistoryEvent")]
	public static class Sexperience_Patch_ThrowVirginHistoryEvent
	{
		public static bool Prepare() => ModsConfig.IsActive("rjw.sexperience");

		public static void Postfix(Pawn exVirgin, Pawn partner, SexProps props, int degree)
		{
			const int femaleAfterSurgery = 1;

			if (props.isRape && exVirgin == props.partner)
				RsiDefOf.HistoryEvent.RSI_VirginStolen.RecordEventWithPartner(exVirgin, partner);
			else if (degree != femaleAfterSurgery)
				RsiDefOf.HistoryEvent.RSI_VirginTaken.RecordEventWithPartner(exVirgin, partner);

			RsiDefOf.HistoryEvent.RSI_TookVirgin.RecordEventWithPartner(partner, exVirgin);
		}
	}
}
