﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RJWSexperience.Ideology
{
	public class LordJob_Ritual_Gangbang : LordJob_Ritual
	{
		public LordJob_Ritual_Gangbang() { }

		public LordJob_Ritual_Gangbang(string targetID, TargetInfo selectedTarget, Precept_Ritual ritual, RitualObligation obligation, List<RitualStage> allStages, RitualRoleAssignments assignments, Pawn organizer = null) : base(selectedTarget, ritual, obligation, allStages, assignments, organizer)
		{
			foreach (RitualRole ritualRole in assignments.AllRolesForReading)
			{
				if (ritualRole != null && ritualRole.id.Contains(targetID))
				{
					Pawn item = assignments.FirstAssignedPawn(ritualRole);
					pawnsDeathIgnored.Add(item);
				}
			}
		}
	}
}
