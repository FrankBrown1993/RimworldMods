using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace SizedApparel
{
    public enum colorOverrideMode
    {
        Default, Multiply, Add
    }
    public class BodyPartVariationWithRace
    {
        public string hediffName;
        public List<string>varName = new List<string>();

    }

    public class SizedApparelBodyPartVariationDef : Def
    {
        public string bodyPartName;
        public List<BodyPartVariationWithRace>variations = new List<BodyPartVariationWithRace>();
    }

    public class SizedApparelBodyPartDetail : HediffComp
    {

        public string variation = null; // null to default
        public string bodyPartName;
        public SizedApparelBodyPartVariationDef variationDef;
        public Color? colorOverride = null;//TODO
        public colorOverrideMode colorMode = colorOverrideMode.Default;//TODO

        public override string CompTipStringExtra => "Variation: " + (variation == null ? "Default" : variation) + " (sized apparel)";

        public override void CompExposeData()
        {
            Scribe_Values.Look<string>(ref this.variation, "variation", null, false);
            //Scribe_Values.Look<Color?>(ref this.colorOverride, "colorOverride",null, false);
            //Scribe_Values.Look<colorOverrideMode>(ref this.colorMode, "colorMode", colorOverrideMode.Default, false);
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            if (SizedApparelUtility.isBreast(parent.def.defName))
                bodyPartName = "Breasts";
            else if (Genital_Helper.is_vagina(parent)) // SizedApparelUtility.isVagina(parent.def.defName)
                bodyPartName = "Vagina";
            else if (SizedApparelUtility.isAnus(parent.def.defName))
                bodyPartName = "Anus";
            else if (SizedApparelUtility.isUdder(parent.def.defName))
                bodyPartName = "Udder";
            else if (Genital_Helper.is_penis(parent)) //SizedApparelUtility.isPenis(parent.def.defName)
                bodyPartName = "Penis";

            else
                bodyPartName = parent.def.defName;


            if (Pawn == null)
                return;
            if (DefDatabase<SizedApparelBodyPartVariationDef>.DefCount == 0)
            {
                Log.Warning("[Sized Apparel] Cannot Find Any BodyPart Variation Def. It can be version issue or other mod's patch issue.");
                variation = null;
                return;
            }
            try
            {
                variationDef = DefDatabase<SizedApparelBodyPartVariationDef>.AllDefs?.FirstOrDefault(b => b.bodyPartName == bodyPartName);
            }
            catch(ArgumentNullException e)
            {
                Log.Warning("[Sized Apparel] Cannot Find Any BodyPart Variation Def of ( " + bodyPartName + " )!. It can be version issue or other mod's patch issue.");
                variation = null;
                return;
            }

            if (variationDef == null)
                return;
            if (variationDef.variations == null)
                return;
            var variations = variationDef.variations?.FirstOrDefault(v => v.hediffName == parent.def.defName);
            if (variations == null)
                variations = variationDef.variations?.FirstOrDefault(v => v.hediffName == bodyPartName);
            if (variations == null)
                return;
            if (variations.varName.NullOrEmpty())
                return;

            this.variation = variations.varName.RandomElement();

            if (variation.ToLower() == "null" || variation.ToLower() == "default")
            {
                variation = null;
            }

        }
    

    }
    public class SizedApparelBodyPartDetailProperties : HediffCompProperties
    {
        public string bodyPartName;

        public SizedApparelBodyPartDetailProperties()
        {
            this.compClass = typeof(SizedApparelBodyPartDetail);
        }
    }


    public class SizedApparelBodyPartDetailGizmo : Command
    {

    }





    public class SizedApparelBodyPartDetailThing : ThingComp
    {

        public string variation = null; // null to default
        public string bodyPartName;
        public SizedApparelBodyPartVariationDef variationDef;
        public Color? colorOverride = null;//TODO
        public colorOverrideMode colorMode = colorOverrideMode.Default;//TODO


        public override string GetDescriptionPart()
        {
            if(SizedApparelSettings.showBodyPartsVariation)
                return "Variation: " +( variation == null?"Default":variation) + " (sized apparel)";

            return string.Empty;
        }

        /*
        public override string TransformLabel(string label)
        {
            return label + "Variation: " + variation == null ? "Default" : variation + " (sized apparel)";
        }
        */

        public override void PostExposeData()
        {
            Scribe_Values.Look<string>(ref this.variation, "variation", null, false);
            //Scribe_Values.Look<Color?>(ref this.colorOverride, "colorOverride",null, false);
            //Scribe_Values.Look<colorOverrideMode>(ref this.colorMode, "colorMode", colorOverrideMode.Default, false);
        }

        public void InitComp(Pawn pawn = null)
        {
            HediffDef named = DefDatabase<HediffDef>.GetNamed(this.parent.def.defName, true);
            List<Pawn> allMaps_FreeColonistsAndPrisonersSpawned = PawnsFinder.AllMaps_FreeColonistsAndPrisonersSpawned;
            pawn = ((allMaps_FreeColonistsAndPrisonersSpawned != null) ? allMaps_FreeColonistsAndPrisonersSpawned.RandomElement<Pawn>() : null);
            if (pawn == null)
            {
                List<Pawn> all_AliveOrDead = PawnsFinder.All_AliveOrDead;
                pawn = ((all_AliveOrDead != null) ? all_AliveOrDead.RandomElement<Pawn>() : null);
            }
            SizedApparelBodyPartDetail compHediffBodyPart = HediffMaker.MakeHediff(named, pawn, null).TryGetComp<SizedApparelBodyPartDetail>();
            if (compHediffBodyPart != null)
            {
                //compHediffBodyPart.initComp(pawn, false);
                //compHediffBodyPart.updatesize(0f);
                this.variation = compHediffBodyPart.variation;
            }
        }


        public override void PostPostMake()
        {
            base.PostPostMake();

            if (SizedApparelUtility.isBreast(parent.def.defName))
                bodyPartName = "Breasts";
            else if (SizedApparelUtility.isVagina(parent.def.defName))
                bodyPartName = "Vagina";
            else if (SizedApparelUtility.isAnus(parent.def.defName))
                bodyPartName = "Anus";
            else if (SizedApparelUtility.isUdder(parent.def.defName))
                bodyPartName = "Udder";
            else if (SizedApparelUtility.isPenis(parent.def.defName))
                bodyPartName = "Penis";

            else
                bodyPartName = parent.def.defName;

            variationDef = DefDatabase<SizedApparelBodyPartVariationDef>.AllDefs?.FirstOrDefault(b => b.bodyPartName == bodyPartName);
            if (variationDef == null)
                return;
            if (variationDef.variations == null)
                return;
            var variations = variationDef.variations?.FirstOrDefault(v => v.hediffName == parent.def.defName);
            if (variations == null)
                variations = variationDef.variations?.FirstOrDefault(v => v.hediffName == bodyPartName);
            if (variations == null)
                return;
            if (variations.varName.NullOrEmpty())
                return;

            this.variation = variations.varName.RandomElement();

            if (variation.ToLower() == "null" || variation.ToLower() == "default")
            {
                variation = null;
            }

        }


    }
    public class SizedApparelBodyPartDetailThingProperties : CompProperties
    {
        public string bodyPartName;

        public SizedApparelBodyPartDetailThingProperties()
        {
            this.compClass = typeof(SizedApparelBodyPartDetailThing);
        }
    }



    [HarmonyPatch(typeof(SexPartAdder), "recipePartAdder")]
    public class recipePartAdderPatch
    {
        public static void Postfix(RecipeDef recipe, Pawn pawn, BodyPartRecord part, List<Thing> ingredients, Hediff __result)
        {
            if (__result == null)
                return;
            Thing thing = ingredients.FirstOrDefault(x => x.def.defName == recipe.addsHediff.defName);
            if (thing == null)
                return;
            SizedApparelBodyPartDetailThing CompThing = thing.TryGetComp<SizedApparelBodyPartDetailThing>();
            SizedApparelBodyPartDetail CompHediff = __result.TryGetComp<SizedApparelBodyPartDetail>();
            CompHediff.variation = CompThing.variation;
        }

    }
    [HarmonyPatch(typeof(SexPartAdder), "recipePartRemover")]
    public class recipePartRemoverPatch
    {
        public static void Postfix(Hediff hd, ref Thing __result)
        {
            //Thanks! "Stardust" helped
            try
            {
                SizedApparelBodyPartDetailThing CompThing = __result.TryGetComp<SizedApparelBodyPartDetailThing>();
                SizedApparelBodyPartDetail CompHediff = hd.TryGetComp<SizedApparelBodyPartDetail>();

                CompThing.variation = CompHediff.variation;
            }
            catch (NullReferenceException e)
            {
                Log.Error(e.StackTrace);
            }
        }
    }

}
