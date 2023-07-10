/*
 * removed because not working
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;

namespace C0ffee_s_RJW_Ideology_Addons
{
    [StaticConstructorOnStartup]
    public static class SoftDependencyChecker
    {
        static SoftDependencyChecker()
        {

            string missingSoftDependencies = "";

            if (!LoadedModManager.RunningModsListForReading.Any((ModContentPack x) => x.PackageId == "c0ffee.rjw.events"))
            {
                missingSoftDependencies += "missing RJW Events, orgies will NOT be included; ";
            }
            if (!LoadedModManager.RunningModsListForReading.Any((ModContentPack x) => x.PackageId == "rjw.milk.humanoid"))
            {
                missingSoftDependencies += "missing RJW Milkable Colonists, hucow and lactation will NOT be included; ";
            }

            if(missingSoftDependencies != "")
            {
                Log.Warning("[c0ffee's RJW Ideology Addons] Warning, you're missing the following soft dependencies: " + missingSoftDependencies + "Features involving those mods will be disabled.");
            }

        }

    }
}*/
