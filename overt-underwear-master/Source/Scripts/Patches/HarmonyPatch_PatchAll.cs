using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace Overt_Underwear
{
    [StaticConstructorOnStartup]
    public static class Harmony_PatchAll
    {
        static Harmony_PatchAll()
        {
            Harmony harmony = new Harmony("Overt_Underwear");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
