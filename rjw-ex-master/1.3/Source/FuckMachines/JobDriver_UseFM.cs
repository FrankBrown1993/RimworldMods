using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using rjw;
using HarmonyLib;
using UnityEngine;
using Verse.Sound;
using System;
using Multiplayer.API;

namespace rjwex
{
	public class JobDriver_UseFM : JobDriverWithOffsets
	{
		//consts
		private const int ticks_maximum = 15000;                    //6 in-game hours hard max time
		private const int ticks_minimum = 416;                      //10 in-game minutes

		private const int tickerFrequency = 60;                     //calculations once per second

		private const float min_rest_when_free = 0.2f;              //cease session if rest below this and not restrained

		private const float RestFallTick = 1.0f / (2500 * 2);       //rest need 1to0 in 2 hours (with mult=1)
		private const float SexTick = 1.0f / 2500;                  //sex need 0to1 in 1 hour (with mult=1)

		private readonly TargetIndex iFM = TargetIndex.A;
		private Building_FuckMachine FM => (Building_FuckMachine)job.GetTarget(iFM);    //machine reference

		//exposable, set on mode selection with random
		private int ticks_duration = 7500;                          //program duration, main time limit, may be extended by program
		private int ticks_elapsed = 0;                              //main time counter
		private Modes CurrentMode = Modes.Normal;                   //selected mode. Normal for all non-automated machines. TODO manual selector

		private int ticks_between_hearts = 60;                      //heart mote frequency
		private int ticks_between_thrusts = 60;                     //thrust animation mote frequency

		private float RestFallMult = 1;                             //multipliers for needs gain/decay
		private float SexMult = 1;
		private float JoyMult = 1;

		private float totalRestFall = 0f;                           //counters for stats info
		private float totalSexSat = 0f;
		private float totalJoy = 0f;

		//set on init, no random
		public float height = 0f;
		public float amplitude = 0f;

		public float quality = 1;
		public bool forced = false;
		public bool auto = false;
		private bool vertical = true;
		private bool powered = false;

		Need_Rest need_rest;
		Need_Sex need_sex;
		Need_Joy need_joy;

		private JitterHandler jitterer;
		private Func<bool> statsTicker;
		private Dictionary<Modes, Func<bool>> tickersDict;
		//=======================
		private float restFall;
		private float sexSat;
		private float joy;

		public enum Modes
		{
			Normal,     // balanced for 0.5-1.5 in-game hours, rest to sex conversion factored by quality
			Long,       // slower pace, same exhaustion but longer time (1-3 hours), more pleasure for longer session
			Intensive,  // faster (0.25-0.75 hour), significant exhaustion, lots of pleasure
			Maxpleasure,// raising sex need to max at fastest rate, even for hompshroom adiicted, moderate exhaustion 
			Tease,      // time from short to long (0.5-2 hours), pawn left at horny threshhold (0.25 by rjw default), gains joy from session
			Exhaust     // moderate pleasure gain, lasts down to 0% rest need, but 2-3 hours minimum, capped at 6 hours
		};

		private void init(bool loadingSave = false)
		{
			tickersDict = new Dictionary<Modes, Func<bool>>() {
				{Modes.Normal,(() =>//default ticker
					{
						restFall = RestFallTick * RestFallMult * tickerFrequency;
						sexSat = SexTick * SexMult * tickerFrequency;
						joy = (Mathf.Max(sexSat - restFall, 0f) + (sexSat / 4)); // (sex - exhaustion or 0 if negative +1/4 sex) TODO better joy calc

						if (need_rest!=null) need_rest.CurLevel -= restFall;
						if (need_sex!=null) need_sex.CurLevel += sexSat;
						if (need_joy!=null) need_joy.CurLevel += joy;

						totalRestFall += restFall;
						totalSexSat += sexSat;
						totalJoy += joy;

						/* TODO quirk checks
						string pawn_quirks = CompRJW.Comp(pawn).quirks.ToString();

						if (pawn_quirks.Contains("Endytophile"))
						{
							if (pawn.apparel.PsychologicallyNude)
						}
						if (pawn_quirks.Contains("Exhibitionist"))
						{
							if (xxx.is_zoophile(pawn))
							{
								if (pawn.Map.mapPawns.AllPawnsSpawned.Any(x => x != pawn && !x.Dead && x.CanSee(pawn)))
									satisfaction *= 1.4f;
							}
							else
							{
								if (pawn.Map.mapPawns.AllPawnsSpawned.Any(x => x != pawn && !x.Dead && x.CanSee(pawn) && !xxx.is_animal(x)))
									satisfaction *= 1.4f;
							}
						}*/
						return true;
					})
				},
				{Modes.Tease,(() =>// custom ticker to cap sex need at threshhold
					{
						restFall = RestFallTick * RestFallMult * tickerFrequency;
						sexSat = SexTick * SexMult * tickerFrequency;
						joy = (Mathf.Max((SexTick * tickerFrequency) - restFall, 0f) + ((SexTick * tickerFrequency) / 4)); //joy fixed by basic sexTick; 3/4hrs 0to1 minus restFall

						if (need_rest!=null) need_rest.CurLevel -= restFall;
						if (need_sex!=null) need_sex.CurLevel += sexSat;
						if (need_joy!=null) need_joy.CurLevel += joy;

						if (Math.Abs(need_sex.CurLevel - need_sex.thresh_horny()) < sexSat * 2)
							need_sex.CurLevel = need_sex.thresh_horny();        // cap at horny, not count to total
						else
							totalSexSat += sexSat;

						totalRestFall += restFall;
						totalJoy += joy;

						return true;
					})
				},
				{Modes.Exhaust,(() =>// custom ticker to to achieve 0 rest if still above after duration, no cap, capped by ticks_maximum check
					{
						restFall = RestFallTick * RestFallMult * tickerFrequency;
						sexSat = SexTick * SexMult * tickerFrequency;
						joy = (Mathf.Max(sexSat - restFall, 0f) + (sexSat / 4));

						if (need_rest!=null) need_rest.CurLevel -= restFall;
						if (need_sex!=null) need_sex.CurLevel += sexSat;
						if (need_joy!=null) need_joy.CurLevel += joy;

						if (ticks_elapsed - ticks_duration < tickerFrequency * 3) ticks_duration += tickerFrequency;

						totalSexSat += sexSat;
						totalRestFall += restFall;
						totalJoy += joy;

						return true;
					})
				}
			};

			//init
			vertical = (FM.def as Building_FuckMachine_Def).vertical;
			powered = (FM.def as Building_FuckMachine_Def).powered;

			quality = (FM.def as Building_FuckMachine_Def).quality;
			forced = (FM.def as Building_FuckMachine_Def).forced;
			auto = (FM.def as Building_FuckMachine_Def).automatic;

			height = (FM.def as Building_FuckMachine_Def).height;
			amplitude = (FM.def as Building_FuckMachine_Def).amplitude;

			need_rest = pawn.needs.TryGetNeed<Need_Rest>();
			need_sex = pawn.needs.TryGetNeed<Need_Sex>();
			need_joy = pawn.needs.TryGetNeed<Need_Joy>();

			this.FailOnDespawnedOrNull(iFM);
			if (!forced)
			{
				this.FailOn(() => pawn.Drafted);
				this.FailOn(() => pawn.IsFighting());
			}

			//jitterer = Traverse.Create(pawn.Drawer).Field("jitterer").GetValue<JitterHandler>();
			jitterer = (AccessTools.Field(typeof(Pawn_DrawTracker), "jitterer")).GetValue(pawn.Drawer) as JitterHandler;

			if (!loadingSave) { SetMode(); }

			if (tickersDict.ContainsKey(CurrentMode))
				statsTicker = tickersDict[CurrentMode];
			else
				statsTicker = tickersDict[Modes.Normal];

		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(FM, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			init();
			Toil riding = new Toil();

			riding.tickAction = FMTick;
			riding.initAction = delegate
			{
				drawOffset.z = height;
				pawn.Rotation = FM.Rotation;
				if (!xxx.has_quirk(pawn, "Endytophile"))
					xxxEx.DrawNudeWithBondage(pawn);
				FM.lastRideInfo = "";
				FM.currentRideInfo = "";
			};
			riding.AddFinishAction(delegate
			{
				pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
				drawOffset.z = 0f;
				FM.currentRideInfo = "";
				FM.lastRideInfo = "Used by " + GetInfoString();

				//private SexUtility.IncreaseTicksToNextLovin(pawn);
				if (pawn == null || pawn.Dead) return;
				int currentTime = Find.TickManager.TicksGame;
				if (pawn.mindState.canLovinTick <= currentTime)
					pawn.mindState.canLovinTick = currentTime + SexUtility.GenerateMinTicksToNextLovin(pawn);
				//==============

				SexUtility.CumFilthGenerator(pawn);
				if (SexUtility.ConsiderCleaning(pawn))
				{
					LocalTargetInfo own_cum = pawn.PositionHeld.GetFirstThing<Filth>(pawn.Map);
					Job clean = new Job(JobDefOf.Clean);
					clean.AddQueuedTarget(TargetIndex.A, own_cum);
					pawn.jobs.jobQueue.EnqueueFirst(clean);
				}
			});

			riding.defaultCompleteMode = ToilCompleteMode.Never;
			riding.socialMode = RandomSocialMode.Off;
			riding.handlingFacing = true;

			yield return Toils_Goto.GotoThing(iFM, PathEndMode.OnCell);
			yield return riding;
		}

		public void FMTick()
		{
			if (!xxx.has_quirk(pawn, "Endytophile"))
				xxxEx.DrawNudeWithBondage(pawn);
			drawOffset.z = height;

			if (pawn.IsHashIntervalTick(ticks_between_hearts))  // <3 mote
			{
				FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, FleckDefOf.Heart);
			}
			if (pawn.IsHashIntervalTick(ticks_between_thrusts)) // thrust animation/sounds
			{
				jitterer.AddOffset(amplitude, vertical ? 0f : FM.Rotation.AsAngle);
				if (RJWSettings.sounds_enabled)
				{

					if (powered)
						SoundDef.Named("FMThrust").PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					else
						SoundDef.Named("Sex").PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
				}
			}
			if (pawn.IsHashIntervalTick(tickerFrequency))   // stats changes/end conditions
			{
				statsTicker();
				FM.currentRideInfo = "In use by " + GetInfoString();
			}
			ticks_elapsed++;

			//End conditions
			if (ticks_elapsed > ticks_minimum) //hard min check
			{
				if (ticks_elapsed > ticks_maximum)  //hard max check
					stopSession(); //stop by max possible time
				if (!auto)
				{
					if (forced && ticks_elapsed >= ticks_duration && (need_sex.CurLevel >= need_sex.thresh_ahegao() || need_rest.CurLevel <= min_rest_when_free))
						// restrained, release by timer, may continue
						stopSession(); //stop if released and satisfied or exhausted

					else if (!forced && (need_sex.CurLevel >= need_sex.thresh_ahegao() || need_rest.CurLevel <= min_rest_when_free))
						// not restrained, normal mode
						stopSession(); //stop any time if satisfied or exhausted
				}
				else
				{
					if (!forced && (need_sex.CurLevel >= need_sex.thresh_ahegao() || need_rest.CurLevel <= min_rest_when_free || ticks_elapsed >= ticks_duration))
						// not restrained, auto mode
						stopSession(); //stop any time if satisfied or exhausted or if program completed
					else if (forced && ticks_elapsed >= ticks_duration)
						// restrained, auto, release and cease by timer
						stopSession(); //stop if program completed
				}
			}
		}
		private void stopSession()
		{
			ReadyForNextToil();
		}

		[SyncMethod]
		public void SetMode()
		{
			if (auto)
			{
				//TODO better mode selection
				float chance = Rand.Value;
				if (chance < 0.02) CurrentMode = Modes.Exhaust;
				else if (chance < 0.05) CurrentMode = Modes.Tease;
				else if (chance < 0.10) CurrentMode = Modes.Maxpleasure;
				else if (chance < 0.20) CurrentMode = Modes.Long;
				else if (chance < 0.50) CurrentMode = Modes.Intensive;
				else CurrentMode = Modes.Normal;
			}
			else
			{
				CurrentMode = Modes.Normal; //TODO manual selector
			}

			/*
			Normal,     // balanced for 0.5-1.5 in-game hours, rest to sex conversion factored by quality
			Long,       // slower pace, same exhaustion but longer time (1-3 hours), more pleasure for longer session
			Intensive,  // faster (0.25-0.75 hour), significant exhaustion, lots of pleasure
			Maxpleasure,// raising sex need to max in 20 minutes, even for humpshroom adiicted, moderate exhaustion 
			Tease,      // time from short to long (0.5-2.5 hours), pawn left at horny threshhold (0.25 by rjw default), but gains joy from session, moderate exhaustion
			Exhaust     // moderate pleasure gain, lasts down to 0% rest need, but 2-3 hours minimum, capped at 6 hours by ticks_maximum
			*/

			if (CurrentMode == Modes.Normal)
			{
				ticks_duration = (int)Math.Round(Rand.Range(0.5f, 1.5f) * 2500);
				ticks_between_thrusts = Rand.RangeInclusive(60, 90);
				ticks_between_hearts = ticks_between_thrusts;
				RestFallMult /= quality;
				SexMult *= quality;
			}
			else if (CurrentMode == Modes.Long)
			{
				ticks_duration = (int)Math.Round(Rand.Range(1f, 3f) * 2500);
				ticks_between_thrusts = Rand.RangeInclusive(90, 120);
				ticks_between_hearts = 60;
				RestFallMult /= (quality * 2);    //less exhaustion for slower pace
				SexMult *= quality;               //normal/2 speed, *2 sex, same exhaustion
			}
			else if (CurrentMode == Modes.Intensive)
			{
				ticks_duration = (int)Math.Round(Rand.Range(0.25f, 0.75f) * 2500);
				ticks_between_thrusts = 60;
				ticks_between_hearts = 40;
				RestFallMult /= (quality / 2);
				SexMult *= (quality * 2);           //normal*2 sex, exhaustion, speed
			}
			else if (CurrentMode == Modes.Maxpleasure)
			{
				ticks_duration = (int)Math.Round(0.33f * 2500); // ~20 minutes
				ticks_between_thrusts = 60;
				ticks_between_hearts = 40;
				RestFallMult /= (quality / 3);                  // normal 1hr exhaustion
				SexMult = 3;                                    // 20 minutes from 0 to max sex need
			}
			else if (CurrentMode == Modes.Tease)
			{
				ticks_duration = (int)Math.Round(Rand.Range(0.5f, 2.5f) * 2500);
				ticks_between_thrusts = Rand.RangeInclusive(60, 90); ;
				ticks_between_hearts = 60;
				RestFallMult /= (quality * (float)(ticks_duration / 2500)); //adjusted by time
				SexMult = need_sex.thresh_horny() - need_sex.CurLevel;
			}
			else if (CurrentMode == Modes.Exhaust)
			{
				ticks_duration = (int)Math.Round(Rand.Range(2f, 3f) * 2500);
				ticks_between_thrusts = Rand.RangeInclusive(60, 90);
				ticks_between_hearts = 60;
				RestFallMult = 1f; //1to0 in 2 hours, will be affected by Vigorous quirk (/2)
				SexMult *= quality;// default, 0to1 in 1hr factored by quality
			}

			// basic quirks handling
			if (CurrentMode != Modes.Maxpleasure)
			{
				if (pawn.health.hediffSet.HasHediff(HediffDef.Named("HumpShroomAddiction")) && !pawn.health.hediffSet.HasHediff(HediffDef.Named("HumpShroomEffect")))
					SexMult = 0.1f;
			}
			if (xxx.has_quirk(pawn, "Vigorous"))
				RestFallMult *= 0.5f;
		}

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look<int>(ref this.ticks_between_hearts, "ticks_between_hearts", 60, false);
			Scribe_Values.Look<int>(ref this.ticks_between_thrusts, "ticks_between_thrusts", 60, false);
			Scribe_Values.Look<int>(ref this.ticks_duration, "ticks_duration", 7500, false);
			Scribe_Values.Look<int>(ref this.ticks_elapsed, "ticks_elapsed", 0, false);
			Scribe_Values.Look<Modes>(ref this.CurrentMode, "CurrentMode", Modes.Normal, false);
			Scribe_Values.Look<float>(ref this.totalRestFall, "totalRestFall", 0f, false);
			Scribe_Values.Look<float>(ref this.totalSexSat, "totalSexSat", 0f, false);
			Scribe_Values.Look<float>(ref this.totalJoy, "totalJoy", 0f, false);
			Scribe_Values.Look<float>(ref this.RestFallMult, "RestFallMult", 1, false);
			Scribe_Values.Look<float>(ref this.SexMult, "SexMult", 1, false);
			Scribe_Values.Look<float>(ref this.JoyMult, "JoyMult", 1, false);

			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.init(true);
			}
		}

		private string GetInfoString()
		{
			return xxx.get_pawnname(pawn) + " for " + new DateTime().AddMinutes((double)ticks_elapsed / 2500 * 60).ToString(@"HH\:mm\:ss") +
					(!auto ? "" : Environment.NewLine + "Mode: " + CurrentMode.ToString()) +
					(!rjw.RJWSettings.DevMode ? "" : Environment.NewLine + "Gained: " + totalSexSat + " sex, " + totalRestFall + " exh, " + totalJoy + " joy");
		}
	}
}