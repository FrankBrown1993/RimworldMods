using rjw;
using RJWSexperience.Ideology.HistoryEvents;
using System.Linq;
using Verse;

namespace RJWSexperience.Ideology
{
	internal class DebugAction
	{
		[DebugAction("RJW Sexperience Ideology", "Test marriage event", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void GenerateMarriageEvent(Pawn p)
		{
			Pawn hero = p.Map.PlayerPawnsForStoryteller.First(x => x.IsDesignatedHero());
			if (hero == null)
				return;
			RsiHistoryEventDefOf.RSI_NonIncestuosMarriage.RecordEventWithPartner(hero, p);
			RsiHistoryEventDefOf.RSI_NonIncestuosMarriage.RecordEventWithPartner(p, hero);
		}
	}
}
