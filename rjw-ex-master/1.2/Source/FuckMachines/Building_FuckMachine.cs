using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace rjwex
{
	public class Building_FuckMachine_Def : ThingDef
	{
		public bool forced = false;
		public bool automatic = false;
		public bool vertical = true;
		public float quality = 1;
		public bool powered = false;
		public float height = 0.12f;
		public float amplitude = 0.08f;
		public JobDriver_UseFM.Modes mode;
	}

	public class Building_FuckMachine : Building
	{
		public bool Private = false;
		public string lastRideInfo = "";
		public string currentRideInfo = "";
		public List<Pawn> owners
		{
			get
			{
				if (this.Private)
				{
					Room room = this.GetRoom(RegionType.Set_Passable);
					if (room != null)
					{
						return room.Owners.ToList<Pawn>();
					}
				}

				return new List<Pawn>();
			}
		}

		public bool IsOwner(Pawn pawn)
		{
			return this.owners.Contains(pawn);
		}
		public bool CanUse(Pawn pawn)
		{
			CompPowerTrader compPower = this.TryGetComp<CompPowerTrader>();
			if (compPower != null && !compPower.PowerOn)
			{
				return false;
			}
			return (this.owners.Contains(pawn) || (this.owners.Count == 0 && !this.Private));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.Private, "Private", false, false);
			Scribe_Values.Look<string>(ref this.lastRideInfo, "lastRideInfo", "", false);
			Scribe_Values.Look<string>(ref this.currentRideInfo, "currentRideInfo", "", false);
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{

			base.SpawnSetup(map, respawningAfterLoad);
			FMHelper.Register(this);
		}

		public override string GetInspectString()
		{
			//TODO status display, mode, progress, etc
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());

			if (stringBuilder.Length > 0) stringBuilder.AppendLine();

			stringBuilder.Append("Access: " + (this.Private ? "Private" : "Public"));
			stringBuilder.AppendLine();

			if (this.Private && this.owners.Count == 0)
			{
				stringBuilder.AppendLine("Owner".Translate() + ": " + "Nobody".Translate());
			}
			else if (this.Private && this.owners.Count == 1)
			{
				stringBuilder.AppendLine("Owner".Translate() + ": " + this.owners[0].Label);
			}
			else if (this.Private)
			{
				stringBuilder.Append("Owners".Translate() + ": ");
				bool flag = false;
				for (int i = 0; i < this.owners.Count; i++)
				{
					if (flag)
					{
						stringBuilder.Append(", ");
					}
					flag = true;
					stringBuilder.Append(this.owners[i].LabelShort);
				}
				stringBuilder.AppendLine();
			}

			if (!lastRideInfo.NullOrEmpty())
			{
				stringBuilder.Append(lastRideInfo);
				stringBuilder.AppendLine();
			}
			else if (!currentRideInfo.NullOrEmpty())
			{
				stringBuilder.Append(currentRideInfo);
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString().TrimEndNewlines();
		}

		public JobDef GetJobDef()
		{
			return DefDatabase<JobDef>.GetNamed("UseFM", true);
		}

		// TODO RMB menu?
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			if (selPawn.Faction == Faction.OfPlayer)
			{
				foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
				{
					FloatMenuOption fmo = floatMenuOption;
					yield return fmo;
					fmo = null;
				}
				if (!selPawn.CanReserve(this, 1, -1, null, false))
					yield return new FloatMenuOption("Reserved", null, MenuOptionPriority.Default, null, null, 0.0f, null, null);
				else if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, TraverseMode.ByPawn))
				{
					yield return new FloatMenuOption("No path", null, MenuOptionPriority.Default, null, null, 0.0f, null, null);
				}
				else if (!this.CanUse(selPawn))
				{
					yield return new FloatMenuOption("Can't use this", null, MenuOptionPriority.Default, null, null, 0.0f, null, null);
				}
				else
				{
					Action UseThisFM = (() =>
					{
						selPawn.drafter.Drafted = false;
						Job job = new Job(this.GetJobDef(), this);
						if (job == null)
							return;
						selPawn.jobs.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds);
					});

					yield return new FloatMenuOption("Use this machine", UseThisFM);
				}
			}
		}
	}
}

//template for vanilla ownership system
/*		:IAssignableBuilding
public List<Pawn> owners = new List<Pawn>();

public bool Unowned
{
	get { return owners.Count <= 0; }
}

public IEnumerable<Pawn> AssigningCandidates
{
	get
	{
		if (!Spawned)
		{
			return Enumerable.Empty<Pawn>();
		}
		return Map.mapPawns.FreeColonists;
	}
}

public IEnumerable<Pawn> AssignedPawns
{
	get
	{
		return owners;
	}
}

public int MaxAssignedPawnsCount
{
	get
	{
		return 5;
	}
}

private int OwnerInspectCount
{
	get
	{
		if (owners.Count > 3)
		{
			return 3;
		}
		return owners.Count;
	}
}

private bool PlayerCanSeeOwners
{
	get
	{
		if (Faction == Faction.OfPlayer)
		{
			return true;
		}
		for (int i = 0; i < this.owners.Count; i++)
		{
			if (owners[i].Faction == Faction.OfPlayer || owners[i].HostFaction == Faction.OfPlayer)
			{
				return true;
			}
		}
		return false;
	}
}


public void TryAssignPawn(Pawn pawn)
{
	if (!owners.Contains(pawn))
	{
		owners.Add(pawn);
	}
}


public void TryUnassignPawn(Pawn pawn)
{
	if (owners.Contains(pawn))
	{
		owners.Remove(pawn);
	}
}


public override void DeSpawn()
{
	RemoveAllOwners();
	base.DeSpawn();
}


public override void ExposeData()
{
	base.ExposeData();
	if (Scribe.mode == LoadSaveMode.Saving)
	{
		owners.RemoveAll((Pawn x) => x.Destroyed);
	}
	Scribe_Collections.Look(ref owners, "owners", LookMode.Reference);
	if (Scribe.mode == LoadSaveMode.PostLoadInit)
	{
		SortOwners();
	}
}


public override IEnumerable<Gizmo> GetGizmos()
{
	foreach (Gizmo g in base.GetGizmos())
	{
		yield return g;
	}
	yield return new Command_Action
	{
		defaultLabel = "CommandBedSetOwnerLabel".Translate(),
		icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
		defaultDesc = Static.CommandDesc_SetArcadeOwner,
		action = delegate {
			Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
		},
		hotKey = KeyBindingDefOf.Misc3
	};
}


public override string GetInspectString()
{
	StringBuilder stringBuilder = new StringBuilder();
	stringBuilder.Append(base.GetInspectString());
	stringBuilder.AppendLine();
	if (PlayerCanSeeOwners)
	{
		stringBuilder.AppendLine("ForColonistUse".Translate());
		if (owners.Count == 0)
		{
			stringBuilder.AppendLine("Owner".Translate() + ": " + "Nobody".Translate().ToLower());
		}
		else if (owners.Count == 1)
		{
			stringBuilder.AppendLine("Owner".Translate() + ": " + owners[0].Label);
		}
		else
		{
			stringBuilder.Append("Owners".Translate() + ": ");
			bool conjugate = false;
			for (int i = 0; i < OwnerInspectCount; i++)
			{
				if (conjugate)
				{
					stringBuilder.Append(", ");
				}
				conjugate = true;
				stringBuilder.Append(owners[i].LabelShort);
			}
			if (owners.Count > 3)
			{
				stringBuilder.Append($" (+ {owners.Count - 3})");
			}
			stringBuilder.AppendLine();
		}
	}
	return stringBuilder.ToString().TrimEndNewlines();
}


public void SortOwners()
{
	owners.SortBy((Pawn x) => x.thingIDNumber);
}


private void RemoveAllOwners()
{
	owners.Clear();
}
*/
