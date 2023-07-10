using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace Patch_SexToysMasturbation
{

    [StaticConstructorOnStartup]
    public static class Harmony_PatchAll {

        static Harmony_PatchAll() {

            Harmony val = new Harmony("animtoyspatch");
            val.PatchAll(Assembly.GetExecutingAssembly());

        }
    }
}
