using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjwex
{
	public static class FMHelper
	{
		public static List<ThingDef> registeredDefs = new List<ThingDef>();

		public static IEnumerable<Thing> ContainedFMs(this Room room)
		{
			List<Thing> things = room.ContainedAndAdjacentThings;
			for (int i = 0; i < things.Count; i++)
			{
				if (things[i] is Building_FuckMachine fm)
				{
					yield return (Thing)fm;
				}
			}
		}

		public static IEnumerable<Thing> GetOwnedFMs(this Pawn pawn)
		{
			Room room = pawn.GetRoom();
			if (room != null)
			{
				return room.ContainedFMs();
			}
			return null;
		}


		public static void Register(Building_FuckMachine fm)
		{
			ThingDef reg = fm.def;
			if (!registeredDefs.Contains(reg))
			{
				registeredDefs.Add(reg);
			}
		}

		public static List<Thing> GetFMs(Map map)
		{
			List<Thing> FMList = new List<Thing>();
			foreach (Building_FuckMachine_Def def in registeredDefs)
			{
				FMList.AddRange(map.listerThings.ThingsOfDef(def).AsEnumerable());
			}
			if (FMList.Count > 0) return FMList;
			return null;
		}

		public static Building_FuckMachine GetFMForPawn(Pawn pawn)
		{
			List<Thing> FMList = GetFMs(pawn.Map);
			if (FMList == null) return null;

			bool validator(Thing t)
			{
				if (!(t as Building_FuckMachine).CanUse(pawn))
					return false;
				if (!pawn.CanReserve(t, 1, -1, null, false) || t.IsForbidden(pawn) || !t.IsSociallyProper(pawn))
					return false;
				CompPowerTrader comp = t.TryGetComp<CompPowerTrader>();
				return (comp == null || comp.PowerOn);
			}

			Func<Thing, float> priorityGetter = (t =>
			{
				float p = 1f;
				p *= (t.def as Building_FuckMachine_Def).quality;

				if ((t as Building_FuckMachine).IsOwner(pawn))
					p *= 1.15f;

				if (xxx.is_masochist(pawn) && (t.def as Building_FuckMachine_Def).forced)
					p *= 1.5f;

				return p;
			});

			return (Building_FuckMachine)GenClosest.ClosestThing_Global_Reachable(pawn.Position,
				pawn.Map,
				FMList,
				PathEndMode.OnCell,
				TraverseParms.For(pawn, Danger.Some, TraverseMode.ByPawn, false),
				300f,
				validator,
				priorityGetter);
		}
	}
}