﻿using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Precepts
{
	public class Comp_KnowsMemoryThought_Gendered : PreceptComp_KnowsMemoryThought
	{
		[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Field value loaded from XML")]
		public Gender doersGender;

		public override void Notify_MemberWitnessedAction(HistoryEvent ev, Precept precept, Pawn member)
		{
			if (ev.args.TryGetArg(HistoryEventArgsNames.Doer, out Pawn doer) && doer.gender == doersGender)
			{
				base.Notify_MemberWitnessedAction(ev, precept, member);
			}
		}
	}
}
