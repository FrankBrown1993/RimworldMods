using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RJW_Events
{
    [StaticConstructorOnStartup]
    class Harmony_PatchAll
    {
        static Harmony_PatchAll()
        {
            Harmony harmony = new Harmony("rimworldevents");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
