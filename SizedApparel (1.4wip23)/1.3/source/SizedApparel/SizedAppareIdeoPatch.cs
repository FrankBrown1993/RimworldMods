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
    //IdeoUIUtility.DoAppearanceItems
    [HarmonyPatch(typeof(IdeoUIUtility), "DoAppearanceItems")]
    static class SizedApparAppearanceUIPatch
    {
        //Todo: Use Transpiler
        static public void oldPostfix(Ideo ideo, IdeoEditMode editMode, ref float curY, float width)
        {
            //DrawPubicBlox()
            Rect rect = new Rect(4f, curY, IdeoUIUtility.PreceptBoxSize.x, IdeoUIUtility.PreceptBoxSize.y);
            Widgets.DrawRectFast(rect, IdeoUIUtility.GetBackgroundColor(PreceptImpact.Medium), null);
            string text = "PubicHair".Translate();
        }

        static void DrawPubicBox(float xOffset, float y, StyleItemTab tab, StyleItemDef defToDisplay)
		{

        }



    }

}
