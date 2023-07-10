using System;
using HugsLib;
using HugsLib.Settings;
using Verse;

namespace rjwcum
{
	public class CumBase : ModBase
	{
		public override string ModIdentifier
		{
			get
			{
				return "RJW_Cum";
			}
		}

		public static SettingHandle<bool> debug;
		public static SettingHandle<bool> cum_on_body;
		public static SettingHandle<bool> cum_overlays;
		public static SettingHandle<float> cum_on_body_amount;
		public static SettingHandle<bool> manual_hero_CleanSelf;
		public static SettingHandle<bool> dubsDBH_block_CleanSelf;

		public override void DefsLoaded()
		{
			debug = Settings.GetHandle("debug",
									Translator.Translate("debug"),
									Translator.Translate("debug_desc"),
									false);
			cum_on_body = Settings.GetHandle("cum_on_body",
									Translator.Translate("cum_on_body"),
									Translator.Translate("cum_on_body_desc"),
									true);
			cum_overlays = Settings.GetHandle("cum_overlays",
									 Translator.Translate("cum_overlays"),
									 Translator.Translate("cum_overlays_desc"),
									 true);
			cum_on_body_amount = Settings.GetHandle("cum_on_body_amount",
										   Translator.Translate("cum_on_body_amount"),
										   Translator.Translate("cum_on_body_amount_desc"),
										   1.0f);
			manual_hero_CleanSelf = Settings.GetHandle("manual_hero_CleanSelf",
									 Translator.Translate("manual_hero_CleanSelf"),
									 Translator.Translate("manual_hero_CleanSelf_desc"),
									 true);
			dubsDBH_block_CleanSelf = Settings.GetHandle("dubsDBH_block_CleanSelf",
									 Translator.Translate("dubsDBH_block_CleanSelf"),
									 Translator.Translate("dubsDBH_block_CleanSelf_desc"),
									 true);
		}
	}
}
