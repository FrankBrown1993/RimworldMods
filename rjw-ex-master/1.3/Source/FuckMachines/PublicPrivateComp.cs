using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Verse;
using Multiplayer.API;

namespace rjwex
{
	public class PublicPrivateComp : ThingComp
	{
		public bool Private
		{
			get
			{
				return (this.parent as Building_FuckMachine).Private;
			}
			set
			{
				if (value == (this.parent as Building_FuckMachine).Private)
				{
					return;
				}
				(this.parent as Building_FuckMachine).Private = value;
			}
		}

		/*
		public override void PostExposeData()
		{
			Scribe_Values.Look<bool>(ref this.privateInt, "private", false, false);
		}
		*/

		[DebuggerHidden]
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo c in base.CompGetGizmosExtra())
			{
				yield return c;
			}

			if (!(this.parent is Building_FuckMachine) || this.parent.Faction == Faction.OfPlayer)
			{
				Command_Action com = new Command_Action();
				
				com.defaultLabel = "Switch";
				if ((this.parent as Building_FuckMachine).Private)
				{
					com.defaultDesc = "Change to Public";
					com.icon = ContentFinder<Texture2D>.Get("UI/private", true);
				}
				else
				{
					com.defaultDesc = "Change to Private";
					com.icon = ContentFinder<Texture2D>.Get("UI/public", true);
				}
				com.action = delegate
				{
					ChangeMode();
				};
				yield return com;
			}
		}

		[SyncMethod]
		public void ChangeMode()
		{
			//Log.Message("Submit button is pressed for " + pawn);
			this.Private = !this.Private;
		}
	}

	public class CompProperties_PublicPrivateComp : CompProperties
	{
		public CompProperties_PublicPrivateComp()
		{
			compClass = typeof(PublicPrivateComp);
		}
	}
}