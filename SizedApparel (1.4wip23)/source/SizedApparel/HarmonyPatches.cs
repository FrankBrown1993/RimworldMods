using HarmonyLib;
using RimWorld;
//using Rimworld_Animations;
//using AlienRace;
using rjw;
using System;
using System.Linq;
using Verse;

namespace SizedApparel
{
    [StaticConstructorOnStartup]
    public class SizedApparelPatch
    {

        public static bool alienRaceActive = false;
        public static bool SJWActive = false;
        public static bool RJWActive = false;
        public static bool DubsApparelTweaksActive = false;
        public static bool rimNudeWorldActive = false;
        public static bool OTYNudeActive = false;
        public static bool LicentiaActive = false;
        public static bool RimworldAnimationActive = false; //rjw animation
        public static bool MenstruationActive = false; //rjw_menstruation
        public static bool StatueOfColonistActive = false;

        static SizedApparelPatch()
        {

            //check SJW
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "SafeJobWorld"))
            {
                SJWActive = true;
            }
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "safe.job.world"))
            {
                SJWActive = true;
            }
            //check RJW
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "RimJobWorld"))
            {
                RJWActive = true;
            }
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "rim.job.world"))
            {
                RJWActive = true;
            }
            //check Dubs Apparel Tweaks
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Dubs Apparel Tweaks"))
            {
                DubsApparelTweaksActive = true;
            }
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "Dubwise.DubsApparelTweaks"))
            {
                DubsApparelTweaksActive = true;
            }

            //check Alien Race
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Humanoid Alien Races 2.0"))
            {
                alienRaceActive = true;
            }
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.Contains("Humanoid Alien Races")))
            {
                alienRaceActive = true;
            }
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "erdelf.HumanoidAlienRaces"))
            {
                alienRaceActive = true;
            }
            //check RimNudeWorld
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId == "shauaputa.rimnudeworld"))
            {
                rimNudeWorldActive = true;
            }
            //check OTYNude
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.Contains("OTY")&& x.PackageId.Contains("Nude")))
            {
                OTYNudeActive = true;
            }

            //check Licentia Lab
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower() == "LustLicentia.RJWLabs".ToLower()))
            {
                LicentiaActive = true;
            }
            if (!LicentiaActive)
            {
                if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower() == "Euclidean.LustLicentia.RJWLabs".ToLower()))
                {
                    LicentiaActive = true;
                }
            }
            if (!LicentiaActive)
            {
                if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower().Contains("LustLicentia.RJWLabs".ToLower())))
                {
                    LicentiaActive = true;
                }
            }



            //check rjw animation
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower() == "c0ffee.rimworld.animations".ToLower()))
            {
                RimworldAnimationActive = true;
            }

            //check rjw_menstruation
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower() == "rjw.menstruation".ToLower()))
            {
                MenstruationActive = true;
            }

            //check statue of Colonist
            if (LoadedModManager.RunningModsListForReading.Any(x => x.PackageId.ToLower() == "tammybee.statueofcolonist".ToLower()))
            {
                StatueOfColonistActive = true;
            }


            Log.Message("[SizedApparel] start");
            var harmony = new Harmony("SizedApparelforRJW");
            
            harmony.PatchAll();
            /*
            try
            {
                ((Action)(() => {
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "OTY_NUDE"))
                    {
                        Log.Message("Sized Apparel for RJW : OTY_NUDE founded");
                        usingOversized = true;
                        usingBackBreaking = true;
                    }
                }))();
            }
            catch (TypeLoadException ex)
            {

            }
            */


            //RJW Patch
            try
            {
                ((Action)(() =>
                {
                    if (RJWActive)
                    {

                        
                        Log.Message("[SizedApparel] RimJobWorld Found");
                        //harmony.Patch(AccessTools.Method(typeof(rjw.JobDriver_SexBaseInitiator), "Start"),
                        //postfix: new HarmonyMethod(typeof(SexStartPatch), "Postfix"));

                        //harmony.Patch(AccessTools.Method(typeof(rjw.JobDriver_SexBaseInitiator), "End"),
                        //postfix: new HarmonyMethod(typeof(SexEndPatch), "Postfix"));

                        //harmony.Patch(AccessTools.Method(typeof(rjw.SexUtility), "DrawNude"),
                        //postfix: new HarmonyMethod(typeof(DrawNudePatch), "Postfix"));

                        harmony.Patch(AccessTools.Method(typeof(Sexualizer), "sexualize_pawn"),
                        postfix: new HarmonyMethod(typeof(SexualizePawnPatch), "Postfix"));

                        Log.Message("[SizedApparel] RimJobWorld Patched");
                        
                    }
                    else
                    {
                        Log.Message("[SizedApparel] RimJobWorld Patch canceled");
                    }
                }))();
            }
            catch (TypeLoadException ex) { }


            //Alien Race Patch
            //Alien Race No longer supported.
            /*
            try
            {
                ((Action)(() =>
                {
                    if (alienRaceActive)
                    {
                        Log.Message("[SizedApparel] AlienRace Found");
                        
                        //harmony.Patch(AccessTools.Method(typeof(AlienRace.HarmonyPatches), "DrawAddons"),
                        //prefix: new HarmonyMethod(typeof(DrawAddonPatch), "Prefix"));
                        
                        Log.Message("[SizedApparel] AlienRace Patched");
                        
                    }
                    else
                    {
                        Log.Message("[SizedApparel] AlienRace Patch canceled");
                    }
                }))();
            }
            catch (TypeLoadException ex) {  }
            */

            try
            {
                ((Action)(() =>
                {
                    if (RimworldAnimationActive)
                    {
                        Log.Message("[SizedApparel] RimworldAnimaion(rjw animation) Found");

                        harmony.Patch(AccessTools.Method(typeof(Rimworld_Animations.CompBodyAnimator), "tickClip"),
                        postfix: new HarmonyMethod(typeof(RimworldAnimationPatch), "TickClipPostfix"));

                        harmony.Patch(AccessTools.Method(typeof(JobDriver_SexBaseInitiator), "End"),
                        postfix: new HarmonyMethod(typeof(RimworldAnimationPatch), "EndClipPostfix"));
                        

                        Log.Message("[SizedApparel] RimworldAnimaion(rjw animation) Patched");
                    }
                    else
                    {
                        Log.Message("[SizedApparel] RimworldAnimaion(rjw animation) Patch canceled");
                    }
                }))();
            }
            catch (TypeLoadException ex) { }

            //Rim Nude World Patch
            try
            {
                ((Action)(() =>
                {
                    if (alienRaceActive && rimNudeWorldActive)
                    {
                        Log.Message("[SizedApparel] RimNudeWorld Found");
                        /*
                        harmony.Patch(AccessTools.Method(typeof(AlienRace.AlienPartGenerator.BodyAddon), "CanDrawAddon"),
                        postfix: new HarmonyMethod(typeof(RimNudeWorldBreastHidePatch), "Postfix"));
                        */
                        //harmony.Patch(AccessTools.Method(typeof(RimNudeWorld.GenitalPatch), "Postfix"),
                        //prefix: new HarmonyMethod(typeof(SizedApparelRNWPatch), "Prefix"));

                        //Log.Message("SizedApparelforRJW::AlienRacePatch");
                        Log.Message("[SizedApparel] RimNudeWorld Patching...: RevealingApparel");
                        harmony.Patch(AccessTools.Method(typeof(RevealingApparel.RevealingApparel), "CanDrawRevealing"),
                        postfix: new HarmonyMethod(typeof(RevealingApparelPatch), "Postfix"));
                        Log.Message("[SizedApparel] RimNudeWorld Patched: RevealingApparel");
                    }
                    else
                    {
                        Log.Message("[SizedApparel] RimNudeWorld Patch canceled");
                    }
                }))();
            }
            catch (TypeLoadException ex)
            {
                Log.Warning("[SizedApparel] Activated RimNudeWorld version not match to patch!\nSome patch for RimNudeWorld may not work!");
            }


            //Dubs Apparel Tweaks Patch
            try
            {
                ((Action)(() =>
                {
                    if (DubsApparelTweaksActive)
                    {
                        Log.Message("[SizedApparel] Dubs Apparel Tweaks Found");
                        //harmony.Patch(AccessTools.Method(typeof(QuickFast.bs), "SwitchIndoors"),
                        //postfix: new HarmonyMethod(typeof(SizedApparelDubsApparelPatch), "indoorPostFixPatch"));
                        Log.Message("[SizedApparel] Dubs Apparel Tweaks (not) Patched (just debug message)");
                    }
                    else
                    {
                        Log.Message("[SizedApparel] Dubs Apparel Tweaks Patch canceled");
                    }
                }))();
            }
            catch (TypeLoadException ex) { }


            //SizedApparelPatch
            try
            {
                ((Action)(() =>
                {
                    Log.Message("[SizedApparel] doing PawnRenderer Patch");

                    //disable for 1.3
                    /*
                    var original = typeof(PawnRenderer).GetMethod("RenderPawnInternal", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof(Vector3),typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) }, null );
                    var postfix = typeof(DrawPawnPatch).GetMethod("RenderPawnInternalPostfix");
                    var prefix = typeof(DrawPawnPatch).GetMethod("RenderPawnInternalPrefix");
                    harmony.Patch(original, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix));
                    */

                    //1.3
                    var original0 = typeof(Pawn_ApparelTracker).GetMethod("Notify_ApparelChanged");
                    var original1 = typeof(Pawn_ApparelTracker).GetMethod("Notify_ApparelAdded");
                    var original2 = typeof(Pawn_ApparelTracker).GetMethod("Notify_ApparelRemoved");
                    var postfix = typeof(ApparelTrackerPatch).GetMethod("Changed");
                    harmony.Patch(original0, postfix: new HarmonyMethod(postfix));
                    //harmony.Patch(original1, postfix: new HarmonyMethod(postfix));
                    //harmony.Patch(original2, postfix: new HarmonyMethod(postfix));

                    Log.Message("[SizedApparel] PawnRenderer Patch complete");

                }))();
            }
            catch (TypeLoadException ex)
            {
                Log.Error("[SizedApparel] Cannot Patch for \"RenderPawnInternal\" Method! the mod may not work!");
            }
        }


    }
}
