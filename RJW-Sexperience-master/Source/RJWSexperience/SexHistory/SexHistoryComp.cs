﻿using rjw;
using RJWSexperience.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RJWSexperience.SexHistory
{
	/// <summary>
	/// Stores pawn's sex history and handles its updates
	/// </summary>
	public class SexHistoryComp : ThingComp
	{
		public const int ARRLEN = 20;

		protected Dictionary<string, SexPartnerHistoryRecord> partnerRecords = new Dictionary<string, SexPartnerHistoryRecord>();
		protected string first = "";
		protected bool dirty = true;
		protected xxx.rjwSextype recentSex = xxx.rjwSextype.None;
		protected float recentSat = 0;
		protected string recentPartner = "";
		protected int[] sextypeCount = new int[ARRLEN];
		protected float[] sextypeSat = new float[ARRLEN];
		protected int[] sextypeRecentTickAbs = new int[ARRLEN];
		protected int virginsTaken = 0;
		protected int incestuous = 0;
		protected int bestiality = 0;
		protected int corpsefuck = 0;
		protected int interspecies = 0;
		protected int firstSexTickAbs = 0;

		protected string mostPartnerCache = "";
		protected xxx.rjwSextype mostSextypeCache = xxx.rjwSextype.None;
		protected xxx.rjwSextype mostSatSextypeCache = xxx.rjwSextype.None;
		protected xxx.rjwSextype bestSextypeCache = xxx.rjwSextype.None;
		protected float bestSextypeSatCache = 0;
		protected string bestPartnerCache = "";
		protected int totalSexCache = 0;
		protected int totalRapedCache = 0;
		protected int totalBeenRapedCache = 0;
		protected ThingDef preferRaceCache = null;
		protected int preferRaceSexCountCache = 0;
		protected Pawn preferRacePawnCache = null;
		protected int recentSexTickAbsCache = 0;
		protected int mostSexTickAbsCache = 0;
		protected int bestSexTickAbsCache = 0;

		public Gizmo Gizmo { get; private set; }
		public Pawn ParentPawn => parent as Pawn;
		public SexPartnerHistoryRecord FirstPartnerRecord => partnerRecords.TryGetValue(first);
		public SexPartnerHistoryRecord MostPartnerRecord
		{
			get
			{
				Update();
				return partnerRecords.TryGetValue(mostPartnerCache);
			}
		}
		public xxx.rjwSextype MostSextype
		{
			get
			{
				Update();
				return mostSextypeCache;
			}
		}
		public xxx.rjwSextype MostSatisfiedSex
		{
			get
			{
				Update();
				return mostSatSextypeCache;
			}
		}
		public SexPartnerHistoryRecord RecentPartnerRecord => partnerRecords.TryGetValue(recentPartner);
		public SexPartnerHistoryRecord BestSexPartnerRecord
		{
			get
			{
				Update();
				return partnerRecords.TryGetValue(bestPartnerCache);
			}
		}
		public float TotalSexHad
		{
			get
			{
				Update();
				return totalSexCache;
			}
		}
		public int VirginsTaken => virginsTaken;
		public IEnumerable<SexPartnerHistoryRecord> PartnerList
		{
			get
			{
				IEnumerable<SexPartnerHistoryRecord> res = Enumerable.Empty<SexPartnerHistoryRecord>();
				Update();
				if (!partnerRecords.NullOrEmpty())
				{
					res = partnerRecords.Values;
				}
				return res;
			}
		}
		public int PartnerCount => partnerRecords?.Count ?? 0;
		public int IncestuousCount => incestuous;
		public int RapedCount
		{
			get
			{
				Update();
				return totalRapedCache;
			}
		}
		public int BeenRapedCount
		{
			get
			{
				Update();
				return totalBeenRapedCache;
			}
		}
		public ThingDef PreferRace
		{
			get
			{
				Update();
				return preferRaceCache;
			}
		}
		public int PreferRaceSexCount
		{
			get
			{
				Update();
				return preferRaceSexCountCache;
			}
		}
		public int BestialityCount => bestiality;
		public int CorpseFuckCount => corpsefuck;
		public int InterspeciesCount => interspecies;
		public float AVGSat
		{
			get
			{
				Update();
				if (totalSexCache == 0) return 0;
				return sextypeSat.Sum() / totalSexCache;
			}
		}
		public int RecentSexTickAbs => recentSexTickAbsCache;
		public int FirstSexTickAbs => firstSexTickAbs;
		public int MostSexTickAbs => mostSexTickAbsCache;
		public int BestSexTickAbs => bestSexTickAbsCache;
		public Pawn PreferRacePawn
		{
			get
			{
				Update();
				return preferRacePawnCache;
			}
		}

		public float GetBestSextype(out xxx.rjwSextype sextype)
		{
			Update();
			sextype = bestSextypeCache;
			return bestSextypeSatCache;
		}

		public float GetRecentSextype(out xxx.rjwSextype sextype)
		{
			Update();
			sextype = recentSex;
			return recentSat;
		}

		public int GetSextypeRecentTickAbs(int sextype) => sextypeRecentTickAbs[sextype];

		public float GetAVGSat(int sextype)
		{
			float res = sextypeSat[sextype] / sextypeCount[sextype];
			return float.IsNaN(res) ? 0f : res;
		}

		public int GetSexCount(int sextype) => sextypeCount[sextype];

		public override void PostExposeData()
		{
			List<int> sextypecountsave;
			List<float> sextypesatsave;
			List<int> sextyperecenttickabssave;

			if (Scribe.mode == LoadSaveMode.Saving)
			{
				sextypecountsave = sextypeCount.ToList();
				sextypesatsave = sextypeSat.ToList();
				sextyperecenttickabssave = sextypeRecentTickAbs.ToList();
			}
			else
			{
				sextypecountsave = new List<int>();
				sextypesatsave = new List<float>();
				sextyperecenttickabssave = new List<int>();
			}

			Scribe_Collections.Look(ref partnerRecords, "histories", LookMode.Value, LookMode.Deep);
			Scribe_Values.Look(ref first, "first", string.Empty);
			Scribe_Values.Look(ref recentSex, "recentsex", xxx.rjwSextype.None);
			Scribe_Values.Look(ref recentSat, "recentsat", 0);
			Scribe_Values.Look(ref recentPartner, "recentpartner", string.Empty);
			Scribe_Values.Look(ref virginsTaken, "virginstaken", 0);
			Scribe_Values.Look(ref incestuous, "incestous", 0);
			Scribe_Values.Look(ref bestiality, "bestiality", 0);
			Scribe_Values.Look(ref corpsefuck, "corpsefuck", 0);
			Scribe_Values.Look(ref interspecies, "interspecies", 0);
			Scribe_Values.Look(ref firstSexTickAbs, "firstsextickabs", 0);
			Scribe_Collections.Look(ref sextypecountsave, "sextypecountsave", LookMode.Value);
			Scribe_Collections.Look(ref sextypesatsave, "sextypesatsave", LookMode.Value);
			Scribe_Collections.Look(ref sextyperecenttickabssave, "sextyperecenttickabssave", LookMode.Value);

			if (partnerRecords == null)
				partnerRecords = new Dictionary<string, SexPartnerHistoryRecord>();

			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				sextypeCount = sextypecountsave?.ToArray() ?? new int[ARRLEN];
				sextypeSat = sextypesatsave?.ToArray() ?? new float[ARRLEN];
				sextypeRecentTickAbs = sextyperecenttickabssave?.ToArray() ?? new int[ARRLEN];

				foreach (KeyValuePair<string, SexPartnerHistoryRecord> element in partnerRecords)
				{
					element.Value.PartnerID = element.Key;
				}
			}
			base.PostExposeData();
		}

		public void RecordSex(Pawn partner, SexProps props)
		{
			SexPartnerHistoryRecord partnerRecord = GetPartnerRecord(partner);
			partnerRecord.RecordSex(props);
			recentPartner = partner.ThingID;
			recentSex = props.sexType;
			sextypeCount[(int)props.sexType]++;
			sextypeRecentTickAbs[(int)props.sexType] = GenTicks.TicksAbs;
			if (partnerRecord.Incest) incestuous++;
			if (partner.Dead) corpsefuck++;
			if (props.IsBestiality()) bestiality++;
			else if (ParentPawn.def != partner.def) interspecies++;
			dirty = true;
		}

		public void RecordOrgasm(Pawn partner, SexProps props, float satisfaction)
		{
			GetPartnerRecord(partner).RecordOrgasm(props.sexType, satisfaction);
			recentSat = satisfaction;
			sextypeSat[(int)props.sexType] += satisfaction; // Several orgasmsms in one sex are messing with this
			dirty = true;
		}

		/// <summary>
		/// Record first partner and time of sex. No checks are performed, the caller should make sure it is the first time.
		/// </summary>
		/// <param name="partner"></param>
		public void RecordFirst(Pawn partner)
		{
			first = partner.ThingID;
			firstSexTickAbs = GenTicks.TicksAbs;
			partner.TryGetComp<SexHistoryComp>()?.TakeSomeonesVirgin(ParentPawn);
		}

		/// <summary>
		/// Retrive an existing partner record or add a new one. Increments SexPartnerCount when new record is added
		/// </summary>
		protected SexPartnerHistoryRecord GetPartnerRecord(Pawn partner)
		{
			string partnerId = partner.ThingID;

			if (partnerRecords.TryGetValue(partnerId, out SexPartnerHistoryRecord record))
			{
				return record;
			}

			SexPartnerHistoryRecord newRecord = new SexPartnerHistoryRecord(partner, partner.IsIncest(ParentPawn));
			partnerRecords.Add(partnerId, newRecord);
			ParentPawn.records.Increment(RsDefOf.Record.SexPartnerCount);
			return newRecord;
		}

		public void TakeSomeonesVirgin(Pawn partner)
		{
			GetPartnerRecord(partner)?.TookVirgin();
			virginsTaken++;
		}

		#region Cache update

		protected void Update()
		{
			if (dirty)
			{
				UpdateStatistics();
				UpdateBestSex();
				dirty = false;
			}
		}

		protected void UpdateStatistics()
		{
			int max = 0;
			float maxsat = 0;
			float maxf = 0;
			int maxindex = 0;
			string mostID = Keyed.Unknown;
			string bestID = Keyed.Unknown;

			totalSexCache = 0;
			totalRapedCache = 0;
			totalBeenRapedCache = 0;
			Dictionary<ThingDef, int> racetotalsat = new Dictionary<ThingDef, int>();
			List<Pawn> allpartners = new List<Pawn>();

			foreach (KeyValuePair<string, SexPartnerHistoryRecord> element in partnerRecords)
			{
				SexPartnerHistoryRecord h = element.Value;

				//find most sex partner
				if (max < h.TotalSexCount)
				{
					max = h.TotalSexCount;
					mostID = element.Key;
				}
				if (maxsat < h.BestSatisfaction)
				{
					maxsat = h.BestSatisfaction;
					bestID = element.Key;
				}

				if (h.Partner != null)
				{
					Pawn partner = h.Partner;
					allpartners.Add(partner);
					if (racetotalsat.ContainsKey(h.Race))
					{
						racetotalsat[h.Race] += h.TotalSexCount - h.RapedMe;
					}
					else
					{
						racetotalsat.Add(h.Race, h.TotalSexCount - h.RapedMe);
					}
				}

				totalSexCache += h.TotalSexCount;
				totalRapedCache += h.Raped;
				totalBeenRapedCache += h.RapedMe;
			}

			if (!racetotalsat.NullOrEmpty())
			{
				KeyValuePair<ThingDef, int> prefer = racetotalsat.MaxBy(x => x.Value);
				preferRaceCache = prefer.Key;
				preferRaceSexCountCache = prefer.Value;
				preferRacePawnCache = allpartners.Find(x => x.def == preferRaceCache);
			}

			for (int i = 0; i < sextypeCount.Length; i++)
			{
				float avgsat = sextypeSat[i] / sextypeCount[i];
				if (maxf < avgsat)
				{
					maxf = avgsat;
					maxindex = i;
				}
			}

			mostSatSextypeCache = (xxx.rjwSextype)maxindex;
			mostSextypeCache = (xxx.rjwSextype)sextypeCount.FirstIndexOf(x => x == sextypeCount.Max());
			mostPartnerCache = mostID;
			bestPartnerCache = bestID;

			recentSexTickAbsCache = partnerRecords.TryGetValue(recentPartner)?.RecentSexTickAbs ?? 0;
			mostSexTickAbsCache = partnerRecords.TryGetValue(mostPartnerCache)?.RecentSexTickAbs ?? 0;
			bestSexTickAbsCache = partnerRecords.TryGetValue(bestPartnerCache)?.BestSexTickAbs ?? 0;

			racetotalsat.Clear();
			allpartners.Clear();
		}

		protected void UpdateBestSex()
		{
			int bestindex = 0;
			float bestsat = 0;
			float avgsat;
			for (int i = 0; i < sextypeCount.Length; i++)
			{
				avgsat = sextypeSat[i] / sextypeCount[i];
				if (bestsat < avgsat)
				{
					bestindex = i;
					bestsat = avgsat;
				}
			}
			bestSextypeCache = (xxx.rjwSextype)bestindex;
			bestSextypeSatCache = bestsat;
		}

		#endregion Cache update

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (SexperienceMod.Settings.HideGizmoWhenDrafted && ParentPawn.Drafted)
				yield break;

			if (Find.Selector.NumSelected > 1)
				yield break;

			yield return Gizmo;
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);

			Gizmo = new Command_Action
			{
				defaultLabel = Keyed.RS_Sex_History,
				icon = HistoryUtility.HistoryIcon,
				defaultIconColor = HistoryUtility.HistoryColor,
				hotKey = RsDefOf.KeyBinding.OpenSexStatistics,
				shrinkable = true,
				Order = 5,
				action = () => UI.SexStatusWindow.ToggleWindow(this)
			};
		}
	}
}
