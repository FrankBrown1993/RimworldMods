using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using rjw;
using Rimworld_Animations;
using System.Reflection;

//since I test system alot, source cord is very dirty and not optimized.


namespace SizedApparel
{
    public struct supportedIndex
    {

    }

    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(Pawn_HealthTracker), "Notify_HediffChanged")]
    public class PawnHealthTrackerPatch
    {
        public static void Postfix(Hediff hediff, Pawn_HealthTracker __instance, ref Pawn ___pawn)
        {
            if (___pawn == null)
                return;
            var comp = ___pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            if (hediff == null)
                return;
            
            if (SizedApparelUtility.isRJWParts(hediff))///
            {
                //comp.ClearAll();
                //comp.Update();
                comp.SetDirty(false,true,false);
                //already doing set dirty in hediffchange method.
                //___pawn.Drawer.renderer.graphics.SetApparelGraphicsDirty();
                //PortraitsCache.SetDirty(___pawn);
                //GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(___pawn);
                return;
            }
            if (SizedApparelUtility.isBellyBulgeHediff(hediff))
            {
                comp.SetDirty(false, true, false);
                return;
            }

        }
    }

    //for 1.4
    /*
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(Pawn_AgeTracker), "PostResolveLifeStageChange")]
    public class PawnAgeTrackerPatch
    {
        public static void Postfix(Pawn ___pawn)
        {
            var comp = ___pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.SetDirty(false,false,false,true,true);//Apparel and Hediff will be changed with other reason. just set skeleton dirty.
        }
    }*/



    [StaticConstructorOnStartup]
    public class HeddifPatchForRimNudeWorld
    {
        //hediff.get_Severity()
        public static void GetSeverityPostFix(Hediff __instance)
        {
            if (__instance.Part != null)
            {

                if (__instance.Part.def.defName.Equals(SizedApparelUtility.chestString))
                {
                    if (__instance.def.defName.EndsWith(SizedApparelUtility.breastsString))
                    {
                        //Log.Message("Found Breast Hediff");
                        //_breastSeverity = __instance.Severity;
                        //_breastHediff = __instance;
                        //result = true;
                        //Log.Message(_breastSeverity.ToString());
                    }
                }
            }
        }

        //...get_severity()
        /*
        public static void BodyAddonHediffSeverityGraphicPatch(AlienRace.AlienPartGenerator.BodyAddonHediffSeverityGraphic __instance, ref float __result)
        {
            if (!SizedApparelPatch.rimNudeWorldActive) { return; }
            if (!SizedApparelSettings.matchBreastToSupportedApparelSize) { return; }
            if (__instance.path.Contains(SizedApparelUtility.breastsString))
            {

            }
        }*/
    }




    [Obsolete]
    [StaticConstructorOnStartup]
    public class BodyPatch
    {
        public static void SetBodyGraphic(Pawn pawn, bool drawClothFlag = true, bool fromGraphicRecord = true, bool revert = false)
        {
            //Pawn_ApparelTracker __instance;


            //GetBreastSeverity(__instance.pawn, out breastSeverity, out breastHediff);
            //bool flag = hasUnSupportedApparel(__instance.pawn, breastSeverity, breastHediff);
            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;

            if (revert)
            {
                var pawnRenderer = pawn.Drawer?.renderer?.graphics;
                //Log.Message("useBodyTexture");

                /*
                if (!pawnRenderer.AllResolved)
                {
                    pawnRenderer.ResolveAllGraphics();
                }*/

                if (pawnRenderer == null)
                    return;
                if (comp.graphicSourceNaked != null)
                    pawnRenderer.nakedGraphic = comp.graphicSourceNaked.GetColoredVersion(pawnRenderer.nakedGraphic.Shader, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo);
                if (comp.graphicSourceRotten != null)
                    pawnRenderer.rottingGraphic = comp.graphicSourceRotten.GetColoredVersion(pawnRenderer.rottingGraphic.Shader, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo);
                //if (comp.graphicSourceFurCovered != null)
                //    pawnRenderer.furCoveredGraphic = comp.graphicSourceRotten.GetColoredVersion(pawnRenderer.furCoveredGraphic.Shader, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo);

            }


            if (!comp.hasUpdateBefore)
            {
                //comp.Update(true, fromGraphicRecord);
            }

            float breastSeverity = comp.breastSeverity;
            Hediff breastHediff = comp.breastHediff;


            if (SizedApparelSettings.drawBodyParts == true && SizedApparelSettings.useBodyTexture)//old:SizedApparelSettings.useBodyTexture
            {
                //if (SizedApparelSettings.Debug)
                //    Log.Message("[Sized Apparel] Trying to change" + pawn.Name + "'s body texture.");

                var pawnRenderer = pawn.Drawer?.renderer?.graphics;
                //Log.Message("useBodyTexture");

                /*
                if (!pawnRenderer.AllResolved)
                {
                    pawnRenderer.ResolveAllGraphics();
                }*/
                if (pawnRenderer == null)
                    return;

                if (!comp.hasUnsupportedApparel || SizedApparelUtility.isPawnNaked(pawn) || !drawClothFlag)
                {

                    if (comp.graphicbaseBodyNaked != null)
                    {
                        pawnRenderer.nakedGraphic = comp.graphicbaseBodyNaked.GetColoredVersion(pawnRenderer.nakedGraphic.Shader, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo);

                    }
                    if (comp.graphicbaseBodyRotten != null)
                    {
                        pawnRenderer.rottingGraphic = comp.graphicbaseBodyRotten.GetColoredVersion(pawnRenderer.rottingGraphic.Shader, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo);

                    }/*
                    if (comp.graphicbaseBodyFurCovered != null)
                    {
                        pawnRenderer.furCoveredGraphic = comp.graphicbaseBodyFurCovered.GetColoredVersion(pawnRenderer.furCoveredGraphic.Shader, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo);

                    }*/
                }
                else
                {
                    if (comp.graphicSourceNaked != null)
                        pawnRenderer.nakedGraphic = comp.graphicSourceNaked.GetColoredVersion(pawnRenderer.nakedGraphic.Shader, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo);
                    if (comp.graphicSourceRotten != null)
                        pawnRenderer.rottingGraphic = comp.graphicSourceRotten.GetColoredVersion(pawnRenderer.rottingGraphic.Shader, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo);
                    //if (comp.graphicSourceFurCovered != null)
                    //    pawnRenderer.furCoveredGraphic = comp.graphicSourceFurCovered.GetColoredVersion(pawnRenderer.furCoveredGraphic.Shader, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo);

                }


                return;//no need to replace full body texture anymore.

                /*
                if (SizedApparelSettings.useBodyTexture == false)
                    return;

                //Log.Message("Hello");
                if (pawnGraphicSet == null)
                    return;

                if (pawnGraphicSet.pawn.RaceProps.Humanlike == false)
                    return;

                return;


                //Log.Message("SetBodyGraphic");
                //pawnGraphicSet.ClearCache();
                string path = pawnGraphicSet.pawn.story.bodyType.bodyNakedGraphicPath;
                string path_bodyDessicated = pawnGraphicSet.pawn.story.bodyType.bodyDessicatedGraphicPath;
                string filename;
                string pathname;
                string filename_bodyDessicated;
                string pathname_bodyDessicated;
                pathname = System.IO.Path.GetDirectoryName(path);
                filename = System.IO.Path.GetFileName(path);
                filename_bodyDessicated = System.IO.Path.GetFileName(path_bodyDessicated);
                pathname_bodyDessicated = System.IO.Path.GetDirectoryName(path_bodyDessicated);

                //Log.Message("SetPath");
                bool validBody = false;
                bool validDessicatedBody = false;

                //pawnGraphicSet.pawn.Drawer.renderer.graphics.nakedGraphic
                //pawnGraphicSet.pawn.Drawer.renderer.graphics.dessicatedGraphic

                if (hasUnsupportApparel)
                {
                    //Log.Message("IfhasUnsupportApparel");
                    //Graphic newBodyGraphic = null;
                    //Graphic newRottingGraphic = null;
                    //string path;

                    //Log.Message("tryUnsupportedApparelBodyTexture");
                    //OLD::pawnGraphicSet.pawn.Drawer.renderer.graphics
                    if (pawnGraphicSet.nakedGraphic != null)
                        if (ContentFinder<Texture2D>.Get((pawnGraphicSet.pawn.Drawer.renderer.graphics.nakedGraphic.path + "_UnsupportedApparel" + "_south"), false) != null)
                        {
                            pawnGraphicSet.pawn.Drawer.renderer.graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(pawnGraphicSet.nakedGraphic.path + "_UnsupportedApparel", pawnGraphicSet.nakedGraphic.Shader, pawnGraphicSet.nakedGraphic.drawSize, pawnGraphicSet.nakedGraphic.color, pawnGraphicSet.nakedGraphic.colorTwo);
                            validBody = true;
                        }
                    if (pawnGraphicSet.dessicatedGraphic != null)
                        if (ContentFinder<Texture2D>.Get((pawnGraphicSet.dessicatedGraphic.path + "_UnsupportedApparel" + "_south"), false) != null)
                        {
                            pawnGraphicSet.dessicatedGraphic = GraphicDatabase.Get<Graphic_Multi>(pawnGraphicSet.dessicatedGraphic.path + "_UnsupportedApparel", pawnGraphicSet.rottingGraphic.Shader, pawnGraphicSet.rottingGraphic.drawSize, pawnGraphicSet.rottingGraphic.color, pawnGraphicSet.rottingGraphic.colorTwo);
                            validDessicatedBody = true;
                        }
                }
                else
                {
                    //Log.Message("undo");
                    string currentPath = pawnGraphicSet.nakedGraphic.path;
                    string currentDessicatedPath = pawnGraphicSet.dessicatedGraphic.path;
                    //Log.Message(currentPath.Substring(0,currentPath.Length - "_UnsupportedApparel".Length));
                    if (pawnGraphicSet.nakedGraphic != null)
                        if (ContentFinder<Texture2D>.Get(currentPath.Substring(0, currentPath.Length - "_UnsupportedApparel".Length) + "_south", false) != null)
                        {
                            pawnGraphicSet.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>((currentPath.Substring(0, currentPath.Length - "_UnsupportedApparel".Length)), pawnGraphicSet.nakedGraphic.Shader, pawnGraphicSet.nakedGraphic.drawSize, pawnGraphicSet.nakedGraphic.color, pawnGraphicSet.nakedGraphic.colorTwo);

                        }
                    if (pawnGraphicSet.dessicatedGraphic != null)
                        if (ContentFinder<Texture2D>.Get(currentDessicatedPath.Substring(0, currentDessicatedPath.Length - "_UnsupportedApparel".Length) + "_south", false) != null)
                        {
                            pawnGraphicSet.dessicatedGraphic = GraphicDatabase.Get<Graphic_Multi>((currentDessicatedPath.Substring(0, currentDessicatedPath.Length - "_UnsupportedApparel".Length)), pawnGraphicSet.nakedGraphic.Shader, pawnGraphicSet.nakedGraphic.drawSize, pawnGraphicSet.nakedGraphic.color, pawnGraphicSet.nakedGraphic.colorTwo);

                        }

                }*/
                //pawnGraphicSet.ClearCache();
            }
        }






        private static void GetBreastSeverity(Pawn pawn, out float breastSeverity, out Hediff breastHediff)
        {
            throw new NotImplementedException();
        }
    }


    [Obsolete]  
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(Pawn_ApparelTracker), "ExposeData")]
    class ApparelTrackerExposePatch
    {
        static void Postfix(Pawn_ApparelTracker __instance)
        {
            if (!UnityData.IsInMainThread)
            {
                return;
            }
            if (__instance.pawn == null)
                return;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ApparelRecorderComp comp = __instance.pawn.GetComp<ApparelRecorderComp>();
                if (comp == null)
                    return;
                if (!comp.hasUpdateBefore)
                    comp.Update(true, false);
            }

        }
    }


    //[StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelChanged")]
    //rimworld 1.2 => apparelChanged, rimworld 1.3 => apparelAdded, apparelRemoved
    public class ApparelTrackerPatch
    {
        //Prefix
        public static void Changed(Pawn_ApparelTracker __instance)
        {
            /*
            if (Current.Game.World == null)
                return;
            */
            if (!UnityData.IsInMainThread)
            {
                return;
            }
            if (__instance.pawn == null)
                return;

            if (SizedApparelSettings.Debug)
                Log.Message("[Sized Apparel] " + __instance.pawn.Name + "'s apparels are changed. updating sizedApparels for it.");

            //GetBreastSeverity(__instance.pawn, out breastSeverity, out breastHediff);
            //bool flag = hasUnSupportedApparel(__instance.pawn, breastSeverity, breastHediff);
            ApparelRecorderComp comp = __instance.pawn.GetComp<ApparelRecorderComp>();
            if (comp != null)
            {
                //comp.ClearBreastCacheValue();
                //comp.ClearAll();
                //bool flag = false;
                //SizedApparelUtility.GetBreastSeverity(__instance.pawn, out breastSeverity, out breastHediff);
                //flag = SizedApparelUtility.hasUnSupportedApparel(__instance.pawn, breastSeverity, breastHediff);
                //flag = SizedApparelUtility.hasUnSupportedApparelFromWornData(__instance.pawn, breastSeverity, breastHediff);
                //comp.hasUpdateBefore = true;
                //comp.hasUnsupportedApparel = flag;
                //comp.breastHediff = breastHediff; ;
                //comp.breastSeverity = breastSeverity;
                //comp.Update(true, false);//TODO: Coverd But No Graphic may cause Big Issue!!!
                //comp.Update(true, true);

                comp.SetDirty(false,false,true);

                /*
                if (SizedApparelSettings.drawBodyParts)//old:SizedApparelSettings.useBodyTexture
                    BodyPatch.SetBodyGraphic(__instance.pawn);
                */
                /*
                //Log.Message(comp.testbool.ToString());
                //Log.Message("ApparelChanged");
                //comp.hasUnsupportedApparel = flag;
                //comp.hasUpdateBefore = true;
                //comp.SetHasUnsupportedApparel(flag);
                //comp.SetHasUpdateBefore(true);
                //if (__instance.pawn.Drawer.renderer.graphics != null)


                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    SetBodyGraphic(__instance.pawn.Drawer.renderer.graphics, flag);
                }
                );
                */

            }

        }
    }
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(PawnGraphicSet), "SetApparelGraphicsDirty")]
    class SetApparelGraphicsDirtyPatch
    {
        public static bool Prefix(PawnGraphicSet __instance)
        {
            if (SizedApparelSettings.useBodyTexture)
            {
                //__instance.SetAllGraphicsDirty();
                //return false;
            }
            return true;
        }
    }

    //TODO
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(PawnGraphicSet), "get_apparelGraphics")]
    class PrivatePartsPatch
    {
        public static void Postfix(PawnGraphicSet __instance, ref List<ApparelGraphicRecord> __result)
        {
            var resualtCach = __result;

            if (SizedApparelSettings.drawBodyParts)
            {
                var privateParts = new List<ApparelGraphicRecord>();
                if (SizedApparelSettings.drawPenis)
                {

                }
                if (SizedApparelSettings.drawVagina)
                {

                }
                if (SizedApparelSettings.drawHips)
                {

                }
                if (SizedApparelSettings.drawHips)
                {

                }
                if (SizedApparelSettings.drawAnus)
                {

                }
            }
        }
    }



    //[HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public class MatBodyBaseAtPatch
    {
        public static void Postfix(PawnGraphicSet __instance, Rot4 facing, RotDrawMode bodyCondition, bool drawClothes, List<Material> __result)
        {
            if (__result == null)
                return;
            int num = facing.AsInt + 1000 * (int)bodyCondition;
            List<Material> copy;
            copy = __result.ListFullCopy();
            for (int i = 0;  i<__result.Count; i++)
            {
                //SizedApparelsDatabase.GetSupportedApparelOriginalPath(__result[i].g)
            }
        }

    }


    //Apparel Graphic Texture injection
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    [HarmonyBefore(new string[]{"QualityOfBuilding"})]
    public class GetApparelGraphicFix
    {
        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec, ref bool __result)
        {
            if (__result == false)
                return;

            if (apparel == null)
                return;

            if (bodyType == null)
                return;

            if (apparel.Wearer != null)
            {
                //rec = new ApparelGraphicRecord(null, null);
                var comp = apparel.Wearer.GetComp<ApparelRecorderComp>();

                //if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize)
                //    BreastSeverity = comp.BreastSeverityCache;
                //int currentBreastSizeIndex = 0;
                //float currentBreastSeverity = -1;
                //int minSupportedBreastSizeIndex = 1000;
                //float minSupportedBreastSeverity = 1000;

                //SizedApparelUtility.GetBreastSeverity(apparel.Wearer, out BreastSeverity, out breastHediff);
                if (comp != null)
                {

                    if (comp.hasUpdateBefore == false)
                    {
                        //SizedApparelUtility.GetBreastSeverity(apparel.Wearer, out BreastSeverity, out breastHediff);
                        //comp.hasUnsupportedApparel = SizedApparelUtility.hasUnSupportedApparelFromWornData(apparel.Wearer, BreastSeverity, breastHediff);
                        //comp.breastSeverity = BreastSeverity;
                        //comp.breastHediff = breastHediff;
                        //comp.hasUpdateBefore = true;
                        //comp.Update(true,false);
                    }
                    if (comp.isDirty == true)
                    {
                        //return;
                        //comp.ClearAll();
                        //comp.Update(true, false);
                    }
                    /*
                    if (comp.needToCheckApparelGraphicRecords)
                    {
                        TODO;
                        if (comp.isApparelGraphicRecordChanged())
                            comp.Update(true, false);
                           
                    }*/
                    if (comp.needToCheckApparelGraphicRecords)
                    {
                        /*
                        if (comp.isApparelGraphicRecordChanged())
                        {
                            //return;
                            //comp.Update(true, true); //1.3
                            //SizedApparelUtility.UpdateAllApparel(___pawn, true);
                        }*/
                    }

                    //Log.Message("1");
                    var breastHediff = comp.breastHediff;
                    float BreastSeverity = comp.breastSeverity;
                    //Log.Message("2");

                    if (SizedApparelSettings.useBreastSizeCapForApparels) //SizedApparelSettings.useBreastSizeCapForApparels //wip
                            BreastSeverity = Math.Min(comp.BreastSeverityCache, BreastSeverity);
                    //Log.Message("3");
                    if (comp.hasUnsupportedApparel == false)//&& (comp.bodyPartBreasts.bodyPartGraphic !=null || comp.bodyPartBreasts.bodyPartGraphicHorny != null)
                    {
                        Graphic sizedGraphic = null;
                        //Log.Message("4");
                        string resultPath = SizedApparelsDatabase.GetSupportedApparelSizedPath(new SizedApparelsDatabase.SizedApparelDatabaseKey(rec.graphic.path, apparel?.Wearer?.def.defName, breastHediff?.def?.defName, apparel.Wearer.gender, apparel?.Wearer?.story?.bodyType?.defName, SizedApparelUtility.BreastSeverityInt(BreastSeverity))).pathWithSizeIndex;
                        if(resultPath != null)
                        {
                            //sizedGraphic = SizedApparelUtility.GetSizedApparelGraphic(rec.graphic, BreastSeverity, apparel?.Wearer?.def.defName, breastHediff.def.defName);
                            sizedGraphic = GraphicDatabase.Get<Graphic_Multi>(resultPath, rec.graphic.Shader, rec.graphic.drawSize, rec.graphic.color, rec.graphic.colorTwo);
                        }



                        if(sizedGraphic != null)
                        rec = new ApparelGraphicRecord(sizedGraphic, rec.sourceApparel);

                        //minSupportedBreastSizeIndex = Math.Min(currentBreastSizeIndex, minSupportedBreastSizeIndex);
                        //comp.breastSeverityCapToDraw = Math.Min(comp.breastSeverityCapToDraw, minSupportedBreastSeverity);
                    }
                }

                else
                {
                    if (SizedApparelSettings.Debug)
                        Log.Warning("[Sized Apparel] " + apparel.Wearer.Name + " doesn't have SizedApparel Compoenet!!");

                }
            }
        }
    }
        
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(PawnGraphicSet), "ClearCache")]
    class GraphicSetClearFix
    {
        public static void Postfix(PawnGraphicSet __instance)
        {
            if (__instance.pawn == null)
            {
                return;
            }
            var comp = __instance.pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            //comp.ClearAll(false);
            //comp.needToCheckApparelGraphicRecords = true;
            comp.SetDirty(false,true,false,false,false); // Check Hediff. If you don't the crotch will not have graphic on first load
        }
    }



       
    

    //[HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public class PawnGraphicSetPatch
    {
        public static void Postfix(ref List<Material> __result, ref PawnGraphicSet __instance, Rot4 facing, RotDrawMode bodyCondition)
        {

        }
    }

    //TODO: Patch After RJW (Sexualize_GenerateNewPawnInternal) or just postfix to Sexualize
    [HarmonyPatch(typeof(PawnGenerator), "GenerateNewPawnInternal")]
    public class PawnGeneratorPatch
    {

    }

    [HarmonyPatch(typeof(Corpse), "RotStageChanged")]
    public class RotStagePatch
    {
        public static void Prefix(CompRottable __instance)
        {
            var comp = __instance.parent.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.SetDirty(false,false,false); // should clear graphicSet....?
        }
    }

    //Should I Patch this?
    //[HarmonyPatch(typeof(Pawn_AgeTracker), "RecalculateLifeStageIndex")]
    public class AgePatch
    {
        public static void Postfix(Pawn_AgeTracker __instance, Pawn ___pawn)
        {
            var comp = ___pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.CheckAgeChanged();
        }
    }



    //Styling Station patch..? wip
    //[HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public class BodyMatPatch
    {
        public static void Postfix(PawnGraphicSet __instance, RotDrawMode bodyCondition, Rot4 facing, ref List<Material> __result, bool drawClothes)
        {
            List<Material> loc = new List<Material>();

            if (bodyCondition == RotDrawMode.Dessicated)
            {
                return;
            }

            for (int i = 0; i< __result.Count; i++)
            {
                if (bodyCondition == RotDrawMode.Fresh)
                {
                    if (__result[i] == __instance.nakedGraphic.MatAt(facing, null))
                    {

                        continue;
                    }

                }
                else if (bodyCondition == RotDrawMode.Rotting || __instance.dessicatedGraphic == null)
                {
                    if (__result[i] == __instance.rottingGraphic.MatAt(facing, null))
                    {


                        continue;
                    }
                }

                if (drawClothes)
                {


                }


                loc.Add(__result[i]);
            }

        }
    }
    //RimWorld 1.3
    //WIPWIPWIP TODO
    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt")]
    public class BaseHeadOffsetAtPatch
    {
        public static void Postfix(ref PawnRenderer __instance, Pawn ___pawn, Rot4 rotation, ref Vector3 __result)
        {
            ApparelRecorderComp apparelRecorder = ___pawn.GetComp<ApparelRecorderComp>();
            if (apparelRecorder == null)
                return;
            if (apparelRecorder.customPose != null)
            {
                //var item = apparelRecorder.currentCustomPose.headOffset.FirstOrDefault(b => b.bodyType == ___pawn.story.bodyType.label);
                //__result += item.offsets.GetOffset(rotation);
            }
        }
    }



    //RimWorld 1.3 Only. Not 1.4
    [HarmonyPatch(typeof(PawnRenderer), "DrawPawnBody")]
    public class DrawPawnBodyPatch
    {
        static MethodInfo overrideMatMethod = AccessTools.Method(typeof(PawnRenderer), "OverrideMaterialIfNeeded");


        public static void Prefix(ref PawnRenderer __instance, Pawn ___pawn, PawnRenderFlags flags)
        {
            if (!SizedApparelSettings.drawBodyParts)
                return;
            if (___pawn == null)
                return;
            ApparelRecorderComp apparelRecorder = ___pawn.GetComp<ApparelRecorderComp>();
            if (apparelRecorder == null)
                return;
            //!flags.FlagSet(PawnRenderFlags.StylingStation)&&
            bool flag = false;
            //if (apparelRecorder.needToCheckApparelGraphicRecords && apparelRecorder.isApparelGraphicRecordChanged())
            /*
            if (apparelRecorder.isApparelGraphicRecordChanged())
            {
                flag = true;
            }
            */

            if (false && flags.FlagSet(PawnRenderFlags.StylingStation))//TODO...?
            {
                //apparelRecorder.isDirty is allways true

                if (true)//StylingStation Doesn't work with cache! patch in postfix
                {
                    if(apparelRecorder.recentClothFlag != flags.FlagSet(PawnRenderFlags.Clothes))
                        apparelRecorder.Update(true, true, true, flags.FlagSet(PawnRenderFlags.Clothes));
                    if(flags.FlagSet(PawnRenderFlags.Clothes))
                        SizedApparelUtility.UpdateAllApparel(___pawn, true);
                }


            }
            else
            {
                if ((!apparelRecorder.hasUpdateBefore || apparelRecorder.isDirty))
                {
                    if (SizedApparelSettings.Debug)
                        Log.Message("[SizedApparel] trying to draw " + ___pawn.Name + " with unupdated component or SetDirty! Updating it.");
                    //apparelRecorder.ClearAll();
                    //Todo. Async Update?
                    apparelRecorder.Update(true, true, true, flags.FlagSet(PawnRenderFlags.Clothes));
                    SizedApparelUtility.UpdateAllApparel(___pawn,true);
                }
                if (flag)
                {
                    //apparelRecorder.Update(true, true, true, flags.FlagSet(PawnRenderFlags.Clothes));
                    //apparelRecorder.Update(true, true); 1.3
                    //SizedApparelUtility.UpdateAllApparel(___pawn, true);
                }
            }
            return;
            //don't change body graphic file. it will inject material in PawnGraphicSet.MatsBodyBaseAt
            if (SizedApparelSettings.drawBodyParts)
                BodyPatch.SetBodyGraphic(___pawn, flags.FlagSet(PawnRenderFlags.Clothes), false);
            else
                BodyPatch.SetBodyGraphic(___pawn, flags.FlagSet(PawnRenderFlags.Clothes), false, false);
        }

        public static void Postfix(ref PawnRenderer __instance, Vector3 rootLoc, float angle, Rot4 facing, RotDrawMode bodyDrawType, PawnRenderFlags flags, Pawn ___pawn, Mesh bodyMesh)
        {
            if (___pawn == null)
                return;
            ApparelRecorderComp apparelRecorder = ___pawn.GetComp<ApparelRecorderComp>();
            if (apparelRecorder == null)
                return;
            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);

            if (bodyMesh == null)
                return;

            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);


            string defName = __instance.graphics.pawn.def.defName;

            /*
            Shader shader = ___pawn.Drawer.renderer.graphics.nakedGraphic.Shader;
            Color skinColor = Color.white;
            Color skinColor2 = Color.white;
            if (bodyDrawType == RotDrawMode.Fresh)
            {
                shader = ___pawn.Drawer.renderer.graphics.nakedGraphic.Shader;
                if (!ShaderUtility.SupportsMaskTex(shader))
                    shader = ShaderDatabase.CutoutSkinOverlay;
                skinColor = ___pawn.Drawer.renderer.graphics.nakedGraphic.Color;
                skinColor2 = ___pawn.Drawer.renderer.graphics.nakedGraphic.ColorTwo;
            }
            else if(bodyDrawType == RotDrawMode.Rotting)
            {
                shader = ___pawn.Drawer.renderer.graphics.rottingGraphic.Shader;
                if (!ShaderUtility.SupportsMaskTex(shader))
                    shader = ShaderDatabase.CutoutSkinOverlay;
                skinColor = ___pawn.Drawer.renderer.graphics.rottingGraphic.Color;
                skinColor2 = ___pawn.Drawer.renderer.graphics.rottingGraphic.ColorTwo;
            }*/

            /*
            //FurCoveredColor
            if(bodyDrawType == RotDrawMode.Fresh && __instance.graphics.furCoveredGraphic != null)
            {
                shader = ___pawn.Drawer.renderer.graphics.furCoveredGraphic.Shader;
                if (!ShaderUtility.SupportsMaskTex(shader))
                    shader = ShaderDatabase.CutoutSkinOverlay;
                skinColor = ___pawn.Drawer.renderer.graphics.furCoveredGraphic.Color;
                skinColor2 = ___pawn.Drawer.renderer.graphics.furCoveredGraphic.ColorTwo;
            }*/



            if (flags.FlagSet(PawnRenderFlags.StylingStation))//Styling station doesn't affect to real pawn data. so cannot cache to component.
            {


            }

            //breasts are only rendered when all of worn apparels are supported or nude.

            if (true)//__instance.graphics.furCoveredGraphic == null
            {
                apparelRecorder.UpdateTickAnim(rootLoc, angle);
                apparelRecorder.DrawAllBodyParts(rootLoc, angle, facing, bodyDrawType, flags, ___pawn, bodyMesh);
            }
            //else. the DrawPawnFur will draw bodyparts becaust it should be upper

        }
    }


    //Base Body Graphic Injection
    [HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public class MatsBodyBastAtPatch
    {
        public static void Postfix(ref List<Material> __result, PawnGraphicSet __instance, Rot4 facing, RotDrawMode bodyCondition, bool drawClothes)
        {
            if (!SizedApparelSettings.useBodyTexture)
                return;

            if (__result.NullOrEmpty())
                return;

            if (!SizedApparelUtility.CanApplySizedApparel(__instance.pawn))
                return;

            var comp = __instance.pawn.GetComp<ApparelRecorderComp>();
            if (comp == null) // maybe it can be null? but why...? mechanoids?
                return;

            if (drawClothes)
            {
                if (comp.hasUnsupportedApparel)
                    return;
            }

            Material bodyMat = null;
            Material sizedApparelBaseBodyMat = null;
            switch (bodyCondition)
            {
                case RotDrawMode.Fresh:
                    if(__instance.nakedGraphic != null && comp.graphicbaseBodyNaked != null)
                    {
                        bodyMat = __instance.nakedGraphic.MatAt(facing, null);
                        sizedApparelBaseBodyMat = comp.graphicbaseBodyNaked.MatAt(facing, null);
                    }
                    break;
                case RotDrawMode.Rotting:
                    if (__instance.rottingGraphic != null && comp.graphicbaseBodyRotten != null)
                    {
                        bodyMat = __instance.rottingGraphic.MatAt(facing, null);
                        sizedApparelBaseBodyMat = comp.graphicbaseBodyRotten.MatAt(facing, null);
                    }
                    break;
                case RotDrawMode.Dessicated:
                    return;// //don't inject for Dessicated graphic. it doesn't need to patch for this
                    break;
                default:
                    bodyMat = null;
                    sizedApparelBaseBodyMat = null;
                    break;
            }

            if (sizedApparelBaseBodyMat == null)
                return;

            //the body mat would be in first index but not sure. so search from start
            for (int i = 0; i < __result.Count; i++)
            {
                if(__result[i] == bodyMat)
                {
                    __result[i] = sizedApparelBaseBodyMat;
                    //should inject body part graphics here?
                    break;
                }
            }
            //should do something more? such as add body parts or somthing?
        }
    }


    //TODO
    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt")]
    public class HeadOffsetPatch
    {

        public static void Postfix(PawnRenderer __instance, Pawn ___pawn, Rot4 rotation, ref Vector3 __result)
        {
            var comp = ___pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
        }

    }



}


