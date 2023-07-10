using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;
using rjw;

namespace Privacy_Please
{
    [StaticConstructorOnStartup]
    public static class Harmony_PatchAll
    {
        static Harmony_PatchAll()
        {
            Harmony harmony = new Harmony("Privacy_Please");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Add quirks to game
            Quirk voyeur = new Quirk("Voyeur", "VoyeurQuirk");
            Quirk.All.AddDistinct(voyeur);

            Quirk cuckold = new Quirk("Cuckold", "CuckoldQuirk");
            Quirk.All.AddDistinct(cuckold);

            Quirk cuckolder = new Quirk("Cuckolder", "CuckolderQuirk");
            Quirk.All.AddDistinct(cuckolder);

            DebugMode.Message("Added RJW quirks");
        }
    }
}
