using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace SizedApparel
{
    //Does it better to optimize?
    public class Graphic_SizedApparel : Graphic_Multi
    {
        public string TargetBodyParts = "Breats";
        //Sized Graphics.
        public List<Graphic_Multi> graphics;
    }
}
