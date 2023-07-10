using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SizedApparel
{

    //This Class is Use for Def BodyParts' Custom Pivot, or Some Special Common Vector Points Per Serverity.
    //Not Consider Acture Graphic Textures. 
    public class SkeletonDef : Def
    {
        //defName = Pawn's race name (such as "Human")
        //public List<BodyPartGraphicBone> graphicBones = new List<BodyPartGraphicBone>();

        //Do Not Directly Use it
        public List<Skeleton> skeletons = new List<Skeleton>();


        public Skeleton CreateSkeleton(string bodyType)
        {
            for(int i = 0; i< skeletons.Count; i++)
            {
                if(skeletons[i].bodyType == bodyType)
                {
                    return new Skeleton(skeletons[i]);
                }
            }
            return null;
        }
    }

    public class Skeleton
    {
        public string bodyType = null; // can be null for animal or etc
        public List<Bone> Bones;

        //only runtime. Do not Set in Def
        public Rot4 BodyFacing; // for flip east-west bones
        public Rot4 HeadFacing; //TODO

        public Skeleton()
        {

        }
        public Skeleton(Skeleton skeletonToCopy)
        {
            this.Bones = new List<Bone>();


            foreach (var s in skeletonToCopy.Bones)
            {
                this.Bones.Add(new Bone(s, this));
            }

        }
        public Bone FindBone(string boneName)
        {
            foreach (var b in this.Bones)
            {
                if (b.name == boneName)
                    return b;
            }
            return null;
        }

    }

    //for deafault bone support?
    public class BodyPartBoneDef : Def
    {
        //DefName is Bone Name To Use.
        public Vector3 customPivot = new Vector3(0.5f, 0, 0.5f);
        public Vector3 Position = new Vector3(0.5f, 0, 0.5f); // Local Position(UV) from Body
        public float Length = 1;
        public float Rotation = 0;
        public float Scale = 1f; // Default Render Scale.
    }

    //Body Parts Graphic can be attached to bone position and rotation
    public class Bone
    {

        public string name = null;
        public Skeleton parentSkeleton = null;
        //public string parentBoneName = null; // ToDo
        public bool isHeadBone = false; // TODO
        public BoneTransform south;
        public BoneTransform north;
        public BoneTransform east;
        public BoneTransform west; // can be null. then use east
        //hiding Graphic parameter will be in that bodygraphic class

        public Bone()
        {
            
        }

        public Bone(Bone boneToCopy, Skeleton parent)
        {
            this.name = boneToCopy.name;
            this.parentSkeleton = parent;
            this.isHeadBone = boneToCopy.isHeadBone;
            if(boneToCopy.south != null)
                this.south = new BoneTransform(boneToCopy.south);
            if (boneToCopy.north != null)
                this.north = new BoneTransform(boneToCopy.north);
            if (boneToCopy.east != null)
                this.east = new BoneTransform(boneToCopy.east);
            if (boneToCopy.west != null)
                this.west = new BoneTransform(boneToCopy.west); // null for use east

        }

        public void SetAngle(float angle)
        {
            if(south != null)
                south.angleOffset = angle;
            if (north != null)
                north.angleOffset = angle;
            if (east != null)
                east.angleOffset = angle;
            if (west != null)
                west.angleOffset = angle;
        }


    }

    public class BoneTransform
    {
        //public Vector3 customPivot = new Vector3(0.5f, 0, 0.5f); // used to calculation rotation. the rotating center will be customPivot.
        //Custom Pivot Doesn't affect to Draw Position on zero Rotated.
        public Vector3 InitialPosition = Vector3.zero; // Local Position(UV) from Body
        public float InitialLength = 1;
        public float InitialAngle = 0;
        public float InitialScale = 1f;
        public bool isHeadBone = false; // TODO

        //public BodyPartGraphicBone parentBone; //TODO
        public Vector3 positionOffset = Vector3.zero;
        public float lengthOffset = 0;
        public float angleOffset = 0;
        public float scaleOffset = 0;

        public BoneTransform() { }

        public BoneTransform(BoneTransform boneToCopy)
        {
            
            //this.parentBoneName = boneToCopy.parentBoneName;
            //this.customPivot = boneToCopy.customPivot;
            this.InitialPosition = boneToCopy.InitialPosition;
            this.InitialLength = boneToCopy.InitialLength;
            this.InitialAngle = boneToCopy.InitialAngle;
            this.InitialScale = boneToCopy.InitialScale;
        }
    }

}
