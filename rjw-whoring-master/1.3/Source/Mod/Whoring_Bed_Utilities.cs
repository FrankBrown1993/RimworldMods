using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using System;
using UnityEngine;
using rjw;

namespace rjwwhoring
{
	public static class WhoreBed_Utility
	{
		public static readonly RoomRoleDef roleDefBrothel = DefDatabase<RoomRoleDef>.GetNamed("Brothel");


		// find the best bed for a customer; whore is needed as parameter to only select beds that are reachable
		public static Building_Bed FindBed(Pawn whore, Pawn customer)
		{
			List<Building_Bed> b = FindReachableAndAvailableWhoreBeds(whore, customer);
			return GetBestBedForCustomer(customer, b);
		}
		public static bool CanUseForWhoring(Pawn pawn, Building_Bed bed)
		{
			bool flag = bed.IsAvailableForWhoring(pawn) && pawn.CanReserveAndReach(bed, PathEndMode.InteractionCell, Danger.Unspecified) && !bed.IsForbidden(pawn);
			return flag;
		}

		public static Building_Bed GetBestBedForCustomer(Pawn customer, List<Building_Bed> beds)
		{
			if (beds != null && beds.Any())
			{
				return beds.MaxBy(bed => CalculateBedScore(customer, bed));
			}
			else
			{
				return null;
			}
		}
		public static float GetCheapestBedFactor(Pawn whore, Pawn customer)
		{
			List<Building_Bed> beds = FindReachableAndAvailableWhoreBeds(whore, customer);
			return GetCheapestBedFactor(beds);
		}
		public static float GetCheapestBedFactor(List<Building_Bed> beds)
		{
			if (beds != null && beds.Any())
			{
				return CalculatePriceFactor(beds.MinBy(bed => CalculatePriceFactor(bed)));
			}
			else
			{
				return 0f;
			}
		}
		// unused
		/*public static float GetMostExpensiveBedFactor(List<Building_Bed> beds)
		{
			if (beds != null && beds.Any())
			{
				return CalculatePriceFactor(beds.MaxBy(bed => CalculatePriceFactor(bed)));
			}
			else
			{
				return 0f;
			}
		}//*/

		public static float CalculateRoomFactor(Room room, int num_humanlike_beds)
		{
			if (room == null || room.Role == RoomRoleDefOf.None || room.OutdoorsForWork)
				return 0.1f;

			float room_multiplier = 1f;

			if (room.Role == roleDefBrothel)
			{
				room_multiplier *= (float)Math.Pow(0.8, num_humanlike_beds - 1);
			}
			else // if(room.Role == RoomRoleDefOf.Barracks)
			{
				room_multiplier /= 2 * (num_humanlike_beds - 1) + 1;
			}

			int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
			//Room impressiveness factor
			//0 < scoreStageIndex < 10 (Last time checked)
			//3 is mediocore
			room_multiplier *= (float)(scoreStageIndex <= 3 ? .4f + scoreStageIndex * .2f : 1f + .3f * (scoreStageIndex - 3));

			return Mathf.Max(room_multiplier, 0);
		}

		public static float CalculateBedFactorsForRoom(Room room, Building_Bed except_this_bed = null)
		{
			float room_factor = 0.1f;

			if (room == null)
				return room_factor;

			// get eligible beds
			IEnumerable<Building_Bed> humanlike_beds = room.ContainedBeds.Where(b => b.def.building.bed_humanlike);
			int num_humanlike_beds = humanlike_beds.Count();
			if (num_humanlike_beds <= 0)
			{
				return room_factor;
			}
			IEnumerable<Building_Bed> whoring_beds = humanlike_beds.Where(b => b.IsAllowedForWhoringOwner());

			if (whoring_beds.Any())
			{
				// if beds exist, calculate room score
				room_factor = CalculateRoomFactor(room, num_humanlike_beds);

				// and update all beds
				foreach (Building_Bed b in whoring_beds)
				{
					// except the bed given as parameter (will be calculated in that bed's function)
					if (except_this_bed == null || b.thingIDNumber != except_this_bed.thingIDNumber)
						CalculatePriceFactor(b, room_factor);
				}
			}
			return room_factor;
		}

		public static void ResetTicksUntilUpdate(Room room)
		{
			IEnumerable<Building_Bed> whoring_beds = room.ContainedBeds.Where(b => b.IsAllowedForWhoringOwner());

			foreach (Building_Bed bed in whoring_beds)
			{
				// set all to 0
				//	if one is needed, it updates all the other beds
				//	if none is needed, it doesn't matter
				// only setting one bed to update has the risk that a different bed's value is required that doesn't trigger an update
				WhoringBase.DataStore.GetBedData(bed).scoreUpdateTickDelay = 0;
			}
		}

		public static float CalculatePriceFactor(Building_Bed bed, float room_multiplier = -1f)
		{

			BedData saved_bed_data = WhoringBase.DataStore.GetBedData(bed);

			// cache result (no need for saving): if no result available, calculate; otherwise save tick at which it has been calculated.
			// additional parameter "room_multiplier" to skip room analysis
			if ((room_multiplier == -1 || room_multiplier >= 0 && room_multiplier == saved_bed_data.roomScore)
				&& saved_bed_data.bedScore >= 0f
				&& saved_bed_data.lastScoreUpdateTick >
					GenTicks.TicksGame - saved_bed_data.scoreUpdateTickDelay)
			{
				if (room_multiplier >= 0 && saved_bed_data.scoreUpdateTickDelay < 720)
				{
					// if saved value is used due to unchanged room multiplier, increase recalc delay
					saved_bed_data.scoreUpdateTickDelay += 60 + Rand.Int % 10;
				}
				//Log.Message("[RJW] lastScoreUpdateTick: " + BukkakeBase.DataStore.GetBedData(bed).lastScoreUpdateTick.ToString() + " / TicksGame: "+ GenTicks.TicksGame.ToString());
				return saved_bed_data.bedScore;
			}

			if (room_multiplier < 0)
			{
				Room room = bed.Map != null && bed.Map.regionAndRoomUpdater.Enabled ? bed.GetRoom() : null;
				room_multiplier = CalculateBedFactorsForRoom(room, bed);
			}

			// uncomfortable beds reduce price, comfortable beds make customers pay a tip
			float comfort = bed.GetStatValue(StatDefOf.Comfort);
			float price_factor = room_multiplier * comfort;

			// delay recalculation if result is the same as before
			// Rand.Int % 10 flattens the spike over time if many beds are toggled at once
			if (price_factor == saved_bed_data.bedScore)
			{
				if (saved_bed_data.scoreUpdateTickDelay < 720)
				{
					// slowly increase recalculation delay to two seconds on speed 3
					saved_bed_data.scoreUpdateTickDelay += 60 + Rand.Int % 10;
				}
			}
			else
			{
				// reset recalculation delay
				saved_bed_data.scoreUpdateTickDelay = 60 + Rand.Int % 10;
			}

			// update bed data
			saved_bed_data.lastScoreUpdateTick = GenTicks.TicksGame;
			saved_bed_data.bedScore = price_factor;
			saved_bed_data.roomScore = room_multiplier;

			// this is quite spammy
			//if (RJWSettings.DebugWhoring)
			//	Log.Message("[RJW]CalculatePriceFactor for bed " + bed.thingIDNumber.ToString() + ": "
			//		+ "room_multiplier (num beds, impressiveness) ("+room_multiplier.ToString() +") * "
			//		+ "comfort (" + comfort.ToString() + ") = " + price_factor.ToString());

			return price_factor;
		}

		// customers would want the best bed
		// TODO: price as factor, rebalance
		public static float CalculateBedScore(Pawn customer, Building_Bed bed)
		{
			float basePriceFactor = CalculatePriceFactor(bed);

			// ascetic pawns want the least impressive room
			if (customer.story.traits.HasTrait(TraitDefOf.Ascetic))
			{
				float comfort = bed.GetStatValue(StatDefOf.Comfort);
				basePriceFactor = comfort * comfort / basePriceFactor; // inverse room effects - may be cheap, but should still be comfortable. ascetic isn't masochistic!
				if (WhoringBase.DebugWhoring)
					Log.Message("[RJW]CalculateBedScore - Customer is ascetic");
			}

			basePriceFactor *= 200; // make a larger number for better distance scaling (and random effect)

			// horny pawns are in a hurry and want a closer bed
			int distance = 0;
			if (xxx.is_hornyorfrustrated(customer))
			{
				distance = (int)bed.Position.DistanceTo(customer.Position);
				//if (RJWSettings.DebugWhoring)
				//	Log.Message("[RJW]CalculateBedScore - Pawn is horny - distance = "+distance.ToString());
			}

			int random_factor = Rand.Int % 100;
			float score = basePriceFactor - distance + random_factor;

			if (WhoringBase.DebugWhoring)
				Log.Message("[RJW]CalculateBedScore for bed " + bed.thingIDNumber.ToString() + ": "
					+ "score from price (" + basePriceFactor.ToString() + ") "
					+ "- distance (" + distance.ToString() + ") "
					+ "+ randomness (" + random_factor.ToString() + ") "
					+ "= " + score.ToString());

			return score;
		}

		public static List<Building_Bed> FindReachableAndAvailableWhoreBeds(Pawn whore, Pawn customer)
		{
			List<Building_Bed> wb = new List<Building_Bed>();

			wb = whore.MapHeld.GetWhoreBeds().Where(bed =>
				!bed.IsForbidden(whore) &&
				!bed.IsForbidden(customer) &&
				!bed.IsBurning() &&
				bed.WhoringIsAllowedForPawn(whore) &&
				bed.IsAvailableForWhoring(whore) &&
				whore.CanReserveAndReach(bed, PathEndMode.OnCell, Danger.Unspecified) &&
				customer.CanReach(bed, PathEndMode.OnCell, Danger.Some)
				// TODO: price/affordable?
				).ToList();
			if (WhoringBase.DebugWhoring)
				Log.Message("[RJW]FindReachableAndAvailableWhoreBeds - found " + wb.Count().ToString() + " beds");
			return wb;
		}

		public static IEnumerable<Building_Bed> GetWhoreBeds(this Map map, Area area = null)
		{
			if (map == null) return new Building_Bed[0];
			if (area == null) return map.listerBuildings.AllBuildingsColonistOfClass<Building_Bed>();
			return map.listerBuildings.AllBuildingsColonistOfClass<Building_Bed>().Where(b => area[b.Position]);
		}

		public static bool WhoringIsAllowedForPawn(this Building_Bed bed, Pawn pawn)
		{
			if (bed.IsAllowedForWhoringAll())
				return true;
			if (bed == pawn.ownership.OwnedBed && bed.IsAllowedForWhoringOwner())
				return true;
			return false;
		}

		public static void SetAllowedForWhoringOwner(this Building_Bed bed, bool isAllowed)
		{
			if (!isAllowed)
			{
				// if whoring is disallowed for owner, also disallow for all
				bed.SetAllowedForWhoringAll(false);
			}
			WhoringBase.DataStore.GetBedData(bed).allowedForWhoringOwner = isAllowed;
		}
		public static void ToggleAllowedForWhoringOwner(this Building_Bed bed)
		{
			bed.SetAllowedForWhoringOwner(!WhoringBase.DataStore.GetBedData(bed).allowedForWhoringOwner);
		}
		public static bool IsAllowedForWhoringOwner(this Building_Bed bed)
		{
			if (!bed.def.building.bed_humanlike || bed.Faction != Faction.OfPlayerSilentFail || bed.Medical || bed.def.defName.Contains("Guest"))
			{
				return false;
			}
			if (bed.ForPrisoners)
			{
				// no toggle on prisoner beds, they may always use their own bed (if they are supposed to whore, anyway)
				return true;
			}
			return WhoringBase.DataStore.GetBedData(bed).allowedForWhoringOwner;
		}

		public static void SetAllowedForWhoringAll(this Building_Bed bed, bool isAllowed)
		{
			if (isAllowed)
			{
				// if whoring is allowed for all, also visibly allow for owner
				bed.SetAllowedForWhoringOwner(true);
				// if bed is designated for whoring, disable prisoner/medical use
				bed.ForPrisoners = false;
				bed.Medical = false;
			}
			WhoringBase.DataStore.GetBedData(bed).allowedForWhoringAll = isAllowed;

			bed.GetRoom()?.Notify_BedTypeChanged();
			bed.Notify_ColorChanged();
		}
		public static void ToggleAllowedForWhoringAll(this Building_Bed bed)
		{
			bed.SetAllowedForWhoringAll(!WhoringBase.DataStore.GetBedData(bed).allowedForWhoringAll);
		}
		public static bool IsAllowedForWhoringAll(this Building_Bed bed)
		{
			if (WhoringBase.DataStore.GetBedData(bed).allowedForWhoringAll)
			{
				if (!bed.def.building.bed_humanlike || bed.Faction != Faction.OfPlayerSilentFail || bed.Medical || bed.ForPrisoners || bed.def.defName.Contains("Guest") || bed.GetRoom()?.IsPrisonCell == true)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static void ReserveForWhoring(this Building_Bed bed, Pawn p, int num_ticks)
		{
			WhoringBase.DataStore.GetBedData(bed).reservedUntilGameTick = GenTicks.TicksGame + num_ticks;
			WhoringBase.DataStore.GetBedData(bed).reservedForPawnID = p.thingIDNumber;
		}
		public static void UnreserveForWhoring(this Building_Bed bed)
		{
			WhoringBase.DataStore.GetBedData(bed).reservedUntilGameTick = 0;
			WhoringBase.DataStore.GetBedData(bed).reservedForPawnID = 0;
		}
		public static bool IsAvailableForWhoring(this Building_Bed bed, Pawn p)
		{
			// check for active reservation
			if (WhoringBase.DataStore.GetBedData(bed).reservedUntilGameTick > GenTicks.TicksGame)
			{
				if (WhoringBase.DataStore.GetBedData(bed).reservedForPawnID != p.thingIDNumber)
				{
					// a different pawn has reserved this bed
					return false;
				}
			}

			if (bed.OwnersForReading.Any())
			{
				for (int i = 0; i < bed.OwnersForReading.Count; i++)
				{
					if (bed.OwnersForReading[i].InBed() && bed.OwnersForReading[i].CurrentBed() == bed)
					{
						// someone sleeping in this bed
						return false;
					}
				}
			}

			return true;
		}
	}
}