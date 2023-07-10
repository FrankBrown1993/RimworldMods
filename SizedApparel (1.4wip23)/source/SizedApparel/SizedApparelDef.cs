using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
//using AlienRace;
using UnityEngine;
using rjw;

namespace SizedApparel
{

    //for avoid first hitch. some day it might need make pre def for fast cache...

    public class ApparelTexture
    {
        public BodyTypeDef bodytype;
        public List<string> textures;

        public string GetTexturePath(int level, out int result)
        {
            result = -1;
            if (level < 0)
                return null;
            if (textures.Count > level)
            {
                result = level;
                return textures[level];
            }
            if (!textures.NullOrEmpty())
            {
                result = textures.Count - 1;
                return textures[result]; //return biggest as supported
            }
            return null;
        }
    }

    public class SizedApparelDef : Def
    {
        //def name must be match with apparels DefName
        public bool hidingGenitals = false; // pants must be true
        public List<ApparelTexture> SizedTextures;

        public ApparelTexture GetApparelTexture(BodyTypeDef bodytype)
        {
            if(SizedTextures != null)
            {
                return SizedTextures.FirstOrDefault(a => a.bodytype == bodytype);
            }
            return null;
        }
    }

}
