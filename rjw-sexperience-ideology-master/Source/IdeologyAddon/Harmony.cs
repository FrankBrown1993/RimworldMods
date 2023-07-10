using System.Reflection;
using Verse;

namespace RJWSexperience.Ideology
{
	[StaticConstructorOnStartup]
	internal static class Harmony
	{
		static Harmony()
		{
			new HarmonyLib.Harmony("RJW_Sexperience.Ideology").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
