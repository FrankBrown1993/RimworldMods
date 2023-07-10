using RimWorld;
using Verse;
using System;
using System.Linq;
using System.Collections.Generic;
using Multiplayer.API;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Extensions;

///RimWorldChildren pregnancy:
//using RimWorldChildren;

namespace rjw
{
	/// <summary>
	/// This handles pregnancy chosing between different types of pregnancy awailable to it
	/// 1a:RimWorldChildren pregnancy for humanlikes
	/// 1b:RJW pregnancy for humanlikes
	/// 2:RJW pregnancy for bestiality
	/// 3:RJW pregnancy for insects
	/// 4:RJW pregnancy for mechanoids
	/// </summary>

	public static class PregnancyHelper
	{
		//called by aftersex (including rape, breed, etc)
		//called by mcevent

		//pawn - "father"; partner = mother

		//TODO: this needs rewrite to account reciever group sex (props?)
		public static void impregnate(SexProps props)
		{

			if (RJWSettings.DevMode) ModLog.Message("Rimjobworld::impregnate(" + props.sexType + "):: " + xxx.get_pawnname(props.pawn) + " + " + xxx.get_pawnname(props.partner) + ":");

			//"mech" pregnancy
			if (props.sexType == xxx.rjwSextype.MechImplant)
			{
				if (RJWPregnancySettings.mechanoid_pregnancy_enabled && xxx.is_mechanoid(props.pawn))
				{
					// removing old pregnancies
					var p = GetPregnancies(props.partner);
					if (!p.NullOrEmpty())
					{
						var i = p.Count;
						while (i > 0)
						{
							i -= 1;
							var h = GetPregnancies(props.partner);
							if (h[i] is Hediff_MechanoidPregnancy)
							{
								if (RJWSettings.DevMode) ModLog.Message(" already pregnant by mechanoid");
							}
							else if (h[i] is Hediff_BasePregnancy)
							{
								if (RJWSettings.DevMode) ModLog.Message(" removing rjw normal pregnancy");
								(h[i] as Hediff_BasePregnancy).Kill();
							}
							else
							{
								if (RJWSettings.DevMode) ModLog.Message(" removing vanilla or other mod pregnancy");
								props.partner.health.RemoveHediff(h[i]);
							}
						}
					}

					// new pregnancy
					if (RJWSettings.DevMode) ModLog.Message(" mechanoid pregnancy started");
					Hediff_MechanoidPregnancy hediff = Hediff_BasePregnancy.Create<Hediff_MechanoidPregnancy>(props.partner, props.pawn);
					return;

					/*
					// Not an actual pregnancy. This implants mechanoid tech into the target.
					//may lead to pregnancy
					//old "chip pregnancies", maybe integrate them somehow?
					//Rand.PopState();
					//Rand.PushState(RJW_Multiplayer.PredictableSeed());
					HediffDef_MechImplants egg = (from x in DefDatabase<HediffDef_MechImplants>.AllDefs	select x).RandomElement();
					if (egg != null)
					{
						if (RJWSettings.DevMode) Log.Message(" planting MechImplants:" + egg.ToString());
						PlantSomething(egg, partner, !Genital_Helper.has_vagina(partner), 1);
						return;
					}
					else
					{
						if (RJWSettings.DevMode) Log.Message(" no mech implant found");
					}*/
				}
				return;
			}

			//"ovi" pregnancy/egglaying
			var AnalOk = props.sexType == xxx.rjwSextype.Anal && RJWPregnancySettings.insect_anal_pregnancy_enabled;
			var OralOk = props.sexType == xxx.rjwSextype.Oral && RJWPregnancySettings.insect_oral_pregnancy_enabled;
			// Sextype can result in pregnancy.
			if (!(props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration ||
				AnalOk || OralOk))
				return;

			Pawn giver = props.pawn; // orgasmer
			Pawn reciever = props.partner;
			List<Hediff> pawnparts = giver.GetGenitalsList();
			List<Hediff> partnerparts = reciever.GetGenitalsList();
			var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(props.dictionaryKey);

			//ModLog.Message(" RaceImplantEggs()" + pawn.RaceImplantEggs());
			//"insect" pregnancy
			//straight, female (partner) recives egg insertion from other/sex starter (pawn)

			if (CouldBeEgging(props, giver, reciever, pawnparts, partnerparts))
			{
				//TODO: add widget toggle for bind all/neutral/hostile pawns
				if (!props.isReceiver)
					if (CanCocoon(giver))
						if (giver.HostileTo(reciever) || reciever.IsPrisonerOfColony || reciever.health.hediffSet.HasHediff(xxx.submitting))
							if (!reciever.health.hediffSet.HasHediff(HediffDef.Named("RJW_Cocoon")))
							{
								reciever.health.AddHediff(HediffDef.Named("RJW_Cocoon"));
							}

				DoEgg(props);
				return;
			}

			if (!(props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration))
				return;

			//"normal" and "beastial" pregnancy
			if (RJWSettings.DevMode) ModLog.Message(" 'normal' pregnancy checks");

			//interaction stuff if for handling futa/see who penetrates who in interaction
			if (!props.isReceiver &&
				interaction.DominantHasTag(GenitalTag.CanPenetrate) &&
				interaction.SubmissiveHasFamily(GenitalFamily.Vagina))
			{
				if (RJWSettings.DevMode) ModLog.Message(" impregnate - by initiator");
			}
			else if (props.isReceiver &&
				interaction.DominantHasFamily(GenitalFamily.Vagina) &&
				interaction.SubmissiveHasTag(GenitalTag.CanPenetrate) &&
				interaction.HasInteractionTag(InteractionTag.Reverse))
			{
				if (RJWSettings.DevMode) ModLog.Message(" impregnate - by receiver (reverse)");
			}
			else
			{
				if (RJWSettings.DevMode) ModLog.Message(" no valid interaction tags/family");
				return;
			}

			if (!Modules.Interactions.Helpers.PartHelper.FindParts(giver, GenitalTag.CanFertilize).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(giver) + " has no parts to Fertilize with");
				return;
			}
			if (!Modules.Interactions.Helpers.PartHelper.FindParts(reciever, GenitalTag.CanBeFertilized).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(reciever) + " has no parts to be Fertilized");
				return;
			}

			if (CanImpregnate(giver, reciever, props.sexType))
			{
				Doimpregnate(giver, reciever);
			}
		}

		private static bool CouldBeEgging(SexProps props, Pawn giver, Pawn reciever, List<Hediff> pawnparts, List<Hediff> partnerparts)
		{
			//no ovipositor or fertilization possible
			if ((Genital_Helper.has_ovipositorF(giver, pawnparts) ||
				Genital_Helper.has_ovipositorM(giver, pawnparts) ||
				(Genital_Helper.has_penis_fertile(giver, pawnparts) && (giver.RaceImplantEggs() || reciever.health.hediffSet.GetHediffs<Hediff_InsectEgg>().Any()))
				) == false)
			{
				return false;
			}

			if ((props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration) &&
				Genital_Helper.has_vagina(reciever, partnerparts))
			{
				return true;
			}

			if ((props.sexType == xxx.rjwSextype.Anal || props.sexType == xxx.rjwSextype.DoublePenetration) && 
				Genital_Helper.has_anus(reciever) &&
				RJWPregnancySettings.insect_anal_pregnancy_enabled)
			{
				return true;
			}

			if (props.sexType == xxx.rjwSextype.Oral && 
				RJWPregnancySettings.insect_oral_pregnancy_enabled)
			{
				return true;
			}

			return false;
		}

		private static bool CanCocoon(Pawn pawn)
		{
			return xxx.is_insect(pawn);
		}


		public static Hediff GetPregnancy(Pawn pawn)
		{
			var set = pawn.health.hediffSet;

			Hediff preg =
				Hediff_BasePregnancy.KnownPregnancies()
				.Select(x => set.GetFirstHediffOfDef(HediffDef.Named(x)))
				.FirstOrDefault(x => x != null) ??
				set.GetFirstHediffOfDef(HediffDefOf.Pregnant);

			return preg;
		}

		public static List<Hediff> GetPregnancies(Pawn pawn)
		{
			List<Hediff> preg = new List<Hediff>();
			preg.AddRange(pawn.health.hediffSet.hediffs.Where(x => x is Hediff_BasePregnancy || x is Hediff_Pregnant));
			return preg;
		}

		///<summary>Can get preg with above conditions, do impregnation.</summary>

		[SyncMethod]
		public static void DoEgg(SexProps props)
		{
			if (RJWPregnancySettings.insect_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" insect pregnancy");

				//female "insect" plant eggs
				//futa "insect" 50% plant eggs
				if ((Genital_Helper.has_ovipositorF(props.pawn) && !Genital_Helper.has_penis_fertile(props.pawn)) ||
					(Rand.Value > 0.5f && Genital_Helper.has_ovipositorF(props.pawn)))
					//penis eggs someday?
					//(Rand.Value > 0.5f && (Genital_Helper.has_ovipositorF(pawn) || Genital_Helper.has_penis_fertile(pawn) && pawn.RaceImplantEggs())))
				{
					float maxeggssize = (props.partner.BodySize / 5) * (xxx.has_quirk(props.partner, "Incubator") ? 2f : 1f) * (Genital_Helper.has_ovipositorF(props.partner) ? 2f : 1f);
					float eggedsize = 0;

					foreach (Hediff_InsectEgg egg in props.partner.health.hediffSet.GetHediffs<Hediff_InsectEgg>())
					{
						if (egg.father != null)
							eggedsize += egg.father.RaceProps.baseBodySize / 5 * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
						else
							eggedsize += egg.implanter.RaceProps.baseBodySize / 5 * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
					}
					if (RJWSettings.DevMode) ModLog.Message(" determine " + xxx.get_pawnname(props.partner) + " size of eggs inside: " + eggedsize + ", max: " + maxeggssize);

					var eggs = props.pawn.health.hediffSet.GetHediffs<Hediff_InsectEgg>().ToList();
					BodyPartRecord partnerGenitals = null;

					if (props.sexType == xxx.rjwSextype.Anal)
						partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
					else if (props.sexType == xxx.rjwSextype.Oral)
						partnerGenitals = Genital_Helper.get_stomachBPR(props.partner);
					else if (props.sexType == xxx.rjwSextype.DoublePenetration && Rand.Value > 0.5f && RJWPregnancySettings.insect_anal_pregnancy_enabled)
						partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
					else
						partnerGenitals = Genital_Helper.get_genitalsBPR(props.partner);

					while (eggs.Any() && eggedsize < maxeggssize)
					{
						if (props.sexType == xxx.rjwSextype.Vaginal)
						{
							// removing old pregnancies
							var p = GetPregnancies(props.partner);
							if (!p.NullOrEmpty())
							{
								var i = p.Count;
								while (i > 0)
								{
									i -= 1;
									var h = GetPregnancies(props.partner);
									if (h[i] is Hediff_MechanoidPregnancy)
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - pregnant by mechanoid, skip");
									}
									else if (h[i] is Hediff_BasePregnancy)
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - removing rjw normal pregnancy");
										(h[i] as Hediff_BasePregnancy).Kill();
									}
									else
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - removing vanilla or other mod pregnancy");
										props.partner.health.RemoveHediff(h[i]);
									}
								}
							}
						}

						var egg = eggs.First();
						eggs.Remove(egg);

						props.pawn.health.RemoveHediff(egg);
						props.partner.health.AddHediff(egg, partnerGenitals);
						
						egg.Implanter(props.pawn);

						eggedsize += egg.eggssize * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
					}
				}
				//male or futa fertilize eggs
				else if (!props.pawn.health.hediffSet.HasHediff(xxx.sterilized))
				{
					if (Genital_Helper.has_penis_fertile(props.pawn))
						if ((Genital_Helper.has_ovipositorF(props.pawn) || Genital_Helper.has_ovipositorM(props.pawn)) || (props.pawn.health.capacities.GetLevel(xxx.reproduction) > 0))
						{
							foreach (var egg in props.partner.health.hediffSet.GetHediffs<Hediff_InsectEgg>())
								egg.Fertilize(props.pawn);
						}
				}
				return;
			}
		}

		[SyncMethod]
		public static void Doimpregnate(Pawn pawn, Pawn partner)
		{
			if (RJWSettings.DevMode) ModLog.Message(" Doimpregnate " + xxx.get_pawnname(pawn) + " is a father, " + xxx.get_pawnname(partner) + " is a mother");

			if (AndroidsCompatibility.IsAndroid(pawn) && !AndroidsCompatibility.AndroidPenisFertility(pawn))
			{
				if (RJWSettings.DevMode) ModLog.Message(" Father is android with no arcotech penis, abort");
				return;
			}
			if (AndroidsCompatibility.IsAndroid(partner) && !AndroidsCompatibility.AndroidVaginaFertility(partner))
			{
				if (RJWSettings.DevMode) ModLog.Message(" Mother is android with no arcotech vagina, abort");
				return;
			}

			// fertility check
			float fertility = RJWPregnancySettings.humanlike_impregnation_chance / 100f;
			if (xxx.is_animal(partner))
				fertility = RJWPregnancySettings.animal_impregnation_chance / 100f;
			// IUD fertility check
			if (partner.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("RJW_IUD")) != null)
				fertility /= 99f;

			// Interspecies modifier
			if (pawn.def.defName != partner.def.defName)
			{
				if (RJWPregnancySettings.complex_interspecies)
					fertility *= SexUtility.BodySimilarity(pawn, partner);
				else
					fertility *= RJWPregnancySettings.interspecies_impregnation_modifier;
			}
			else
			{
				//Egg fertility check
				CompEggLayer compEggLayer = partner.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
					fertility = 1.0f;
			}

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float ReproductionFactor = Math.Min(pawn.health.capacities.GetLevel(xxx.reproduction), partner.health.capacities.GetLevel(xxx.reproduction));
			float pregnancy_chance = fertility * ReproductionFactor;
			float pregnancy_roll_to_fail = Rand.Value;
			//BodyPartRecord torso = partner.RaceProps.body.AllParts.Find(x => x.def == BodyPartDefOf.Torso);

			if (pregnancy_roll_to_fail > pregnancy_chance || pregnancy_chance <= 0)
			{
				if (RJWSettings.DevMode) ModLog.Message(" Impregnation failed. Chance: " + pregnancy_chance.ToStringPercent() + " < " + pregnancy_roll_to_fail.ToStringPercent());
				return;
			}
			if (RJWSettings.DevMode) ModLog.Message(" Impregnation succeeded. Chance: " + pregnancy_chance.ToStringPercent() + " > " + pregnancy_roll_to_fail.ToStringPercent());

			PregnancyDecider(partner, pawn);
		}


		///<summary>For checking normal pregnancy, should not for egg implantion or such.</summary>
		public static bool CanImpregnate(Pawn fucker, Pawn fucked, xxx.rjwSextype sexType  = xxx.rjwSextype.Vaginal )
		{
			if (fucker == null || fucked == null) return false;

			if (RJWSettings.DevMode) ModLog.Message("Rimjobworld::CanImpregnate checks (" + sexType + "):: " + xxx.get_pawnname(fucker) + " + " + xxx.get_pawnname(fucked) + ":");

			if (sexType == xxx.rjwSextype.MechImplant && !RJWPregnancySettings.mechanoid_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" mechanoid 'pregnancy' disabled");
				return false;
			}
			if (!(sexType == xxx.rjwSextype.Vaginal || sexType == xxx.rjwSextype.DoublePenetration))
			{
				if (RJWSettings.DevMode) ModLog.Message(" sextype cannot result in pregnancy");
				return false;
			}

			if (AndroidsCompatibility.IsAndroid(fucker) && AndroidsCompatibility.IsAndroid(fucked))
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " androids cant breed/reproduce androids");
				return false;
			}

			if ((fucker.IsUnsexyRobot() || fucked.IsUnsexyRobot()) && !(sexType == xxx.rjwSextype.MechImplant))
			{
				if (RJWSettings.DevMode) ModLog.Message(" unsexy robot cant be pregnant");
				return false;
			}

			if (!fucker.RaceHasPregnancy())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " filtered race that cant be pregnant");
				return false;
			}

			if (!fucked.RaceHasPregnancy())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucker) + " filtered race that cant impregnate");
				return false;
			}


			if (fucked.IsPregnant())
			{
				if (RJWSettings.DevMode) ModLog.Message(" already pregnant.");
				return false;
			}

			if ((from x in fucked.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.def == DefDatabase<HediffDef_InsectEgg>.GetNamed(x.def.defName) select x).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " cant get pregnant while eggs inside");
				return false;
			}

			var pawnparts = fucker.GetGenitalsList();
			var partnerparts = fucked.GetGenitalsList();
			if (!(Genital_Helper.has_penis_fertile(fucker, pawnparts) && Genital_Helper.has_vagina(fucked, partnerparts)) && !(Genital_Helper.has_penis_fertile(fucked, partnerparts) && Genital_Helper.has_vagina(fucker, pawnparts)))
			{
				if (RJWSettings.DevMode) ModLog.Message(" missing genitals for impregnation");
				return false;
			}

			if (fucker.health.capacities.GetLevel(xxx.reproduction) <= 0 || fucked.health.capacities.GetLevel(xxx.reproduction) <= 0)
			{
				if (RJWSettings.DevMode) ModLog.Message(" one (or both) pawn(s) infertile");
				return false;
			}

			if (xxx.is_human(fucked) && xxx.is_human(fucker) && (RJWPregnancySettings.humanlike_impregnation_chance == 0 || !RJWPregnancySettings.humanlike_pregnancy_enabled))
			{
				if (RJWSettings.DevMode) ModLog.Message(" human pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (((xxx.is_animal(fucker) && xxx.is_human(fucked)) || (xxx.is_human(fucker) && xxx.is_animal(fucked))) && !RJWPregnancySettings.bestial_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" bestiality pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (xxx.is_animal(fucked) && xxx.is_animal(fucker) && (RJWPregnancySettings.animal_impregnation_chance == 0 || !RJWPregnancySettings.animal_pregnancy_enabled))
			{
				if (RJWSettings.DevMode) ModLog.Message(" animal-animal pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (fucker.def.defName != fucked.def.defName && (RJWPregnancySettings.interspecies_impregnation_modifier <= 0.0f && !RJWPregnancySettings.complex_interspecies))
			{
				if (RJWSettings.DevMode) ModLog.Message(" interspecies pregnancy disabled.");
				return false;
			}

			if (!(fucked.RaceProps.gestationPeriodDays > 0))
			{
				CompEggLayer compEggLayer = fucked.TryGetComp<CompEggLayer>();
				if (compEggLayer == null)
				{
					if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " mother.RaceProps.gestationPeriodDays is " + fucked.RaceProps.gestationPeriodDays + " cant impregnate");
					return false;
				}
			}
			
			return true;
		}

		//Plant babies for human/bestiality pregnancy
		public static void PregnancyDecider(Pawn mother, Pawn father)
		{
			//human-human
			if (RJWPregnancySettings.humanlike_pregnancy_enabled && xxx.is_human(mother) && xxx.is_human(father))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					// fertilize eggs of humanlikes ?!
					if (!compEggLayer.FullyFertilized)
					{
						compEggLayer.Fertilize(father);
						//if (!mother.kindDef.defName.Contains("Chicken"))
						//	if (compEggLayer.Props.eggFertilizedDef.defName.Contains("RJW"))
					}
				}
				else
					Hediff_BasePregnancy.Create<Hediff_HumanlikePregnancy>(mother, father);
			}
			//human-animal
			//maybe make separate option for human males vs female animals???
			else if (RJWPregnancySettings.bestial_pregnancy_enabled && (xxx.is_human(mother) ^ xxx.is_human(father)))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					if (!compEggLayer.FullyFertilized)
						compEggLayer.Fertilize(father);
				}
				else
					Hediff_BasePregnancy.Create<Hediff_BestialPregnancy>(mother, father);
			}
			//animal-animal
			else if (xxx.is_animal(mother) && xxx.is_animal(father))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					// fertilize eggs of same species
					if (!compEggLayer.FullyFertilized)
						if (mother.kindDef == father.kindDef)
							compEggLayer.Fertilize(father);
				}
				else if (RJWPregnancySettings.animal_pregnancy_enabled)
				{
					Hediff_BasePregnancy.Create<Hediff_BestialPregnancy>(mother, father);
				}
			}
		}

		//Plant Insect eggs/mech chips/other preg mod hediff?
		public static bool PlantSomething(HediffDef def, Pawn target, bool isToAnal = false, int amount = 1)
		{
			if (def == null)
				return false;
			if (!isToAnal && !Genital_Helper.has_vagina(target))
				return false;
			if (isToAnal && !Genital_Helper.has_anus(target))
				return false;

			BodyPartRecord Part = (isToAnal) ? Genital_Helper.get_anusBPR(target) : Genital_Helper.get_genitalsBPR(target);
			if (Part != null || Part.parts.Count != 0)
			{
				//killoff normal preg
				if (!isToAnal)
				{
					if (RJWSettings.DevMode) ModLog.Message(" removing other pregnancies");
					var p = GetPregnancies(target);
					if (!p.NullOrEmpty())
					{
						foreach (var x in p)
						{
							if (x is Hediff_BasePregnancy)
							{
								var preg = x as Hediff_BasePregnancy;
								preg.Kill();
							}
							else
							{
								target.health.RemoveHediff(x);
							}
						}
					}
				}

				for (int i = 0; i < amount; i++)
				{

					if (RJWSettings.DevMode) ModLog.Message(" planting something weird");
					target.health.AddHediff(def, Part);
				}

				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove CnP Pregnancy, that is added without passing rjw checks
		/// </summary>
		public static void cleanup_CnP(Pawn pawn)
		{
			//They do subpar probability checks and disrespect our settings, but I fail to just prevent their doloving override.
			//probably needs harmonypatch
			//So I remove the hediff if it is created and recreate it if needed in our handler later

			if (RJWSettings.DevMode) ModLog.Message(" cleanup_CnP after love check");

			var h = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("HumanPregnancy"));
			if (h != null && h.ageTicks < 100)
			{
				pawn.health.RemoveHediff(h);
				if (RJWSettings.DevMode) ModLog.Message(" removed hediff from " + xxx.get_pawnname(pawn));
			}
		}

		/// <summary>
		/// Remove Vanilla Pregnancy
		/// </summary>
		public static void cleanup_vanilla(Pawn pawn)
		{
			if (RJWSettings.DevMode) ModLog.Message(" cleanup_vanilla after love check");

			var h = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
			if (h != null && h.ageTicks < 100)
			{
				pawn.health.RemoveHediff(h);
				if (RJWSettings.DevMode) ModLog.Message(" removed hediff from " + xxx.get_pawnname(pawn));
			}
		}

		/// <summary>
		/// Below is stuff for RimWorldChildren
		/// its not used, we rely only on our own pregnancies
		/// </summary>

		/// <summary>
		/// This function tries to call Children and pregnancy utilities to see if that mod could handle the pregnancy
		/// </summary>
		/// <returns>true if cnp pregnancy will work, false if rjw one should be used instead</returns>
		public static bool CnP_WillAccept(Pawn mother)
		{
			//if (!xxx.RimWorldChildrenIsActive)
				return false;

			//return RimWorldChildren.ChildrenUtility.RaceUsesChildren(mother);
		}

		/// <summary>
		/// This funtcion tries to call Children and Pregnancy to create humanlike pregnancy implemented by the said mod.
		/// </summary>
		public static void CnP_DoPreg(Pawn mother, Pawn father)
		{
			if (!xxx.RimWorldChildrenIsActive)
				return;

			//RimWorldChildren.Hediff_HumanPregnancy.Create(mother, father);
		}

	}
}