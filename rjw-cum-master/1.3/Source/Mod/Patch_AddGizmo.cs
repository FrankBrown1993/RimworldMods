using System.Collections.Generic;
using Verse;
using HarmonyLib;
using rjw;
//using Multiplayer.API;

namespace rjwcum
{
	//adds new gizmo for adding cum for testing
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	class Patch_AddGizmo
	{
		[HarmonyPriority(99),HarmonyPostfix]
		static IEnumerable<Gizmo> AddCum_test(IEnumerable<Gizmo> __result, Pawn __instance)
		{

			foreach (Gizmo entry in __result)
			{
				yield return entry;
			}

			if (Prefs.DevMode )//&& RJWSettings.DevMode && !MP.IsInMultiplayer)
			{
				Command_Action addCum = new Command_Action();
				addCum.defaultDesc = "AddCumHediff";
				addCum.defaultLabel = "AddCum";
				addCum.action = delegate ()
				{
					AddCum(__instance);
				};

				yield return addCum; 
			}
		}

		//[SyncMethod]
		static void AddCum(Pawn pawn)
		{
			//Log.Message("add cum button is pressed for " + pawn);

			if (!pawn.Dead && pawn.records != null)
			{
				//get all acceptable body parts:
				IEnumerable<BodyPartRecord> filteredParts = CumHelper.getAvailableBodyParts(pawn);

				//select random part:
				BodyPartRecord randomPart;
				//filteredParts.TryRandomElement<BodyPartRecord>(out randomPart);
				//for testing - choose either genitals or anus:
				//Rand.PopState();
				//Rand.PushState(RJW_Multiplayer.PredictableSeed());
				if (Rand.Value > 0.5f)
				{
					randomPart = pawn.RaceProps.body.AllParts.Find(x => x.def == xxx.anusDef);
				}
				else
				{
					randomPart = pawn.RaceProps.body.AllParts.Find(x => x.def == xxx.genitalsDef);
				}

				if (randomPart != null)
				{
					CumHelper.cumOn(pawn, randomPart, 0.2f, null, CumHelper.CUM_NORMAL);
				}
			};
		}
	}
}
