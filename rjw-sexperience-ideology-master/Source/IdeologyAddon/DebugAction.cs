using rjw;
using RJWSexperience.Ideology.HistoryEvents;
using RJWSexperience.Ideology.Patches;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RJWSexperience.Ideology
{
	internal static class DebugAction
	{
		[DebugAction("RJW Sexperience Ideology", "Test marriage event", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void GenerateMarriageEvent(Pawn p)
		{
			Pawn hero = p.Map.PlayerPawnsForStoryteller.First(x => x.IsDesignatedHero());
			if (hero == null)
				return;
			RsiDefOf.HistoryEvent.RSI_NonIncestuosMarriage.RecordEventWithPartner(hero, p);
			RsiDefOf.HistoryEvent.RSI_NonIncestuosMarriage.RecordEventWithPartner(p, hero);
		}

		[DebugAction("RJW Sexperience Ideology", "Manual romance check", false, true, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void DisplayDebugTable()
		{
			IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawnsSpawned.Where(pawn => pawn.IsColonist);

			IEnumerable<TableDataGetter<Pawn>> columns = pawns
				.Select(pawn => new TableDataGetter<Pawn>(pawn.Name.ToStringShort, (Pawn p) => Rimworld_Patch_IncestuousManualRomance.RsiIncestuous(p, pawn)));

			var name = new TableDataGetter<Pawn>("Name", (Pawn pawn) => pawn.Name.ToStringShort);

			TableDataGetter<Pawn>[] getters = (new List<TableDataGetter<Pawn>>() { name }).Concat(columns).ToArray();

			DebugTables.MakeTablesDialog(pawns, getters);
		}
	}
}
