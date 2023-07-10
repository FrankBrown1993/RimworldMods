using System;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using Verse;

namespace rjwwhoring
{
	public class WhoringBase : ModBase
	{
		public override string ModIdentifier
		{
			get
			{
				return "RJW_Whoring";
			}
		}

		public static DataStore DataStore;//reference to savegame data, hopefully

		public override void SettingsChanged()
		{
			//ToggleTabIfNeeded();
		}
		public override void WorldLoaded()
		{
			DataStore = Find.World.GetComponent<DataStore>();
			//ToggleTabIfNeeded();
		}

		private void ToggleTabIfNeeded()
		{
			//DefDatabase<MainButtonDef>.GetNamed("RJW_Brothel").buttonVisible = whoringtab_enabled;
		}

		//public static SettingHandle<bool> whoringtab_enabled;
		public static SettingHandle<bool> show_whore_price_factor_on_bed;
		public static SettingHandle<bool> show_whore_widgets_on_bed;
		public static SettingHandle<bool> DebugWhoring;
		public static SettingHandle<bool> MoneyPrinting;
		public static SettingHandle<bool> ClientAlwaysAccept;

		public override void DefsLoaded()
		{
			//whoringtab_enabled = Settings.GetHandle("whoringtab_enabled",
			//						"whoringtab_enabled".Translate(),
			//						"whoringtab_enabled_desc".Translate(),
			//						true);
			show_whore_price_factor_on_bed = Settings.GetHandle("show_whore_price_factor_on_bed",
									"show_whore_price_factor_on_bed".Translate(),
									"show_whore_price_factor_on_bed_desc".Translate(),
									false);
			show_whore_widgets_on_bed = Settings.GetHandle("show_whore_widgets_on_bed",
									"show_whore_widgets_on_bed".Translate(),
									"show_whore_widgets_on_bed_desc".Translate(),
									false);
			MoneyPrinting = Settings.GetHandle("MoneyPrinting",
									"MoneyPrinting".Translate(),
									"MoneyPrinting_desc".Translate(),
									false);
			ClientAlwaysAccept = Settings.GetHandle("ClientAlwaysAccept",
									"ClientAlwaysAccept".Translate(),
									"ClientAlwaysAccept_desc".Translate(),
									false);
			DebugWhoring = Settings.GetHandle("DebugWhoring",
									"DebugWhoring".Translate(),
									"DebugWhoring_desc".Translate(),
									false);
		}
	}
}
