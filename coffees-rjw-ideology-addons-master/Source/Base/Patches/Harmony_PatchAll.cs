using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace C0ffeeRIA
{

    [StaticConstructorOnStartup]
    public static class Harmony_PatchAll
    {
        static Harmony_PatchAll()
        { 
            Harmony harmony = new Harmony("C0ffeeRIA");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

    }
}
