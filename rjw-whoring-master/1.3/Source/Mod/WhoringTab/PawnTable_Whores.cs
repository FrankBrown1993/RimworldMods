﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using rjw;
using Verse;

namespace rjwwhoring.MainTab
{
	public class PawnTable_Whores : PawnTable_PlayerPawns
	{
		public PawnTable_Whores(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight) { }

		//default sorting
		protected override IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
		{
			//return input.OrderBy(p => p.Name);
			foreach (Pawn p in input)
				p.UpdatePermissions();
			return input.OrderByDescending(p => (p.IsPrisonerOfColony || p.IsSlaveOfColony) != false).ThenBy(p => xxx.get_pawnname(p));
			//return input.OrderByDescending(p => (p.IsPrisonerOfColony || p.IsSlaveOfColony) != false).ThenBy(p => (p.Name.ToStringShort.Colorize(Color.yellow)));
			//return input.OrderBy(p => xxx.get_pawnname(p));
		}

		protected override IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
		{
			foreach (Pawn p in input)
				p.UpdatePermissions();
			return input;
			//return base.PrimarySortFunction(input);
		}
	}
}