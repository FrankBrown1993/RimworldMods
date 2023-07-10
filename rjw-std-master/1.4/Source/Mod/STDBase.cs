using System;
using HugsLib;
using HugsLib.Settings;
using Verse;

namespace rjwstd
{
	public class STDBase : ModBase
	{
		public override string ModIdentifier
		{
			get
			{
				return "RJW_STD";
			}
		}

		public static SettingHandle<bool> std_sex;
		public static SettingHandle<bool> std_floor;
		public static SettingHandle<bool> std_show_roll_to_catch;
		public static SettingHandle<float> std_min_severity_to_pitch;
		public static SettingHandle<float> std_env_pitch_cleanliness_exaggeration;
		public static SettingHandle<float> std_env_pitch_dirtiness_exaggeration;
		public static SettingHandle<float> std_outdoor_cleanliness;
		public static SettingHandle<float> opp_inf_initial_immunity;
		public static SettingHandle<float> pawn_spawn_with_std_mul;
		public static SettingHandle<float> nymph_spawn_with_std_mul;

		public override void DefsLoaded()
		{
			std_sex = Settings.GetHandle("std_sex", Translator.Translate("std_sex"), Translator.Translate("std_sex_desc"), true);
			std_floor = Settings.GetHandle("std_floor", Translator.Translate("std_floor"), Translator.Translate("std_floor_desc"), true);

			std_show_roll_to_catch = Settings.GetHandle("std_show_roll_to_catch", Translator.Translate("std_show_roll_to_catch"), Translator.Translate("std_show_roll_to_catch_desc"), false);
			std_min_severity_to_pitch = Settings.GetHandle("std_min_severity_to_pitch", Translator.Translate("std_min_severity_to_pitch"), Translator.Translate("std_min_severity_to_pitch_desc"), 0.21f);

			std_env_pitch_cleanliness_exaggeration = Settings.GetHandle("std_env_pitch_cleanliness_exaggeration", Translator.Translate("std_env_pitch_cleanliness_exaggeration"), Translator.Translate("std_env_pitch_cleanliness_exaggeration_desc"), 2.0f);
			std_env_pitch_dirtiness_exaggeration = Settings.GetHandle("std_env_pitch_dirtiness_exaggeration", Translator.Translate("std_env_pitch_dirtiness_exaggeration"), Translator.Translate("std_env_pitch_dirtiness_exaggeration_desc"), 0.5f);
			std_outdoor_cleanliness = Settings.GetHandle("std_outdoor_cleanliness", Translator.Translate("std_outdoor_cleanliness"), Translator.Translate("std_outdoor_cleanliness_desc"), -1.0f);

			opp_inf_initial_immunity = Settings.GetHandle("opp_inf_initial_immunity", Translator.Translate("opp_inf_initial_immunity"), Translator.Translate("opp_inf_initial_immunity_desc"), 0.55f);
			pawn_spawn_with_std_mul = Settings.GetHandle("pawn_spawn_with_std_mul", Translator.Translate("pawn_spawn_with_std_mul"), Translator.Translate("pawn_spawn_with_std_mul_desc"), 1.0f);
			nymph_spawn_with_std_mul = Settings.GetHandle("nymph_spawn_with_std_mul", Translator.Translate("nymph_spawn_with_std_mul"), Translator.Translate("nymph_spawn_with_std_mul_desc"), 3.0f);
		}
	}
}
