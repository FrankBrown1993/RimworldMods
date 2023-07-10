using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using rjw;

namespace SizedApparel
{

    //sexualize_pawn patch
    class SexualizePawnPatch
    {
        static void Postfix(Pawn pawn)
        {
            if (!UnityData.IsInMainThread)
                return;
            ApparelRecorderComp comp = pawn?.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.SetDirty(true,true,true);
            /*
            comp.ClearAll();
            var graphicSet =  pawn.Drawer?.renderer?.graphics;
            if (graphicSet != null)
            {
                pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                //pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                BodyPatch.SetBodyGraphic(pawn);
            }*/

            /*
            float breastSeverity = comp.breastSeverity;
            Hediff breastHediff = comp.breastHediff;
            SizedApparelUtility.GetBreastSeverity(pawn, out breastSeverity, out breastHediff);
            SizedApparelUtility.hasUnSupportedApparelFromWornData(pawn, breastSeverity, breastHediff, true);
            comp.hasUpdateBefore = true;*/

        }
    }
 //since Sized Apparel Logic Changed, it doesn't need to do job patch for rjw.
    [Obsolete]
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(SexUtility), "DrawNude")]
    class DrawNudePatch
    {
        static void Prefix(Pawn pawn, bool keep_hat_on)
        {

            if (!UnityData.IsInMainThread)
                return;

            //Log.Message("Hello");
            if (RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Clothed)
            {

            }
            else
            {
                //SetBodyGraphic(pawn.Drawer.renderer.graphics, false);
            }
        }
        static void OldPostfix(Pawn pawn)
        {
            if (pawn == null)
                return;
            //GetBreastSeverity(__instance.pawn, out breastSeverity, out breastHediff);
            //bool flag = hasUnSupportedApparel(__instance.pawn, breastSeverity, breastHediff);
            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp != null)
            {
                /*
                LongEventHandler.ExecuteWhenFinished(delegate
                    {
                        float breastSeverity;
                        Hediff breastHediff;
                        bool flag = false;
                        GetBreastSeverity(pawn, out breastSeverity, out breastHediff);
                        flag = hasUnSupportedApparel(pawn, breastSeverity, breastHediff);
                        if (!comp.hasUpdateBefore)
                            comp.SetHasUpdateBefore(true);
                        comp.SetHasUnsupportedApparel(flag);
                    }
                );
                */

                //Log.Message("CheckApparel");
                if (!comp.hasUpdateBefore)
                    comp.hasUpdateBefore = true;
                if (!comp.hasUpdateForSex)
                {
                    comp.hasUpdateForSex = true;
                    float breastSeverity;
                    Hediff breastHediff;
                    bool flag = false;
                    SizedApparelUtility.GetBreastSeverity(pawn, out breastSeverity, out breastHediff);
                    //Log.Message("GetServerity");
                    //flag = SizedApparelUtility.hasUnSupportedApparel(pawn, breastSeverity, breastHediff);
                    flag = SizedApparelUtility.hasUnSupportedApparelFromWornData(pawn, breastSeverity, breastHediff);
                    comp.hasUnsupportedApparel = flag;
                }

                //Log.Message("SetFlag");
                /*
                //Log.Message(comp.testbool.ToString());
                //Log.Message("ApparelChanged");
                //comp.hasUnsupportedApparel = flag;
                //comp.hasUpdateBefore = true;
                //comp.SetHasUnsupportedApparel(flag);
                //comp.SetHasUpdateBefore(true);
                //if (__instance.pawn.Drawer.renderer.graphics != null)


                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    SetBodyGraphic(__instance.pawn.Drawer.renderer.graphics, flag);
                }
                );
                */

            }

        }
        public void todoPostfix(Pawn pawn)
        {
            if (pawn == null)
                return;
            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.SetDirty();
        }
    }

    [Obsolete]
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "Start")]
    class SexStartPatch
    {
        static void Postfix(JobDriver_SexBaseInitiator __instance)
        {
            //Log.Warning("SexStart");



            ApparelRecorderComp pawnARC = __instance.pawn.GetComp<ApparelRecorderComp>();
            if (pawnARC != null)
            {
                if (__instance.pawn.RaceProps.Humanlike)
                {
                    pawnARC.havingSex = true;
                    pawnARC.hasUpdateForSex = false;
                    __instance.pawn.Drawer.renderer.graphics.SetApparelGraphicsDirty();
                }
            }
            if (__instance.Partner == null)
                return;
            var partner = __instance.Partner as Pawn;
            if (partner == null)
                return;
            ApparelRecorderComp partnerARC = partner.GetComp<ApparelRecorderComp>();
            if (partnerARC != null)
            {
                if (partner.RaceProps.Humanlike)
                {
                    partnerARC.havingSex = true;
                    pawnARC.hasUpdateForSex = false;
                    partner.Drawer.renderer.graphics.SetApparelGraphicsDirty();
                }
            }
        }
    }
    [Obsolete]
    [StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "End")]
    class SexEndPatch
    {
        static void Postfix(JobDriver_SexBaseInitiator __instance)
        {


            ApparelRecorderComp pawnARC = __instance.pawn.GetComp<ApparelRecorderComp>();

            if (pawnARC != null)
            {
                pawnARC.havingSex = false;
                pawnARC.hasUpdateForSex = false;
                pawnARC.hasUpdateBefore = false;
                float breastSeverity;
                Hediff breastHediff;
                bool flag = true;
                if (__instance.pawn.RaceProps.Humanlike)
                {
                    SizedApparelUtility.GetBreastSeverity(__instance.pawn, out breastSeverity, out breastHediff);
                    //Log.Message("GetServerity");
                    flag = SizedApparelUtility.hasUnSupportedApparelFromWornData(__instance.pawn, breastSeverity, breastHediff);
                    pawnARC.hasUnsupportedApparel = flag;
                    PortraitsCache.SetDirty(__instance.pawn);
                    __instance.pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                }
                pawnARC.hasUnsupportedApparel = flag;
            }

            if (__instance.Partner == null)
                return;
            var partner = __instance.Partner as Pawn;
            if (partner == null)
                return;
            ApparelRecorderComp partnerARC = partner.GetComp<ApparelRecorderComp>();
            if (partnerARC != null)
            {
                partnerARC.havingSex = false;
                partnerARC.hasUpdateForSex = false;
                partnerARC.hasUpdateBefore = false;
                float breastSeverity;
                Hediff breastHediff;
                bool flag = true;

                if (partner.RaceProps.Humanlike)
                {
                    SizedApparelUtility.GetBreastSeverity(partner, out breastSeverity, out breastHediff);
                    //Log.Message("GetServerity");
                    flag = SizedApparelUtility.hasUnSupportedApparelFromWornData(partner, breastSeverity, breastHediff);
                    partnerARC.hasUnsupportedApparel = flag;
                    partner.Drawer.renderer.graphics.ResolveApparelGraphics();
                    PortraitsCache.SetDirty(partner);
                }
                partnerARC.hasUnsupportedApparel = flag;

            }


        }

    }

}
