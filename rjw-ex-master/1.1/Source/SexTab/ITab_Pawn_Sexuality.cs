/*using RimWorld;
using System;
using UnityEngine;
using Verse;
using rjw;

namespace rjwex
{
	public class ITab_Pawn_Sexuality : ITab
	{
		// useful if you have criteria to show/hide this per Thing
		private Pawn _selected = null;
		public Pawn SelectedPawn
		{
			get
			{
				if (base.SelPawn != null)
				{
					return base.SelPawn;
				}
				Corpse corpse = base.SelThing as Corpse;
				if (corpse != null)
				{
					return corpse.InnerPawn;
				}
				return null;
			}
		}
		public override bool IsVisible
		{
			get
			{
				if (SelectedPawn != null && SelectedPawn.TryGetComp<CompRJW>()!=null) { return true; }
				else return false;
			}
		}

		public ITab_Pawn_Sexuality()
		{
			Vector2 offsets = new Vector2(17f, 17f);
			this.size = SexCardUtility.CardSize + offsets;
			this.labelKey = "rjwex_SexTab";  // used to get the Tab's label
		}


		protected override void FillTab()
		{
			Pawn pawnForHealth = this.SelectedPawn;
			if (pawnForHealth == null)
			{
				Log.Error("Health tab found no selected pawn to display.", false);
				return;
			}
			Rect rect = new Rect(17f, 17f, SexCardUtility.CardSize.x, SexCardUtility.CardSize.y);
			SexCardUtility.DrawCard(rect, SelectedPawn);
		}
	}
}*/