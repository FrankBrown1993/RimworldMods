using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using rjw;

namespace SizedApparel
{
    public static class SizedApparelUtility
    {
        //DefName, BodyTypeName, targetTextureInt(index is breasts hediff)
        //static Dictionary<string, Dictionary<string, List<int>>> sizedApparelSupportCache = new Dictionary<string, Dictionary<string, List<int>>>();


        //those breasts size codes are hard coded. maybe some days, It needs to be fixed
        public static string chestString = "Chest";
        public static string breastsString = "Breasts";

        static string Titanic = "_10";
        static string Colossal = "_9";
        static string Gargantuan = "_8";
        static string Massive = "_7";
        static string Enormous = "_6";
        static string Huge = "_5";
        static string Large = "_4";
        static string Average = "_3";
        static string Small = "_2";
        static string Tiny = "_1";
        static string Nipples = "_0";
        //static String[] size = new string[10] { "_0", "_1", "_2", "_3", "_4", "_5" , "_6", "_7", "_8", "_9"};
        public static string[] breastsSizeStrings = new string[11] { Nipples, Tiny, Small, Average, Large, Huge, Enormous, Massive, Gargantuan, Colossal, Titanic };
        public static string[] commonSizeStrings = new string[6] { Nipples, Tiny, Small, Average, Large, Huge};

        [Obsolete]
        public static int findAvailableSmallerSizeFromSetting(int current)
        {

            int target = current;
            target = Math.Min(target, breastsSizeStrings.Length - 1);
            while (target > 0)
            {
                if (SizedApparelSettings.getUseSettingFromIndex(target) == false)
                    target--;
                else
                    break;
            }
            return target;
        }
        [Obsolete]
        public static int findAvailableBiggerSizeFromSetting(int current)
        {

            int target = current;

            while (target < breastsSizeStrings.Length)
            {
                if (SizedApparelSettings.getUseSettingFromIndex(target) == false)
                    target++;
                else
                    break;
            }
            target = Math.Min(target, breastsSizeStrings.Length - 1);
            return target;
        }
        [Obsolete]
        public static int findAvailableSizeFromSetting(int current, bool findBigger)
        {
            if (findBigger)
                return findAvailableBiggerSizeFromSetting(current);
            else
                return findAvailableSmallerSizeFromSetting(current);
        }


        public static bool GetBreastSeverity(Pawn pawn, out float BreastSeverity, out Hediff breastHediff)
        {

            //string breastsString = 
            ;
            float _breastSeverity = -1;
            Hediff _breastHediff = null;
            bool result = false;

            if (SizedApparelPatch.RJWActive || (SizedApparelPatch.SJWActive&&SizedApparelSettings.useSafeJobBreasts))
            {
                //__instance.pawn.health.hediffSet.HasHediff(Hediff ,BodyPartRecord ,false);
                //__instance.pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Any((BodyPartRecord bpr) => bpr.untranslatedCustomLabel == label || bpr.def.defName == label);

                //--------------------------------------------------------------------------------------------------
                /*
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff != null)
                    {

                        if (hediff.Part != null)
                        {

                            if (hediff.Part.def.defName.Equals(chestString))
                            {
                                if (hediff.def.defName.EndsWith(breastsString))
                                {
                                    //Log.Message("Found Breast Hediff");
                                    _breastSeverity = hediff.Severity;
                                    _breastHediff = hediff;
                                    result = true;
                                    //Log.Message(_breastSeverity.ToString());
                                }
                            }
                        }
                    }
                }*/
                var breastList = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_breastsBPR(pawn));
                Hediff hediff = null;
                if(!breastList.NullOrEmpty())
                    hediff = breastList.FirstOrDefault((Hediff h) => h.def.defName.ToLower().Contains("breast"));
                if(hediff != null)
                {
                    _breastSeverity = hediff.Severity;
                    _breastHediff = hediff;
                    result = true;
                }
            }//Find Breasts in chest
            if(SizedApparelSettings.Debug)
                Log.Message("[Sized Apparel]" + pawn.Name + "'s breasts severity: " + _breastSeverity.ToString());
            BreastSeverity = _breastSeverity;
            breastHediff = _breastHediff;
            return result;
            
        }
        public static Graphic GetSizedApparelGraphic(Graphic sourceGraphic, float breastSeverity, string wearerDefName = null, string breastHediff = null, string customPose = null, Gender gender = Gender.None)
        {
            int currentBreastSizeIndex = -1;
            float currentBreastSeverity = -1;
            string s;
            bool flag = false;
            return GetSizedApparelGraphic(sourceGraphic, breastSeverity, out currentBreastSizeIndex, out currentBreastSeverity, out flag, out s, wearerDefName, breastHediff, customPose, gender);
        }

        public static Graphic GetSizedApparelGraphic(Graphic sourceGraphic, float breastSeverity , out int indexOut, out float severityOut, out bool result, out string hediffResult, string wearerDefName = null, string breastHediffName = null, string customPose = null, Gender gender = Gender.None)
        {
            indexOut = -1;
            severityOut = -1;
            result = false;
            hediffResult = null;

            if (sourceGraphic == null)
            {
                return null;
            }

            
            string path;
            string extraPath = null;
            string raceExtraPath = null;
            string racePath = null;




            //path = agr.sourceApparel.def.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName;
            path = sourceGraphic.path;

            if (customPose != null)
            {
                path = path.Insert(Math.Max(path.LastIndexOf('/'), 0), "/CustomPose/"+ customPose);
            }

            string genderSting;
            if (gender == Gender.Female)
            {
                genderSting = "F";
            }
            if (gender == Gender.Male)
            {
                genderSting = "M";
            }
            else
                genderSting = string.Empty;

            path = path + genderSting;

            if (wearerDefName != null)
                racePath = path + "_" + wearerDefName;
            if (breastHediffName != null)
            {
                extraPath = path + "_" + breastHediffName;
                if (wearerDefName != null)
                    raceExtraPath = path + "_" + wearerDefName + "_" + breastHediffName;
            }






            int offset = 0;
            //int offsetLimit = 10;


            bool validTexture = false;
            Graphic graphic = null;
            bool findBigger = true;  // if false : search smaller first
            string pathString = "";
            while (offset < SizedApparelUtility.breastsSizeStrings.Length)
            {
                if (breastHediffName != null)
                {
                    if(raceExtraPath != null)
                    {
                        pathString = raceExtraPath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, findBigger, ref indexOut, ref severityOut);
                        if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                            validTexture = true;
                            result = true;
                            hediffResult = breastHediffName;
                            //Log.Message(extraPath + BreastSeverityString(breastSeverity, offset, findBigger) + ":Extra Texture Found");
                            break;
                        }
                    }


                    pathString = extraPath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, findBigger, ref indexOut, ref severityOut);
                    if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                        validTexture = true;
                        result = true;
                        hediffResult = breastHediffName;
                        //Log.Message(extraPath + BreastSeverityString(breastSeverity, offset, findBigger) + ":Extra Texture Found");
                        break;
                    }

                }
                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")apparel texture is missing");

                if(racePath != null)
                {
                    pathString = racePath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, findBigger, ref indexOut, ref severityOut);
                    if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                        validTexture = true;
                        result = true;
                        hediffResult = null;
                        //Log.Message(path + BreastSeverityString(breastSeverity, offset, findBigger) + ":Texture Found");
                        break;
                    }
                }


                pathString = path + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, findBigger, ref indexOut, ref severityOut);
                if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                {
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                    graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                    validTexture = true;
                    result = true;
                    hediffResult = null;
                    //Log.Message(path + BreastSeverityString(breastSeverity, offset, findBigger) + ":Texture Found");
                    break;
                }
                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")apparel texture is missing");

                //Log.Warning(path + BreastSeverityString(breastSeverity, offset, findBigger) + ":Texture Not Found. Try bigger Texture.");
                offset++;
                if (indexOut == -1)
                    break;
            }
            if (validTexture == false)
            {
                offset = 0;
                while (offset < SizedApparelUtility.breastsSizeStrings.Length)
                {
                    if (breastHediffName != null)
                    {
                        if (raceExtraPath != null)
                        {
                            pathString = raceExtraPath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, !findBigger, ref indexOut, ref severityOut);
                            if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                            {
                                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                    Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");

                                graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                                //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                                validTexture = true;
                                result = true;
                                hediffResult = breastHediffName;
                                //Log.Message(extraPath + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Extra Texture Found");
                                break;
                            }
                        }

                        pathString = extraPath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, !findBigger, ref indexOut, ref severityOut);
                        if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                            //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                            validTexture = true;
                            result = true;
                            hediffResult = breastHediffName;
                            //Log.Message(extraPath + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Extra Texture Found");
                            break;
                        }
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel]     (" + pathString + ")apparel texture is missing");

                        //Log.Warning(extraPath + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Extra Texture Not Found.");
                    }

                    if(racePath != null)
                    {
                        pathString = racePath + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, !findBigger, ref indexOut, ref severityOut);
                        if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                            //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                            validTexture = true;
                            result = true;
                            hediffResult = null;
                            //Log.Message(path + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Texture Found");
                            break;
                        }
                    }

                    pathString = path + SizedApparelUtility.BreastSeverityString(breastSeverity, offset, !findBigger, ref indexOut, ref severityOut);
                    if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")apparel texture is found");
                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathString, sourceGraphic.Shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                        //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                        validTexture = true;
                        result = true;
                        hediffResult = null;
                        //Log.Message(path + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Texture Found");
                        break;
                    }
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel]     (" + pathString + ")apparel texture is missing");

                    //Log.Warning(path + BreastSeverityString(breastSeverity, offset, !findBigger) + ":Texture Not Found. Try smaller Texture.");
                    offset++;
                    if (indexOut == -1)
                        break;
                }
            }

            if (validTexture == false)
            {

                //Log.Warning(path + BreastSeverityString(breastSeverity, offset, findBigger) + ":Texture Not Found. try smaller instead of bigger .");
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] (" + path + ")apparel texture is not patched::missing texture");

                graphic = sourceGraphic;
            }
            else
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] (" + path + ")apparel texture has been patched");

            }



            //rec = new ApparelGraphicRecord(graphic, rec.sourceApparel);
            result = validTexture;


            //Find Humanlike for Alien. ps. null for search defealt texturepath
            if(result == false & wearerDefName != null)
            {
                GetSizedApparelGraphic(sourceGraphic, breastSeverity, null , breastHediffName, customPose);
            }
            return graphic;
            
        }

        public static bool isPragnencyHediff(Hediff h)
        {

            //1.4
            /*
            //TODO. remove contain pregnancy side effect hediffs
            if (h.def == HediffDefOf.PregnantHuman || h.def == HediffDefOf.Pregnant)
                return true;

            //pawn who Giving birth. the pregnant hediff will be removed when the pawn is in labor
            if (h.def == HediffDefOf.PregnancyLabor || h.def == HediffDefOf.PregnancyLaborPushing)
                return true;
            */
            return h.def.defName.ToLower().Contains("rjw_pregnancy") || h.def.defName.ToLower().EndsWith("pregnant") || h.def.defName.ToLower().EndsWith("pregnancy"); // h.def.defName.ToLower().Contains("pregnancy");
        }
        public static bool isRJWEggHediff(Hediff h)
        {
            var e = h as rjw.Hediff_InsectEgg;
            if (e == null)
                return false;
            return true;
        }

        public static bool isBellyBulgeHediff(Hediff h)
        {
            if (isPragnencyHediff(h))
                return true;
            if (isRJWEggHediff(h))
                return true;
            if (SizedApparelPatch.LicentiaActive)
            {
                if (h.def.defName.ToLower().Contains("cumflation"))
                    return true;
                if (h.def.defName.ToLower().Contains("cumstuffed"))
                    return true;
            }
            return false;


        }

        public static bool isRJWParts(Hediff h)
        {
            return (isBreast(h.def.defName) || Genital_Helper.is_penis(h) || Genital_Helper.is_vagina(h) || isAnus(h.def.defName));
            //return (isBreast(defname)|| isPenis(defname)|| isVagina(defname)|| isAnus(defname));
        }
        public static bool isBreast(string defname)
        {
            string lower = defname.ToLower();
            if (lower.Contains("breast"))
                return true;
            return false;
        }
        public static bool isUdder(string defname)
        {
            string lower = defname.ToLower();
            if (lower.Contains("udder"))
                return true;
            return false;
        }

        [Obsolete]
        public static bool isPenis(string defname)
        {
            string lower = defname.ToLower();
            if (lower.Contains("penis") || lower.Contains("dick") || (lower.Contains("tentacle") || lower.Contains("ovipositorm")))//(lower.Contains("tentacle")&&lower.Contains("penis") is for fertility. not for graphic
                return true;
            return false;
        }
        [Obsolete]
        public static bool isVagina(string defname)
        {
            string lower = defname.ToLower();
            if (lower.Contains("vagina") || lower.Contains("ovipositorf"))
                return true;
            return false;
        }
        public static bool isAnus(string defname)
        {
            string lower = defname.ToLower();
            if (lower.Contains("anus"))
                return true;
            return false;
        }

        //find valid breasts texture to choose apparel.
        public static float GetBreastSeverityValidTextures(Pawn pawn, Hediff hediff, string customDefName = null)
        {
            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return -1;


            if (comp.hasUpdateBefore == false)
            {
                //comp.Update(true, true);
            }

            if (pawn == null)
                return -1;
            if (hediff == null)
                return -1;
            string defName;
            if (customDefName == null)
                defName = pawn.def.defName;
            else
                defName = customDefName;

            const string bodyPartsFolderPath = "SizedApparel/BodyParts/";
            string defaultHediffName = "Breasts";
            string graphicFolderPath = bodyPartsFolderPath + defName + "/" + "Breasts" + "/";
            string fileName;
            string extraFileName;
            string bodyType = null;
            if (pawn.story != null)
                bodyType = pawn.story.bodyType?.defName;
            if (bodyType != null)
            {
                fileName = defaultHediffName + ("_" + bodyType);
                extraFileName = hediff.def.defName + ("_" + bodyType);
            }
            else
            {
                fileName = defaultHediffName;
                extraFileName = hediff.def.defName;
            }
            string path = graphicFolderPath + fileName;
            string extraPath = graphicFolderPath + extraFileName;
            //path = agr.sourceApparel.def.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName;

            //SizedApparelsDatabase.BodyPartDatabaseKey key = new SizedApparelsDatabase.BodyPartDatabaseKey(,)
            //SizedApparelsDatabase.GetSupportedBodyPartPath()

            int offset = 0;
            float targetBreastSeverity = hediff.Severity;

            bool validTexture = false;
            bool findBigger = true;  // if false : search smaller first
            string pathString = "";
            int currentSizeIndex = -1;
            float currentSeverity = -1;
            while (offset < SizedApparelUtility.breastsSizeStrings.Length)
            {
                if (hediff != null)
                {
                     pathString = extraPath + SizedApparelUtility.BreastSeverityString(targetBreastSeverity, offset, findBigger, ref currentSizeIndex, ref currentSeverity);
                     if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                     {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")Breasts texture is found");


                        validTexture = true;
                        break;
                    }
                }

                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")Breasts texture is missing");

                    pathString = path + SizedApparelUtility.BreastSeverityString(targetBreastSeverity, offset, findBigger, ref currentSizeIndex, ref currentSeverity);
                if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                {
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel] (" + pathString + ")Breasts texture is found");

                    validTexture = true;
                    break;
                }
                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")Breasts texture is missing");

                offset++;
                if (currentSizeIndex == -1)
                    break;
            }
            if (validTexture == false)
            {
                offset = 0;
                while (offset < SizedApparelUtility.breastsSizeStrings.Length)
                {
                    if (hediff != null)
                    {
                            pathString = extraPath + SizedApparelUtility.BreastSeverityString(targetBreastSeverity, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);
                        if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathString + ")Breasts texture is found");

                            validTexture = true;
                            break;
                        }
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel]     (" + pathString + ")Breasts texture is missing");

                    }
                    else
                        pathString = path + SizedApparelUtility.BreastSeverityString(targetBreastSeverity, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);
                    if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")Breasts texture is found");

                        validTexture = true;
                        break;
                    }
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel]     (" + pathString + ")Breasts texture is missing");

                    offset++;
                    if (currentSizeIndex == -1)
                        break;
                }
            }

            if (validTexture == false)
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] (" + path + ")Breasts texture is not patched::missing texture");
                return -10;
            }
            else
            {// ValidTextureFound
                return currentSeverity;
            }

        }

        //It's heavy search. Do Not Call this function rapidly
        public static Graphic GetBodyPartGraphic(string raceDefName, string bodyTypeName, string hediffDefName, bool isBreast, int sizeIndex, string folderName, string defaultHediffName, out int indexOut, out string hediffResult, bool hornyGraphic = false, string customRaceDefName = null, string variation = null, Gender gender = Gender.None)
        {
            Graphic graphic = null; //for return
            //rec = new ApparelGraphicRecord(null, null);
            string defName = raceDefName;

            if (customRaceDefName != null)
                defName = customRaceDefName;

            string bodyType = bodyTypeName;
            const string bodyPartsFolderPath = "SizedApparel/BodyParts/";
            string graphicFolderPath;
            string targetFolderName = folderName;
            if (hornyGraphic)
                graphicFolderPath = bodyPartsFolderPath + defName + "/" + targetFolderName + "/Horny/";
            else
                graphicFolderPath = bodyPartsFolderPath + defName + "/" + targetFolderName + "/";
            string fileName;
            string extraFileName = null;
            if (bodyType != null)
            {
                fileName = defaultHediffName + ("_" + bodyType);
                if(hediffDefName != null)
                    extraFileName = hediffDefName + ("_" + bodyType);
            }
            else
            {
                fileName = defaultHediffName;
                if (hediffDefName != null)
                    extraFileName = hediffDefName;
            }
            hediffResult = null;
            //if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize)
            //    BreastSeverity = comp.BreastSeverityCache;
            int currentSizeIndex = -1;
            float currentSeverity = -1;
            //int minSupportedBreastSizeIndex = 1000;
            //float minSupportedBreastSeverity = 1000;

            //SizedApparelUtility.GetBreastSeverity(apparel.Wearer, out BreastSeverity, out breastHediff);

            string genderString;
            if(gender == Gender.Female)
            {
                genderString = "F";
            }
            if (gender == Gender.Male)
            {
                genderString = "M";
            }
            else
                genderString = string.Empty;


            string path = graphicFolderPath + fileName + genderString;
            string extraPath = graphicFolderPath + extraFileName + genderString;
            //path = agr.sourceApparel.def.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName;


            int offset = 0;
            int offsetLimit = 10 ; // = SizedApparelUtility.breastsSizeStrings.Length;

            

            float SeverityCapped;

            if (isBreast)
            {
                SeverityCapped = SizedApparelUtility.BreastSizeIndexToSeverity(sizeIndex);
                offsetLimit = SizedApparelUtility.breastsSizeStrings.Length;
            }
            else
            {
                SeverityCapped = SizedApparelUtility.PrivatePartSizeIndexToSeverity(sizeIndex);
                offsetLimit = SizedApparelUtility.commonSizeStrings.Length;
            }

            bool validTexture = false;

            bool findBigger = true;  // if false : search smaller first
            string pathString = "";
            string pathStringWithVariatione = "";
            while (offset < offsetLimit)
            {
                if (hediffDefName != null)
                {
                    if (isBreast)
                        pathString = extraPath + SizedApparelUtility.BreastSeverityString(SeverityCapped, offset, findBigger, ref currentSizeIndex, ref currentSeverity);
                    else
                        pathString = extraPath + SizedApparelUtility.PrivatePartsSevertyStringNotBreast(SeverityCapped, offset, findBigger, ref currentSizeIndex, ref currentSeverity);

                    if (variation != null)
                    {
                        pathStringWithVariatione = pathString + "_" + variation;
                        if (ContentFinder<Texture2D>.Get((pathStringWithVariatione + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathStringWithVariatione + ")BodyPart texture is found");

                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathStringWithVariatione);
                            validTexture = true;
                            hediffResult = hediffDefName;
                            break;
                        }
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel]     (" + pathStringWithVariatione + ")BodyPart texture is missing");
                    }



                    if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")BodyPart texture is found");


                        //minSupportedBreastSizeIndex = Math.Min(currentBreastSizeIndex, minSupportedBreastSizeIndex);
                        //minSupportedBreastSeverity = Math.Min(currentBreastSeverity, minSupportedBreastSeverity);
                        //graphic = new Graphic();
                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathString);
                        //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                        validTexture = true;
                        hediffResult = hediffDefName;
                        //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Found");
                        break;
                    }
                    //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Not Found.");
                }

                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")BodyPart texture is missing");

                if (isBreast)
                    pathString = path + SizedApparelUtility.BreastSeverityString(SeverityCapped, offset, findBigger, ref currentSizeIndex, ref currentSeverity);
                else
                    pathString = path + SizedApparelUtility.PrivatePartsSevertyStringNotBreast(SeverityCapped, offset, findBigger, ref currentSizeIndex, ref currentSeverity);

                if (variation != null)
                {
                    pathStringWithVariatione = pathString + "_" + variation;
                    if (ContentFinder<Texture2D>.Get((pathStringWithVariatione + "_south"), false) != null) // checking special texture like udder
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathStringWithVariatione + ")BodyPart texture is found");

                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathStringWithVariatione);
                        validTexture = true;
                        hediffResult = hediffDefName;
                        break;
                    }
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel]     (" + pathStringWithVariatione + ")BodyPart texture is missing");
                }

                if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                {
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel] (" + pathString + ")BodyPart texture is found");

                    //minSupportedBreastSizeIndex = Math.Min(currentBreastSizeIndex, minSupportedBreastSizeIndex);
                    //minSupportedBreastSeverity = Math.Min(currentBreastSeverity, minSupportedBreastSeverity);
                    graphic = GraphicDatabase.Get<Graphic_Multi>(pathString);
                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                    validTexture = true;
                    hediffResult = defaultHediffName;
                    //Log.Message(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Found");
                    break;
                }
                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                    Log.Message("[Sized Apparel]     (" + pathString + ")BodyPart texture is missing");

                //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. Try bigger Texture.");
                offset++;
                if (currentSizeIndex == -1)
                    break;
            }
            if (validTexture == false)
            {
                offset = 0;
                while (offset < offsetLimit)
                {
                    if (hediffDefName != null)
                    {
                        if (isBreast)
                            pathString = extraPath + SizedApparelUtility.BreastSeverityString(SeverityCapped, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);
                        else
                            pathString = extraPath + SizedApparelUtility.PrivatePartsSevertyStringNotBreast(SeverityCapped, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);

                        if (variation != null)
                        {
                            pathStringWithVariatione = pathString + "_" + variation;
                            if (ContentFinder<Texture2D>.Get((pathStringWithVariatione + "_south"), false) != null) // checking special texture like udder
                            {
                                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                    Log.Message("[Sized Apparel] (" + pathStringWithVariatione + ")BodyPart texture is found");

                                graphic = GraphicDatabase.Get<Graphic_Multi>(pathStringWithVariatione);
                                validTexture = true;
                                hediffResult = hediffDefName;
                                break;
                            }
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel]     (" + pathStringWithVariatione + ")BodyPart texture is missing");
                        }

                        if (ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathString + ")BodyPart texture is found");

                            //minSupportedBreastSizeIndex = Math.Min(currentBreastSizeIndex, minSupportedBreastSizeIndex);
                            //minSupportedBreastSeverity = Math.Min(currentBreastSeverity, minSupportedBreastSeverity);
                            //graphic = new Graphic();
                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathString);
                            //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                            validTexture = true;
                            hediffResult = hediffDefName;
                            //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Found");
                            break;
                        }
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel]     (" + pathString + ")BodyPart texture is missing");

                        //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Not Found.");
                    }
                    if (isBreast)
                        pathString = path + SizedApparelUtility.BreastSeverityString(SeverityCapped, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);
                    else
                        pathString = path + SizedApparelUtility.PrivatePartsSevertyStringNotBreast(SeverityCapped, offset, !findBigger, ref currentSizeIndex, ref currentSeverity);

                    if (variation != null)
                    {
                        pathStringWithVariatione = pathString + "_" + variation;
                        if (ContentFinder<Texture2D>.Get((pathStringWithVariatione + "_south"), false) != null) // checking special texture like udder
                        {
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] (" + pathStringWithVariatione + ")BodyPart texture is found");

                            graphic = GraphicDatabase.Get<Graphic_Multi>(pathStringWithVariatione);
                            validTexture = true;
                            hediffResult = hediffDefName;
                            break;
                        }
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel]     (" + pathStringWithVariatione + ")BodyPart texture is missing");
                    }

                    if ((ContentFinder<Texture2D>.Get((pathString + "_south"), false) != null))
                    {
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] (" + pathString + ")BodyPart texture is found");

                        //minSupportedBreastSizeIndex = Math.Min(currentBreastSizeIndex, minSupportedBreastSizeIndex);
                        //minSupportedBreastSeverity = Math.Min(currentBreastSeverity, minSupportedBreastSeverity);
                        graphic = GraphicDatabase.Get<Graphic_Multi>(pathString);
                        //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                        validTexture = true;
                        hediffResult = defaultHediffName;
                        //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                        break;
                    }
                    if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                        Log.Message("[Sized Apparel]     (" + pathString + ")BodyPart texture is missing");

                    //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Not Found. Try smaller Texture.");
                    offset++;
                    if (currentSizeIndex == -1)
                        break;
                }
            }

            if (validTexture == false)
            {

                //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. try smaller instead of bigger .");
                //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                //graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, agr.graphic.drawSize, agr.graphic.color);
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] (" + path + ")BodyPart texture is not patched::missing texture");

            }
            else
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] (" + path + ")BodyPart texture has been patched");

            }

            indexOut = currentSizeIndex;
            
            if(graphic == null & gender != Gender.None)
            {
                //try search genderless graphic
                graphic = GetBodyPartGraphic(raceDefName, bodyTypeName, hediffDefName, isBreast, sizeIndex, folderName, defaultHediffName, out indexOut, out hediffResult, hornyGraphic, customRaceDefName, variation, Gender.None);
            }

            if (graphic == null & hornyGraphic == true)
            {
                //try search normal Graphic instead of HornyGraphic
                graphic = GetBodyPartGraphic(raceDefName, bodyTypeName, hediffDefName, isBreast, sizeIndex, folderName, defaultHediffName, out indexOut, out hediffResult, false, customRaceDefName, variation, gender);
            }

            return graphic;

        }


        public static PubicHairDef GetRandomPubicHair()
        {
            return DefDatabase<PubicHairDef>.GetRandom();
        }
        public static PubicHairDef GetPubicHairEmpty()
        {
            return DefDatabase<PubicHairDef>.GetNamed("None");
        }


        public static bool IsHorny(Pawn pawn)
        {
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if(comp != null)
            {
                if (comp.forceHorny == true)
                    return true;
            }

            JobDriver_Sex sexDriver = null;
            if (pawn.jobs != null)
                sexDriver = pawn.jobs?.curDriver as rjw.JobDriver_Sex;
            if (sexDriver != null)
            {
                return true;
            }
            if (pawn.needs == null)
                return false;

            bool flag = (xxx.is_hornyorfrustrated(pawn));
            return flag;
            bool erect = false;

            Need_Sex needSex = null;
            if (pawn.needs != null)
                needSex = pawn.needs.TryGetNeed<Need_Sex>();

            JobDriver_Sex dri = null;
            if (pawn.jobs != null)
                dri = pawn.jobs.curDriver as rjw.JobDriver_Sex;


            //Log.Message("find needSex");
            if (needSex == null)
                return false;

            //Log.Warning(needSex.CurLevel.ToString());
            if (needSex.CurLevel >= needSex.thresh_ahegao() || needSex.CurLevel < needSex.thresh_neutral())
            {
                erect = true;
            }
            

            if (dri != null)
            {
                erect = true;
            }

            return false;
        }


        public static float BreastSizeIndexToSeverity(int index)
        {
            switch (index)
            {
                case -1:
                    return 0;
                case 0:
                    return 0.01f;
                case 1:
                    return 0.02f;
                case 2:
                    return 0.2f;
                case 3:
                    return 0.4f;
                case 4:
                    return 0.6f;
                case 5:
                    return 0.8f;
                case 6:
                    return 1.0f;
                case 7:
                    return 1.2f;
                case 8:
                    return 1.4f;
                case 9:
                    return 1.6f;
                case 10:
                    return 1.8f;

                default:
                    return 0;
            }
        }
        public static float PrivatePartSizeIndexToSeverity(int index)
        {
            switch (index)
            {
                case -1:
                    return 0;
                case 0:
                    return 0.01f;
                case 1:
                    return 0.2f;
                case 2:
                    return 0.4f;
                case 3:
                    return 0.6f;
                case 4:
                    return 0.8f;
                case 5:
                    return 1.01f;
                default:
                    return 0;
            }
        }
        public static int PrivatePartSeverityInt(float Severity)
        {
            if (Severity < 0f)
            {
                return -1;
            }
            else if (Severity < 0.01f)
            {
                return 0;
            }
            else if (Severity < 0.2f)
            {
                return 0;
            }
            else if (Severity < 0.40f)
            {
                return 1;
            }
            else if (Severity < 0.60f)
            {
                return 2;
            }
            else if (Severity < 0.80f)
            {
                return 3;
            }
            else if (Severity < 1.01f)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public static int BreastSeverityInt(float BreastSeverity)
        {
            if (BreastSeverity < 0f)
            {
                return -1;
            }
            if (BreastSeverity < 0.01f)
            {
                return 0;
            }
            else if (BreastSeverity < 0.02f)
            {
                return  0;
            }
            else if (BreastSeverity < 0.2f)
            {
                return  1;
            }
            else if (BreastSeverity < 0.40f)
            {
                return  2;
            }
            else if (BreastSeverity < 0.60f)
            {
                return  3;
            }
            else if (BreastSeverity < 0.80f)
            {
                return 4;
            }
            else if (BreastSeverity < 1.0f)
            {
                return 5;
            }
            else if (BreastSeverity < 1.2f)
            {
                return 6;
            }
            else if (BreastSeverity < 1.4f)
            {
                return 7;
            }
            else if (BreastSeverity < 1.6f)
            {
                return 8;
            }
            else if (BreastSeverity < 1.8f)
            {
                return 9;
            }
            else
            {
                return 10;
            }
        }

        public static string PrivatePartsSevertyStringNotBreast(float severity, int offset, bool findBigger, ref int outTargetIndex, ref float outTargetSeverity)
        {
            int targetIndex = -1;
            float targetSeverity = 0;
            int result = -1;
            if (offset >= 0)
            {
                {
                    if (severity < 0f)//Error Serverity
                    {
                        targetIndex = -1;
                        targetSeverity = 0;
                    }
                    else if (severity < 0.2f)
                    {
                        targetIndex = 0;
                        targetSeverity = 0.01f;
                    }
                    else if (severity < 0.4f)
                    {
                        targetIndex = 1;
                        targetSeverity = 0.2f;
                    }
                    else if (severity < 0.6f)
                    {
                        targetIndex = 2;
                        targetSeverity = 0.4f;
                    }
                    else if (severity < 0.8f)
                    {
                        targetIndex = 3;
                        targetSeverity = 0.6f;
                    }
                    else if (severity < 1.01f)
                    {
                        targetIndex = 4;
                        targetSeverity = 0.80f;
                    }
                    else
                    {
                        targetIndex = 5;
                        targetSeverity = 1.01f;
                    }
                    //if (targetIndex - offset < 0)
                    //    return "_-1";
                }

                if (findBigger)
                {
                    if (breastsSizeStrings.Length - (targetIndex + offset) > 0)
                    {

                        //size.Length< targetIndex + offset
                        /*
                        if (size[targetIndex + offset] != null)
                            return size[targetIndex + offset];
                        */
                        //result = findAvailableBiggerSizeFromSetting(targetIndex + offset);
                        result = (targetIndex + offset);
                        outTargetIndex = result;
                        //targetSeverity = BreastSizeIndexToSeverity(result);
                        targetSeverity = PrivatePartSizeIndexToSeverity(result);
                        outTargetSeverity = targetSeverity;
                        return breastsSizeStrings[result];
                    }
                }
                else
                {
                    if (targetIndex - offset < 0)
                        return "_-1";
                    if (breastsSizeStrings.Length - (targetIndex - offset) > 0)
                    {
                        /*
                        if (size[targetIndex - offset] != null)
                            return size[targetIndex - offset];
                            */
                        //result = findAvailableSmallerSizeFromSetting(targetIndex - offset);
                        result = (targetIndex - offset);
                        outTargetIndex = result;
                        //targetSeverity = BreastSizeIndexToSeverity(result);
                        targetSeverity = PrivatePartSizeIndexToSeverity(result);
                        outTargetSeverity = targetSeverity;
                        return breastsSizeStrings[result];
                    }
                }
            }
            return "_-1";
        }
    

        public static string BreastSeverityString(float BreastSeverity, int offset, bool findBigger)
        {
            int breastResultIndex = 0;
            float breastResultFloat = 0;
            return BreastSeverityString(BreastSeverity, offset, findBigger, ref breastResultIndex, ref breastResultFloat);
        }

        

        public static string BreastSeverityString(float BreastSeverity, int offset, bool findBigger, ref int outTargetIndex, ref float outTargetSeverity)
        {
            int targetIndex = -1;
            float targetSeverity = 0;
            int result = -1;
            if (offset >= 0)
            {
                {
                    if (BreastSeverity < 0f)//Error Serverity
                    {
                        targetIndex = -1;
                        targetSeverity = 0;
                    }
                    else if (BreastSeverity < 0.02f)
                    {
                        targetIndex = 0;
                        //targetIndex = findAvailableSmallerSizeFromSetting(0);
                        targetSeverity = 0.01f;
                    }
                    else if (BreastSeverity < 0.2f)
                    {
                        targetIndex = 1;
                        //targetIndex = findAvailableSmallerSizeFromSetting(1);
                        targetSeverity = 0.02f;
                    }
                    else if (BreastSeverity < 0.40f)
                    {
                        targetIndex = 2;
                        //targetIndex = findAvailableSmallerSizeFromSetting(2);
                        targetSeverity = 0.2f;
                    }
                    else if (BreastSeverity < 0.60f)
                    {
                        targetIndex = 3;
                        //targetIndex = findAvailableSmallerSizeFromSetting(3);
                        targetSeverity = 0.40f;
                    }
                    else if (BreastSeverity < 0.80f)
                    {
                        targetIndex = 4;
                        //targetIndex = findAvailableSmallerSizeFromSetting(4);
                        targetSeverity = 0.60f;
                    }
                    else if (BreastSeverity < 1.0f)
                    {
                        targetIndex = 5;
                        //targetIndex = findAvailableSmallerSizeFromSetting(5);
                        targetSeverity = 0.80f;
                    }
                    else if (BreastSeverity < 1.2f)
                    {
                        targetIndex = 6;
                        //targetIndex = findAvailableSmallerSizeFromSetting(6);
                        targetSeverity = 1.0f;
                    }
                    else if (BreastSeverity < 1.4f)
                    {
                        targetIndex = 7;
                        //targetIndex = findAvailableSmallerSizeFromSetting(7);
                        targetSeverity = 1.2f;
                    }
                    else if (BreastSeverity < 1.6f)
                    {
                        targetIndex = 8;
                        //targetIndex = findAvailableSmallerSizeFromSetting(8);
                        targetSeverity = 1.4f;
                    }
                    else if (BreastSeverity < 1.8f)
                    {
                        targetIndex = 9;
                        //targetIndex = findAvailableSmallerSizeFromSetting(9);
                        targetSeverity = 1.6f;
                    }
                    else
                    {
                        targetIndex = 10;
                        //targetIndex = findAvailableSmallerSizeFromSetting(10);
                        targetSeverity = 1.8f;
                    }
                    //if (targetIndex - offset < 0)
                    //    return "_-1";
                }

                if (findBigger)
                {
                    if (breastsSizeStrings.Length - (targetIndex + offset) > 0)
                    {

                        //size.Length< targetIndex + offset
                        /*
                        if (size[targetIndex + offset] != null)
                            return size[targetIndex + offset];
                        */
                        result = findAvailableBiggerSizeFromSetting(targetIndex + offset);
                        outTargetIndex = result;
                        targetSeverity = BreastSizeIndexToSeverity(result);
                        outTargetSeverity = targetSeverity;
                        return breastsSizeStrings[result];
                    }
                }
                else
                {
                    if (targetIndex - offset < 0)
                        return "_-1";
                    if (breastsSizeStrings.Length - (targetIndex - offset) > 0)
                    {
                        /*
                        if (size[targetIndex - offset] != null)
                            return size[targetIndex - offset];
                            */
                        result = findAvailableSmallerSizeFromSetting(targetIndex - offset);
                        outTargetIndex = result;
                        targetSeverity = BreastSizeIndexToSeverity(result);
                        outTargetSeverity = targetSeverity;
                        return breastsSizeStrings[result];
                    }
                }
            }
            return "_-1";
        }

        public static bool isPawnNaked(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.Clothes, bool fromGraphicRecord = true)
        {
            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
            if (fromGraphicRecord)
            {
                if (pawn.Drawer?.renderer?.graphics?.apparelGraphics == null)
                    return true;
                foreach (ApparelGraphicRecord ap in pawn.Drawer.renderer.graphics.apparelGraphics)//Apparel ap in pawn.apparel.WornApparel
                {
                    foreach (BodyPartGroupDef bpgd in ap.sourceApparel.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        if (bpgd.defName == "Chest" || bpgd.defName == "Torso")
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (pawn.apparel == null)
                    return true;
                foreach (Apparel ap in pawn.apparel.WornApparel)
                {
                    foreach (BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        if (bpgd.defName == "Chest" || bpgd.defName == "Torso")
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }






        //for displayed apparel only
        [Obsolete]
        public static bool hasUnSupportedApparel(Pawn pawn, float BreastSeverity, Hediff breastHediff)//only check chest and torso for now
        {

            /*
            ApparelRecorderComp apparelRecorder = pawn.GetComp<ApparelRecorderComp>();
            if(apparelRecorder == null)
            {
                Log.Message("No ApparelRecorder Found");
                //Log.Message("Add ApparelRecorder");
                apparelRecorder = new ApparelRecorderComp();
                pawn.AllComps.Add(apparelRecorder);
                apparelRecorder.Initialize(new ApparelRecorderCompProperties());
                

            }
            */
            //Log.Message("Check hasUnSupportedApparel");
            bool hasUnsupportedApparel = false;

            foreach (ApparelGraphicRecord ap in pawn.Drawer.renderer.graphics.apparelGraphics)//Apparel ap in pawn.apparel.WornApparel
            {
                bool isChest = false;
                foreach (BodyPartGroupDef bpgd in ap.sourceApparel.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                {
                    if (bpgd.defName == "Chest" || bpgd.defName == "Torso")
                    {
                        isChest = true;
                        break;
                    }
                }
                if (isChest)
                {
                    string path;
                    string extraPath;
                    string searchingPath;
                    int offset = 0;
                    int currentIndex = 0;
                    float currentSeverity = 0;
                    //path = agr.sourceApparel.def.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName;
                    //path = agr.graphic.path;
                    //path = ap.def.apparel.wornGraphicPath + "_" + pawn.story.bodyType.defName;
                    if (pawn.story?.bodyType?.defName != null)
                        path = ap.sourceApparel.def.apparel.wornGraphicPath + "_" + pawn.story.bodyType.defName;
                    else
                        path = ap.sourceApparel.def.apparel.wornGraphicPath;
                    if (breastHediff != null)
                    {
                        extraPath = path + "_" + breastHediff.def.defName;
                    }
                    else
                        extraPath = path;

                    bool validTexture = false;

                    bool findBigger = true;  // if false : search smaller first
                    while (offset < breastsSizeStrings.Length)
                    {
                        if (breastHediff != null)
                        {
                            searchingPath = extraPath + BreastSeverityString(BreastSeverity, offset, findBigger,ref currentIndex,ref currentSeverity) + "_south";
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);

                            if (ContentFinder<Texture2D>.Get((searchingPath), false) != null) // checking special texture like udder
                            {
                                validTexture = true;
                                //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Found");
                                break;
                            }
                            //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Not Found.");
                        }
                        searchingPath = path + BreastSeverityString(BreastSeverity, offset, findBigger, ref currentIndex,ref currentSeverity) + "_south";
                        if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                            Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                        if ((ContentFinder<Texture2D>.Get((searchingPath), false) != null))
                        {
                            validTexture = true;
                            //Log.Message(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Found");
                            break;
                        }
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. Try bigger Texture.");
                        offset++;

                    }

                    if (validTexture == false)
                    {
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. try smaller instead of bigger .");
                    }


                    if (validTexture == false)
                    {
                        offset = 0;
                        while (offset < breastsSizeStrings.Length)
                        {
                            if (breastHediff != null)
                            {
                                searchingPath = extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger, ref currentIndex,ref currentSeverity) + "_south";
                                if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                    Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                                if (ContentFinder<Texture2D>.Get((searchingPath), false) != null) // checking special texture like udder
                                {
                                    validTexture = true;
                                    //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Found");
                                    break;
                                }
                                //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Not Found.");
                            }
                            searchingPath = path + BreastSeverityString(BreastSeverity, offset, !findBigger, ref currentIndex, ref currentSeverity) + "_south";
                            if (SizedApparelSettings.Debug && SizedApparelSettings.DetailLog)
                                Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                            if ((ContentFinder<Texture2D>.Get((searchingPath), false) != null))
                            {
                                validTexture = true;
                                //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                break;
                            }
                            //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Not Found. Try smaller Texture.");
                            offset++;
                        }
                    }

                    if (validTexture == false)
                    {
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. hide breasts .");
                        //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                        //graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, agr.graphic.drawSize, agr.graphic.color);
                        hasUnsupportedApparel = true;
                        if (SizedApparelSettings.Debug == false)
                            break;
                        Log.Warning("[Sized Apparel] " + pawn.Name + "'s Unsupported Apparel: " + path);
                    }

                }


            }

            //apparelRecorder.SetHasUnsupportedApparel(hasUnsupportedApparel);
            if (SizedApparelSettings.Debug)
                Log.Message("[Sized Apparel] "+pawn.Name + " has unsupported apparel?: " + hasUnsupportedApparel.ToString());
            return hasUnsupportedApparel;
        }

        public static bool hasUnSupportedApparelFromWornData(Pawn pawn, float BreastSeverity, Hediff breastHediff, bool cacheToComp = false, bool fromGraphicRecord = false)//only check chest and torso for now
        {
            if (SizedApparelSettings.Debug)
                Log.Message("[Sized Apparel] " + pawn.Name + ": hasUnSupportedApparel Testing...");
            /*
            ApparelRecorderComp apparelRecorder = pawn.GetComp<ApparelRecorderComp>();
            if(apparelRecorder == null)
            {
                Log.Message("No ApparelRecorder Found");
                //Log.Message("Add ApparelRecorder");
                apparelRecorder = new ApparelRecorderComp();
                pawn.AllComps.Add(apparelRecorder);
                apparelRecorder.Initialize(new ApparelRecorderCompProperties());
                

            }
            */
            //Log.Message("Check hasUnSupportedApparel");
            bool hasUnsupportedApparel = false;

            /*
            if (!pawn.RaceProps.Humanlike) //Animals or Mechanoids would have apparels?
                return true;
            */

            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;
            List<Apparel> apparels = new List<Apparel>();
            if (!fromGraphicRecord)
            {
                foreach (Apparel worn in pawn.apparel.WornApparel)
                {
                    //Only apparel that has graphic
                    if(worn.Graphic!=null)
                        apparels.Add(worn);
                }
            }

            /*
            if (isPawnNaked(pawn, fromGraphicRecord))
            {
                if (cacheToComp)
                    comp.hasUnsupportedApparel = false;
                return false;
            }*/

            else
            {
                foreach (ApparelGraphicRecord agr in pawn.Drawer.renderer.graphics.apparelGraphics)
                {
                    apparels.Add(agr.sourceApparel);
                }
            }
            if(cacheToComp)
                comp.BreastSeverityCache = 1000;

            foreach (Apparel ap in apparels)//Apparel ap in pawn.apparel.WornApparel
            {
                bool isChest = false;

                if (ap.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_IgnorBreastSize".ToLower()))//skip tags
                    continue;
                
                foreach (BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                {
                    if (bpgd.defName == "Chest" || bpgd.defName == "Torso")
                    {
                        isChest = true;
                        break;
                    }
                }

               
                //isChest = (ap.def.apparel.bodyPartGroups.First((BodyPartGroupDef bpgd) => bpgd.defName == "Chest" || bpgd.defName == "Torso")!=null);
                if (isChest)
                {
                    /////TODO: caching Apparel Check Data
                    /*
                    if (!sizedApparelSupportCache.ContainsKey(ap.def.defName))
                        sizedApparelSupportCache.Add(ap.def.defName, new Dictionary<string, List<int>>());
                    var apparelCache = sizedApparelSupportCache[ap.def.defName];
                    if (!apparelCache.ContainsKey(pawn.story.bodyType.defName))
                        apparelCache.Add(pawn.story.bodyType.defName, new List<int>());
                    var apparelBodyTypeCache = apparelCache[pawn.story.bodyType.defName];
                    int breastSeverityInt = BreastSeverityInt(BreastSeverity);
                    if (apparelBodyTypeCache.Count <= breastSeverityInt)
                    {

                    }*/


                    string path;
                    string extraPath;
                    string searchingPath;
                    int offset = 0;
                    int currentIndex = 0;
                    float currentSeverity = BreastSeverity;
                    //path = agr.sourceApparel.def.apparel.wornGraphicPath + "_" + __instance.pawn.story.bodyType.defName;
                    //path = agr.graphic.path;
                    //path = ap.def.apparel.wornGraphicPath + "_" + pawn.story.bodyType.defName;
                    path = ap.WornGraphicPath + "_" + pawn.story.bodyType.defName;
                    if (breastHediff != null)
                    {
                        extraPath = path + "_" + breastHediff.def.defName;
                    }
                    else
                        extraPath = path;
                    string resultPath = null;
                    bool resultOut = false;
                    var key = new SizedApparelsDatabase.SizedApparelDatabaseKey(path, pawn.def.defName, pawn.story?.bodyType?.defName, pawn.gender, breastHediff?.def.defName, SizedApparelUtility.BreastSeverityInt(BreastSeverity));
                    if (SizedApparelSettings.useGenderSpecificTexture)
                        key.gender = Gender.None;
                    var result = SizedApparelsDatabase.GetSupportedApparelSizedPath(key,out currentIndex, out currentSeverity).pathWithSizeIndex;
                    if (comp != null)
                    {
                        if (cacheToComp)
                        {
                            if (SizedApparelSettings.Debug)
                                Log.Message("[Sized Apparel]  apparel's breasts severity" + currentSeverity);
                            if (SizedApparelSettings.ForcedSupportApparel)
                                comp.BreastSeverityCache = currentSeverity;
                            else
                                comp.BreastSeverityCache = Math.Min(currentSeverity, comp.BreastSeverityCache);
                        }
                    }
                    if (result == null)
                    {
                        if (!SizedApparelSettings.ForcedSupportApparel)
                        {
                            hasUnsupportedApparel = true;
                            break;
                        }
                    }
                    continue;

                    bool validTexture = false;

                    bool findBigger = true;  // if false : search smaller first
                    while (offset < breastsSizeStrings.Length)
                    {
                        if (breastHediff != null)
                        {
                            searchingPath = extraPath + BreastSeverityString(BreastSeverity, offset, findBigger, ref currentIndex, ref currentSeverity) + "_south";
                            if (SizedApparelSettings.Debug)
                                Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                            if (ContentFinder<Texture2D>.Get((searchingPath), false) != null) // checking special texture like udder
                            {
                                validTexture = true;
                                //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Found");
                                if (comp != null)
                                {
                                    if (cacheToComp)
                                    {
                                        if (SizedApparelSettings.Debug)
                                            Log.Message("[Sized Apparel] apparel's breasts severity" + currentSeverity);
                                        comp.BreastSeverityCache = Math.Min(currentSeverity, comp.BreastSeverityCache);
                                    }
                                }
                                break;
                            }
                            //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Extra Texture Not Found.");
                        }
                        searchingPath = path + BreastSeverityString(BreastSeverity, offset, findBigger, ref currentIndex, ref currentSeverity) + "_south";
                        if (SizedApparelSettings.Debug)
                            Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                        if ((ContentFinder<Texture2D>.Get((searchingPath), false) != null))
                        {
                            validTexture = true;
                            //Log.Message(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Found");
                            if (comp != null)
                            {
                                if (cacheToComp)
                                {
                                    if (SizedApparelSettings.Debug)
                                        Log.Message("[Sized Apparel] apparel's breasts severity" + currentSeverity);
                                    comp.BreastSeverityCache = Math.Min(currentSeverity, comp.BreastSeverityCache);
                                }
                            }
                            break;
                        }
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. Try bigger Texture.");
                        offset++;

                    }

                    if (validTexture == false)
                    {
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. try smaller instead of bigger .");
                    }


                    if (validTexture == false)
                    {
                        offset = 0;
                        while (offset < breastsSizeStrings.Length)
                        {
                            if (breastHediff != null)
                            {
                                searchingPath = extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger, ref currentIndex, ref currentSeverity) + "_south";
                                if (SizedApparelSettings.Debug)
                                    Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                                if (ContentFinder<Texture2D>.Get((searchingPath), false) != null) // checking special texture like udder
                                {
                                    validTexture = true;
                                    //Log.Message(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Found");
                                    if (comp != null)
                                    {
                                        if (cacheToComp)
                                        {
                                            if (SizedApparelSettings.Debug)
                                                Log.Message("[Sized Apparel] apparel's breasts severity" + currentSeverity);
                                            comp.BreastSeverityCache = Math.Min(currentSeverity, comp.BreastSeverityCache);
                                        }
                                    }
                                    break;
                                }
                                //Log.Warning(extraPath + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Extra Texture Not Found.");
                            }
                            searchingPath = (path + BreastSeverityString(BreastSeverity, offset, !findBigger, ref currentIndex, ref currentSeverity) + "_south");
                            if (SizedApparelSettings.Debug)
                                Log.Message("[Sized Apparel] " + pawn.Name + ": ApparelSearching: " + searchingPath);
                            if ((ContentFinder<Texture2D>.Get(searchingPath, false) != null))
                            {
                                validTexture = true;
                                //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                if (comp != null)
                                {
                                    if (cacheToComp)
                                    {
                                        if (SizedApparelSettings.Debug)
                                            Log.Message("[Sized Apparel] apparel's breasts severity" + currentSeverity);
                                        comp.BreastSeverityCache = Math.Min(currentSeverity, comp.BreastSeverityCache);
                                    }
                                }
                                break;
                            }
                            //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Not Found. Try smaller Texture.");
                            offset++;
                        }
                    }

                    if (validTexture == false)
                    {
                        //Log.Warning(path + BreastSeverityString(BreastSeverity, offset, findBigger) + ":Texture Not Found. hide breasts .");
                        //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                        //graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, agr.graphic.drawSize, agr.graphic.color);
                        hasUnsupportedApparel = true;

                        if (SizedApparelSettings.Debug == false)
                            break;
                        Log.Warning("[Sized Apparel] " + pawn.Name + "'s Unsupported Apparel: " + path);
                    }

                }


            }

            //apparelRecorder.SetHasUnsupportedApparel(hasUnsupportedApparel);
            if (SizedApparelSettings.Debug)
            {
                Log.Message("[Sized Apparel]" + pawn.Name + " has unsupported apparel?: " + hasUnsupportedApparel.ToString());
                if(cacheToComp)
                    Log.Message("[Sized Apparel] cached breasts severity" + comp.BreastSeverityCache);
            }




            return hasUnsupportedApparel;
        }

        public static void UpdateAllApparel(Pawn pawn, bool onlyGraphicRecords = false)//need to be update before call it
        {

            if (pawn == null)
                return;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            PawnGraphicSet pawnGraphicSet = pawn.Drawer?.renderer?.graphics;
            if (pawnGraphicSet == null)
                return;
            if (pawnGraphicSet.apparelGraphics.NullOrEmpty())
                return;
            if(onlyGraphicRecords)
            {
                if (pawn.story == null)
                    return;

                //List<ApparelGraphicRecord> copy = pawnGraphicSet.apparelGraphics.ToList();
                List<ApparelGraphicRecord> loc = new List<ApparelGraphicRecord>();
                for (int i = 0; i< pawnGraphicSet.apparelGraphics.Count; i++)
                {
                    ApparelGraphicRecord agr = pawnGraphicSet.apparelGraphics[i];
                    ApparelGraphicRecord graphicOut;

                    BodyTypeDef bodyType = null;
                    if (pawn.story != null)
                        bodyType = pawn.story.bodyType;

                    if (ApparelGraphicRecordGetter.TryGetGraphicApparel(agr.sourceApparel, bodyType, out graphicOut))
                    {
                        //agr.graphic = graphicOut.graphic;
                        loc.Add(graphicOut);
                    }
                    //

                    //bool flag = false;
                    //GetApparelGraphicFix.Postfix(agr.sourceApparel, pawn.story.bodyType, ref agr, ref flag);
                    //agr.graphic = graphicOut.graphic;
                }
                pawnGraphicSet.apparelGraphics = loc;

                return;
            }

            //pawnGraphicSet.ResolveApparelGraphics();
        }


        public static bool CanDrawBreasts(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None, bool fromGraphicRecord = true)
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;
            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
            if (comp.hasUnsupportedApparel)
                return isPawnNaked(pawn, flags, fromGraphicRecord);
            return true;
        }

        public static bool CanDrawPenis(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None, bool fromGraphicRecord = true)//Notion: like pants, there is apparel with no graphic but still cover penis.
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;
            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
            if (fromGraphicRecord)
            {
                if (pawn.Drawer?.renderer?.graphics?.apparelGraphics == null)
                    return true;
                foreach (ApparelGraphicRecord ap in pawn.Drawer.renderer.graphics.apparelGraphics)//Apparel ap in pawn.apparel.WornApparel
                {
                    foreach (BodyPartGroupDef bpgd in ap.sourceApparel.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        
                        if (ap.sourceApparel.def.apparel.CoversBodyPart(Genital_Helper.get_genitalsBPR(pawn)) || ap.sourceApparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                        {
                            if (ap.sourceApparel.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_ShowPrivateCrotch".ToLower()))
                            {
                                //if (SizedApparelSettings.Debug)
                                //    Log.Message("[SizedApparel]" + pawn.Name + "'s Genitals has coverd but showing for graphic. apparel:" + ap.sourceApparel.def.defName);
                                continue;
                            }
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (pawn.apparel == null)
                    return true;
                foreach (Apparel ap in pawn.apparel.WornApparel)
                {
                    foreach (BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        if (ap.def.apparel.CoversBodyPart(Genital_Helper.get_genitalsBPR(pawn)) || ap.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                        {
                            if (ap.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_ShowPrivateCrotch".ToLower()))
                            {
                                //if (SizedApparelSettings.Debug)
                                //    Log.Message("[SizedApparel]" + pawn.Name + "'s Genitals has coverd but showing for graphic. apparel:" + ap.def.defName);
                                continue;
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool CanDrawVagina(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None)
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;

            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
            return true;
        }

        public static bool CanDrawPubicHair(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None)
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;

            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
            return true;
        }

        public static bool CanDrawAnus(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None)
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;
            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;

            return true;
        }

        public static bool CanDrawUdder(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None)//TODO
        {

            return false;

            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;
        }

        public static bool CanDrawBelly(Pawn pawn, PawnRenderFlags flags = PawnRenderFlags.None, bool fromGraphicRecord = true)
        {
            if (pawn == null)
                return false;
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return false;

            if (!flags.FlagSet(PawnRenderFlags.Clothes))
                return true;

            if (fromGraphicRecord)
            {
                if (pawn.Drawer?.renderer?.graphics?.apparelGraphics == null)
                    return true;
                foreach (ApparelGraphicRecord ap in pawn.Drawer.renderer.graphics.apparelGraphics)//Apparel ap in pawn.apparel.WornApparel
                {
                    if (ap.sourceApparel.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_ShowBelly".ToLower()))
                        continue;
                    foreach (BodyPartGroupDef bpgd in ap.sourceApparel.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        if (bpgd.defName == "Torso")
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (pawn.apparel == null)
                    return true;
                foreach (Apparel ap in pawn.apparel.WornApparel)
                {
                    if (ap.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_ShowBelly".ToLower()))
                        continue;
                    foreach (BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups)//BodyPartGroupDef bpgd in ap.def.apparel.bodyPartGroups
                    {
                        if (bpgd.defName == "Torso")
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
            
        }


        public static bool CanApplySizedApparel(Pawn pawn)
        {
            if (pawn == null)
                return false;

            //RaceProps Check
            if (!pawn.RaceProps.Humanlike) //apply humanlikes always
            {
                if (pawn.RaceProps.Animal)
                {
                    if (!SizedApparelSettings.ApplyAnimals)
                        return false;
                }
                else if (pawn.RaceProps.IsMechanoid)
                {
                    if (!SizedApparelSettings.ApplyMechanoid)
                        return false;
                }
            }
            else
            {
                if (!SizedApparelSettings.ApplyHumanlikes)
                    return false;
            }
            //Faction Check TODO
            if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
            {
                switch (pawn.Faction.PlayerRelationKind)
                {
                    case FactionRelationKind.Neutral:
                        break;
                    case FactionRelationKind.Ally:
                        break;
                    case FactionRelationKind.Hostile:
                        break;
                    default:
                        break;
                }
            }
            return true;
        }


        public static bool CanPoseApparels(Pawn pawn, string targetPose, string currentHediffName = null, int currentSeverityInt = 0, int cappedSeverityInt = 1000)
        {
            if (pawn == null)
                return false;
            foreach (ApparelGraphicRecord agr in pawn.Drawer.renderer.graphics.apparelGraphics)
            {
                if (agr.graphic == null)
                    continue;
                /*
                if (!agr.sourceApparel.def.apparel.bodyPartGroups.Any(bpgd => bpgd.defName == "Torso" || bpgd.defName == "Chest"))
                    continue;

                if (agr.sourceApparel.def.apparel.tags.Any(s => s.ToLower() == "SizedApparel_IgnorePose".ToLower()))
                    continue;
                */
                //Only Check Torso Apparel Only
                if (!agr.sourceApparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
                    continue;

                string originalPath = SizedApparelsDatabase.GetSupportedApparelOriginalPath(agr.graphic.path);
                if (originalPath == null)
                    return false;

                int outInt = -1;
                float outFloat = -1;
                SizedApparelsDatabase.SizedApparelDatabaseKey key = new SizedApparelsDatabase.SizedApparelDatabaseKey(originalPath, pawn.def.defName, pawn.story?.bodyType?.defName, pawn.gender, currentHediffName, Math.Min(currentSeverityInt, cappedSeverityInt), false, targetPose);
                if (SizedApparelSettings.useGenderSpecificTexture)
                    key.gender = Gender.None;
                var result = SizedApparelsDatabase.GetSupportedApparelSizedPath(key, out outInt, out outFloat);
                if (!result.isCustomPose)
                    return false;
            }
            return true;
        }






        
    }
}
