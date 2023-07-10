using Verse;
using HarmonyLib;
using rjw;
using System;

namespace rjwstd
{
	///<summary>
	///roll for STD with unprotected rjw sex
	///</summary>
	[HarmonyPatch(typeof(SexUtility), "Aftersex")]
	[StaticConstructorOnStartup]
	static class Aftersex_STD_Apply
	{
		[HarmonyPostfix]
		private static void Aftersex_STD_Patch(SexProps props)
		{
			try
			{
				//TODO: add/test a roll_to_catch_from_corpse to std
				if (!props.usedCondom)
					if (!props.pawn.Dead && !props.partner.Dead)
					{
						//TODO: animal env probably always dirty, so skip, maybe add/test someday human-animal transfer
						if (!(xxx.is_animal(props.pawn) || xxx.is_animal(props.partner)))
						{
							std_spreader.roll_to_catch(props, props.pawn, props.partner);
							std_spreader.roll_to_catch(props, props.partner, props.pawn);
						}
					}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}
	}
}
