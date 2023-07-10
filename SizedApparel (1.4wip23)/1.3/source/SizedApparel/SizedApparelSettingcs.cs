using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using Verse;

namespace SizedApparel
{
    //include Human, set humanlike's custom setting
    //Only few settings are allow in ModSetting. other needs to be set in Def (xml) file

    public class AlienRaceSetting : IExposable
    {
        public string raceName = null;
        public bool overrideDef = false;
        public bool asHuman = false;
        public float drawMinAge = -1; //pawn's Biological age. -1 to ignore.

        public AlienRaceSetting(string raceName)
        {
            this.raceName = raceName;
        }
        public AlienRaceSetting()
        {

        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref raceName, "raceName", null);
            Scribe_Values.Look(ref overrideDef, "overrideDef", false);
            Scribe_Values.Look(ref asHuman, "asHuman", false);
            Scribe_Values.Look(ref drawMinAge, "drawMinAge", -1);
        }

        //public int ageYoung = -1; //use receDefName_young folder. -1 to ignore
        //public int ageOld = -1; //use raceDefName_old folder. -1 to ignore.
    }


    public class SizedApparelSettings : ModSettings
    {

        public static bool Debug = false;
        public static bool DetailLog = false;
        public static bool autoClearCacheOnWriteSetting = true;

        public static bool PreCacheOnLoad = true;
        public static float PreCacheRandomFactor = 0.5f;

        public static bool useBodyTexture = true;//for user who not use rimnudeworld

        public static bool useGenderSpecificTexture = true;

        public static bool matchBodyTextureToMinimumApparelSize = true;//for avoiding breasts cliping

        public static bool useBreastSizeCapForApparels = true;

        public static bool DontReplaceBodyTextureOnNude = true;
        //public static bool DontReplaceBodyTextureOnUnsupported = true;



        //Apply Target Pawn Category
        public static bool ApplyHumanlikes = true; //Always true. 
        public static bool ApplyAnimals = true;
        public static bool ApplyAnimalsPlayerFactionOnly = true; //TODO
        public static bool ApplyMechanoid = true;

        public static bool ApplyApparelPatchForMale = false; //only ApparelServerityWork.

        public static bool ApplyColonists = true;
        public static bool ApplySlaves = true;
        public static bool ApplyPrisoner = true;
        public static bool ApplyNeutralAndAlly = true;
        public static bool ApplyHostile = false;


        public static bool AnimationPatch = true;

            //TODO: Standalone render bodyparts.
        public static bool drawBodyParts = true;//for user who not use rimnudeworld
        public static bool drawBreasts = true;
        public static bool drawPenis = true;
        public static bool drawVagina = true;
        public static bool drawMuscleOverlay = true;
        public static bool drawHips = true;//TODO
        public static bool drawAnus = true;
        public static bool drawBelly = false;//TODO
        public static bool drawUdder = false;//TODO
        public static bool drawPubicHair = true;
        public static bool hideBallOfFuta = false;
        public static bool hidePenisOfMale = false;
        public static bool matchBreastToSupportedApparelSize = true;//for avoiding breasts cliping

        public static bool useBodyPartsVariation = true;
        public static bool showBodyPartsVariation = true;

        public static bool breastsPhysics = false;

        //RimNudeWorld
        public static bool drawSizedApparelBreastsOnlyWorn = false;
        public static bool hideRimNudeWorldBreasts = false;//disable rimnudeworld breasts.

        [Obsolete]
        public static bool useUnsupportedBodyTexture = true;//bodytexture when wearing unsupported apparel.
        public static bool useSafeJobBreasts = true;

        public static bool changeBodyType;
        public static bool fatToFemale;
        public static bool hulkToThin;

        public static bool onlyForFemale = true;
        public static bool useRandomSize = true;//for user who play without rimjobworld
        public static float randomSizeMin = 0.01f;
        public static float randomSizeMax = 1.01f;

        //Lagacy Variable
        public static bool useTitanic = true;
        public static bool useColossal = true;
        public static bool useGargantuan = true;
        public static bool useMassive = true;
        public static bool useEnormous = true;
        public static bool useHuge = true;
        public static bool useLarge = true;
        public static bool useAverage = true;
        public static bool useSmall = true;
        public static bool useTiny = true;
        public static bool useNipples = true;



        //Alien Race Settings
        [Obsolete]
        public static bool UnsupportedRaceToUseHumanlike = false;
        public static List<string> alienRaces = new List<string>();
        public static List<string> alienRacesAllowHumanlikTextures = new List<string>(); //This Value Will be Saved and loaded
        public static string alienRaceSearch = "";
        public static Vector2 alienRacesListScrollPos;
        public static List<AlienRaceSetting> alienRaceSettings = new List<AlienRaceSetting>();

        //Force All Apparel as supported
        public static bool ForcedSupportApparel = false;



        public static bool getUseSettingFromIndex(int target)
        {
            if (target < 0)
                return false;
            else if (target == 0)
                return useNipples;
            else if (target == 1)
                return useTiny;
            else if (target == 2)
                return useSmall;
            else if (target == 3)
                return useAverage;
            else if (target == 4)
                return useLarge;
            else if (target == 5)
                return useHuge;
            else if (target == 6)
                return useEnormous;
            else if (target == 7)
                return useMassive;
            else if (target == 8)
                return useGargantuan;
            else if (target == 9)
                return useColossal;
            else if (target == 10)
                return useTitanic;
            else
                return false;
        }

        
       

        public static bool useUnderBreasts = true;
        public static float UnderBreastsOffset = -0.0013f;

        public override void ExposeData()
        {
            
            Scribe_Values.Look(ref Debug, "Debug", false);
            Scribe_Values.Look(ref DetailLog, "DetailLog", false);
            Scribe_Values.Look(ref autoClearCacheOnWriteSetting, "autoClearCacheOnWriteSetting", true);

            //force to use it. this is important thing.
            //Scribe_Values.Look(ref useBreastSizeCapForApparels, "useBreastSizeCapForApparels", true);

            //Apply Categories.
            Scribe_Values.Look(ref ApplyAnimals, "ApplyAnimals", true);
            Scribe_Values.Look(ref ApplyHumanlikes, "ApplyHumanlikes", true);
            Scribe_Values.Look(ref ApplyMechanoid, "ApplyMechanoid", true);

            Scribe_Values.Look(ref AnimationPatch, "AnimationPatch", true);

            Scribe_Values.Look(ref useBodyTexture, "useBodyTexture", true);
            Scribe_Values.Look(ref useGenderSpecificTexture, "useGenderSpecificTexture", true);

            Scribe_Values.Look(ref matchBodyTextureToMinimumApparelSize, "matchBodyTextureToMinimumApparelSize", true);
            Scribe_Values.Look(ref matchBreastToSupportedApparelSize, "matchBreastToSupportedApparelSize", true);

            //Unsurpported to forced Surpported
            //Scribe_Values.Look(ref UnsupportedRaceToUseHumanlike, "UnsupportedRaceToUseHumanlike", false);
            Scribe_Values.Look(ref ForcedSupportApparel, "ForcedSupportApparel", false);

            
            Scribe_Collections.Look<AlienRaceSetting>(ref alienRaceSettings, "alienSettings", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (alienRaceSettings == null)
                {
                    alienRaceSettings = new List<AlienRaceSetting>();
                }

                SizedApparelMod.CheckAndLoadAlienRaces();

                //Initialize Setting for missing Race
                foreach (var raceName in SizedApparelMod.alienDefList)
                {
                    AlienRaceSetting raceSetting = null;
                    foreach (var r in SizedApparelSettings.alienRaceSettings)
                    {
                        if (r.raceName == null)
                            continue;
                        if (raceName == r.raceName)
                        {
                            raceSetting = r;
                            break;
                        }
                    }
                    if (raceSetting == null)
                    {
                        raceSetting = new AlienRaceSetting(raceName);
                        SizedApparelSettings.alienRaceSettings.Add(raceSetting);
                    }
                }



            }
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                if(!alienRaceSettings.NullOrEmpty())
                    alienRaceSettings.RemoveAll((AlienRaceSetting x) => string.IsNullOrEmpty(x.raceName));
            }

            Scribe_Values.Look(ref useUnsupportedBodyTexture, "useUnsupportedBodyTexture", true);
            Scribe_Values.Look(ref DontReplaceBodyTextureOnNude, "DontReplaceBodyTextureOnNude", false);

            Scribe_Values.Look(ref hideRimNudeWorldBreasts, "hideRimNudeWorldBreasts", false);
            Scribe_Values.Look(ref useSafeJobBreasts, "useSafeJobBreasts", true);

            Scribe_Values.Look(ref useRandomSize, "useRandomSize", true);
            Scribe_Values.Look(ref randomSizeMin, "randomSizeMin", 0.01f);
            Scribe_Values.Look(ref randomSizeMax, "randomSizeMax", 1.01f);

            Scribe_Values.Look(ref drawBodyParts, "drawBodyParts", true);
            Scribe_Values.Look(ref useBodyPartsVariation, "useBodyPartsVariation", true); // forse true for now. TODO
            Scribe_Values.Look(ref showBodyPartsVariation, "showBodyPartsVariation", true);

            Scribe_Values.Look(ref drawMuscleOverlay, "drawMuscleOverlay", true);
            Scribe_Values.Look(ref drawBreasts, "drawBreasts", true);
            Scribe_Values.Look(ref drawSizedApparelBreastsOnlyWorn, "drawSizedApparelBreastsOnlyWorn", false);
            Scribe_Values.Look(ref drawPenis, "drawPenis", true);
            Scribe_Values.Look(ref drawVagina, "drawVagina", true);
            Scribe_Values.Look(ref drawAnus, "drawAnus", true);
            Scribe_Values.Look(ref drawUdder, "drawUdder", true);
            Scribe_Values.Look(ref drawBelly, "drawBelly", true);
            Scribe_Values.Look(ref drawPubicHair, "drawPubicHair", true);


            //force to draw all size type
            /*
            Scribe_Values.Look(ref useTitanic, "useTitanic", true);
            Scribe_Values.Look(ref useColossal, "useColossal", true);
            Scribe_Values.Look(ref useGargantuan, "useGargantuan", true);
            Scribe_Values.Look(ref useMassive, "useMassive", true);
            Scribe_Values.Look(ref useEnormous, "useEnormous", true);
            Scribe_Values.Look(ref useHuge, "useHuge", true);
            Scribe_Values.Look(ref useLarge, "useLarge", true);
            Scribe_Values.Look(ref useAverage, "useAverage", true);
            Scribe_Values.Look(ref useSmall, "useSmall", true);
            Scribe_Values.Look(ref useTiny, "useTiny", true);
            Scribe_Values.Look(ref useNipples, "useNipples", true);
            */

            Scribe_Values.Look(ref useUnderBreasts, "useUnderBreasts",true);
            Scribe_Values.Look(ref UnderBreastsOffset, "UnderBreastsOffset", -0.0013f);

            //TODO: Humanlike Setting Per Race
            //Scribe_Values.Look(ref alienRacesAllowHumanlikTextures, "alienRacesAllowHumanlikTextures");

            //BreastsPhysics
            Scribe_Values.Look(ref breastsPhysics, "breastsPhysics", false);


            base.ExposeData();
        }

    }

    public class SizedApparelMod : Mod
    {

        SizedApparelSettings settings;
        private static Vector2 ScrollPosL = Vector2.zero;
        private static Vector2 ScrollPosR = Vector2.zero;
        public static List<string> alienDefList = new List<string>(); // to load aliens and compare with modsetting

        public override void WriteSettings()
        {
            base.WriteSettings();
            if(SizedApparelSettings.autoClearCacheOnWriteSetting)
                ClearCache();
        }

        public static void CheckAndLoadAlienRaces()
        {
            if(alienDefList == null)
                alienDefList = new List<string>();
            if (true)//alienDefList.Count == 0
            {
                IEnumerable<ThingDef> HumanlikeRaces;
                HumanlikeRaces = DefDatabase<ThingDef>.AllDefs.Where(b => b.race?.Humanlike == true);

                foreach (ThingDef raceDef in HumanlikeRaces)
                {
                    //Default Value Is True
                    alienDefList.Add(raceDef.defName);
                }
            }
        }

        public static void ClearCache(bool clearPawnGraphicSet = true)
        {
            SizedApparelsDatabase.ClearAll();

            if (Find.CurrentMap != null)
            {
                foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns)
                {
                    if (pawn == null)
                        continue;
                    var comp = pawn.GetComp<ApparelRecorderComp>();
                    if (comp != null)
                    {
                        comp.UpdateRaceSettingData();
                        comp.SetDirty(clearPawnGraphicSet,true,true,true);
                    }

                }
            }
        }




        public SizedApparelMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<SizedApparelSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {

            const float alienRaceSettingHeight = 120;

            //CheckAndLoadAlienRaces();
            Listing_Standard listingStandard = new Listing_Standard();
            //Rect rect = new Rect(0f, 0f, inRect.width, 950);
            //Rect rect = inRect.ExpandedBy(0.9f);
            Rect leftRect = new Rect(inRect.position, new Vector2(inRect.width / 2, inRect.height));
            Rect rightRect = new Rect(inRect.position + new Vector2(inRect.width / 2,0), new Vector2(inRect.width / 2, inRect.height));
            //rect.xMax *= 0.9f;
            //leftRect = leftRect.ContractedBy(10f);
            rightRect = rightRect.ContractedBy(10f);
            Rect scrollRect = new Rect(0, 0, leftRect.width - 30f, Math.Max(leftRect.height + (float)alienDefList.Count* alienRaceSettingHeight, 1f));
            Widgets.BeginScrollView(leftRect, ref ScrollPosL, scrollRect, true);
            leftRect = new Rect(leftRect.x, leftRect.y, leftRect.width - 30f, leftRect.height + Math.Max((float)alienDefList.Count * alienRaceSettingHeight, 1f) + 250f);
            listingStandard.Begin(leftRect);
            listingStandard.maxOneColumn = true;
            
            listingStandard.CheckboxLabeled("Debug Log", ref SizedApparelSettings.Debug, "Debug logs.\nDefault: false");
            if (SizedApparelSettings.Debug)
            {
                listingStandard.CheckboxLabeled("Debug Log (Detail)", ref SizedApparelSettings.DetailLog, "Debug logs for checking missing textures.\nDefault: false");

            }
            listingStandard.GapLine(1f);
            listingStandard.CheckboxLabeled("Auto Clear Cache On Close Setting", ref SizedApparelSettings.autoClearCacheOnWriteSetting, "Auto clear cache data to apply Setting.\nDefault: true");
            if(SizedApparelSettings.autoClearCacheOnWriteSetting == false)
            {
                listingStandard.Label("If you changed options, try Clear Cache to apply. Some options may need to restart");
                if (listingStandard.ButtonTextLabeled("Clear Cache", "Press If you changed Option."))//\n do not push unless you really need. \n if this button still not work. reload savefile. if still not work, reload rimworld"
                {
                    ClearCache();
                }
                if (listingStandard.ButtonTextLabeled("RandomPreCache", "15 sec ~ 2 min"))
                {
                    SizedApparelsDatabase.RandomPreCacheForApparels();
                    SizedApparelsDatabase.RandomPreCacheForBodyParts();
                }
            }


            listingStandard.Gap();
            listingStandard.GapLine();





            /*
            listingStandard.Label("Optimization",-1,"");
            //listingStandard.CheckboxLabeled("Debug Log", ref SizedApparelSettings.Debug, "Debug logs.\nDefault: false");
            listingStandard.Gap(8);
            listingStandard.Label("no more optimization setting yet.\n It's hardly optimized already :)", -1, "");
            listingStandard.GapLine(5f);
            */
            //listingStandard.CheckboxLabeled("Use Breast Size Cap from Apparels(recommended: true)", ref SizedApparelSettings.useBreastSizeCapForApparels, "unite breast size from apparels. Smallest value will be used.\nIf you change this option, you need to redress Pawn\nDefault: true");


            //listingStandard.BeginScrollView(rect, ref ScrollPos, ref rect);
            //listingStandard.Label("Body(Torso)Texture Option", -1, "");
            //listingStandard.CheckboxLabeled("use Body (Torso) Texture", ref SizedApparelSettings.useBodyTexture, "change body texture if the pawn is wearing supported apparels only.\nIt may override bodytexture you use.\nIf you use rimnudeworld, don't use this option.\nDefault: false");
            //if (SizedApparelSettings.useBodyTexture == true)
            //{
            //    listingStandard.CheckboxLabeled("  use unsupportedApparel Body Texture", ref SizedApparelSettings.useUnsupportedBodyTexture, "Use \"_UnsupportedApparel\" Body Texture when pawn is wearing unsupported apparel.\nIf this option is false, the body will be default texture.\nDefault: true");
            //    listingStandard.CheckboxLabeled("  Match BodyTexture To Minimum ApparelSize", ref SizedApparelSettings.matchBodyTextureToMinimumApparelSize, "Avoid Clipping When breasts bigger than supported sized apparel.\nDefault: true");
            //
            //}
            listingStandard.Gap(8);
            listingStandard.Label("Other Mod Compatibility");
            listingStandard.Gap(8);
            listingStandard.CheckboxLabeled(" Force ignore unsupported apparel", ref SizedApparelSettings.ForcedSupportApparel, "It may have clipping issue from unsupported parts.\nDefault: false");


            /*
            if(SizedApparelPatch.DubsApparelTweaksActive == true)
            {
                listingStandard.Label("   Dubs Apparel Tweaks Patched! (may not work in 1.3)");
                listingStandard.Gap(8);
            }*/

            //sizeList.EndScrollView(ref rect);
            //listingStandard.EndSection(sizeList);
            //listingStandard.EndScrollView(ref sizeScrollRect);
            listingStandard.Label("Non RimJobWorld Compatibility (wip)", -1, "User Who play without RimJobWorld");
            if (!SizedApparelPatch.RJWActive)
            {
                if (SizedApparelPatch.SJWActive)
                {
                    listingStandard.Label("  SafeJobWorld is Actived ", -1, "");
                    listingStandard.CheckboxLabeled("     use SafeJobWorld's Breasts(Hidden to player but it exist)", ref SizedApparelSettings.useSafeJobBreasts, "use BreastsSize from SJW.\nDefault: true");

                }
                if(SizedApparelPatch.SJWActive? SizedApparelSettings.useSafeJobBreasts==false : true)
                {
                    listingStandard.CheckboxLabeled("       use Random Breasts Size(not yet work)", ref SizedApparelSettings.useRandomSize, "use breasts random size for pawn.\nDefault: true");
                    listingStandard.Label("     random Size Min: " + SizedApparelSettings.UnderBreastsOffset.ToString(), -1, "Defualt: 0.01");
                    SizedApparelSettings.randomSizeMin = listingStandard.Slider(SizedApparelSettings.randomSizeMin, 0f, 2f);
                    listingStandard.Label("     random Size Max: " + SizedApparelSettings.UnderBreastsOffset.ToString(), -1, "Defualt: 1.00");
                    SizedApparelSettings.randomSizeMax = listingStandard.Slider(SizedApparelSettings.randomSizeMax, SizedApparelSettings.randomSizeMin, 2f);

                }
                if (SizedApparelPatch.SJWActive == false)
                {


                }
            }
            else
            {
                //RimJobWorld is Actived
                listingStandard.Label("  RimJobWorld is Actived ", -1, "");

                if (SizedApparelPatch.MenstruationActive)
                {
                    listingStandard.Label("     RJW Menstuation is Active ", -1, "");
                }
                else
                {
                    listingStandard.Label("     RJW Menstuation is Not Active", -1, "");
                }


            }
            listingStandard.GapLine(1);
            listingStandard.Gap(12);
            listingStandard.Label("AlienRace Compatibility (wip)", -1, "");

            //listingStandard.CheckboxLabeled("  Unsupported race render as \"Humanlike\" ", ref SizedApparelSettings.UnsupportedRaceToUseHumanlike, "If unchecked, unsupported humanlike race will not be patched!\nIf you change this option, you need to restart rimworld or clear cache\nDefault: false");





            if (SizedApparelPatch.alienRaceActive)
            {
                listingStandard.Label("  AlienRace is Actived ", -1, "");
                //listingStandard.CheckboxLabeled("Force to use Human's BodyParts for unsuported Alien races", null, "");

                //TODO: Allow Humanlike Settings Per Races
                /*
                SizedApparelSettings.alienRaces = SizedApparelsDatabase.GetAlienRacesDefNames();

                listingStandard.Label("Alien Race Settings", -1f, null);

                SizedApparelSettings.alienRaceSearch = listingStandard.TextEntry(SizedApparelSettings.alienRaceSearch, 1);
                RaceSettings(new Rect(0f, 60f, 16f, 300f), SizedApparelSettings.alienRaces, ref SizedApparelSettings.alienRacesAllowHumanlikTextures, null);

                listingStandard.Gap(listingStandard.verticalSpacing);
                */
                /*
                Widgets.BeginScrollView(aliensRect, ref ScrollPos, leftRect);
                List<bool> alienSettingListLoc = new List<bool>();
                foreach(var item in SizedApparelsDatabase.AlienRaceUseHumanlike)
                {
                    //Widgets.CheckboxLabeled(aliensRect, item.Key, ref);
                }
                Widgets.EndScrollView();
                */
            }
            else
            {
                listingStandard.Label("  AlienRace is not Actived ", -1, "");
            }

            if (alienDefList.NullOrEmpty())
                CheckAndLoadAlienRaces();

            float h = alienDefList.Count * alienRaceSettingHeight;
            Listing_Standard Race_ListingStandard = listingStandard.BeginSection(h);
            foreach (var raceName in alienDefList)
            {
                Race_ListingStandard.Label(raceName);
                Race_ListingStandard.GapLine(1f);
                AlienRaceSetting raceSetting = null;
                foreach (var r in SizedApparelSettings.alienRaceSettings)
                {
                    if (r.raceName == null)
                        continue;
                    if (raceName == r.raceName)
                    {
                        raceSetting = r;
                        break;
                    }
                }
                if (raceSetting == null)
                {
                    raceSetting = new AlienRaceSetting(raceName);

                    SizedApparelSettings.alienRaceSettings.Add(raceSetting);
                }
                if(raceName != "Human")
                    Race_ListingStandard.CheckboxLabeled("If Unsupported, As Human race", ref raceSetting.asHuman, "This Race will use Human race if it doesn't have own textures. useful for race that Unsupported but simillar to human");
                Race_ListingStandard.Label((raceSetting.drawMinAge <= 100 ? "" : "[overdrive]") + "SA BodyPart Draw Min Age: " + raceSetting.drawMinAge.ToString(), -1, "Lower than this age will not use Sized Apparel. Useful for BnC. -1 for disable");
                raceSetting.drawMinAge = Mathf.Round(Race_ListingStandard.Slider(raceSetting.drawMinAge, raceSetting.drawMinAge <= 100 ? -1: 100, raceSetting.drawMinAge >= 100 ? 1000 : 100));
                Race_ListingStandard.Gap();
            }

            listingStandard.EndSection(Race_ListingStandard);
            Widgets.EndScrollView();
            //listingStandard.Gap(alienSettingHeight);
            listingStandard.Gap(8);
            listingStandard.GapLine(4f);

            listingStandard.Gap(8);
            listingStandard.Gap(8);
            listingStandard.Label("RimNudeWorld Compatibility (WIP)", -1, "");

            //listingStandard.CheckboxLabeled("  Don't Replace Body Texture On Nude", ref SizedApparelSettings.DontReplaceBodyTextureOnNude, "Only Replace BodyTexture On Not Nude. Trigers are Torso And Chests.\nDefault: False");
            if (SizedApparelPatch.rimNudeWorldActive)
            {
                listingStandard.Label("  RimNudeWorld is On!. Please check SizedApparel's Body parts render option", -1, "disable all body parts render except the breasts. and set it to only worn option true");
                listingStandard.CheckboxLabeled(" use Sized Apparel Breasts graphic when worn.", ref SizedApparelSettings.drawSizedApparelBreastsOnlyWorn, "use Sized Apparel's breasts render for breasts of apparel. it will be hidden when the pawn is naked and rimnudeworld will handle nude. \ndefault = true;");
                //listingStandard.CheckboxLabeled("  Hide RimNudeWorld Breasts Addon", ref SizedApparelSettings.hideRimNudeWorldBreasts, "For User Who Use Body(Torso) Texture option, remove double drawn breasts.\nYou can use this option as only using Rimnudeworld genital and else without breasts.\nDefault: False");

                if (listingStandard.ButtonTextLabeled("Easy Setting Button", "Apply Rimnude Setting"))
                {
                    SizedApparelSettings.drawSizedApparelBreastsOnlyWorn = true;
                    SizedApparelSettings.useBodyTexture = false;
                    SizedApparelSettings.drawBreasts = true;
                    SizedApparelSettings.drawPenis = false;
                    SizedApparelSettings.drawVagina = false;
                    SizedApparelSettings.drawAnus = false;
                    SizedApparelSettings.drawBelly = false;
                    SizedApparelSettings.drawPubicHair = false;
                }


                if (false)//SizedApparelSettings.hideRimNudeWorldBreasts == false
                {
                    //listingStandard.CheckboxLabeled("  match Breast Texture To Minimum ApparelSize (not work)", ref SizedApparelSettings.matchBreastTextureToMinimumApparelSize, "Avoid Clipping When breasts bigger than supported sized apparel.\nDefault: true");

                    //listingStandard.CheckboxLabeled("  use Under Breasts addon (RimNudeWorld)(not recomanded)", ref SizedApparelSettings.useUnderBreasts, "draw breasts under apparel.");
                    listingStandard.Label("  Under Breasts Offset: " + SizedApparelSettings.UnderBreastsOffset.ToString(), -1, "offset from defeault layer offset. Defualt: -0.0013");
                    SizedApparelSettings.UnderBreastsOffset = listingStandard.Slider(SizedApparelSettings.UnderBreastsOffset, -0.025f, 0.025f);
                }
                if (SizedApparelSettings.useBodyTexture)
                {

                }
            }
            else
            {
                listingStandard.Label("  RimNudeWorld is not Actived ", -1, "");
                if (listingStandard.ButtonTextLabeled("Easy Setting Button", "Apply Non Rimnude Setting"))
                {
                    SizedApparelSettings.drawSizedApparelBreastsOnlyWorn = false;
                    SizedApparelSettings.useBodyTexture = true;
                    SizedApparelSettings.drawBreasts = true;
                    SizedApparelSettings.drawPenis = true;
                    SizedApparelSettings.drawVagina = true;
                    SizedApparelSettings.drawAnus = true;
                    SizedApparelSettings.drawBelly = true;
                    SizedApparelSettings.drawPubicHair = true;
                }
            }
            listingStandard.Gap(8);
            listingStandard.GapLine(5f);
            if (SizedApparelPatch.RimworldAnimationActive)
            {
                listingStandard.Label("RimworldAnimation (rjwAnimation) is Actived ", -1, "");
                listingStandard.CheckboxLabeled(" Animated SizedApparel BodyParts (wip)", ref SizedApparelSettings.AnimationPatch, "this option may animated breasts jiggle during animation.\n but not copatable with rimnudeworld.\ndefault: true");
            }
            else
            {
                listingStandard.Label("RimworldAnimation (rjwAnimation) is not Actived ", -1, "");
            }


            listingStandard.End();




            ////RightRect
            scrollRect = new Rect(0, 0, rightRect.width - 30f, Math.Max(rightRect.height + 100f, 1f));

            Widgets.BeginScrollView(rightRect, ref ScrollPosR, scrollRect, true);
            rightRect = new Rect(0, 0, rightRect.width - 30f, rightRect.height + 100f + 250f);
            listingStandard.maxOneColumn = true;

            listingStandard.Begin(rightRect);

            listingStandard.Label("SizedApparel System Apply (apparel and body parts)");
            listingStandard.CheckboxLabeled(" Apply Humanlikes", ref SizedApparelSettings.ApplyHumanlikes, "Try to Apply SizedApparel to Humanlikes if The textures are valid.\nDefault: true");
            listingStandard.CheckboxLabeled(" Apply Animals", ref SizedApparelSettings.ApplyAnimals, "Try to Apply SizedApparel to Animals if The textures are valid.\nDefault: false");
            //TODO
            /*
            if (SizedApparelSettings.ApplyAnimals)
                listingStandard.CheckboxLabeled("   Apply Player Faction Animals Only", ref SizedApparelSettings.ApplyAnimalsPlayerFactionOnly, "Default: true");
            */
            listingStandard.CheckboxLabeled(" Apply Mechanoid", ref SizedApparelSettings.ApplyMechanoid, "Try to Apply SizedApparel to Mech if The textures are valid.\nDefault: true");
            listingStandard.GapLine(5f);

            listingStandard.Label("Apparel Patch (Breasts Sized Apparel)");
            listingStandard.CheckboxLabeled(" Apply Apparel Patch for Male", ref SizedApparelSettings.ApplyApparelPatchForMale, "It Skips breasts size test for male. Do you need man with breasts...? may be not.\nDefault: false");
            listingStandard.GapLine(5f);

            listingStandard.Label("If you changed the option, try change apparels or reload save", -1);
            listingStandard.Label("Body Part Render Option (wip)",-1,"standalone BodyPart Render System from this mod. It's for user who don't use RimNudeWorld\nIf you use RimNudeWorld, you should turn off this.");

            listingStandard.CheckboxLabeled("Use Gender Specific Textures.", ref SizedApparelSettings.useGenderSpecificTexture,"Use Gender Specific texture for body and apparel if it's valid. \nDefault: true");
            listingStandard.GapLine(5);
            listingStandard.CheckboxLabeled("Draw Body Parts", ref SizedApparelSettings.drawBodyParts, "Draw Breasts..etc. when the pawn is wearing supported apparels. \nDefault: true");
            if (SizedApparelSettings.drawBodyParts)
            {
                listingStandard.CheckboxLabeled("  Use (Sized Apparel) Base Body Texture", ref SizedApparelSettings.useBodyTexture, "change pawn's body texture when the pawn is wearing supported apparels. Recommanded\nDefault: true");

                //listingStandard.CheckboxLabeled("  Draw Muscle Overlay (wip)", ref SizedApparelSettings.drawMuscleOverlay, "\nDisable this option when you use RimNudeWorld");

                listingStandard.CheckboxLabeled("  Draw Breasts", ref SizedApparelSettings.drawBreasts, "this option is why this mod exist.\nDefault: true");
                if (SizedApparelSettings.drawBreasts)
                {
                    listingStandard.CheckboxLabeled("     Match Breasts size to supported apparels",ref SizedApparelSettings.matchBreastToSupportedApparelSize, "to avoid breasts clipping(when breasts are bigger), you need this option.\nDefault: true");
                    listingStandard.CheckboxLabeled("     draw Breasts on worn pawn only (RimNudeWorld)", ref SizedApparelSettings.drawSizedApparelBreastsOnlyWorn, "when the pawn is nude, the breasts graphic for sized apparel will be hidden. \nDefault: false" );
                    listingStandard.CheckboxLabeled("     (Wip) Breasts Physics", ref SizedApparelSettings.breastsPhysics, "Breasts can be jiggled (for now. it works when nude only). It may be heavy for performance. \n Won't work with RimNudeWorld Breasts Rendering. \ndefault = false");
                }
                listingStandard.CheckboxLabeled("  Draw Penis", ref SizedApparelSettings.drawPenis,"Disable this option when you use RimNudeWorld");
                listingStandard.CheckboxLabeled("  Draw Vagina", ref SizedApparelSettings.drawVagina, "Disable this option when you use RimNudeWorld");
                listingStandard.CheckboxLabeled("  Draw Anus", ref SizedApparelSettings.drawAnus, "Disable this option when you use RimNudeWorld");
                listingStandard.CheckboxLabeled("  Draw Belly Buldge", ref SizedApparelSettings.drawBelly, "Disable this option when you use RimNudeWorld");

                listingStandard.CheckboxLabeled("  Draw Pubic Hair", ref SizedApparelSettings.drawPubicHair, "Disable this option when you use RimNudeWorld");

                listingStandard.CheckboxLabeled("  Hide Balls of Futa", ref SizedApparelSettings.hideBallOfFuta, "Hide Balls from penis of Futa.\nDefault: false");
                listingStandard.CheckboxLabeled("  Hide Penis of Man(Not Work yet)", ref SizedApparelSettings.hidePenisOfMale, "this option is for someone who really hate to see male's dick around.\nDefault: false");

                //listingStandard.Gap();
                //listingStandard.CheckboxLabeled(" Use Body Part Variation", ref SizedApparelSettings.useBodyPartsVariation, "Use graphic variation such as inverted nipple.\nDefault: true");
                //listingStandard.CheckboxLabeled(" Show Body Part Variaion Button(WIP)", ref SizedApparelSettings.showBodyPartsVariationIcon, "WIP. Not work for now.\nDefault: true");

                listingStandard.Gap();
                //listingStandard.CheckboxLabeled(" Use BodyPart Variation", ref SizedApparelSettings.useBodyPartsVariation, ""); //TODO
                listingStandard.CheckboxLabeled(" Show BodyPart Variation Description", ref SizedApparelSettings.showBodyPartsVariation, "Show more info in bodyparts that sized apparel added. such as inverted nipple");
            }
            /*
            listingStandard.Gap(4);
            listingStandard.Label("Breast Size Toggle Option", -1, "default option is setted for RimnudeWorld. you should not change this unless you have the textures for that size.");
            listingStandard.Gap(4);
            //Rect sizeScrollRect = new Rect(inRect.position+ new Vector2(0, listingStandard.CurHeight), inRect.size/3);
            //Vector2 sizeScrollPosition = new Vector2(0.9f, 0.5f);
            //listingStandard.BeginScrollView(sizeScrollRect, ref sizeScrollPosition, ref sizeScrollRect);
            //Listing_Standard sizeList = listingStandard.BeginSection_NewTemp(150);

            //sizeList.BeginScrollView(rect, ref ScrollPos, ref rect);
            
            listingStandard.CheckboxLabeled("  use Nipples", ref SizedApparelSettings.useNipples, "use Nipples(Flat breasts) Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Tiny", ref SizedApparelSettings.useTiny, "use Tiny breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Small", ref SizedApparelSettings.useSmall, "use Small breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Average", ref SizedApparelSettings.useAverage, "use Average breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Large", ref SizedApparelSettings.useLarge, "use Large breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Huge", ref SizedApparelSettings.useHuge, "use Huge breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Enormous", ref SizedApparelSettings.useEnormous, "use Enormous breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Massive", ref SizedApparelSettings.useMassive, "use Massive breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Gargantuan", ref SizedApparelSettings.useGargantuan, "use Gargantuan breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Colossal", ref SizedApparelSettings.useColossal, "use Colossal breasts Sized Apparel.\nDefault: true");
            listingStandard.CheckboxLabeled("  use Titanic", ref SizedApparelSettings.useTitanic, "use Titanic breasts Sized Apparel.\nDefault: true");
            */
            Widgets.EndScrollView();
            listingStandard.End();

            //listingStandard.EndScrollView(ref rect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Sized Apparel for RJW";
        }






        //copy from BnC
        public static bool Contai(string source, string toCheck, StringComparison comp)
        {
            return source != null && source.IndexOf(toCheck, comp) >= 0;
        }
        //copy from BnC
        public static void RaceSettings(Rect rect, List<string> label, ref List<string> alienRacesToAllowHumanlikes, string tooltip = null)
        {
            bool flag = !GenText.NullOrEmpty(tooltip);
            if (flag)
            {
                bool flag2 = Mouse.IsOver(rect);
                if (flag2)
                {
                    Widgets.DrawHighlight(rect);
                }
                TooltipHandler.TipRegion(rect, tooltip);
            }
            bool flag3 = GenList.NullOrEmpty<string>(alienRacesToAllowHumanlikes);
            if (flag3)
            {
                alienRacesToAllowHumanlikes = new List<string>();
            }
            Listing_Standard listing_Standard = new Listing_Standard();
            Rect rect2 = rect;
            Rect rect3 = rect;
            rect3.height = (float)label.Count * 30f;
            rect3.width -= 16f;
            Widgets.BeginScrollView(rect2, ref SizedApparelSettings.alienRacesListScrollPos, rect3, true);
            listing_Standard.Begin(rect3);
            for (int i = 0; i < SizedApparelSettings.alienRaces.Count; i++)
            {
                bool flag4 = GenList.NullOrEmpty<string>(alienRacesToAllowHumanlikes) || !alienRacesToAllowHumanlikes.Contains(label[i]);
                bool flag5 = SizedApparelSettings.alienRaceSearch == null || Contai(SizedApparelSettings.alienRaces[i], SizedApparelSettings.alienRaceSearch.ToLower(), StringComparison.OrdinalIgnoreCase);
                if (flag5)
                {
                    WidgetRow widgetRow = new WidgetRow(rect.x,listing_Standard.CurHeight, UIDirection.RightThenUp, 99999f, 1f);
                    widgetRow.Label(label[i], rect.width * 0.8f, null, -1f);

                    
                    bool flag6 = label[i] != "Human";
                    if (flag6)
                    {
                        widgetRow.ToggleableIcon(ref flag4, TexButton.IconBook, "Use Shared Body Parts texture (\"Humanlike\" body plarts).\nIf Unchecked, the race will use race's owned texture. \n If the race not have supported textures, try this option.", null, null);
                    }
                    else
                    {
                        //TODO: should I let human can use humanlike textures?
                        widgetRow.Icon(TexButton.IconBook, "Human Race is forced to use \"Human\" Textures. not \"Humanlike\"");
                    }


                    /*
                    bool flag7 = widgetRow.ButtonIcon(TexButton.ToggleTweak, "Allow Humanlike Body parts rendering from Sized Apparel(NOT RIMNUDE!)", null, null, null, true, -1f);
                    if (flag7)
                    {
                        bool flag8 = Current.Game != null;
                        if (flag8)
                        {
                            bool flag9 = !Find.WindowStack.TryRemove(typeof(RaceEditorWindow), true);
                            if (flag9)
                            {
                                RaceSettings sizeSettings = RaceUtility.GetSizeSettings(DefDatabase<ThingDef>.GetNamed(label[i], false));
                                RaceEditorWindow raceEditorWindow = new RaceEditorWindow();
                                raceEditorWindow.alienRace = DefDatabase<ThingDef>.GetNamed(label[i], true);
                                bool flag10 = sizeSettings != null;
                                if (flag10)
                                {
                                    raceEditorWindow.raceSettings = sizeSettings;
                                    raceEditorWindow.headOffset = sizeSettings.headOffset;
                                    raceEditorWindow.sizeModifier = sizeSettings.sizeModifier;
                                    raceEditorWindow.hairSizeModifier = sizeSettings.hairSizeModifier;
                                    raceEditorWindow.headSizeModifier = sizeSettings.headSizeModifier;
                                    raceEditorWindow.scaleChild = sizeSettings.scaleChild;
                                    raceEditorWindow.scaleTeen = sizeSettings.scaleTeen;
                                }
                                Find.WindowStack.Add(raceEditorWindow);
                            }
                        }
                        else
                        {
                            Messages.Message("You need to be in-game to open size editor", MessageTypeDefOf.RejectInput, true);
                        }
                    }
                    */
                    bool flag11 = !flag4;
                    if (flag11)
                    {
                        bool flag12 = !alienRacesToAllowHumanlikes.Contains(label[i]);
                        if (flag12)
                        {
                            alienRacesToAllowHumanlikes.Add(label[i]);
                        }
                    }
                    else
                    {
                        bool flag13 = alienRacesToAllowHumanlikes.Contains(label[i]);
                        if (flag13)
                        {
                            alienRacesToAllowHumanlikes.Remove(label[i]);
                        }
                    }
                    listing_Standard.Gap(30f);
                }
            }
            listing_Standard.End();
            Widgets.EndScrollView();
        }
    }

}
