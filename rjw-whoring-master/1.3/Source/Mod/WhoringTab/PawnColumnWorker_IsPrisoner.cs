﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using rjw;
using UnityEngine;
using Verse;

namespace rjwwhoring.MainTab
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_IsPrisoner : PawnColumnWorker_Icon
	{
		private static readonly Texture2D comfortOn = ContentFinder<Texture2D>.Get("UI/Tab/ComfortPrisoner_on");
		private readonly Texture2D comfortOff = ContentFinder<Texture2D>.Get("UI/Tab/ComfortPrisoner_off");
		private readonly Texture2D comfortOff_nobg = ContentFinder<Texture2D>.Get("UI/Tab/ComfortPrisoner_off_nobg");

		protected override Texture2D GetIconFor(Pawn pawn)
		{
			return pawn.IsPrisonerOfColony ? comfortOff_nobg : null;
		}
		protected override string GetIconTip(Pawn pawn)
		{
			//string str = (pawn != null) ? pawn.guest.GetLabel() : null;
			//if (!str.NullOrEmpty())
			//{
			//	return str.CapitalizeFirst();
			//}
			//return null;
			return "BrothelTabIsPrisoner".Translate();
		}
	}
}