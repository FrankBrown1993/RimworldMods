using RimWorld;
using Verse;
using Multiplayer.API;

namespace rjw
{
	public class IncidentWorker_NymphVisitorGroupHard : IncidentWorker_NeutralGroup
	{

		private static readonly SimpleCurve PointsCurve = new SimpleCurve
		{
			new CurvePoint(45f, 0f),
			new CurvePoint(50f, 1f),
			new CurvePoint(100f, 1f),
			new CurvePoint(200f, 0.25f),
			new CurvePoint(300f, 0.1f),
			new CurvePoint(500f, 0f)
		};

		[SyncMethod]
		protected override void ResolveParmsPoints(IncidentParms parms)
		{
			if (!(parms.points >= 0f))
			{
				parms.points = Rand.ByCurve(PointsCurve);
			}
		}

		[SyncMethod]
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			//--Log.Message("IncidentWorker_NymphVisitorGroup::TryExecute() called");

			if (!RJWSettings.NymphRaidHard) return false;
			if (MP.IsInMultiplayer) return false;

			Map map = (Map)parms.target;

			if (map == null)
			{
				//--Log.Message("IncidentWorker_NymphJoins::TryExecute() - map is null, abort!");
				return false;
			}
			else
			{
				//--Log.Message("IncidentWorker_NymphJoins::TryExecute() - map is ok");
			}

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 loc, map, CellFinder.EdgeRoadChance_Friendly + 0.2f))
			{
				//--Log.Message("IncidentWorker_NymphJoins::TryExecute() - no entry, abort!");
				return false;
			}
			var count = map.mapPawns.AllPawnsSpawnedCount;
			//Log.Message("IncidentWorker_NymphJoins::TryExecute() -count:" + count);
			for (int i = 1; i <= count || i <= 1000; ++i)
			{
				Pawn pawn = Nymph_Generator.GenerateNymph(loc, ref map);
				//pawn.SetFaction(Faction.OfPlayer);
				GenSpawn.Spawn(pawn, loc, map);

				pawn.ChangeKind(PawnKindDefOf.WildMan);
				//if (pawn.Faction != null)
				//	pawn.SetFaction(null);
				if (RJWSettings.NymphPermanentManhunter)
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
				else
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
			}
			Find.LetterStack.ReceiveLetter("Nymphs!!!", "A huge group of nymphs has wandered into your settlement.", LetterDefOf.ThreatBig, null);

			return true;
		}
	}
}