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
    [HarmonyPatch(typeof(PawnGraphicSet), "CalculateHairMats")]
    public static class CalculateHairMatsPatch
    {
        public static void Postfix()
        {

        }
    }

    public class PubicHairDef : StyleItemDef
    {

    }

    class SizedApparelPubicHair
    {
    }


    public static class IdeoStyleTackerPatch
    {

    }
}
