using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace s16_extension
{
    [StaticConstructorOnStartup]
    internal class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("com.hediffapparel.rimworld.mod").PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("HediffApparel Harmony Patches:\n  Postfix:\n    Pawn_ApparelTracker.Notify_ApparelAdded\n    Pawn_ApparelTracker.Notify_ApparelRemoved");
        }
    }
}
