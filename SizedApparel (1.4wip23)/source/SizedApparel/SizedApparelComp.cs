using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using rjw;
using Verse;

namespace SizedApparel
{
    [StaticConstructorOnStartup]
    public class ApparelRecorderComp : ThingComp
    {
        Pawn pawn;// Parent Cache

        public bool recentClothFlag = true;

        public bool isDrawAge = true;

        public bool testbool = false;
        public ApparelRecorderCompProperties Props => (ApparelRecorderCompProperties)this.props;
        public bool hasUpdateBefore = false;
        public bool hasUpdateBeforeSuccess = false;
        public bool hasGraphicUpdatedBefore = false; // not yet

        public bool needToCheckApparelGraphicRecords = false;
        public bool isDirty = true;
        public bool isHediffDirty = true;
        public bool isApparelDirty = true;
        public bool isSkeletonDirty = true;
        public bool isBodyAddonDirty = true; // reset all body addon graphics.
        public bool hasUnsupportedApparel = true;
        public bool havingSex = false;//Obsolete
        public bool hasUpdateForSex = false;//Obsolete
        string cachedBodytype;


        public List<ApparelGraphicRecord> cachedApparelGraphicRecord = new List<ApparelGraphicRecord>();



        public Hediff breastHediff = null; 
        public Hediff vaginaHediff = null;
        public List<Hediff> penisHediffs = null; // RJW can attach multiple penis
        public Hediff anusHediff = null;
        public Hediff udderHediff = null;//RJW 4.6.8: Udder is not partof chest's breast. it attached to torso.

        public Color? nippleColor; //for menstruation cycles Mod 

        //TODO Optimize Update Hediff Filter
        private bool hasBreastsAddon = false;
        private bool hasVaginaAddon = false;
        private bool hasPenisAddon = false;
        private bool hasAnusAddon = false;
        private bool hasUdderAddon = false;
        private bool hasPubicHairAddon = false;

        public float breastSeverity = -1;

        public float BreastSeverityCache = 0;
        //public float BiggestBreastSeverityInAvailableTextures = 0;


        //for breasts animation or something.
        public bool ForceUpdateTickAnimation = false;
        public bool ForceBlockTickAnimation = false; // useful when you have make fixed pose
        public bool PrePositionCache;
        public Vector3? prePositionCache;
        public float? preAngleCache;
        public int? preTickCache;
        public Vector3 preVelocity = Vector3.zero;
        public float preRotation = 0;
        public Dictionary<string, int> tickCache = new Dictionary<string, int>();



        public BodyDef bodyDef = null;
        public List<SizedApparelBodyPart> bodyAddons = new List<SizedApparelBodyPart>(); // BodyParts Added form Defs

        public SkeletonDef skeletonDef; // for rebuild Skeleton
        public Skeleton skeleton;
        public bool skipSkeleton = false;

        //native BodyParts
        public SizedApparelBodyPart bodyPartBreasts;//TODO: Make this as List
        public SizedApparelBodyPart bodyPartNipple;//TODO
        public List<SizedApparelBodyPart> bodyPartPenises = new List<SizedApparelBodyPart>();
        public List<SizedApparelBodyPart> bodyPartBalls = new List<SizedApparelBodyPart>();
        public SizedApparelBodyPart bodyPartVagina;
        public SizedApparelBodyPart bodyPartAnus;
        public SizedApparelBodyPart bodyPartBelly;
        public SizedApparelBodyPart bodyPartMuscleOverlay;//TODO
        public SizedApparelBodyPart bodyPartUdder;


        public SizedApparelBodyPart bodyPartHips;
        public List<SizedApparelBodyPart> bodyPartThighs = new List<SizedApparelBodyPart>();
        public List<SizedApparelBodyPart> bodyPartHands = new List<SizedApparelBodyPart>();
        public List<SizedApparelBodyPart> bodyPartFeet = new List<SizedApparelBodyPart>();

        public PubicHairDef pubicHairDef = null;
        public PubicHairDef initialPubicHairDef = null; // For StyleStaitionWork
        public PubicHairDef nextPubicHairDef = null; // For StyleStaitionWork
        public SizedApparelBodyPart bodyPartPubicHair;


        public Graphic graphicSourceNaked = null; //original Graphic
        public Graphic graphicSourceRotten = null; //original Graphic
        public Graphic graphicSourceFurCovered = null; //original Graphic


        public Graphic graphicbaseBodyNaked = null;
        public SizedApparelTexturePointDef baseBodyNakedPoints;
        public Graphic graphicbaseBodyCorpse = null;
        public SizedApparelTexturePointDef baseBodyCorpsePoints;
        public Graphic graphicbaseBodyRotten = null;
        public SizedApparelTexturePointDef baseBodyRottenPoints;
        public Graphic graphicbaseBodyFurCovered = null;
        public SizedApparelTexturePointDef baseBodyFurCoveredPoints;


        public AlienRaceSetting raceSetting = null;

        public string customPose = null;
        public SizedApparelPose currentCustomPose = null;


        public bool forceHorny = false;


        public bool canDrawBreasts = false;
        public bool canDrawPenis = false;
        public bool canDrawVaginaAndAnus = false;
        public bool canDrawTorsoParts = false; //belly and udder


        //this may reset skeleton animation. also little have process
        public void InitSkeleton()
        {
            skeletonDef = DefDatabase<SkeletonDef>.GetNamedSilentFail(pawn.def.defName);
            if (skeletonDef == null)
            {
                if (raceSetting != null && raceSetting.asHuman == true) //old: !SizedApparelSettings.UnsupportedRaceToUseHumanlike
                {
                    skeletonDef = DefDatabase<SkeletonDef>.GetNamedSilentFail("Human");
                }
            }
            if (skeletonDef != null)
            {
                if (this.pawn.story == null || this.pawn.story.bodyType == null)
                {
                    foreach (Skeleton s in skeletonDef.skeletons)
                    {
                        if (s.bodyType == null)
                        {
                            this.skeleton = new Skeleton(s);
                            if (SizedApparelSettings.Debug)
                                Log.Message("[SizedApparel] Apply SkeletonDef: pawn withouth bodytype " + pawn.Name);
                            return;
                        }
                    }
                }
                else
                {
                    //Setting Up Skeleton
                    foreach (Skeleton s in skeletonDef.skeletons)
                    {
                        if (s.bodyType == this.pawn.story.bodyType.defName)
                        {
                            this.skeleton = new Skeleton(s);
                            if (SizedApparelSettings.Debug)
                                Log.Message("[SizedApparel] Apply SkeletonDef: pawn with bodytype " + pawn.Name + " with SkeletonDef : " + this.pawn.story.bodyType.defName);
                            return;
                        }
                    }
                }
                this.skeleton = null;
            }
            if (SizedApparelSettings.Debug)
                Log.Message("[SizedApparel] Apply SkeletonDef: No Skeleton Found for " + pawn.Name);
            isSkeletonDirty = false;
        }

        public void InitBodyAddons()
        {

            bodyAddons.Clear();

            //TODO
            //bodyDef = DefDatabase<SizedApparelBodyDef>.GetNamed(pawn.def.defName);

            bodyDef = DefDatabase<BodyDef>.GetNamedSilentFail(pawn.def.defName);

            //bodyDef can be null

            if (bodyDef == null)
            {
                if (raceSetting != null && raceSetting.asHuman) //old: !SizedApparelSettings.UnsupportedRaceToUseHumanlike
                {
                    bodyDef = DefDatabase<BodyDef>.GetNamedSilentFail("Human");
                }
            }


            if (bodyDef != null && bodyDef.bodies != null)
            {
                if (pawn.story?.bodyType == null)
                {
                    if(SizedApparelSettings.Debug)
                        Log.Message("[SizedApparel] Apply BodyDef: pawn has no bodytype " + pawn.Name);
                    BodyWithBodyType body = null;
                    if(!bodyDef.bodies.NullOrEmpty())
                        body = bodyDef.bodies[0];
                    if (body != null && body.Addons != null)
                    {
                        foreach (var bodyaddon in body.Addons)
                        {
                            if (bodyaddon == null)
                                continue;
                            var a = new SizedApparelBodyPart(pawn, this, bodyaddon.partName, bodyaddon.bodyPartOf, bodyaddon.defaultHediffName, bodyaddon.isBreasts, false, bodyaddon.customPath, bodyaddon.colorType, bodyaddon.mustHaveBone);
                            a.SetDepthOffsets(bodyaddon.depthOffset.south, bodyaddon.depthOffset.north, bodyaddon.depthOffset.east, bodyaddon.depthOffset.west);
                            //a.SetDepthOffsets(bodyaddon.depthOffset);
                            a.SetCenteredTexture(bodyaddon.centeredTexture);
                            bodyAddons.Add(a);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (BodyWithBodyType body in bodyDef.bodies)
                    {
                        if (body.bodyType != null && body.bodyType == this.pawn.story.bodyType.defName)
                        {
                            if (SizedApparelSettings.Debug)
                                Log.Message("[SizedApparel] Apply BodyDef: matched BodyTyped Body for " + pawn.Name);
                            if (body != null && body.Addons != null)
                            {
                                foreach (var bodyaddon in body.Addons)
                                {
                                    if (bodyaddon == null)
                                        continue;
                                    var a = new SizedApparelBodyPart(pawn, this, bodyaddon.partName, bodyaddon.bodyPartOf, bodyaddon.defaultHediffName, bodyaddon.isBreasts, false, bodyaddon.customPath, bodyaddon.colorType, bodyaddon.mustHaveBone);
                                    a.SetDepthOffsets(bodyaddon.depthOffset.south, bodyaddon.depthOffset.north, bodyaddon.depthOffset.east, bodyaddon.depthOffset.west);
                                    //a.SetDepthOffsets(bodyaddon.depthOffset);
                                    a.SetCenteredTexture(bodyaddon.centeredTexture);
                                    bodyAddons.Add(a);
                                }
                            }
                            break;
                        }

                    }

                }

            }
            else
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[SizedApparel] Cannot find BodyDef for " + pawn.def.defName);
            }

            //bodyPartBreasts = new SizedApparelBodyPart(pawn, this, "Breasts", SizedApparelBodyPartOf.Breasts, "Breasts", true, false, null);
            //bodyPartBreasts.SetDepthOffsets(0.0113f, 0.001f, 0.0113f, 0.0113f);

            //bodyPartBreasts.SetPositionOffsets(Vector2.one, Vector2.one, Vector2.one, Vector2.one);
            //bodyPartBreasts.scale = 2f;
            //Nipple. should I separate from Breats?
            //bodyPartNipple = new SizedApparelBodyPart(pawn, this, "Nippple", SizedApparelBodyPartOf.Breasts, "Breasts", true, false);
            //bodyPartBreasts.SetDepthOffsets(0.0114f, 0.000f, 0.0114f, 0.0114f);

            //bodyPartUdder = new SizedApparelBodyPart(pawn, this, "Udder", SizedApparelBodyPartOf.Udder, "UdderBreasts", true, false);
            //bodyPartUdder.SetDepthOffsets(0.0112f, 0.0005f, 0.0112f, 0.0112f);

            //UdderNipple...?
            //bodyPartUdder = new SizedApparelBodyPart(p, "Udder", SizedApparelBodyPartOf.Udder, "UdderBreasts", true, false);
            //bodyPartUdder.SetDepthOffsets(0.0112f, 0.0005f, 0.0112f, 0.0112f);\

            //bodyPartVagina = new SizedApparelBodyPart(pawn, this, "Vagina", SizedApparelBodyPartOf.Vagina, "Vagina", false, false);
            //bodyPartVagina.SetDepthOffsets(0.0088f, 0.0100f, 0.0088f, 0.0088f);

            //bodyPartAnus = new SizedApparelBodyPart(pawn, this, "Anus", SizedApparelBodyPartOf.Anus, "Anus", false, false);
            //bodyPartAnus.SetDepthOffsets(0.0093f, 0.0105f, 0.0093f, 0.0093f);

            //bodyPartBelly = new SizedApparelBodyPart(pawn, this, "Belly", SizedApparelBodyPartOf.Belly, "BellyBulge", false, false);
            //bodyPartBelly.SetDepthOffsets(0.0098f, 0.0002f, 0.0098f, 0.0098f);

            //bodyPartPubicHair = new SizedApparelBodyPart(pawn, this, "PubicHair", SizedApparelBodyPartOf.PubicHair, "Default", false, false, null, ColorType.Hair);
            //bodyPartPubicHair.SetDepthOffsets(0.0099f, 0.0099f, 0.0089f, 0.0089f);
        }

        public void ResetBodyAddonBoneLink()
        {
            string s;
            foreach(var a in bodyAddons)
            {
                s = a.bone?.name;
                if (s == null)
                    continue;
                a.SetBone(skeleton.FindBone(s));
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.pawn = this.parent as Pawn;


        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            UpdateRaceSettingData(); // include race Setting
            InitSkeleton();
            InitBodyAddons();
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<string>(ref customPose, "customPose"); // save pawn's custom pose. Each bodyparts will not saved.
            Scribe_Defs.Look<PubicHairDef>(ref pubicHairDef, "PubicHairDef");
            if (pubicHairDef == null)
            {
                pubicHairDef = SizedApparelUtility.GetRandomPubicHair();
            }

            //Scribe_Values.Look<>(); //TODO: save pubic hair data
        }




        /*
        public override void CompTick()
        {
            if (PrePositionCache)
            {

            }
            base.CompTick();
        }*/
        /*
        public override void CompTickRare()
        {
            base.CompTickRare();
        }*/



        float penisAngle = 0;
        public void SetPenisAngle(float angle)
        {
            penisAngle = angle;

            if(this.skeleton != null)
            {
                Bone penisBone = skeleton.FindBone("Penis");
                if (penisBone != null)
                {
                    //Log.Message("SetPenisAngle : " + angle.ToString());
                    penisBone.SetAngle(angle);
                }
            }


            bool penisDown = false;
            if (angle >= 120 && angle <= 250)
                penisDown = true;
            foreach (var p in bodyPartPenises)
            {
                if (penisDown)
                {
                    p.SetCustomPose("PenisDown");
                }
                else
                {
                    p.SetCustomPose(null);
                }
            }
        }
        //do not call this in character creation from new game.
        public void SetBreastJiggle(bool jiggle, int cooltime = 5, bool checkApparelForCanPose = false)
        {
            //SetJiggle has cooltime. 


            bool flag1 = true;

            //should set apparels pose?
            //Use First BodyAddon which is partof Breasts
            foreach(var a in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Breasts))
            {
                if (flag1)
                {
                    if (Find.TickManager.TicksGame < a.lastPoseTick + cooltime)
                        return;
                    flag1 = false;
                }


                if (jiggle)
                {
                    bool flag2 = true;
                    if (flag2 && checkApparelForCanPose)
                    {
                        if (!a.CheckCanPose("JiggleUp", true, false, true, true, false))
                            return;
                        flag2 = false;
                    }

                    //may need to check apparels for aply pose
                    //bodyPartBreasts.CheckCanPose("JiggleUp",)
                    a.SetCustomPose("JiggleUp");
                }
                else
                {
                    //SetPoseFromPoseSet(null);
                    a.SetCustomPose(null);
                }
            }


        }

        string[] testPose = { null, "JiggleUp", "JiggleCenter" };
        string[] testPose2 = { null, "PenisDown" };

        public void UpdateTickAnim(Vector3 rootLoc, float angle) // call this in DrawPawnBody <- only  called when it rendered
        {
            if (ForceBlockTickAnimation)// prevent breasts jiggle from other pose
                return;

            if (skeleton == null)
                return;

            if (Find.CameraDriver == null)
                return;

            //do not calculate physics camera is far
            if (Find.CameraDriver.CurrentZoom >= CameraZoomRange.Furthest)
                return;
            //int IdTick = parent.thingIDNumber * 20; //hint from yayo animation mod

            if (!SizedApparelUtility.CanApplySizedApparel(pawn))
                return;


            if (SizedApparelSettings.breastsPhysics || ForceUpdateTickAnimation)//SizedApparelSettings.autoJiggleBreasts
            {

                int tick;
                if (this.preTickCache != null)
                    tick = Find.TickManager.TicksGame - this.preTickCache.Value;
                else
                    tick = Find.TickManager.TicksGame;

                //if tick is not updated, don't update animation.
                if (tick == 0)
                    return;

                Vector3 velocity;
                if (this.prePositionCache != null)
                    velocity = (rootLoc - this.prePositionCache.Value);// /tick
                else
                    velocity = Vector3.zero;

                float rotation;
                
                if (this.preAngleCache != null)
                    rotation = angle - this.preAngleCache.Value;
                else
                    rotation = 0;

                float rotAcc = rotation - preRotation;
                

                //Log.Message(velocity.ToString() + " , " + preVelocity.ToString());
                //UnityEngine's vector.normalize is safe for zero vector
                //(Vector3.Dot(velocity.normalized, preVelocity.normalized)) < 0.2f
                
                float dotV = Vector3.Dot(velocity.normalized, preVelocity.normalized);
                float velocityOffset = (velocity - preVelocity).magnitude;
                
                //Log.Message(pawn.ToString());
                //Log.Message("rotAcc : " + rotAcc.ToString());
                //Log.Message("Velocity.x : " + velocity.x.ToString());
                //Log.Message("Velocity.z : " + velocity.z.ToString());
                //Log.Message("dotV : " + dotV.ToString());
                //Log.Message("velocityOffset : " + velocityOffset.ToString());
                //&& dotV > 0.4f
                if (((preVelocity != Vector3.zero && velocityOffset >= 0.02))|| Mathf.Abs(rotAcc) > 0.5) //(dotV == 0 ? 0:1), Mathf.Abs(dotV) // || Mathf.Abs(rotation) > 20
                {
                    //tickCache.Add("BreastsJiggleUp", Find.TickManager.TicksGame);
                    SetBreastJiggle(true,10,true);

                }
                else
                {
                    SetBreastJiggle(false,10, false);
                }


                //cache pre data

                this.prePositionCache = rootLoc;
                this.preAngleCache = angle;
                this.preRotation = rotation;
                this.preTickCache = Find.TickManager.TicksGame;
                this.preVelocity = velocity;




            }

            //SetPoseFromPoseSet(testPose2.RandomElement());
        }


        public bool isApparelGraphicRecordChanged()
        {
            if (pawn == null)
                return false;

            var apparelGraphics = pawn.Drawer?.renderer?.graphics?.apparelGraphics;
            if (apparelGraphics == null)
                return false;

            //return false; //since 1.3 broken. force to return false;
            if (!apparelGraphics.SequenceEqual(cachedApparelGraphicRecord))
            {
                if (SizedApparelSettings.Debug)
                    Log.Message("[SizedApparel]:" + pawn + "'s apparel Record Changed! need to updating comp");
                return true;
            }


            return false;
        }

        public void DrawAllBodyParts(Vector3 rootLoc, float angle, Rot4 facing, RotDrawMode bodyDrawType, PawnRenderFlags flags, Pawn ___pawn, Mesh bodyMesh)
        {
            if (this.isDrawAge && (!flags.FlagSet(PawnRenderFlags.Clothes) || !this.hasUnsupportedApparel || SizedApparelUtility.isPawnNaked(___pawn))) //TODO : Move it to CanDraw
            {
                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawVagina && SizedApparelUtility.CanDrawVagina(___pawn, flags))
                {
                    //if (this.bodyPartVagina != null)
                    //    this.bodyPartVagina.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Vagina))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }
                }
                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawAnus && SizedApparelUtility.CanDrawAnus(___pawn, flags))
                {
                    //if (this.bodyPartAnus != null)
                    //    this.bodyPartAnus.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Anus))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }
                }

                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawBelly && SizedApparelUtility.CanDrawBelly(___pawn, flags))
                {
                    //if (this.bodyPartBelly != null)
                    //    this.bodyPartBelly.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Belly))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }
                }

                //Draw Pubic Hair
                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawPubicHair && SizedApparelUtility.CanDrawPubicHair(___pawn, flags))
                {
                    if (this.bodyPartPubicHair != null)
                        this.bodyPartPubicHair.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.PubicHair))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }
                }


                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawUdder && SizedApparelUtility.CanDrawUdder(___pawn, flags))
                {
                    //if (this.bodyPartUdder != null)
                    //    this.bodyPartUdder.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Udder))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }

                }
                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawBreasts && SizedApparelUtility.CanDrawBreasts(___pawn, flags) && (SizedApparelSettings.drawSizedApparelBreastsOnlyWorn ? !SizedApparelUtility.isPawnNaked(___pawn, flags) : true))
                {
                    //if (this.bodyPartBreasts != null)
                    //    this.bodyPartBreasts.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Breasts))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }
                }

                if (bodyDrawType != RotDrawMode.Dessicated && SizedApparelSettings.drawPenis && SizedApparelUtility.CanDrawPenis(___pawn, flags, true))
                {

                    if (SizedApparelSettings.hideBallOfFuta ? !Genital_Helper.is_futa(___pawn) : true)
                    {
                        /*
                        foreach (SizedApparelBodyPart b in this.bodyPartBalls)
                        {
                            b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                        }*/
                        foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Balls))
                        {
                            b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                        }
                    }
                    /*
                    foreach (SizedApparelBodyPart p in this.bodyPartPenises)
                    {
                        p.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }*/
                    foreach (var b in GetSizedApparelBodyParts(SizedApparelBodyPartOf.Penis))
                    {
                        b.DrawBodyPart(rootLoc, angle, facing, bodyDrawType, flags, bodyMesh);
                    }

                }//Draw BodyParts

                //Draw Modular Apparel Parts... TODO...? Is it passible?
                if (flags.FlagSet(PawnRenderFlags.Clothes))
                {

                }




            }

        }


        //not working
        public override void PostPostMake()
        {
            /*
            Pawn pawn;
            try
            {
                pawn = (Pawn)parent;
                if (pawn != null)
                {
                    if (!pawn.RaceProps.Humanlike)
                        return;
                    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                    if (SizedApparelSettings.Debug)
                        Log.Message("[SizedApparels] Component Inint And Resolve all Graphics for "+ pawn.Name);
                }
            }
            catch
            {

            }
            */
                }
                public void ClearHediffs()
        {
            breastHediff = null;
            vaginaHediff = null;
            if(penisHediffs != null)
                penisHediffs.Clear();
            anusHediff = null;
            udderHediff = null;
        }
        public void ClearBreastCacheValue()
        {
            breastHediff = null;
            breastSeverity = -1;
            BreastSeverityCache = 0;
        }
        public void ClearPenisCacheValue()
        {
            //TODO
        }
        public void ClearCanDraw()
        {
            canDrawBreasts = false;
            canDrawPenis = false;
            canDrawTorsoParts = false;
            canDrawVaginaAndAnus = false;
        }
        public void ClearGraphics(bool clearBreasts = true)
        {
            //Since Injection of base body is now in Matarial. no need to keep
            graphicSourceNaked = null;
            graphicSourceFurCovered = null;
            graphicSourceRotten = null;

            hasGraphicUpdatedBefore = false;
            graphicbaseBodyNaked = null;
            graphicbaseBodyRotten = null;
            graphicbaseBodyFurCovered = null;

            return;
            //Clear BodyParts
            if (clearBreasts && bodyPartBreasts!= null)
                bodyPartBreasts.Clear();
            if(bodyPartAnus != null)
                bodyPartAnus.Clear();
            if (bodyPartBelly != null)
                bodyPartBelly.Clear();
            if (bodyPartVagina != null)
                bodyPartVagina.Clear();
            if (bodyPartPubicHair != null)
                bodyPartPubicHair.Clear();
            
            foreach(var a in bodyAddons)
            {
                a.Clear();
            }

        }
        public void ClearAll(bool clearGraphics = true)
        {
            ClearBreastCacheValue();
            if(clearGraphics)
                ClearGraphics();
            ClearHediffs();
            ClearCanDraw();
            hasUnsupportedApparel = false;
            hasUpdateBefore = false;
            hasUpdateBeforeSuccess = false;
            needToCheckApparelGraphicRecords = false;
        }

        public void SetDirty(bool clearPawnGraphicSet = false, bool dirtyHediff = true, bool dirtyApparel = true, bool dirtySkeleton = false, bool dirtyBodyAddons = false)
        {
            this.isDirty = true;
            this.isHediffDirty = dirtyHediff;
            this.isApparelDirty = dirtyApparel;
            this.isSkeletonDirty = dirtySkeleton;
            this.isBodyAddonDirty = dirtyBodyAddons;
            if (clearPawnGraphicSet)
            {
                if (pawn == null)
                    return;
                if (pawn.Drawer == null)
                    return;
                if (pawn.Drawer.renderer == null)
                    return;
                pawn.Drawer.renderer.graphics.ClearCache();
            }

        }
        public void UpdateRaceSettingData()
        {
            if (pawn == null)
                return;

            var loc_raceSetting = SizedApparelSettings.alienRaceSettings.FirstOrDefault((AlienRaceSetting s) => s.raceName == pawn.def.defName);
            if (loc_raceSetting == null)
                return;
            raceSetting = loc_raceSetting;
        }

        public void CheckAgeChanged()
        {
            if (pawn == null)
                return;
            if (pawn.ageTracker == null)
                return;

            //TODO. Cleanup
            UpdateRaceSettingData();
            if (raceSetting == null)
                return;

            if (raceSetting.drawMinAge < 0 || pawn.ageTracker.AgeBiologicalYearsFloat >= raceSetting.drawMinAge)
                isDrawAge = true;
            else
                isDrawAge = false;
        }

        public void FindAndApplyBodyGraphic(Pawn pawn, Graphic sourceGraphic, ref Graphic targetGraphicBaseBody, ref Graphic cachedSourceGraphic, string debugName)
        {
            const string baseBodyString = "_BaseBody";
            string baseBodyStringWithSex;

            if (SizedApparelSettings.useGenderSpecificTexture)
            {
                if (pawn.gender == Gender.Female)
                {
                    baseBodyStringWithSex = baseBodyString + "F";
                }
                else if (pawn.gender == Gender.Male)
                {
                    baseBodyStringWithSex = baseBodyString + "M";
                }
                else
                {
                    baseBodyStringWithSex = baseBodyString; // + "N" //Does it need to add N?
                }
            }
            else
                baseBodyStringWithSex = baseBodyString;
            string targetGraphicPath = null;
            if (sourceGraphic != null)
            {
                //path = path.Insert(Math.Max(path.LastIndexOf('/'), 0), "/CustomPose/"+ customPose);
                if (cachedSourceGraphic == null)
                    cachedSourceGraphic = sourceGraphic;
                targetGraphicPath = cachedSourceGraphic.path;

                if (customPose != null)
                    targetGraphicPath = targetGraphicPath.Insert(Math.Max(targetGraphicPath.LastIndexOf('/'), 0), "/CustomPose/" + customPose);

                if (!targetGraphicPath.Contains(baseBodyString))
                {
                    if (SizedApparelSettings.useGenderSpecificTexture && pawn.gender != Gender.None)
                    {
                        if (targetGraphicBaseBody == null)
                        {
                            if (ContentFinder<Texture2D>.Get((targetGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                            {
                                //cachedSourceGraphic = sourceGraphic;
                                Shader shader = sourceGraphic.Shader;
                                //if (!ShaderUtility.SupportsMaskTex(shader))
                                //    shader = ShaderDatabase.CutoutSkinOverlay;
                                targetGraphicBaseBody = GraphicDatabase.Get<Graphic_Multi>(targetGraphicPath + baseBodyStringWithSex, shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                                //sourceGraphic = targetGraphicBaseBody;
                            }
                            else if (customPose != null)
                            {
                                targetGraphicPath = sourceGraphic.path;
                                if (ContentFinder<Texture2D>.Get((targetGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                {
                                    //cachedSourceGraphic = sourceGraphic;
                                    Shader shader = sourceGraphic.Shader;
                                    //if (!ShaderUtility.SupportsMaskTex(shader))
                                    //    shader = ShaderDatabase.CutoutSkinOverlay;
                                    targetGraphicBaseBody = GraphicDatabase.Get<Graphic_Multi>(targetGraphicPath + baseBodyStringWithSex, shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                                    //sourceGraphic = targetGraphicBaseBody;
                                }
                                else
                                {
                                    if (SizedApparelSettings.Debug)
                                        Log.Warning("[SizedApparel] Missing BaseBodyTexture for "+ debugName + " Graphic: " + targetGraphicPath + baseBodyStringWithSex + "_south");
                                }
                            }
                        }
                    }
                    if (targetGraphicBaseBody == null)
                    {
                        if (ContentFinder<Texture2D>.Get((targetGraphicPath + baseBodyString + "_south"), false) != null)
                        {
                            // cachedSourceGraphic = sourceGraphic;
                            Shader shader = sourceGraphic.Shader;
                            //if (!ShaderUtility.SupportsMaskTex(shader))
                            //    shader = ShaderDatabase.CutoutSkinOverlay;
                            targetGraphicBaseBody = GraphicDatabase.Get<Graphic_Multi>(targetGraphicPath + baseBodyString, shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                            //sourceGraphic = targetGraphicBaseBody;
                        }
                        else if (customPose != null)
                        {
                            targetGraphicPath = sourceGraphic.path;
                            if (ContentFinder<Texture2D>.Get((targetGraphicPath + baseBodyString + "_south"), false) != null)
                            {
                                //cachedSourceGraphic = sourceGraphic;
                                Shader shader = sourceGraphic.Shader;
                                //if (!ShaderUtility.SupportsMaskTex(shader))
                                //    shader = ShaderDatabase.CutoutSkinOverlay;
                                targetGraphicBaseBody = GraphicDatabase.Get<Graphic_Multi>(targetGraphicPath + baseBodyString, shader, sourceGraphic.drawSize, sourceGraphic.color, sourceGraphic.colorTwo, sourceGraphic.data);
                                //sourceGraphic = targetGraphicBaseBody;
                            }
                            else
                            {
                                if (SizedApparelSettings.Debug)
                                    Log.Warning("[SizedApparel] Missing BaseBodyTexture for " + debugName + " Graphic: " + targetGraphicPath + baseBodyString + "_south");
                            }

                        }
                    }

                }
                else
                    targetGraphicBaseBody = sourceGraphic;

            }

        }

        public void Update(bool cache = true, bool fromGraphicRecord = true, bool updateGraphics = true, bool CheckApparel = true)
        {
            if (cache)
                recentClothFlag = CheckApparel;



            isDirty = false;
            bool flag = fromGraphicRecord;
            needToCheckApparelGraphicRecords = false;
            //flag = false; //TODO:fromGraphicRecord is forced not to do for a while. it will update later



            UpdateRaceSettingData();



            if (!hasUpdateBefore)
            {
                InitSkeleton();
                InitBodyAddons();
                hasUpdateBefore = true;
            }
            else
            {
                //check is bodytype changed
                if(cachedBodytype != pawn.story?.bodyType?.defName)
                {
                    isSkeletonDirty = true;
                }
                if (isSkeletonDirty)
                {
                    InitSkeleton();

                    if (isBodyAddonDirty)
                        InitBodyAddons();
                    else
                        ResetBodyAddonBoneLink();
                }
                else
                {
                    if (isBodyAddonDirty)
                        InitBodyAddons();
                }
            }

            if (skeleton == null)
            {
                if (Find.CurrentMap != null)
                    return;
            }


            if (!SizedApparelUtility.CanApplySizedApparel(pawn))
                return;


            if (pubicHairDef == null)
            {
                pubicHairDef = SizedApparelUtility.GetRandomPubicHair();
            }


            if (SizedApparelSettings.Debug)
                Log.Message("[SizedApparel] Updating Component of " + pawn.Name);
            if (updateGraphics)
            {
                ClearGraphics();
                //ClearHediffs();
            }



            //float breastSeverityCapped = 1000;
            string bodyTypeDefName = null;
            if (pawn.story != null)
                bodyTypeDefName = pawn.story.bodyType?.defName;
            float bellySeverity = 0;

            if (isHediffDirty) //Update Hediff Data
            {
                ClearHediffs();


                if(SizedApparelSettings.drawPenis || SizedApparelSettings.drawVagina || SizedApparelSettings.drawAnus)
                {
                    BodyPartRecord genitalPart = Genital_Helper.get_genitalsBPR(pawn);
                    if (genitalPart != null)
                    {
                        var genitalList = Genital_Helper.get_PartsHediffList(pawn, genitalPart);
                        if (!genitalList.NullOrEmpty())
                        {
                            if (SizedApparelSettings.Debug)
                            {
                                foreach (var g in genitalList)
                                {
                                    Log.Message(" [SizedApparel] " + pawn.Name + "has hediff in genital (" + g.def.defName + ")");
                                }
                            }

                            //penisHediffs = genitalList.FindAll((Hediff h) => SizedApparelUtility.isPenis(h.def.defName));
                            penisHediffs = genitalList.FindAll((Hediff h) => Genital_Helper.is_penis(h));
                            //vaginaHediff = genitalList.FirstOrDefault((Hediff h) => SizedApparelUtility.isVagina(h.def.defName));
                            vaginaHediff = genitalList.FirstOrDefault((Hediff h) => Genital_Helper.is_vagina(h));

                        }
                    }
                }

                if (SizedApparelSettings.drawBelly)
                {
                    //EggImplement as Pregnant
                    //need to Optimize... TODO...
                    List<Hediff> pregnancies = pawn.health?.hediffSet?.hediffs?.FindAll((Hediff h) => SizedApparelUtility.isPragnencyHediff(h) || SizedApparelUtility.isRJWEggHediff(h));//pregnancy and pregnant. has some issue with "pregnancy mood"
                    if (!pregnancies.NullOrEmpty())
                    {
                        foreach (Hediff h in pregnancies)
                        {
                            //Set Labor Belly as Big Belly.
                            if (h.def == HediffDefOf.PregnancyLabor || h.def == HediffDefOf.PregnancyLaborPushing)
                                bellySeverity = Math.Max(bellySeverity, 1f);
                            else
                                bellySeverity = Math.Max(bellySeverity, h.Severity);
                        }
                    }

                    //Licentia Lab Hediff
                    if (SizedApparelPatch.LicentiaActive)
                    {
                        Hediff cumflation = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("Cumflation"));
                        Hediff cumstuffed = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("Cumstuffed"));
                        bellySeverity += cumflation != null ? cumflation.Severity : 0;
                        bellySeverity += cumstuffed != null ? cumstuffed.Severity : 0;
                    }


                }



            }

            CheckAgeChanged();

            if (SizedApparelSettings.drawBreasts)
            {
                SizedApparelUtility.GetBreastSeverity(pawn, out breastSeverity, out breastHediff);

            }

            if (breastHediff != null)
            {

                //get nipple color from Menstruation.
                if (SizedApparelPatch.MenstruationActive)
                {
                    nippleColor = Patch_Menstruation.GetNippleColor(breastHediff);
                }



                if (pawn.gender == Gender.Male && !SizedApparelSettings.ApplyApparelPatchForMale)
                {
                    CheckApparel = false;
                }


                if (CheckApparel || this.isApparelDirty)
                {
                    hasUnsupportedApparel = SizedApparelUtility.hasUnSupportedApparelFromWornData(pawn, breastSeverity, breastHediff, true, flag);

                }
                else
                {
                    hasUnsupportedApparel = false;

                }
                //float validBreastTextureSeverity = SizedApparelUtility.GetBreastSeverityValidTextures(pawn, breastHediff);
                if (SizedApparelSettings.useBreastSizeCapForApparels) // && hasUnsupportedApparel
                {
                    BreastSeverityCache = Math.Min(BreastSeverityCache, breastSeverity);

                }
                else
                {
                    BreastSeverityCache = 1000;
                }
                //if (SizedApparelSettings.useBreastSizeCapForApparels) //SizedApparelSettings.useBreastSizeCapForApparels //wip
                //    breastSeverityCapped = Math.Min(BreastSeverityCache, breastSeverityCapped);


                //float validBreastTextureSeverity = SizedApparelUtility.GetBreastSeverityValidTextures(pawn, breastHediff);

                //breast cache forApparel brests Check. This ignore variation!
                //SizedApparelMod.CheckAndLoadAlienRaces();

                var key = new SizedApparelsDatabase.BodyPartDatabaseKey(pawn.def.defName, pawn.story?.bodyType?.defName, breastHediff.def.defName, "Breasts", pawn.gender ,SizedApparelUtility.BreastSeverityInt(breastHediff.Severity));
                var raceSetting = SizedApparelSettings.alienRaceSettings.FirstOrDefault((AlienRaceSetting s) => s.raceName == key.raceName);

                //TODO? Remove ValidBreastsTextureCheck...
                float validBreastTextureSeverity = SizedApparelUtility.BreastSizeIndexToSeverity(SizedApparelsDatabase.GetSupportedBodyPartPath(key,true,"Breasts","Breasts").size);
                //if (validBreastTextureSeverity < -5 && SizedApparelSettings.alienRaceSettings.settings[key.raceName].asHumanlike)//old: SizedApparelSettings.UnsupportedRaceToUseHumanlike
                //    validBreastTextureSeverity = SizedApparelUtility.GetBreastSeverityValidTextures(pawn, breastHediff, "Humanlike");
                BreastSeverityCache = Math.Min(BreastSeverityCache, validBreastTextureSeverity);
                if (SizedApparelSettings.Debug)
                    Log.Message("[Sized Apparel] cached Severity : " + BreastSeverityCache);
            }
            //penisHediff = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_genitalsBPR(pawn)).FirstOrDefault((Hediff h) => h.def.defName.ToLower().Contains("penis"));

            //since rjw race support's part name are too variation, not handling it.


            var anusList = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_anusBPR(pawn));
            if(!anusList.NullOrEmpty())
                anusHediff = anusList.FirstOrDefault((Hediff h) => h.def.defName.ToLower().Contains("anus"));
            //udderHediff = Genital_Helper.get_PartsHediffList(pawn, pawn.RaceProps.body.AllParts.Find((BodyPartRecord bpr) => bpr.def.defName == "Torso")).FirstOrDefault((Hediff h) => h.def.defName.ToLower().Contains("anus")); //not yet supported

            udderHediff = pawn.health?.hediffSet?.hediffs?.FirstOrDefault((Hediff h) => h.def.defName.ToLower().Contains("udder"));

            var pawnRenderer = pawn.Drawer?.renderer?.graphics;

            if (updateGraphics)
            {

                if (pawnRenderer != null)
                {
                    //pawnRenderer.ResolveAllGraphics();


                    //TODO Use Function To Make Clear Code
                    /*
                    FindAndApplyBodyGraphic(pawn, pawnRenderer.nakedGraphic, ref graphicbaseBodyNaked, ref graphicSourceNaked,"Naked");
                    FindAndApplyBodyGraphic(pawn, pawnRenderer.rottingGraphic, ref graphicbaseBodyRotten, ref graphicSourceRotten, "Rotting");
                    FindAndApplyBodyGraphic(pawn, pawnRenderer.nakedGraphic, ref graphicbaseBodyFurCovered, ref graphicSourceFurCovered, "FurCuvered");
                    */
                    const string baseBodyString = "_BaseBody";
                    string baseBodyStringWithSex;

                    if (SizedApparelSettings.useGenderSpecificTexture)
                    {
                        if (pawn.gender == Gender.Female)
                        {
                            baseBodyStringWithSex = baseBodyString + "F";
                        }
                        else if (pawn.gender == Gender.Male)
                        {
                            baseBodyStringWithSex = baseBodyString + "M";
                        }
                        else
                        {
                            baseBodyStringWithSex = baseBodyString; // + "N" //Does it need to add N?
                        }
                    }
                    else
                        baseBodyStringWithSex = baseBodyString;

                    string nakedGraphicPath = null;
                    if (pawnRenderer.nakedGraphic != null)
                    {
                        //path = path.Insert(Math.Max(path.LastIndexOf('/'), 0), "/CustomPose/"+ customPose);
                        if(graphicSourceNaked == null)
                            graphicSourceNaked = pawnRenderer.nakedGraphic;
                        nakedGraphicPath = graphicSourceNaked.path;

                        if (customPose != null)
                            nakedGraphicPath = nakedGraphicPath.Insert(Math.Max(nakedGraphicPath.LastIndexOf('/'), 0), "/CustomPose/" + customPose);

                        if (!nakedGraphicPath.Contains(baseBodyString))
                        {
                            if (SizedApparelSettings.useGenderSpecificTexture & pawn.gender != Gender.None)
                            {
                                if (graphicbaseBodyNaked == null)
                                {
                                    if (ContentFinder<Texture2D>.Get((nakedGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                    {
                                        //graphicSourceNaked = pawnRenderer.nakedGraphic;
                                        Shader shader = pawnRenderer.nakedGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyNaked = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                        //pawnRenderer.nakedGraphic = graphicbaseBodyNaked;
                                    }
                                    else if (customPose != null)
                                    {
                                        nakedGraphicPath = pawnRenderer.nakedGraphic.path;
                                        if (ContentFinder<Texture2D>.Get((nakedGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                        {
                                            //graphicSourceNaked = pawnRenderer.nakedGraphic;
                                            Shader shader = pawnRenderer.nakedGraphic.Shader;
                                            //if (!ShaderUtility.SupportsMaskTex(shader))
                                            //    shader = ShaderDatabase.CutoutSkinOverlay;
                                            graphicbaseBodyNaked = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                            //pawnRenderer.nakedGraphic = graphicbaseBodyNaked;
                                        }
                                        else
                                        {
                                            if (SizedApparelSettings.Debug)
                                                Log.Warning("[SizedApparel] Missing BaseBodyTexture for naked Graphic: " + nakedGraphicPath + baseBodyStringWithSex + "_south");
                                        }
                                    }
                                }
                            }
                            if (graphicbaseBodyNaked == null)
                            {
                                if (ContentFinder<Texture2D>.Get((nakedGraphicPath + baseBodyString + "_south"), false) != null)
                                {
                                   // graphicSourceNaked = pawnRenderer.nakedGraphic;
                                    Shader shader = pawnRenderer.nakedGraphic.Shader;
                                    //if (!ShaderUtility.SupportsMaskTex(shader))
                                    //    shader = ShaderDatabase.CutoutSkinOverlay;
                                    graphicbaseBodyNaked = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + baseBodyString, shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                    //pawnRenderer.nakedGraphic = graphicbaseBodyNaked;
                                }
                                else if (customPose != null)
                                {
                                    nakedGraphicPath = pawnRenderer.nakedGraphic.path;
                                    if (ContentFinder<Texture2D>.Get((nakedGraphicPath + baseBodyString + "_south"), false) != null)
                                    {
                                        //graphicSourceNaked = pawnRenderer.nakedGraphic;
                                        Shader shader = pawnRenderer.nakedGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyNaked = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + baseBodyString, shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                        //pawnRenderer.nakedGraphic = graphicbaseBodyNaked;
                                    }
                                    else
                                    {
                                        if (SizedApparelSettings.Debug)
                                            Log.Warning("[SizedApparel] Missing BaseBodyTexture for naked Graphic: " + nakedGraphicPath + baseBodyString + "_south");
                                    }

                                }
                            }

                        }
                        else
                            graphicbaseBodyNaked = pawnRenderer.nakedGraphic;

                    }
                    //Finding Texture Points
                    if (false && graphicbaseBodyNaked != null)
                    {
                        SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphicbaseBodyNaked.path);
                        if (SizedApparelSettings.Debug && PointsDef != null)
                        {
                            Log.Message("[SizedApparel] : NakedBaseBody Texture Points Def Found : " + PointsDef.defName);
                        }
                        baseBodyNakedPoints = PointsDef;
                    }
                    else
                        baseBodyNakedPoints = null;

                    string rottingGraphicPath = null;
                    if (pawnRenderer.rottingGraphic != null)
                    {
                        if (graphicSourceRotten == null)
                            graphicSourceRotten = pawnRenderer.rottingGraphic;
                        rottingGraphicPath = graphicSourceRotten.path;

                        if (customPose != null)
                            rottingGraphicPath = rottingGraphicPath.Insert(Math.Max(rottingGraphicPath.LastIndexOf('/'), 0), "/CustomPose/" + customPose);

                        if (!rottingGraphicPath.Contains(baseBodyString))
                        {
                            if (graphicbaseBodyRotten == null)
                            {
                                if (ContentFinder<Texture2D>.Get((rottingGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                {
                                    //graphicSourceRotten = pawnRenderer.rottingGraphic;
                                    Shader shader = pawnRenderer.rottingGraphic.Shader;
                                    //if (!ShaderUtility.SupportsMaskTex(shader))
                                    //    shader = ShaderDatabase.CutoutSkinOverlay;
                                    graphicbaseBodyRotten = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                    //pawnRenderer.rottingGraphic = graphicbaseBodyRotten;
                                }
                                else if (customPose != null)
                                {
                                    rottingGraphicPath = pawnRenderer.rottingGraphic.path;
                                    if (ContentFinder<Texture2D>.Get((rottingGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                    {
                                        graphicSourceRotten = pawnRenderer.rottingGraphic;
                                        Shader shader = pawnRenderer.rottingGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyRotten = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                        //pawnRenderer.rottingGraphic = graphicbaseBodyRotten;
                                    }
                                    else
                                    {
                                        if (SizedApparelSettings.Debug)
                                            Log.Warning("[SizedApparel] Missing BaseBodyTexture for rotting Graphic: " + rottingGraphicPath + baseBodyStringWithSex + "_south");
                                    }
                                }
                                if (graphicbaseBodyRotten == null)
                                {
                                    if (ContentFinder<Texture2D>.Get((rottingGraphicPath + baseBodyString + "_south"), false) != null)
                                    {
                                        //graphicSourceRotten = pawnRenderer.rottingGraphic;
                                        Shader shader = pawnRenderer.rottingGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyRotten = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + baseBodyString, shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                        //pawnRenderer.rottingGraphic = graphicbaseBodyRotten;
                                    }
                                    else if (customPose != null)
                                    {
                                        rottingGraphicPath = pawnRenderer.rottingGraphic.path;
                                        if (ContentFinder<Texture2D>.Get((rottingGraphicPath + baseBodyString + "_south"), false) != null)
                                        {
                                            graphicSourceRotten = pawnRenderer.rottingGraphic;
                                            Shader shader = pawnRenderer.rottingGraphic.Shader;
                                            //if (!ShaderUtility.SupportsMaskTex(shader))
                                            //    shader = ShaderDatabase.CutoutSkinOverlay;
                                            graphicbaseBodyRotten = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + baseBodyString, shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                            //pawnRenderer.rottingGraphic = graphicbaseBodyRotten;
                                        }
                                        else
                                        {
                                            if (SizedApparelSettings.Debug)
                                                Log.Warning("[SizedApparel] Missing BaseBodyTexture for rotting Graphic: " + rottingGraphicPath + baseBodyString + "_south");
                                        }
                                    }
                                }
                            }
                        }
                        else
                            graphicbaseBodyRotten = pawnRenderer.rottingGraphic;

                    }
                    //Finding Texture Points
                    if (false && graphicbaseBodyRotten != null)
                    {
                        SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphicbaseBodyRotten.path);
                        if (SizedApparelSettings.Debug && PointsDef != null)
                        {
                            Log.Message("[SizedApparel] : RottenBaseBody Texture Points Def Found : " + PointsDef.defName);
                        }
                        baseBodyRottenPoints = PointsDef;
                    }
                    else
                        baseBodyRottenPoints = null;


                    string furCoveredGraphicPath = null;
                    if (pawnRenderer.furCoveredGraphic != null)
                    {
                        if (graphicSourceFurCovered == null)
                            graphicSourceFurCovered = pawnRenderer.furCoveredGraphic;
                        furCoveredGraphicPath = graphicSourceFurCovered.path;

                        if (customPose != null)
                            furCoveredGraphicPath = furCoveredGraphicPath.Insert(Math.Max(furCoveredGraphicPath.LastIndexOf('/'), 0), "/CustomPose/" + customPose);

                        if (!furCoveredGraphicPath.Contains(baseBodyString))
                        {
                            if (graphicbaseBodyFurCovered == null)
                            {
                                if (ContentFinder<Texture2D>.Get((furCoveredGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                {
                                    Shader shader = pawnRenderer.furCoveredGraphic.Shader;
                                    //if (!ShaderUtility.SupportsMaskTex(shader))
                                    //    shader = ShaderDatabase.CutoutSkinOverlay;
                                    graphicbaseBodyFurCovered = GraphicDatabase.Get<Graphic_Multi>(furCoveredGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.furCoveredGraphic.drawSize, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo, pawnRenderer.furCoveredGraphic.data);
                                }
                                else if (customPose != null)
                                {
                                    furCoveredGraphicPath = pawnRenderer.furCoveredGraphic.path;
                                    if (ContentFinder<Texture2D>.Get((furCoveredGraphicPath + baseBodyStringWithSex + "_south"), false) != null)
                                    {
                                        graphicSourceFurCovered = pawnRenderer.furCoveredGraphic;
                                        Shader shader = pawnRenderer.furCoveredGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyFurCovered = GraphicDatabase.Get<Graphic_Multi>(furCoveredGraphicPath + baseBodyStringWithSex, shader, pawnRenderer.furCoveredGraphic.drawSize, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo, pawnRenderer.furCoveredGraphic.data);
                                    }
                                    else
                                    {
                                        if (SizedApparelSettings.Debug)
                                            Log.Warning("[SizedApparel] Missing BaseBodyTexture for furCovered Graphic: " + furCoveredGraphicPath + baseBodyStringWithSex + "_south");
                                    }
                                }
                                if (graphicbaseBodyFurCovered == null)
                                {
                                    if (ContentFinder<Texture2D>.Get((furCoveredGraphicPath + baseBodyString + "_south"), false) != null)
                                    {
                                        Shader shader = pawnRenderer.furCoveredGraphic.Shader;
                                        //if (!ShaderUtility.SupportsMaskTex(shader))
                                        //    shader = ShaderDatabase.CutoutSkinOverlay;
                                        graphicbaseBodyFurCovered = GraphicDatabase.Get<Graphic_Multi>(furCoveredGraphicPath + baseBodyString, shader, pawnRenderer.furCoveredGraphic.drawSize, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo, pawnRenderer.furCoveredGraphic.data);
                                    }
                                    else if (customPose != null)
                                    {
                                        furCoveredGraphicPath = pawnRenderer.furCoveredGraphic.path;
                                        if (ContentFinder<Texture2D>.Get((furCoveredGraphicPath + baseBodyString + "_south"), false) != null)
                                        {
                                            graphicSourceFurCovered = pawnRenderer.furCoveredGraphic;
                                            Shader shader = pawnRenderer.furCoveredGraphic.Shader;
                                            graphicbaseBodyFurCovered = GraphicDatabase.Get<Graphic_Multi>(furCoveredGraphicPath + baseBodyString, shader, pawnRenderer.furCoveredGraphic.drawSize, pawnRenderer.furCoveredGraphic.color, pawnRenderer.furCoveredGraphic.colorTwo, pawnRenderer.furCoveredGraphic.data);

                                        }
                                        else
                                        {
                                            if (SizedApparelSettings.Debug)
                                                Log.Warning("[SizedApparel] Missing BaseBodyTexture for naked Graphic: " + furCoveredGraphicPath + baseBodyString + "_south");
                                        }
                                    }
                                }
                            }
                        }
                        else
                            graphicbaseBodyFurCovered = pawnRenderer.furCoveredGraphic;

                    }
                    //Finding Texture Points
                    if (false && graphicbaseBodyFurCovered != null)
                    {
                        SizedApparelTexturePointDef PointsDef = DefDatabase<SizedApparelTexturePointDef>.AllDefs.FirstOrDefault((SizedApparelTexturePointDef s) => s.Path == graphicbaseBodyFurCovered.path);
                        if (SizedApparelSettings.Debug && PointsDef != null)
                        {
                            Log.Message("[SizedApparel] : FurCoveredBaseBody Texture Points Def Found : " + PointsDef.defName);
                        }
                        baseBodyFurCoveredPoints = PointsDef;
                    }
                    else
                        baseBodyFurCoveredPoints = null;


                    /*
                    //Try to find sized Body if it's valid
                    int offset = 9999;
                    int currentBreastSizeIndex = 0;
                    float currentBreastSeverity = 0;
                    bool validNakedTexture = false;
                    bool validRottingTexture = false;
                    Graphic nakedGraphic;
                    Graphic rottingGraphic;
                    while (offset < SizedApparelUtility.size.Length)
                    {
                        string breastSeverityStringCache = SizedApparelUtility.BreastSeverityString(breastSeverity, offset, true, ref currentBreastSizeIndex, ref currentBreastSeverity);
                        //search bigger
                        //SizedApparelSettings.matchBodyTextureToMinimumApparelSize? currentBreastSizeIndex <= minSupportedBreasSizeIndex:true
                        if (validNakedTexture == false && nakedGraphicPath !=null)
                        {
                            if ((ContentFinder<Texture2D>.Get((nakedGraphicPath + breastSeverityStringCache + "_south"), false) != null))
                            {
                                if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize ? SizedApparelUtility.BreastSizeIndexToSeverity(currentBreastSizeIndex) <= breastSeverityCapToDraw : true)
                                {
                                    nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + breastSeverityStringCache, pawnRenderer.nakedGraphic.Shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                                    //validTexture = true;
                                    //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                    pawnRenderer.nakedGraphic = nakedGraphic;
                                    validNakedTexture = true;
                                }
                            }
                        }
                        if (validRottingTexture == false && rottingGraphicPath != null)
                        {
                            if ((ContentFinder<Texture2D>.Get((rottingGraphicPath + breastSeverityStringCache + "_south"), false) != null))
                            {
                                if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize ? SizedApparelUtility.BreastSizeIndexToSeverity(currentBreastSizeIndex) <= breastSeverityCapToDraw : true)
                                {
                                    rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + breastSeverityStringCache, pawnRenderer.rottingGraphic.Shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                                    //validTexture = true;
                                    //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                    pawnRenderer.rottingGraphic = rottingGraphic;
                                    validRottingTexture = true;
                                }
                            }
                        }

                        //search smaller

                        breastSeverityStringCache = SizedApparelUtility.BreastSeverityString(breastSeverity, offset, false, ref currentBreastSizeIndex, ref currentBreastSeverity);

                        if (validNakedTexture == false)
                        {
                            if ((ContentFinder<Texture2D>.Get((nakedGraphicPath + breastSeverityStringCache + "_south"), false) != null))
                            {
                                if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize ? SizedApparelUtility.BreastSizeIndexToSeverity(currentBreastSizeIndex) <= breastSeverityCapToDraw : true)
                                {
                                    nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(nakedGraphicPath + breastSeverityStringCache, pawnRenderer.nakedGraphic.Shader, pawnRenderer.nakedGraphic.drawSize, pawnRenderer.nakedGraphic.color, pawnRenderer.nakedGraphic.colorTwo, pawnRenderer.nakedGraphic.data);
                                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                                    //validTexture = true;
                                    //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                    pawnRenderer.nakedGraphic = nakedGraphic;
                                    validNakedTexture = true;
                                }
                            }
                        }
                        if (validRottingTexture == false)
                        {
                            if ((ContentFinder<Texture2D>.Get((rottingGraphicPath + breastSeverityStringCache + "_south"), false) != null))
                            {
                                if (SizedApparelSettings.matchBodyTextureToMinimumApparelSize ? SizedApparelUtility.BreastSizeIndexToSeverity(currentBreastSizeIndex) <= breastSeverityCapToDraw : true)
                                {
                                    rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(rottingGraphicPath + breastSeverityStringCache, pawnRenderer.rottingGraphic.Shader, pawnRenderer.rottingGraphic.drawSize, pawnRenderer.rottingGraphic.color, pawnRenderer.rottingGraphic.colorTwo, pawnRenderer.rottingGraphic.data);
                                    //newAgr.Add(new ApparelGraphicRecord(graphic, agr.sourceApparel));
                                    //validTexture = true;
                                    //Log.Message(path + BreastSeverityString(BreastSeverity, offset, !findBigger) + ":Texture Found");
                                    pawnRenderer.rottingGraphic = rottingGraphic;
                                    validRottingTexture = true;
                                }
                            }
                        }

                        if (validNakedTexture == true && validRottingTexture == true)
                        {
                            if (SizedApparelSettings.Debug)
                                Log.Message("[Sized Apparel] " + pawn.Name + "'s body texture has changed.");
                            break;
                        }

                        offset++;
                    }
                    */
                }

                //graphicBreasts = SizedApparelUtility.GetBodyPartGraphic(pawn, breastHediff, true, "Breasts", "Breasts");
                //if(graphicBreasts == null && pawn.RaceProps.Humanlike && SizedApparelSettings.UnsupportedRaceToUseHumanlike)
                //    graphicBreasts = SizedApparelUtility.GetBodyPartGraphic(pawn, breastHediff, true, "Breasts", "Breasts", false, true, "Humanlike");


                if (SizedApparelSettings.drawBodyParts)//body parts update
                {
                    if (SizedApparelSettings.drawBreasts)
                    {

                        if (breastHediff != null)
                        {
                            var breastvar = breastHediff.TryGetComp<SizedApparelBodyPartDetail>();

                            foreach(var addon in bodyAddons)
                            {
                                if(addon.bodyPartOf == SizedApparelBodyPartOf.Breasts) //Include SizedApparelBodyPartOf.Nipples
                                {
                                    addon.SetHediffData(breastHediff.def.defName, SizedApparelUtility.BreastSeverityInt(breastHediff.Severity), SizedApparelUtility.BreastSeverityInt(BreastSeverityCache), breastvar?.variation);
                                    addon.SetBone(skeleton?.FindBone("Breasts"));
                                    addon.UpdateGraphic();
                                }


                                if (SizedApparelPatch.MenstruationActive)
                                {
                                    //Nipple Patch?




                                }
                            }
                            //bodyPartBreasts.SetHediffData(breastHediff.def.defName, SizedApparelUtility.BreastSeverityInt(breastHediff.Severity), SizedApparelUtility.BreastSeverityInt(breastSeverityCapped), breastvar?.variation);
                            //bodyPartBreasts.UpdateGraphic();
                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Breasts)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }





                    }




                    if (SizedApparelSettings.drawUdder)
                    {
                        if (udderHediff != null)
                        {
                            var udderVar = udderHediff.TryGetComp<SizedApparelBodyPartDetail>();
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Udder)
                                {
                                    addon.SetHediffData(udderHediff.def.defName, SizedApparelUtility.BreastSeverityInt(udderHediff.Severity), 1000, udderVar?.variation);
                                    addon.SetBone(skeleton?.FindBone("Udder"));
                                    addon.UpdateGraphic();
                                }
                            }
                            //bodyPartUdder.SetHediffData(udderHediff.def.defName, SizedApparelUtility.BreastSeverityInt(udderHediff.Severity), 1000, udderVar?.variation);
                            //bodyPartUdder.UpdateGraphic();

                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Udder)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }


                    }


                    if (SizedApparelSettings.drawBelly)
                    {



                        if (bellySeverity >= 0)
                        {
                            //var bellyVar = breastHediff.GetComp<SizedApparelBodyPartDetail>();
                            string BellyVar = null;
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Belly)
                                {
                                    addon.SetHediffData("BellyBulge", SizedApparelUtility.PrivatePartSeverityInt(bellySeverity), 1000, BellyVar);
                                    addon.SetBone(skeleton?.FindBone("Belly"));
                                    addon.UpdateGraphic();
                                }
                            }
                            //bodyPartBelly.SetHediffData("BellyBulge", SizedApparelUtility.PrivatePartSeverityInt(bellySeverity), 1000, BellyVar);
                            //bodyPartBelly.UpdateGraphic();
                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Belly)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }

                    }


                    if (SizedApparelSettings.drawVagina)
                    {
                        if (vaginaHediff != null)
                        {
                            var vaginaVar = vaginaHediff.TryGetComp<SizedApparelBodyPartDetail>();
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Vagina)
                                {
                                    addon.SetHediffData(vaginaHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(vaginaHediff.Severity), 1000, vaginaVar?.variation);
                                    addon.SetBone(skeleton?.FindBone("Vagina"));
                                    addon.UpdateGraphic();

                                }
                            }
                            //bodyPartVagina.SetHediffData(vaginaHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(vaginaHediff.Severity), 1000, vaginaVar?.variation);
                            //bodyPartVagina.UpdateGraphic();

                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Vagina)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }
                    }

                    if (SizedApparelSettings.drawPubicHair)
                    {
                        if (pubicHairDef != null && pubicHairDef.defName != "None") // pubicHairDef != null // for testing
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.PubicHair)
                                {
                                    addon.SetHediffData(pubicHairDef.defName, 0, 1000, null);
                                    addon.SetBone(skeleton?.FindBone("PubicHair"));
                                    addon.UpdateGraphic();
                                }
                            }

                            //bodyPartPubicHair.SetHediffData(pubicHairDef.defName, 0, 1000, null);
                            //bodyPartPubicHair.UpdateGraphic();
                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.PubicHair)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }
                    }

                    if (SizedApparelSettings.drawPenis)
                    {
                        //TODO: Optimize.... memory leak issue?
                        //bodyPartPenises.Clear();
                        //bodyPartBalls.Clear();
                        if (!penisHediffs.NullOrEmpty())
                        {
                            var penisHediff = penisHediffs[0];
                            
                            if (penisHediff != null)
                            {
                                var penisVar = penisHediff.TryGetComp<SizedApparelBodyPartDetail>();
                                foreach (var addon in bodyAddons)
                                {
                                    if (addon.bodyPartOf == SizedApparelBodyPartOf.Penis)
                                    {
                                        addon.SetHediffData(penisHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(penisHediff.Severity), 1000, penisVar?.variation);
                                        addon.SetBone(skeleton?.FindBone("Penis"));
                                        addon.UpdateGraphic();
                                    }
                                    if (addon.bodyPartOf == SizedApparelBodyPartOf.Balls)
                                    {
                                        addon.SetHediffData(penisHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(penisHediff.Severity), 1000, penisVar?.variation);
                                        addon.SetBone(skeleton?.FindBone("Balls"));
                                        addon.UpdateGraphic();
                                    }
                                }
                            }
                            else
                            {
                                foreach (var addon in bodyAddons)
                                {
                                    if (addon.bodyPartOf == SizedApparelBodyPartOf.Penis)
                                    {
                                        addon.SetHediffData(null, -1);
                                    }
                                    if (addon.bodyPartOf == SizedApparelBodyPartOf.Balls)
                                    {
                                        addon.SetHediffData(null, -1);
                                    }
                                }
                            }


                            // Multiple Penis Drawing. obsolte
                            /*
                            for (int i = 0; i < penisHediffs.Count; i++)
                            {
                                penisHediff = penisHediffs[i];
                                float offsetX = 0; // right and left
                                float offsetZ = 0; // up and down
                                float t = (i + 1f) / (penisHediffs.Count + 1);
                                offsetX = Mathf.Lerp(-0.05f, 0.05f, t);
                                offsetZ = Mathf.Lerp(-0.02f, 0.02f, t);

                                if (penisHediff == null)
                                    continue;

                                Bone penisBone = null;
                                Bone ballsBone = null;
                                if(skeleton != null)
                                {
                                    penisBone = skeleton.FindBone("Penis");
                                    ballsBone = skeleton.FindBone("Balls");
                                    if (ballsBone == null)
                                        ballsBone = penisBone;
                                }



                                var penisVar = penisHediff.TryGetComp<SizedApparelBodyPartDetail>();

                                SizedApparelBodyPart penis = new SizedApparelBodyPart(pawn, this, "Penis", SizedApparelBodyPartOf.Penis, "Penis", false, false, null,ColorType.Skin, penisBone);
                                penis.SetDepthOffsets(0.0108f, 0.0025f, 0.0108f, 0.0108f);
                                penis.SetPositionOffsets(new Vector2(offsetX, 0), new Vector2(-offsetX, 0), new Vector2(offsetX, offsetZ), new Vector2(offsetX, -offsetZ));
                                penis.SetHediffData(penisHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(penisHediff.Severity), 1000, penisVar?.variation);
                                penis.centeredTexture = true; // Test Bone
                                penis.UpdateGraphic();
                                bodyPartPenises.Add(penis);

                                SizedApparelBodyPart balls = new SizedApparelBodyPart(pawn, this, "Balls", SizedApparelBodyPartOf.Balls, "Penis", false, false, "Penis/Balls", ColorType.Skin, ballsBone);
                                balls.SetDepthOffsets(0.0096f, 0.0085f, 0.0096f, 0.0096f);
                                balls.SetPositionOffsets(new Vector2(offsetX, 0), new Vector2(-offsetX, 0), new Vector2(offsetX, offsetZ), new Vector2(offsetX, -offsetZ));
                                balls.SetHediffData(penisHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(penisHediff.Severity), 1000, penisVar?.variation);
                                balls.centeredTexture = true; // Test Bone
                                balls.UpdateGraphic();
                                bodyPartBalls.Add(balls);
                                */

                        }
                    }
                    
                    else
                    {
                        bodyPartPenises.Clear();
                        bodyPartBalls.Clear();
                    }

                    if (SizedApparelSettings.drawAnus)
                    {

                        /*
                        graphicAnus = SizedApparelUtility.GetBodyPartGraphic(pawn, anusHediff, false, "Anus", "Anus");
                        if (graphicAnus == null && pawn.RaceProps.Humanlike && SizedApparelSettings.UnsupportedRaceToUseHumanlike)
                            graphicAnus = SizedApparelUtility.GetBodyPartGraphic(pawn, anusHediff, false, "Anus", "Anus", false, true, "Humanlike");

                        graphicAnus_horny = SizedApparelUtility.GetBodyPartGraphic(pawn, anusHediff, false, "Anus", "Anus", true);
                        if (graphicAnus_horny == null && pawn.RaceProps.Humanlike && SizedApparelSettings.UnsupportedRaceToUseHumanlike)
                            graphicAnus_horny = SizedApparelUtility.GetBodyPartGraphic(pawn, anusHediff, false, "Anus", "Anus", true, true, "Humanlike");
                        */
                        if (anusHediff != null)
                        {
                            var anusVar = anusHediff.TryGetComp<SizedApparelBodyPartDetail>();
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Anus)
                                {
                                    addon.SetHediffData(anusHediff.def.defName, SizedApparelUtility.PrivatePartSeverityInt(anusHediff.Severity), 1000, anusVar?.variation);
                                    addon.SetBone(skeleton?.FindBone("Anus"));
                                    addon.UpdateGraphic();
                                }
                            }
                        }
                        else
                        {
                            foreach (var addon in bodyAddons)
                            {
                                if (addon.bodyPartOf == SizedApparelBodyPartOf.Anus)
                                {
                                    addon.SetHediffData(null, -1);
                                }
                            }
                        }
                    }

                }
                hasGraphicUpdatedBefore = true;
            }

            if(CheckApparel)
                cachedApparelGraphicRecord = pawnRenderer.apparelGraphics.ToList();
            else
                cachedApparelGraphicRecord = new List<ApparelGraphicRecord>();

            hasUpdateBeforeSuccess = true;
            this.isHediffDirty = false;
            this.isApparelDirty = false;
            this.isSkeletonDirty = false;
        }





        //public bool hasUnsupportedApparel => Props.hasUnsupportedApparel;
        //public bool hasUpdateBefore => Props.hasUpdateBefore;
        /*
        public void SetHasUnsupportedApparel(bool _hasUnsupportedApparel)
        {
            if(Props !=null)
            Props.hasUnsupportedApparel = _hasUnsupportedApparel;
            this.hasUnsupportedApparel = _hasUnsupportedApparel;
        }
        public void SetHasUpdateBefore(bool _hasUpdateBefore)
        {
            if (Props != null)
                Props.hasUpdateBefore = _hasUpdateBefore;
            this.hasUpdateBefore = _hasUpdateBefore;
        }*/
        public IEnumerable<SizedApparelBodyPart> GetAllSizedApparelBodyPart() // can return null bodyparts
        {
            yield return bodyPartBreasts;
            yield return bodyPartNipple;
            /*
            foreach (SizedApparelBodyPart bp in bodyPartBreasts)
            {
                yield return bp;
            }
            foreach (SizedApparelBodyPart bp in bodyPartNipple)
            {
                yield return bp;
            }*/
            foreach (SizedApparelBodyPart bp in bodyPartPenises)
            {
                yield return bp;
            }
            foreach (SizedApparelBodyPart bp in bodyPartBalls)
            {
                yield return bp;
            }
            yield return bodyPartVagina;
            yield return bodyPartAnus;
            yield return bodyPartBelly;
            yield return bodyPartMuscleOverlay;//TODO
            yield return bodyPartUdder;
            yield return bodyPartPubicHair; //TODO
            yield return bodyPartHips;
            foreach (SizedApparelBodyPart bp in bodyPartThighs)
            {
                yield return bp;
            }
            foreach (SizedApparelBodyPart bp in bodyPartHands)
            {
                yield return bp;
            }
            foreach (SizedApparelBodyPart bp in bodyPartFeet)
            {
                yield return bp;
            }
            foreach (SizedApparelBodyPart bp in bodyAddons)
            {
                yield return bp;
            }
        }
        public IEnumerable<SizedApparelBodyPart> GetSizedApparelBodyParts(SizedApparelBodyPartOf targetPartOf, bool returnNullPart = false)
        {
            foreach(SizedApparelBodyPart bp in GetAllSizedApparelBodyPart())
            {
                if (bp == null)
                {
                    if (returnNullPart)
                        yield return bp;
                    else
                        continue;
                }

                
                if (bp.bodyPartOf.IsPartOf(targetPartOf))
                    yield return bp;
            }
        }

        public void SetPoseFromPoseSet(string poseSetName, bool autoUpdate = true, bool autoSetPawnGraphicDirty = false)
        {
            if (poseSetName.NullOrEmpty())
            {
                ClearAllPose(autoUpdate, autoSetPawnGraphicDirty);
                return;
            }

            var poseSetDef = DefDatabase<SizedApparelPoseSetDef>.GetNamed(poseSetName,false);
            if (poseSetDef == null)
                return;
            if (poseSetDef.poses.NullOrEmpty())
                return;
            foreach (SizedApparelPose pose in poseSetDef.poses)
            {
                var bodyParts = GetSizedApparelBodyParts(pose.targetBodyPart);
                //if (bodyParts == null)
                //    continue;
                if (bodyParts.EnumerableNullOrEmpty())
                    continue;
                foreach (SizedApparelBodyPart bp in bodyParts.ToList())
                {
                    if(bp != null)
                        bp.SetCustomPose(poseSetName, autoUpdate, autoSetPawnGraphicDirty);
                }
            }
        }
        public void ClearAllPose(bool autoUpdate = true, bool autoSetPawnGraphicDirty = false)
        {
            foreach (SizedApparelBodyPart bp in GetAllSizedApparelBodyPart())
            {
                if(bp != null)
                    bp.SetCustomPose(null, autoUpdate, autoSetPawnGraphicDirty);
            }
        }
        public void ClearPose(SizedApparelBodyPartOf targetPartOf , bool autoUpdate = true, bool autoSetPawnGraphicDirty = false)
        {
            foreach (SizedApparelBodyPart bp in GetSizedApparelBodyParts(targetPartOf))
            {
                if(bp != null)
                    bp.SetCustomPose(null, autoUpdate, autoSetPawnGraphicDirty);
            }
        }

    }
    [StaticConstructorOnStartup]
    public class ApparelRecorderCompProperties : CompProperties
    {
        public bool hasUnsupportedApparel = false;
        public bool hasUpdateBefore = false;

        public ApparelRecorderCompProperties()
        {
            this.compClass = typeof(ApparelRecorderComp);
        }
        public ApparelRecorderCompProperties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }

    }
}
