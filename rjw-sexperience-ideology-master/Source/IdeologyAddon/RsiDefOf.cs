using RimWorld;
using Verse;

namespace RJWSexperience.Ideology
{
	public static class RsiDefOf
	{
		[DefOf]
		public static class Job
		{
			public static readonly JobDef RapeVictim;
			public static readonly JobDef Gangbang;
			public static readonly JobDef GettinGangbang;
			public static readonly JobDef DrugSex;
			public static readonly JobDef GettinDrugSex;
			public static readonly JobDef DrugMasturbate;
		}

		[DefOf]
		public static class Meme
		{
			public static readonly MemeDef Zoophile;
			public static readonly MemeDef Rapist;
			public static readonly MemeDef Necrophile;
		}

		[DefOf]
		public static class Issue
		{
			public static readonly IssueDef Incestuos;
		}

		[DefOf]
		public static class Precept
		{
			public static readonly PreceptDef Incestuos_IncestOnly;
			public static readonly PreceptDef Bestiality_OnlyVenerated;
			public static readonly PreceptDef BabyFaction_AlwaysFather;
			public static readonly PreceptDef BabyFaction_AlwaysColony;
			public static readonly PreceptDef Submissive_Male;
			public static readonly PreceptDef Submissive_Female;
			public static readonly PreceptDef ProselyzingByOrgasm;
		}

		[DefOf]
		public static class HistoryEvent
		{
			public static readonly HistoryEventDef RSI_SexWithAnimal;
			public static readonly HistoryEventDef RSI_Raped;
			public static readonly HistoryEventDef RSI_NonIncestuosMarriage;
			public static readonly HistoryEventDef RSI_NonIncestuosSex;
			public static readonly HistoryEventDef RSI_SexWithCorpse;
			public static readonly HistoryEventDef RSI_VirginTaken;
			public static readonly HistoryEventDef RSI_VirginStolen;
			public static readonly HistoryEventDef RSI_TookVirgin;
			public static readonly HistoryEventDef RSI_Masturbated;
		}

		[DefOf]
		public static class Hediff
		{
			[MayRequireBiotech] public static readonly HediffDef PregnantHuman;
		}
	}
}
