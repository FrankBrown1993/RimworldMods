using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SizedApparel
{
    //Consider Graphic and Graphic_Multi(Graphic with Facing such as south)
    public class SizedApparelTexturePointDef : Def
    {
        //Path must be texture file name with path
        //Path example: "Things/Pawn/Humanlike/Bodies/Naked_Female_BaseBody"
        //Facing Text such as "_south" must not be included.
        //Use "/" instead of "\"

        public string Path;

        public List<BodyPartPoint> SouthBodyPartPoints = new List<BodyPartPoint>();
        public List<BodyPartPoint> NorthBodyPartPoints = new List<BodyPartPoint>();
        public List<BodyPartPoint> EastBodyPartPoints = new List<BodyPartPoint>();

        //can be null. then use EastBodyPartPoints
        public List<BodyPartPoint> WestBodyPartPoints = new List<BodyPartPoint>();



    }
}
