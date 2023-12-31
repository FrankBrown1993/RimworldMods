using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace rjw
{
	/// <summary>
	/// Calculates a pawn's fertility based on its age and fertility sources
	/// </summary>
	public class PawnCapacityWorker_Fertility : PawnCapacityWorker
	{
		public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
		{
			Pawn pawn = diffSet.pawn;
			var parts = pawn.GetGenitalsList();

			if (!Genital_Helper.has_penis_fertile(pawn, parts) && !Genital_Helper.has_vagina(pawn, parts))
				return 0;

			if (Genital_Helper.has_ovipositorF(pawn, parts) || Genital_Helper.has_ovipositorM(pawn, parts))
				return 0;

			//ModLog.Message("PawnCapacityWorker_Fertility::CalculateCapacityLevel is called for: " + xxx.get_pawnname(pawn));
			RaceProperties race = diffSet.pawn.RaceProps;

			if (!pawn.RaceHasFertility())
			{
				//Log.Message(" Fertility_filter, no fertility for : " + pawn.kindDef.defName);
				return 0f;
			}

			//androids only fertile with archotech parts
			if (AndroidsCompatibility.IsAndroid(pawn) && !(AndroidsCompatibility.AndroidPenisFertility(pawn) || AndroidsCompatibility.AndroidVaginaFertility(pawn)))
			{
				//Log.Message(" Android has no archotech genitals set fertility to 0 for: " + pawn.kindDef.defName);
				return 0f;
			}

			float startAge = 0f;	//raise fertility
			float startMaxAge = 0f;	//max fertility
			float endAge = race.lifeExpectancy * (RJWPregnancySettings.fertility_endage_male * 0.7f); // Age when males start to lose potency.
			float zeroFertility = race.lifeExpectancy * RJWPregnancySettings.fertility_endage_male;	// Age when fertility hits 0%.

			if (xxx.is_female(pawn))
			{
				if (xxx.is_animal(pawn))
				{
					endAge = race.lifeExpectancy * (RJWPregnancySettings.fertility_endage_female_animal * 0.6f);
					zeroFertility = race.lifeExpectancy * RJWPregnancySettings.fertility_endage_female_animal;
				}
				else
				{
					endAge = race.lifeExpectancy * (RJWPregnancySettings.fertility_endage_female_humanlike * 0.6f); // Age when fertility begins to drop.
					zeroFertility = race.lifeExpectancy * RJWPregnancySettings.fertility_endage_female_humanlike; // Age when fertility hits 0%.
				}
			}
			//If pawn is an animal, first reproductive lifestage is (usually) adult, so set startMaxAge at first reproductive stage, and startAge at stage before that. 
			int adult;
			if (xxx.is_animal(pawn) && (adult = race.lifeStageAges.FirstIndexOf((ls) => ls.def.reproductive)) > 0)
			{
				startAge = race.lifeStageAges[adult - 1].minAge;
				startMaxAge = race.lifeStageAges[adult].minAge;
			}
			else
			{
				foreach (LifeStageAge lifestage in race.lifeStageAges)
				{
					if (lifestage.def.reproductive)
						//presumably teen stage
						if (startAge == 0f && startMaxAge == 0f)
						{
							startAge = lifestage.minAge;
							startMaxAge = (Mathf.Max(startAge + (startAge + endAge) * 0.08f, startAge));
						}
						//presumably adult stage
						else
						{
							if (startMaxAge > lifestage.minAge) // ensure peak fertility stays at start or a bit before adult stage
								startMaxAge = lifestage.minAge;
						}
				}
			}
			//Log.Message(" Fertility ages for " + pawn.Name + " are: " + startAge + ", " + startMaxAge + ", " + endAge + ", " + endMaxAge);

			float result = PawnCapacityUtility.CalculateTagEfficiency(diffSet, BodyPartTagDefOf.RJW_Fertility, 1f, FloatRange.ZeroToOne, impactors);
			result *= GenMath.FlatHill(startAge, startMaxAge, endAge, zeroFertility, pawn.ageTracker.AgeBiologicalYearsFloat);

			//ModLog.Message("PawnCapacityWorker_Fertility::CalculateCapacityLevel result is: " + result);
			return result;

		}

		public override bool CanHaveCapacity(BodyDef body)
		{
			return body.HasPartWithTag(BodyPartTagDefOf.RJW_Fertility);
		}
	}
}
 