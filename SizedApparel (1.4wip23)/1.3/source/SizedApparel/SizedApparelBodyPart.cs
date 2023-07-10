using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using AlienRace;
using UnityEngine;
using Verse;

namespace SizedApparel
{
    public class Depth4Offsets
    {
        public float south=0;
        public float north=0;
        public float east=0;
        public float west=0;

        public Depth4Offsets() { }

        public Depth4Offsets(Vector4 arg)
        {
            south = arg.x;
            north = arg.y;
            east = arg.z;
            west = arg.w;
        }
        public Depth4Offsets(float s, float n, float e, float w)
        {
            south = s;
            north = n;
            east = e;
            west = w;
        }
    }

    public class Rot4Offsets
    {
        //X: right and left
        //Y: Frong or Back
        //Z: Up and Down
        Vector3 South;

        Vector3 North;

        Vector3 East;

        Vector3 West;

        public Rot4Offsets(Vector3 vector)
        {
            South = vector;
            North = vector;
            East = vector;
            West = vector;
        }

        public Rot4Offsets(Vector3 south, Vector3 north, Vector3 east, Vector3 west)
        {
            South = south;
            North = north;
            East = east;
            West = west;
        }

        public Vector3 GetOffset(Rot4 rotation)
        {
            if (rotation == Rot4.East)
                return East;
            if (rotation == Rot4.West)
                return West;
            if (rotation == Rot4.South)
                return South;
            if (rotation == Rot4.North)
                return North;
            else
                return Vector3.zero;
        }

    }

    public struct RaceNameAndBodyType
    {
        public string raceName;
        public string bodyType;
    }

    public class BodyWithBodyType
    {
        public string bodyType;
        public List<BodyPart> Addons = new List<BodyPart>();
    }

    public class BodyPart
    {
        public string partName = null;
        public string customPath = null;
        public string defaultHediffName = null; // for missing Hediff
        public bool isBreasts = false;
        public bool centeredTexture = true;
        public bool mustMatchBodyType = false; // TODO

        public string boneName = null;
        public Bone bone = null; // For Graphic Positioning System
        public bool mustHaveBone = true; // when bone is missing, don't draw

        public SizedApparelBodyPartOf bodyPartOf = SizedApparelBodyPartOf.None;
        public ColorType colorType = ColorType.Skin;
        public Depth4Offsets depthOffset = new Depth4Offsets();
        public BodyTypeAndOffset offsets = new BodyTypeAndOffset();
    }

    public class BodyTypeAndOffset
    {
        //public RaceNameAndBodyType bodyTypeData;
        public string bodyType;
        public Rot4Offsets offsets = new Rot4Offsets(Vector3.zero);

        public BodyTypeAndOffset()
        {

        }

        public BodyTypeAndOffset(bool useCenter)
        {
            if (useCenter)
            {
                offsets = new Rot4Offsets(Vector3.zero);
            }
        }
        public BodyTypeAndOffset(Vector3 defaultOffset)
        {
            offsets = new Rot4Offsets(defaultOffset);
        }
    }

    public enum ColorType
    {
        Skin, Hair, Nipple, Custom, None
    }


    public enum SizedApparelBodyPartOf
    {
        All, Torso, Breasts, Crotch, Penis, Balls, Vagina, Anus, Belly, PubicHair, Udder, Hips, Thighs, hands, feet, None
    }
    public static class SizedApparelBodyPartOfExtension
    {
        public static bool IsPartOf(this SizedApparelBodyPartOf source, SizedApparelBodyPartOf target)
        {
            if (source == SizedApparelBodyPartOf.None)
                return false;

            switch (target)
            {
                case SizedApparelBodyPartOf.All:
                    return true;
                case SizedApparelBodyPartOf.Torso:
                    if (source == SizedApparelBodyPartOf.hands || source == SizedApparelBodyPartOf.feet)
                        return false;
                    return true;
                case SizedApparelBodyPartOf.Breasts:
                    if (source == SizedApparelBodyPartOf.Breasts)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Crotch:
                    if (source == SizedApparelBodyPartOf.Crotch || source == SizedApparelBodyPartOf.Penis || source == SizedApparelBodyPartOf.Vagina || source == SizedApparelBodyPartOf.Anus || source == SizedApparelBodyPartOf.PubicHair || source == SizedApparelBodyPartOf.Balls)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Penis:
                    if (source == SizedApparelBodyPartOf.Penis)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Balls:
                    if (source == SizedApparelBodyPartOf.Balls)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Vagina:
                    if (source == SizedApparelBodyPartOf.Vagina)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Anus:
                    if (source == SizedApparelBodyPartOf.Anus)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Belly:
                    if (source == SizedApparelBodyPartOf.Belly)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Udder:
                    if (source == SizedApparelBodyPartOf.Udder)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Hips:
                    if (source == SizedApparelBodyPartOf.Hips || source == SizedApparelBodyPartOf.Thighs || source == SizedApparelBodyPartOf.Penis || source == SizedApparelBodyPartOf.Vagina || source == SizedApparelBodyPartOf.Anus)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.Thighs:
                    if (source == SizedApparelBodyPartOf.Thighs)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.hands:
                    if (source == SizedApparelBodyPartOf.hands)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.feet:
                    if (source == SizedApparelBodyPartOf.feet)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.PubicHair:
                    if (source == SizedApparelBodyPartOf.PubicHair)
                        return true;
                    return false;
                case SizedApparelBodyPartOf.None:
                    return false;

            }
            Log.Error("[SizedApparel] missing SizedApparelBodyPartOf!");
            return false;
        }
    }


    public class GraphicPointsDef : Def
    {
        public List<TextureWithGraphicPoints> points;
    }

    public class TextureWithGraphicPoints
    {
        public string texturePath; // texture is already classified with bodytype 
        public List<GraphicPoint> points = new List<GraphicPoint>();
    }

    public class GraphicPoint
    {
        public string pointName;
        public Vector2 point = Vector2.zero;
    }
    public class GraphicPointsWithBodyType
    {
        public string pointName;
        public List<PointWithBodyType> points = new List<PointWithBodyType>();
    }

    public class PointWithBodyType
    {
        public string bodyTypeName; //null can be used too
        public Vector2 point = Vector2.zero;
    }

    public class BodyPartPoint
    {
        string name;
        Vector2 position = Vector2.zero;//Uv position. not pixel
    }

    [Obsolete]//todo
    public struct BodyPartSpline
    {

    }
   

    //Def for Hediff Graphic color options or else.
    public class SizedApparelHeddifDef : Def
    {
        public HediffDef hediffDef;
        //public string hediffDefName;

    }

    //Def per graphic(texture)
    [Obsolete]
    public class SizedApparelBodyPartGraphicDef : Def
    {
        public string graphicPath;
        public int severityIndex;
        public Vector2 pivot = new Vector2(0.5f, 0.5f); // custom pivot of texture. UV. not pixel
        //public Dictionary<string, BodyPartPoint> points = new Dictionary<string, BodyPartPoint>();
        //public Dictionary<string, BodyPartSpline> splines = new Dictionary<string, BodyPartSpline>();
    }

    //Def per BodyParts
    public class SizedApparelBodyPartDef : Def
    {
        SizedApparelBodyPartOf bodyPartOf = SizedApparelBodyPartOf.None;
        public bool canPose = true;
        public List<string> TexturePaths;

    }

    public class SizedApparelBodyPart
    {
        static MethodInfo overrideMatMethod = AccessTools.Method(typeof(PawnRenderer), "OverrideMaterialIfNeeded");
        static Color defaultNippleColor = Color.white;//nipple texture is already colored with pink. so make it white as default to avoid double coloring pink //Strong Pink Color = new ColorInt(255, 121, 121).ToColor

        //this is for RGB Channel Edit
        static string texturePath_White = "SizedApparel/Masks/White";
        static string texturePath_Black = "SizedApparel/Masks/Black";
        static string texturePath_Red = "SizedApparel/Masks/Red";
        static string texturePath_Green = "SizedApparel/Masks/Green";
        static string texturePath_Blue = "SizedApparel/Masks/Blue";


        public bool AutoOffsetForFurCoveredBody = true;

        public SizedApparelBodyPart(Pawn pawn, ApparelRecorderComp apparelRecorderComp, string bodyPartName, SizedApparelBodyPartOf bodyPartOf, string defaultHediffName, bool isBreast, bool isOverlay, string customPathName = null, ColorType colorOf = ColorType.Skin, bool needBoneToRender = true, Bone parentBone = null, bool isCenteredTexture = false )
        {
            this.pawn = pawn; //owner

            this.apparelRecorderCompCache = apparelRecorderComp; //for reduce GetComp Call; if it is null, it will try to get pawn's comp.

            this.bodyPartName = bodyPartName;

            this.def = DefDatabase<SizedApparelBodyPartDef>.AllDefs.FirstOrDefault(b => b.defName == bodyPartName);

            this.bodyPartOf = bodyPartOf;
            this.defaultHediffName = defaultHediffName;
            this.isBreast = isBreast;
            this.isOverlay = isOverlay;
            this.customPath = customPathName;
            this.colorType = colorOf;

            this.bone = parentBone;
            this.mustHaveBone = needBoneToRender;
            this.centeredTexture = isCenteredTexture;
        }

        public void SetCenteredTexture(bool isCentered)
        {
            this.centeredTexture = isCentered;
        }

        public Vector2 OffsetFromUVOffset(Vector2 vector, Mesh mesh , bool isFliped = false)
        {
            //treat mesh as plane
            //Vector3 width = mesh.vertices[2] - mesh.vertices[1];
            //Vector3 height = mesh.vertices[1] - mesh.vertices[2];

            
            if(!isFliped)
                return new Vector2((mesh.vertices[2].x - mesh.vertices[0].x)*vector.x,(mesh.vertices[0].z - mesh.vertices[2].z)*vector.y);
            return new Vector2((mesh.vertices[2].x - mesh.vertices[0].x)*vector.x, (mesh.vertices[2].z - mesh.vertices[0].z)*vector.y);
            /*
             * Vector2 loc = new Vector2(0.5f, 0.5f) - vector;
                         if(!isFliped)
                return new Vector2(Mathf.Lerp(mesh.vertices[0].x, mesh.vertices[2].x, loc.x), Mathf.Lerp(mesh.vertices[0].z, mesh.vertices[2].z, loc.y));
            return new Vector2(Mathf.Lerp(mesh.vertices[3].x, mesh.vertices[1].x, loc.x), Mathf.Lerp(mesh.vertices[3].z, mesh.vertices[1].z, loc.y));

             
             
             */

        }

        //public Vector2 OffestFromUVOffset(Vector2 vector, Vector2 drawSize, bool isFliped = false)

        public SizedApparelBodyPartDef def;

        public Pawn pawn;
        public ApparelRecorderComp apparelRecorderCompCache; // for reduce getComp call;
        public Bone bone;
        private bool mustHaveBone;

        public bool centeredTexture = false; // false to keep original position from mesh. and consider this graphics pivot as bone position

        public string bodyPartName; //breast, penis, belly, pubichair... etc. just name. not like architech something
        public string customPath = null;
        public SizedApparelBodyPartOf bodyPartOf = SizedApparelBodyPartOf.None;
        public string defaultHediffName;

        public bool isBreast = false;

        public bool isOverlay = false; //write z cache?

        public string currentHediffName;

        public bool isVisible = true;

        public int lastPoseTick = -1;

        public ColorType colorType = ColorType.Skin;
        public Color? customColorOne;
        public Color? customColorTwo;


        //customize
        public string customPose = null;
        public Vector2? lookAnLocation = null;
        public Rot4? rotOverride = null;

        //variation
        public string variation = null;
        public Color? variationColor;
        public colorOverrideMode variationColorMode = colorOverrideMode.Default;


        //TODO. age setting?
        public int minDrawAge = -1;
        public int maxDrawAge = -1;


        public void SetBone(Bone bone)
        {
            this.bone = bone;
        }

        public void SetCustomPose(string newPose, bool autoUpdate = true, bool autoSetPawnGraphicDirty = false)
        {
            if (customPose == newPose)
                return;
            if(SizedApparelSettings.Debug)
                Log.Message("[SizedApparel] Setting Custom Pose : " + newPose);
            customPose = newPose;
            if (autoUpdate)
            {
                this.UpdateGraphic();
                this.lastPoseTick = Find.TickManager.TicksGame;
            }

            if(autoSetPawnGraphicDirty)
            {
                if (pawn == null)
                    return;
                PortraitsCache.SetDirty(pawn);
                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
            }
        }

        public bool CheckCanPose(string targetPose, bool checkApparels, bool checkBodyParts, bool mustMatchSize, bool mustMatchBodytype, bool mustMatchRace)
        {
            if (checkApparels)
            {
                if (!SizedApparelUtility.CanPoseApparels(pawn, targetPose, currentHediffName, currentSeverityInt, cappedSeverityInt))
                    return false;
            }
            if (checkBodyParts)
            {
                Graphic graphic = GetBodyPartGraphics(false, mustMatchSize, mustMatchBodytype, mustMatchRace);
                Graphic graphicH = GetBodyPartGraphics(true, mustMatchSize, mustMatchBodytype, mustMatchRace);
                if (graphic != null || graphicH != null)
                    return true;
                return false;
            }
            return true;
        }


        //TODO...
        public int currentSeverityInt = -1;
        public int cappedSeverityInt = 1000; // supported severity from worn apparel graphics

        public Vector2 pivot = Vector2.zero;

        public Vector2 position = Vector2.zero;//offset from pivot //UV. not pixel

        public SizedApparelTexturePointDef points;
        public SizedApparelTexturePointDef pointsHorny;


        public float rotation = 0; // +: rotate right, -: rotate left
        public float scale = 1f;

        public Graphic bodyPartGraphic;
        public Graphic bodyPartGraphicHorny;



        public Vector2 positionOffset = Vector2.zero; //offset from position //UV. not pixel
        public Vector2 positionOffsetSouth = Vector2.zero;
        public Vector2 positionOffsetNorth = Vector2.zero;
        public Vector2 positionOffsetEast = Vector2.zero;
        public Vector2 positionOffsetWest = Vector2.zero;

        public float depthOffset = 0f;

        //0.008f
        public float depthOffsetEast = 0.008f;
        public float depthOffsetWest = 0.008f;
        public float depthOffsetSouth = 0.008f;
        public float depthOffsetNorth = 0.008f;


        //bigger = in front
        public void SetDepthOffsets(float south, float north, float east, float west)
        {
            depthOffsetSouth = south;
            depthOffsetNorth = north;
            depthOffsetEast = east;
            depthOffsetWest = west;
        }
        public void SetDepthOffsets(Depth4Offsets oppsets)
        {
            depthOffsetSouth = oppsets.south;
            depthOffsetNorth = oppsets.north;
            depthOffsetEast = oppsets.east;
            depthOffsetWest = oppsets.west;
        }
        public void SetPositionOffsets(Vector2 south, Vector2 north, Vector2 east, Vector2 west)
        {
            positionOffsetSouth = south;
            positionOffsetNorth = north;
            positionOffsetEast = east;
            positionOffsetWest = west;
        }
        public Graphic GetBodyPartGraphics(bool isHorny, bool mustMatchSize = false, bool mustMatchBodytype = false, bool mustMatchRace = false, string poseOverride = null)
        {
            SizedApparelTexturePointDef var;
            return GetBodyPartGraphics(isHorny, out var, mustMatchBodytype, mustMatchSize, mustMatchRace, poseOverride);
        }

        public Graphic GetBodyPartGraphics(bool isHorny, out SizedApparelTexturePointDef outPoints, bool mustMatchSize = false, bool mustMatchBodyType = false , bool mustMatchRace = false ,string poseOverride = null, string variationOverride = null)
        {
            if (pawn == null)
            {
                outPoints = null;
                return null;
            }
            var comp = apparelRecorderCompCache;
            if (comp == null)
                comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
            {
                outPoints = null;
                return null;
            }

            string bodyTypeString = pawn.story?.bodyType?.defName;

            var key = new SizedApparelsDatabase.BodyPartDatabaseKey(pawn.def.defName, bodyTypeString, currentHediffName, customPath==null?bodyPartName: customPath, pawn.gender, Math.Min(currentSeverityInt, cappedSeverityInt), isHorny, poseOverride==null?customPose:poseOverride, variationOverride==null?variation: variationOverride);
            var result = SizedApparelsDatabase.GetSupportedBodyPartPath(key, isBreast, customPath == null ? bodyPartName : customPath, defaultHediffName);




            if (mustMatchSize)
                if (Math.Min(currentSeverityInt, cappedSeverityInt) != result.size)
                {
                    outPoints = null;
                    return null;
                }
            if (mustMatchBodyType)
            {
                if(result.bodyType != pawn.story?.bodyType?.defName)
                {
                    outPoints = null;
                    return null;
                }
            }
            if (mustMatchRace)
            {
                if (result.raceName != pawn.def.defName)
                {
                    outPoints = null;
                    return null;
                }
            }

            if (result.pathWithSizeIndex == null)
            {
                outPoints = null;
                return null;
            }
            outPoints = result.points;
            return GraphicDatabase.Get<Graphic_Multi>(result.pathWithSizeIndex);
        }

        public void UpdateGraphic()
        {
            bodyPartGraphic = GetBodyPartGraphics(false, out points, false);
            bodyPartGraphicHorny = GetBodyPartGraphics(true, out pointsHorny, false);
        }

        public void UpdateGraphic(int index, int indexCapped = 1000)
        {
            this.currentSeverityInt = index;
            this.cappedSeverityInt = indexCapped;

            UpdateGraphic();
        }

        public void ResetTransform()
        {
            this.position = Vector2.zero;
            this.scale = 1f;
            this.rotation = 0;
        }

        public void ClearGraphics()
        {
            this.bodyPartGraphic = null;
            this.bodyPartGraphicHorny = null;
            this.points = null;
            this.pointsHorny = null;
        }
        public void Clear()
        {
            currentHediffName = null;
            currentSeverityInt = -1;
            cappedSeverityInt = 1000;
            customPose = null;
            rotOverride = null;

            ClearGraphics();
        }

        /*
        public void SetHediffData(string name, int severityIndex , string variation = null)
        {
            currentHediffName = name;
            currentSeverityInt = severityIndex;
        }*/

        public void SetHediffData(string name, int severityIndex, int cappedSeverityIndex = 1000, string variation = null)
        {
            currentHediffName = name;
            currentSeverityInt = severityIndex;
            this.cappedSeverityInt = cappedSeverityIndex;
            this.variation = variation;
        }

        public void DrawBodyPart (Vector3 rootLoc, float angle, Rot4 facing, RotDrawMode bodyDrawType, PawnRenderFlags flags, Mesh bodyMesh)
        {
            if (!isVisible)
                return;
            if (scale == 0f)
                return; //Don't draw if scale is zero
            if (pawn == null)
                return;

            if (mustHaveBone && bone == null)
                return;

            if (bodyMesh == null)
            {
                if (SizedApparelSettings.Debug)
                    Log.Warning("[SizedApparel] DrawBodyParts But Null Body Mesh! : " + pawn.Name);
                return;
            }


            PawnRenderer pawnRenderer = pawn.Drawer.renderer;
            Shader shader = shader = ShaderDatabase.CutoutSkinOverlay;
            Color drawColor1 = Color.white;
            Color drawColor2 = Color.white;


            bool forceWriteZ = true;

            bool HasFurSkin = false;
            //Furskin Check
            /*
            if (pawn.Drawer.renderer.graphics.furCoveredGraphic != null)
            {
                HasFurSkin = true;
            }*/

            if (colorType == ColorType.Skin)
            {
                forceWriteZ = true;
                if (bodyDrawType == RotDrawMode.Fresh)
                {
                    if (HasFurSkin)
                    {

                    }
                    else
                    {
                        shader = pawn.Drawer.renderer.graphics.nakedGraphic.Shader;
                        if (!ShaderUtility.SupportsMaskTex(shader))
                            shader = shader = ShaderDatabase.CutoutSkinOverlay;
                        drawColor1 = pawnRenderer.graphics.nakedGraphic.Color;
                        drawColor2 = pawnRenderer.graphics.nakedGraphic.ColorTwo;
                    }

                }
                else if (bodyDrawType == RotDrawMode.Rotting)
                {

                    shader = pawn.Drawer.renderer.graphics.rottingGraphic.Shader;
                    if (!ShaderUtility.SupportsMaskTex(shader))
                        shader = shader = ShaderDatabase.CutoutSkinOverlay;
                    drawColor1 = pawnRenderer.graphics.rottingGraphic.Color;
                    drawColor2 = pawnRenderer.graphics.nakedGraphic.ColorTwo;
                }
            }
            else if (colorType == ColorType.Nipple)
            {
                forceWriteZ = true;

                //Get SkinColor first.
                if (bodyDrawType == RotDrawMode.Fresh)
                {
                    if (HasFurSkin)
                    {

                    }
                    else
                    {
                        shader = pawn.Drawer.renderer.graphics.nakedGraphic.Shader;
                        if (!ShaderUtility.SupportsMaskTex(shader))
                            shader = shader = ShaderDatabase.CutoutSkinOverlay;
                        drawColor1 = pawnRenderer.graphics.nakedGraphic.Color;
                        drawColor2 = pawnRenderer.graphics.nakedGraphic.ColorTwo;
                    }

                }
                else if (bodyDrawType == RotDrawMode.Rotting)
                {

                    shader = pawn.Drawer.renderer.graphics.rottingGraphic.Shader;
                    if (!ShaderUtility.SupportsMaskTex(shader))
                        shader = shader = ShaderDatabase.CutoutSkinOverlay;
                    drawColor1 = pawnRenderer.graphics.rottingGraphic.Color;
                    drawColor2 = pawnRenderer.graphics.nakedGraphic.ColorTwo;
                }

                if(apparelRecorderCompCache != null)
                {
                    if(apparelRecorderCompCache.nippleColor != null)
                    {
                        drawColor1 = apparelRecorderCompCache.nippleColor.Value; //* drawColor1;
                        drawColor2 = apparelRecorderCompCache.nippleColor.Value; //* drawColor2; //maybe can be issue
                    }
                    else
                    {
                        //nipple Color is null
                        //Ust Default Color for Nipple with SkinColor
                        drawColor1 = defaultNippleColor * drawColor1;
                        drawColor2 = defaultNippleColor * drawColor2;

                    }
                }


            }
            else if (colorType == ColorType.Hair)
            {
                forceWriteZ = false;
                shader = ShaderDatabase.Transparent;
                if(pawn.story != null)
                    drawColor1 = pawn.story.hairColor;
            }
            else if (colorType == ColorType.Custom)
            {
                forceWriteZ = true;
                shader = ShaderDatabase.Transparent;
                if(customColorOne != null)
                    drawColor1 = customColorOne.Value;
                if (customColorTwo != null)
                    drawColor2 = customColorTwo.Value;
            }
            else if (colorType == ColorType.None)
            {
                forceWriteZ = false;
                shader = ShaderDatabase.Cutout;
            }
            if (isOverlay)
            {
                if (shader == ShaderDatabase.Cutout)
                    shader = ShaderDatabase.Transparent;
                else if (shader == ShaderDatabase.CutoutSkin || shader == ShaderDatabase.CutoutSkinColorOverride)
                    shader = ShaderDatabase.CutoutSkinOverlay;
                else
                    shader = ShaderDatabase.Transparent;
            }


            Mesh scaledBodyMesh;

            BoneTransform boneTransform = null;
            if(bone != null)
            {
                if (facing == Rot4.South)
                {
                    boneTransform = bone.south;
                }
                else if (facing == Rot4.North)
                {
                    boneTransform = bone.north;
                }
                else if (facing == Rot4.East)
                {
                    boneTransform = bone.east;

                }
                else if (facing == Rot4.West)
                {
                    boneTransform = bone.west;
                    if (boneTransform == null)
                        boneTransform = bone.east;

                }
            }

            float drawScale = scale;
            float drawRotation = angle;
            Vector3 drawPosition = rootLoc;

            /*
            if (this.pawn.ageTracker.CurLifeStage.bodyDrawOffset != null)
            {
                drawPosition += this.pawn.ageTracker.CurLifeStage.bodyDrawOffset.Value;
            }*/

            if (boneTransform != null)
            {
                //TODO fixed angle for IK?

                float width = (bodyMesh.vertices[2].x - bodyMesh.vertices[0].x);
                if (centeredTexture)
                {
                    bool westUsingEast = false;
                    if (facing == Rot4.West && bone.west == null)
                        westUsingEast = true;

                    drawRotation = boneTransform.InitialAngle + boneTransform.angleOffset;
                    //not sure it work correct
                    Vector3 v = (boneTransform.InitialPosition + boneTransform.positionOffset) * width; 
                    var q = Quaternion.AngleAxis (westUsingEast? -angle : angle, Vector3.up);
                    v = q * v;
                    drawPosition = (v) ; // calculate rotated point

                    //Log.Message(boneTransform.angleOffset.ToString());
                    if (westUsingEast)
                    {
                        //already scaled with "width"?
                        //its using east as west. so flip position
                        drawPosition.Scale(new Vector3(-1f,0f,1f));
                        //drawRotation = boneTransform.InitialAngle - boneTransform.angleOffset;
                    }
                    drawPosition += rootLoc;
                    drawRotation += angle ;
                }
                else
                {
                    //NotCentered Texture not yet support Rotation System.
                    //wip
                    Vector3 v = Vector3.zero;

                    bool westUsingEast = false;
                    if (facing == Rot4.West && bone.west == null)
                        westUsingEast = true;
                    drawRotation = boneTransform.InitialAngle; // + boneTransform.angleOffset
                    //not sure it work correct
                    //Vector3 v = (boneTransform.InitialPosition); //initialpos as custom pivot 
                    //var q = Quaternion.AngleAxis(westUsingEast ? -drawRotation : drawRotation, Vector3.up);
                    //var q2 = Quaternion.AngleAxis(westUsingEast ? -angle : angle, Vector3.up);
                    
                    //v = q * v; // calculate final draw position with torso angle
                    //v = v + boneTransform.InitialPosition + boneTransform.positionOffset;
                    //v = q2 * v; //NotCentered Texture not yet support Rotation System.

                    drawPosition = (v) * width;
                    if (westUsingEast)
                    {
                        //already scaled with "width"?
                        //its using east as west. so flip position
                        drawPosition.Scale(new Vector3(-1f, 0f, 1f));
                        //drawRotation = boneTransform.InitialAngle - boneTransform.angleOffset;
                    }
                    drawPosition += rootLoc ; // adjust with result
                    //Log.Message(boneTransform.angleOffset.ToString());
                    drawRotation += angle;
                }
            }

            /*
            if (drawScale != 1f)
            {
                // scale Only Rimworld Plane Mesh
                if(bodyMesh.vertexCount == 4)
                {
                    float width = (bodyMesh.vertices[2].x - bodyMesh.vertices[0].x);

                    //var meshSet = MeshPool.GetMeshSetForWidth(scale * width);
                    var meshSet = MeshPool.GetMeshSetForWidth(scale * width);
                    scaledBodyMesh = meshSet.MeshAt(facing);
                }
                else
                    scaledBodyMesh = bodyMesh;
            }
            else
            {
                scaledBodyMesh = bodyMesh;
            }*/
            scaledBodyMesh = bodyMesh;

            Quaternion quaternion = Quaternion.AngleAxis(drawRotation, Vector3.up);




            Rot4 targetRot = facing;
            if (rotOverride != null)
                targetRot = rotOverride.Value;

            if (targetRot == Rot4.South)
            {
                var loc = OffsetFromUVOffset(positionOffsetSouth, scaledBodyMesh);
                drawPosition.x += loc.x;
                drawPosition.z += loc.y;
                drawPosition.y += depthOffsetSouth;
            }
            else if(targetRot == Rot4.North)
            {
                var loc = OffsetFromUVOffset(positionOffsetNorth, scaledBodyMesh);
                drawPosition.x += loc.x;
                drawPosition.z += loc.y;
                drawPosition.y += depthOffsetNorth;
            }
            else if (targetRot == Rot4.East)
            {
                var loc = OffsetFromUVOffset(positionOffsetEast, scaledBodyMesh);
                drawPosition.x += loc.x;
                drawPosition.z += loc.y;
                drawPosition.y += depthOffsetEast;
            }
            else if (targetRot == Rot4.West)
            {
                var loc = OffsetFromUVOffset(positionOffsetWest, scaledBodyMesh);
                drawPosition.x += loc.x;
                drawPosition.z += loc.y;
                drawPosition.y += depthOffsetWest;
            }




            Graphic graphic = null;
            if (SizedApparelUtility.IsHorny(pawn))
                graphic = bodyPartGraphicHorny;
            if (graphic == null)
                graphic = bodyPartGraphic;

            if (graphic == null)
                return;

            //ForFurskinOffset
            if(bodyDrawType == RotDrawMode.Fresh && HasFurSkin && AutoOffsetForFurCoveredBody)
            {
                //vector.y += 0.009187258f; //in PawnRenderer, it adds 0.009187258f.

                //graphic.maskPath does error? need to check
                // worn fur body and naked fur body has different offsets... wtf
                //TODO Need to Fix
            }




            Material mat;
            
            //If ForceWriteDepth, draw Cutout mesh for write depth. this is for avoid problem of body addons drawing under other meshes. such as breasts front of body but behind tail.
            if (forceWriteZ || (!flags.FlagSet(PawnRenderFlags.Cache) && !isOverlay))// //(!flags.FlagSet(PawnRenderFlags.Cache) && !isOverlay)
            {
                Graphic Zgraphic = graphic.GetColoredVersion(ShaderDatabase.CutoutComplex, drawColor1, drawColor2); // ShaderDatabase.Cutout
                Vector3 drawPositionZ = drawPosition;
                drawPositionZ.y = drawPositionZ.y - 0.00001f; //Send Back of body, but it can still write depth. 1.4: send to 0

                mat = flags.FlagSet(PawnRenderFlags.Cache) ? Zgraphic.MatAt(targetRot) : (Material)overrideMatMethod.Invoke(pawnRenderer, new object[] { Zgraphic.MatAt(facing), pawn, flags.FlagSet(PawnRenderFlags.Portrait) });
                GenDraw.DrawMeshNowOrLater(scaledBodyMesh, drawPositionZ, quaternion, mat, flags.FlagSet(PawnRenderFlags.DrawNow)); // draw for writeZ data to solve shadow issue
            }

            //shader must be mask
            graphic = graphic.GetColoredVersion(shader, drawColor1, drawColor2);
            
            if (graphic.maskPath == null)
            {
                //Test
                //graphic.maskPath = texturePath_Red;
            }

            //drawPosition.y += 0.00001f;
            mat = flags.FlagSet(PawnRenderFlags.Cache) ? graphic.MatAt(targetRot) : (Material)overrideMatMethod.Invoke(pawnRenderer, new object[] { graphic.MatAt(facing), pawn, flags.FlagSet(PawnRenderFlags.Portrait) });
            GenDraw.DrawMeshNowOrLater(scaledBodyMesh, drawPosition, quaternion, mat, flags.FlagSet(PawnRenderFlags.DrawNow));

        }
    }


    //TODO: Torso Pose?


    public class BodyDef : Def
    {
        //public List<SizedApparelBodyPartDef> BodyParts;
        
            
            //defName = raceName ?? could it work?

        public List<BodyWithBodyType> bodies = new List<BodyWithBodyType>();

        
        //public List<BodyTypeAndOffset> penisOffset;
        //public List<BodyTypeAndOffset> vaginaOffset;
        //public List<BodyTypeAndOffset> pubicHairOffset;
        //public List<BodyTypeAndOffset> udderOffset;
        //public List<BodyTypeAndOffset> bellyOffset;
        //public List<BodyTypeAndOffset> breastsOffset;
        //public List<BodyTypeAndOffset> anusOffset;
        
    }

    public class SizedApparelBody
    {
        public string customPoseOfBody = null;

        public bool canCustomPose()
        {
            //check apparels
            return false;
        }
    }

    public class SizedApparelBodyPartOfssetDef : Def
    {
        //defName IsRaceName

    }

}
