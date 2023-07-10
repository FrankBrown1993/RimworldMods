using RimWorld;
using Verse;
using rjw;
using System.Linq;

namespace rjwex
{
	public class anal_plug_def : bondage_gear_def
	{
		public int plug_size = 0;   //min 1 - max 3; 0 for expandable plug
		//new public bool blocks_anus = false;    // only for lockable plug
	}

	public class anal_plug_soul : bondage_gear_soul
	{
		float[,] severity ={
			{ 0.6f, 0.6f, 0.6f, 0.6f, 0.4f, 0.4f },		//0 size = expandable plug
			{ 0.4f, 0.4f, 0.2f, 0.2f, 0.1f, 0.1f },		//1 size = S plug
			{ 0.4f, 0.6f, 0.4f, 0.2f, 0.1f, 0.1f },		//2 size = M plug
			{ 0.6f, 0.8f, 0.6f, 0.4f, 0.2f, 0.1f }		//3 size = L plug
		};
		//bionic,micro,tight,mid,loose,gaping/oversized

		//0   - loose;
		//0.2 - comfortable;
		//0.4 - tightly;
		//0.6 - very tight;
		//0.8 - cracking tight;

		public override void on_wear(Pawn wearer, Apparel gear)
		{
			anal_plug_def def = (anal_plug_def)gear.def;
			//Log.Message("PLUG On_Wear; size: " + def.plug_size);
			var anus = wearer.GetAnusList();
			if (!anus.NullOrEmpty())
			{
				var anusHD = wearer.health.hediffSet.hediffs.FirstOrDefault((Hediff hed) =>
							(hed is Hediff_PartBaseNatural || hed is Hediff_PartBaseArtifical) &&
							(hed.def.defName.ToLower().Contains("anus")));

				int aSize = 0;   //min 1-5 max, 0 for variable (bionic etc.)
				//if (wearer.health.hediffSet.HasHediff(Genital_Helper.archotech_anus) ||
				//   wearer.health.hediffSet.HasHediff(Genital_Helper.bionic_anus) ||
				//   wearer.health.hediffSet.HasHediff(Genital_Helper.hydraulic_anus))
				//{
				//    aSize = 0;
				//}

				if (anusHD != null)
				{
					BodyPartRecord bpr = anusHD.Part;
					switch (anusHD.CurStageIndex)
					{
						case 0:
							aSize = 1;
							break;
						case 1:
							aSize = 2;
							break;
						case 2:
							aSize = 3;
							break;
						case 3:
							aSize = 4;
							break;
						case 4:
							aSize = 5;
							break;
						case 5:
							aSize = 5;
							break;
						default:
							aSize = 0;
							break;
					}

					//if (wearer.apparel.AnyApparel)
					//{
					//	foreach (var a in wearer.apparel.WornApparel)
					//	{
					//		if (a != gear && a.def.thingClass.FullName.Contains("rjwex.anal_plug"))
					//		{
					//			wearer.apparel.TryDrop(a);
					//			break;
					//		}
					//	}
					//}

					float hediffSeverity = severity[def.plug_size, aSize];

					Hediff pluggedStatus = wearer.health.AddHediff(HediffDef.Named("Plugged"), bpr);
					pluggedStatus.Severity = hediffSeverity;
				}
			}

			var gear_stamp = gear.TryGetComp<CompHoloCryptoStamped>();
			if (gear_stamp != null)
			{
				var key = ThingMaker.MakeThing(ThingDef.Named("Holokey"));
				var key_stamp = key.TryGetComp<CompHoloCryptoStamped>();
				key_stamp.copy_stamp_from(gear_stamp);
				if (wearer.Map != null)
					GenSpawn.Spawn(key, wearer.Position, wearer.Map);
				else
					wearer.inventory.TryAddItemNotForSale(key);
			}
		}

		// Removes the gear's HediffDef
		public override void on_remove(Apparel gear, Pawn former_wearer)
		{
			var hed = former_wearer.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("Plugged"));
			if (hed != null)
				former_wearer.health.RemoveHediff(hed);
		}
	}

	public class anal_plug : bondage_gear { }

	/*
	//temporary solution for on_wer not being called on gear without hololock
	[HarmonyPatch]
	class TryDrop_Patch
	{
		static MethodBase TargetMethod()
		{
			MethodBase result = AccessTools.Method(typeof(Pawn_ApparelTracker), "TryDrop", new Type[] { typeof(Apparel), typeof(Apparel).MakeByRefType(), typeof(IntVec3), typeof(bool) });
			return result;
		}

		public static void Postfix(Apparel ap, bool __result, Pawn_ApparelTracker __instance)
		{
			if (!__result) return;
			if (ap.def is anal_plug_def def)
			{
				var tra = __instance;
				(def.soul as anal_plug_soul).on_remove(ap, tra.pawn);
			}
		}
	}
	*/

}