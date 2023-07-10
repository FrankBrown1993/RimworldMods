using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using rjw;

namespace SizedApparel
{
    public class BodyTypeAndPath
    {
        public string BodyType;
        public string Path;
    }

    public class ApparelData
    {
        public string WornPath;
        public List<BodyTypeAndPath> Data;
    }
    public class PreDefinedApparelDate : Def
    {
        public string HediffName;
        public bool IsBreasts = false;
        public List<BodyTypeAndPath> Data;
    }
    public class PreDefinedBodyPartGraphicDate : Def
    {

    }

    public static class SizedApparelsDatabase
    {

        public struct BodyGraphicKey
        {

        }

        public static void LoadPreDefinedData()
        {
            
        }



        public struct SizedApparelDatabaseKey
        {
            public string pathWithoutSizeIndex; // Do Not Include Size Data to path! bodytype could be included
            public string raceName;
            public string bodyTypeName;
            public Gender gender;
            public string hediffName;
            public int targetSize;
            public bool isHorny;
            public string customPose;
            public string variation;


            public SizedApparelDatabaseKey(string path, string race, string bodyType = null, Gender genderInput = Gender.None , string hediff = null, int size = -1, bool horny = false, string customPose = null, string variation = null)
            {
                this.pathWithoutSizeIndex = path;
                this.raceName = race;
                this.bodyTypeName = bodyType;
                this.gender = genderInput;
                this.hediffName = hediff;
                this.targetSize = size;
                this.isHorny = horny;
                this.customPose = customPose;
                this.variation = variation;
            }
        }
        public struct SizedApparelDatabaseKeyComparer : IEqualityComparer<SizedApparelDatabaseKey>
        {
            public bool Equals(SizedApparelDatabaseKey x, SizedApparelDatabaseKey y)
            {
                if (x.targetSize != y.targetSize)
                    return false;
                return true && (x.pathWithoutSizeIndex == y.pathWithoutSizeIndex) && (x.bodyTypeName == y.bodyTypeName) && (x.raceName == y.raceName)&& (x.gender == y.gender) && (x.hediffName == y.hediffName) && (x.isHorny == y.isHorny) && (x.customPose == y.customPose) && (x.variation == y.variation);
            }

            public int GetHashCode(SizedApparelDatabaseKey obj)
            {
                return obj.GetHashCode();
            }
        }
        public struct BodyPartDatabaseKey
        {
            public string raceName;
            public string bodyTypeName;
            public string hediffName;
            public string folderPath;
            public Gender gender;
            public int targetSize;
            public bool isHorny;
            public string customPose; // null custom pose as default pose
            public string variation; // null variation as default graphic

            public BodyPartDatabaseKey(string race, string bodyType = null, string hediff = null, string path = null, Gender pawnGender = Gender.None, int size = -1, bool horny = false, string customPose = null, string variation = null)
            {
                this.raceName = race;
                this.bodyTypeName = bodyType;
                this.hediffName = hediff;
                this.folderPath = path;
                this.gender = pawnGender;
                this.targetSize = size;
                this.isHorny = horny;
                this.customPose = customPose;
                this.variation = variation;
            }
        }
        public struct BodyPartDatabaseKeyComparer : IEqualityComparer<BodyPartDatabaseKey>
        {
            public bool Equals(BodyPartDatabaseKey x, BodyPartDatabaseKey y)
            {
                if (x.targetSize != y.targetSize)
                    return false;
                return true && (x.raceName == y.raceName) && (x.bodyTypeName == y.bodyTypeName) && (x.hediffName == y.hediffName) && ( x.folderPath == y.folderPath)&& (x.gender == y.gender) && (x.isHorny == y.isHorny) && (x.customPose == y.customPose) && (x.variation == y.variation);
            }

            public int GetHashCode(BodyPartDatabaseKey obj)
            {
                return obj.GetHashCode();
            }
        }
        public struct PathAndSize
        {
            public string pathWithSizeIndex;
            public int size;
            public bool isUnsupportedHumanlikePath;
            public bool isCustomPose;
            public string hediffName;
            public string bodyType; // useful for bodypart Graphic (body addons).
            public string raceName; // for check is Human or Alien or Alien as Human.
            public SizedApparelTexturePointDef points;

            public PathAndSize(string path, int index, bool unsupportedHumanlike = false, bool customPose = false, string hediff = null, string bodytype = null, string raceName = null, SizedApparelTexturePointDef pointsInput = null)
            {
                this.pathWithSizeIndex = path;
                this.size = index;
                this.isUnsupportedHumanlikePath = unsupportedHumanlike;
                this.isCustomPose = customPose;
                this.hediffName = hediff;
                this.bodyType = bodytype;
                this.points = pointsInput;
                this.raceName = raceName;
            }
        }
        private static Dictionary<Pawn, ApparelRecorderComp> ApparelRecordersCache = new Dictionary<Pawn, ApparelRecorderComp>();

        private static Dictionary<BodyGraphicKey, Graphic> SizedApparelBodyGraphic = new Dictionary<BodyGraphicKey, Graphic>(); // TODO
        private static Dictionary<SizedApparelDatabaseKey, PathAndSize> SupportedApparelResultPath = new Dictionary<SizedApparelDatabaseKey, PathAndSize>(new SizedApparelDatabaseKeyComparer());
        private static Dictionary<BodyPartDatabaseKey, PathAndSize> SupportedBodyPartResultPath = new Dictionary<BodyPartDatabaseKey, PathAndSize>(new BodyPartDatabaseKeyComparer());
        private static Dictionary<string, string> SupportedApparelOriginalPath = new Dictionary<string, string>();

        //AlienRace AllowHumanlike. Need to Restart or Clear cache to change options


        public static ApparelRecorderComp GetApparelCompFast(Pawn pawn)
        {
            if (ApparelRecordersCache.ContainsKey(pawn))
                return ApparelRecordersCache[pawn];
            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>(); // comp can be null
            ApparelRecordersCache.Add(pawn, comp);
            return comp;
        }

        public static Dictionary<string, bool> AlienRaceUseHumanlike = new Dictionary<string, bool>();
        private static void ResetAlienRaceUseHumanlike()
        {
            AlienRaceUseHumanlike.Clear();
            IEnumerable<ThingDef> HumanlikeRaces;
            HumanlikeRaces = DefDatabase<ThingDef>.AllDefs.Where(b =>b.race?.Humanlike == true);

            foreach (ThingDef raceDef in HumanlikeRaces)
            {
                //Default Value Is True
                AlienRaceUseHumanlike.Add(raceDef.defName, true);
            }
        }
        
        [Obsolete]
        public static bool GetAlienRaceUseHumanlike(string raceDef)
        {
            if (AlienRaceUseHumanlike.NullOrEmpty())
            {
                ResetAlienRaceUseHumanlike();
            }
            if (AlienRaceUseHumanlike.ContainsKey(raceDef))
                return AlienRaceUseHumanlike[raceDef];
            return false;  
        }

        public static List<string> GetAlienRacesDefNames()
        {
            if (AlienRaceUseHumanlike.NullOrEmpty())
            {
                ResetAlienRaceUseHumanlike();
            }
            //It must have one or more elements: human.
            return AlienRaceUseHumanlike.Keys.ToList();
        }


        public static void ClearAll()
        {
            SupportedApparelResultPath.Clear();
            SupportedBodyPartResultPath.Clear();
            SupportedApparelOriginalPath.Clear();
            AlienRaceUseHumanlike.Clear();
        }

        //Apparels, Bodyparts can be used
        public static PathAndSize GetSupportedApparelSizedPath(SizedApparelDatabaseKey key)
        {
            int currentSize = -1;
            float currentSeverity = -1;
            return GetSupportedApparelSizedPath(key, out currentSize, out currentSeverity);

        }

        public static string GetSupportedApparelOriginalPath(string path)
        {
            string outString;
            if (SupportedApparelOriginalPath.TryGetValue(path, out outString))
                return outString;
            return null;
        }

        public static Dictionary<string, BodyPartPoint> GetGraphicPoints(string textuerPath)
        {
            //DefDatabase<>
            return null;
        }

        public static PathAndSize GetSupportedApparelSizedPath(SizedApparelDatabaseKey key, out int indexOut, out float currentSeverityOut)
        {
            if (SupportedApparelResultPath.ContainsKey(key))
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] SizedApparelDataBase::ValidKey: "+ key.pathWithoutSizeIndex);
                var value =  SupportedApparelResultPath.TryGetValue(key);
                indexOut = value.size;
                currentSeverityOut = SizedApparelUtility.BreastSizeIndexToSeverity(value.size);
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] SizedApparelDataBase::Result Path: " + value.pathWithSizeIndex);
                return value;
            }


            if (SizedApparelSettings.Debug)
                Log.Message("  [Sized Apparel] SizedApparelDataBase::Key Not Found: " + key.pathWithoutSizeIndex);

            PathAndSize result;
            Graphic sourceGraphic = GraphicDatabase.Get<Graphic_Multi>(key.pathWithoutSizeIndex);
            bool flag;
            bool customPose = true; //default none pose also custom pose
            string hediffResult;
            string targetRaceName;
            //if (GetAlienRaceUseHumanlike(key.raceName))
            //TODO. AlienRaceHumanlike
            /*
            var pawnDef = DefDatabase<SizedApparelPawnDef>.GetNamed(key.raceName);
            if (pawnDef != null)
            {
                if(pawnDef.isHumanlike)
                    targetRaceName = "Humanlike";
            }
                
            else
                targetRaceName = key.raceName;*/


            AlienRaceSetting raceSetting;
            raceSetting = SizedApparelSettings.alienRaceSettings.FirstOrDefault((AlienRaceSetting s) => s.raceName == key.raceName);

            targetRaceName = key.raceName;
            if(raceSetting != null && raceSetting.allowRaceNamedApparel == false)
            {
                targetRaceName = null;
            }

            Graphic graphic = null;
            graphic = SizedApparelUtility.GetSizedApparelGraphic(sourceGraphic, SizedApparelUtility.BreastSizeIndexToSeverity(key.targetSize), out indexOut, out currentSeverityOut, out flag, out hediffResult, targetRaceName, key.hediffName, key.customPose, key.gender);//key.customPose
            if(graphic == null && key.gender != Gender.None)
            {
                //try Genderless
                graphic = SizedApparelUtility.GetSizedApparelGraphic(sourceGraphic, SizedApparelUtility.BreastSizeIndexToSeverity(key.targetSize), out indexOut, out currentSeverityOut, out flag, out hediffResult, targetRaceName, key.hediffName, key.customPose, Gender.None);//key.customPose
            }

            if (key.customPose != null && graphic == null)
            {
                customPose = false;
                graphic = SizedApparelUtility.GetSizedApparelGraphic(sourceGraphic, SizedApparelUtility.BreastSizeIndexToSeverity(key.targetSize), out indexOut, out currentSeverityOut, out flag, out hediffResult , targetRaceName, key.hediffName , null ,key.gender);
                if (graphic == null && key.gender != Gender.None)
                {
                    //try Genderless
                    graphic = SizedApparelUtility.GetSizedApparelGraphic(sourceGraphic, SizedApparelUtility.BreastSizeIndexToSeverity(key.targetSize), out indexOut, out currentSeverityOut, out flag, out hediffResult, targetRaceName, key.hediffName, null, Gender.None);//key.customPose
                }
            }

            //Try Find Different Target Size
            if (flag == true)
            {
                result = new PathAndSize(graphic.path, indexOut, false, customPose, hediffResult, key.bodyTypeName, key.raceName);
                SupportedApparelResultPath.SetOrAdd(key, result);
                SupportedApparelOriginalPath.SetOrAdd(result.pathWithSizeIndex, key.pathWithoutSizeIndex);
                return result;
            }
            else
            {
                //Try Search Human Apparel. then null
                if (key.raceName != "Human" && key.raceName != null) // null check for avoid infinite loop
                {
                    if (raceSetting != null && raceSetting.asHuman)
                    {
                        var newKey = new SizedApparelDatabaseKey(key.pathWithoutSizeIndex, "Human", key.bodyTypeName, key.gender, key.hediffName, key.targetSize, key.isHorny, key.customPose, key.variation);
                        result = GetSupportedApparelSizedPath(newKey, out indexOut, out currentSeverityOut);
                        SupportedApparelResultPath.SetOrAdd(key, result);
                        SupportedApparelOriginalPath.SetOrAdd(key.pathWithoutSizeIndex, key.pathWithoutSizeIndex);
                        return result;
                    }
                    else
                    {
                        var newKey = new SizedApparelDatabaseKey(key.pathWithoutSizeIndex, null, key.bodyTypeName, key.gender, key.hediffName, key.targetSize, key.isHorny, key.customPose, key.variation);
                        result = GetSupportedApparelSizedPath(newKey, out indexOut, out currentSeverityOut);
                        SupportedApparelResultPath.SetOrAdd(key, result);
                        SupportedApparelOriginalPath.SetOrAdd(key.pathWithoutSizeIndex, key.pathWithoutSizeIndex);
                        return result;
                    }
                }
                if (key.raceName == "Human")
                {
                    var newKey = new SizedApparelDatabaseKey(key.pathWithoutSizeIndex, null, key.bodyTypeName, key.gender, key.hediffName, key.targetSize, key.isHorny, key.customPose, key.variation);
                    result = GetSupportedApparelSizedPath(newKey, out indexOut, out currentSeverityOut);
                    SupportedApparelResultPath.SetOrAdd(key, result);
                    SupportedApparelOriginalPath.SetOrAdd(key.pathWithoutSizeIndex, key.pathWithoutSizeIndex);
                    return result;
                }

                result = new PathAndSize(null, -1);
                SupportedApparelResultPath.SetOrAdd(key, result);
                SupportedApparelOriginalPath.SetOrAdd(key.pathWithoutSizeIndex, key.pathWithoutSizeIndex);
                return result;
            }
            return result;
        }

        public static PathAndSize GetSupportedBodyPartPath(BodyPartDatabaseKey key, bool isBreast, string folderName, string defaultHediffName)
        {

            PathAndSize result;
            if (SupportedBodyPartResultPath.ContainsKey(key))
                return SupportedBodyPartResultPath.TryGetValue(key);
            int currentSize = -1;
            string hediffResult;
            Graphic graphic = null;

            //Find Points from result's path
            //TODO: Build SizedApparel DataBase separate?


            if (key.customPose != null)
            {
                graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, key.bodyTypeName, key.hediffName, isBreast, key.targetSize, folderName+"/CustomPose/"+key.customPose, defaultHediffName, out currentSize, out hediffResult, key.isHorny, null, key.variation, key.gender);
                if (graphic != null)
                {
                    result = new PathAndSize(graphic.path, currentSize, false, true, hediffResult, key.bodyTypeName, key.raceName);
                    SupportedBodyPartResultPath.SetOrAdd(key, result);
                    //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                    //if (SizedApparelSettings.Debug && PointsDef != null)
                    //{
                    //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                    //}
                    return result;
                }
                if (key.bodyTypeName != null)
                    graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, null, key.hediffName, isBreast, key.targetSize, folderName + "/CustomPose/" + key.customPose, defaultHediffName, out currentSize, out hediffResult, key.isHorny, null, key.variation, key.gender);
                if (graphic != null)
                {
                    result = new PathAndSize(graphic.path, currentSize, false, true, hediffResult , null, key.raceName);
                    SupportedBodyPartResultPath.SetOrAdd(key, result);
                    //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                    //if (SizedApparelSettings.Debug && PointsDef != null)
                    //{
                    //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                    //}
                    return result;
                }

            }
            graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, key.bodyTypeName, key.hediffName, isBreast, key.targetSize, folderName, defaultHediffName, out currentSize, out hediffResult, key.isHorny, null, key.variation, key.gender);
            if (graphic != null)
            {
                //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                //if (SizedApparelSettings.Debug && PointsDef != null)
                //{
                //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                //}
                result = new PathAndSize(graphic.path, currentSize, false, key.customPose == null ? true : false, hediffResult , key.bodyTypeName, key.raceName);
                SupportedBodyPartResultPath.SetOrAdd(key, result);

                return result;
            }

            if (key.bodyTypeName != null)
                graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, null, key.hediffName, isBreast, key.targetSize, folderName, defaultHediffName, out currentSize, out hediffResult, key.isHorny, null, key.variation, key.gender);
            if (graphic != null)
            {
                //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                //if (SizedApparelSettings.Debug && PointsDef != null)
                //{
                //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                //}
                result = new PathAndSize(graphic.path, currentSize, false, key.customPose == null ? true : false, hediffResult, null, key.raceName);
                SupportedBodyPartResultPath.SetOrAdd(key, result);
                return result;
            }


            //SizedApparelMod.CheckAndLoadAlienRaces();
            //HumanLike Search
            AlienRaceSetting raceSetting;
            raceSetting = SizedApparelSettings.alienRaceSettings.FirstOrDefault((AlienRaceSetting s) => s.raceName == key.raceName);
            if (raceSetting !=null && key.raceName == "Human" || !raceSetting.asHuman) //old: !SizedApparelSettings.UnsupportedRaceToUseHumanlike
            {
                //Cannot find Any result
                result = new PathAndSize(null, -1);
                SupportedBodyPartResultPath.SetOrAdd(key, result);
                return result;
            }

            if (key.customPose != null)
            {
                graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, key.bodyTypeName, key.hediffName, isBreast, key.targetSize, folderName + "/CustomPose/" + key.customPose, defaultHediffName, out currentSize, out hediffResult, key.isHorny, "Human", key.variation, key.gender);
                if (graphic != null)
                {
                    //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                    //if (SizedApparelSettings.Debug && PointsDef != null)
                    //{
                    //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                    //}
                    result = new PathAndSize(graphic.path, currentSize, true, true, hediffResult, key.bodyTypeName);
                    SupportedBodyPartResultPath.SetOrAdd(key, result);
                    return result;
                }
                if (key.bodyTypeName != null)
                    graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, null, key.hediffName, isBreast, key.targetSize, folderName + "/CustomPose/" + key.customPose, defaultHediffName, out currentSize, out hediffResult, key.isHorny, "Human", key.variation, key.gender);
                if (graphic != null)
                {
                    //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                    //if (SizedApparelSettings.Debug && PointsDef != null)
                    //{
                    //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                    //}
                    result = new PathAndSize(graphic.path, currentSize, true, true, hediffResult, null);
                    SupportedBodyPartResultPath.SetOrAdd(key, result);
                    return result;
                }

            }
            graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, key.bodyTypeName, key.hediffName, isBreast, key.targetSize, folderName, defaultHediffName, out currentSize, out hediffResult, key.isHorny, "Human", key.variation, key.gender);
            if (graphic != null)
            {
                //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                //if (SizedApparelSettings.Debug && PointsDef != null)
                //{
                //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                //}
                result = new PathAndSize(graphic.path, currentSize, true, key.customPose == null ? true : false, hediffResult, key.bodyTypeName, "Human");
                SupportedBodyPartResultPath.SetOrAdd(key, result);
                return result;
            }

            if (key.bodyTypeName != null)
                graphic = SizedApparelUtility.GetBodyPartGraphic(key.raceName, null, key.hediffName, isBreast, key.targetSize, folderName, defaultHediffName, out currentSize, out hediffResult, key.isHorny, "Human", key.variation, key.gender);
            if (graphic != null)
            {
                //SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphic.path);
                //if (SizedApparelSettings.Debug && PointsDef != null)
                //{
                //    Log.Message("[SizedApparel] : Points Def Found : " + PointsDef.defName);
                //}
                result = new PathAndSize(graphic.path, currentSize, true, key.customPose == null ? true : false, hediffResult, null,  "Human");
                SupportedBodyPartResultPath.SetOrAdd(key, result);
                return result;
            }



            //Cannot find Any result
            result = new PathAndSize(null, -1);
            SupportedBodyPartResultPath.SetOrAdd(key, result);
            return result;
        }

        public static void RandomPreCacheForBodyParts()
        {
            //variationDef = DefDatabase<SizedApparelBodyPartVariationDef>.AllDefsListForReading;
            BodyPartDatabaseKey key;
            for (int i = 0; i<11; i++)
            {
                key = new BodyPartDatabaseKey("Human", "Female", "Breasts", "Breasts", Gender.Female, i);
                GetSupportedBodyPartPath(key, true, "Breasts", "Breasts");
                key = new BodyPartDatabaseKey("Human", "Thin", "Breasts", "Breasts", Gender.Female, i);
                GetSupportedBodyPartPath(key, true, "Breasts", "Breasts");
            }

            for (int i = 0; i < 6; i++)
            {
                key = new BodyPartDatabaseKey("Human", "Female", "Vagina", "Vagina", Gender.Female, i);
                GetSupportedBodyPartPath(key, false, "Vagina", "Vagina");
                key = new BodyPartDatabaseKey("Human", "Thin", "Vagina", "Vagina", Gender.Female, i);
                GetSupportedBodyPartPath(key, false, "Vagina", "Vagina");
            }

            for (int i = 0; i < 6; i++)
            {
                key = new BodyPartDatabaseKey("Human", "Female", "Anus", "Anus", Gender.Female, i);
                GetSupportedBodyPartPath(key, false, "Anus", "Anus");
                key = new BodyPartDatabaseKey("Human", "Thin", "Anus", "Anus", Gender.Female, i);
                GetSupportedBodyPartPath(key, false, "Anus", "Anus");
                key = new BodyPartDatabaseKey("Human", "Male", "Anus", "Anus", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Anus", "Anus");
                key = new BodyPartDatabaseKey("Human", "Thin", "Anus", "Anus", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Anus", "Anus");
            }
            for (int i = 0; i < 6; i++)
            {
                key = new BodyPartDatabaseKey("Human", "Male", "Penis", "Penis", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");
                key = new BodyPartDatabaseKey("Human", "Male", "Penis", "Penis/Balls", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");

                key = new BodyPartDatabaseKey("Human", "Thin", "Penis", "Penis", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");
                key = new BodyPartDatabaseKey("Human", "Thin", "Penis", "Penis/Balls", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");

                key = new BodyPartDatabaseKey("Human", "Fat", "Penis", "Penis", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");
                key = new BodyPartDatabaseKey("Human", "Fat", "Penis", "Penis/Balls", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");

                key = new BodyPartDatabaseKey("Human", "Hulk", "Penis", "Penis", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");
                key = new BodyPartDatabaseKey("Human", "Hulk", "Penis", "Penis/Balls", Gender.Male, i);
                GetSupportedBodyPartPath(key, false, "Penis", "Penis");

            }

        }


        public static void RandomPreCacheForApparels()
        {
            //random precache should be useful. so it target female and thin body only.
            //don't target male because the male doesn't need breasts textures usually.

            List<string> bodyTypes = new List<string>();
            bodyTypes.Add("Female");
            bodyTypes.Add("Thin");


            // DefDatabase<BodyTypeDef>.AllDefsListForReading
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (thingDef.IsApparel && !thingDef.apparel.wornGraphicPath.NullOrEmpty())
                {
                    
                    for(int i = 0; i<11; i++)
                    {
                        var key = new SizedApparelDatabaseKey(thingDef.apparel.wornGraphicPath, "Human", "Female", Gender.Female, "Breasts", i);
                        GetSupportedApparelSizedPath(key);
                        key = new SizedApparelDatabaseKey(thingDef.apparel.wornGraphicPath, "Human", "Thin", Gender.Female, "Breasts", i);
                        GetSupportedApparelSizedPath(key);
                        //need Humanlike Cache?
                    }

                }
            }


        }


    }
}
