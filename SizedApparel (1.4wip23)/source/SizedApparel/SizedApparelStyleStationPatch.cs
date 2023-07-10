using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;
using UnityEngine;
using rjw;
using System.Reflection;
using System.Reflection.Emit;

namespace SizedApparel
{

    [HarmonyPatch(typeof(Dialog_StylingStation), "DrawTabs")]
    public class SizedApparelStyleStationDrawTabsPatch
    {
        public static void Postfix(Rect rect, ref Vector2 ___hairScrollPosition, Dialog_StylingStation __instance, List<TabRecord> ___tabs, Dialog_StylingStation.StylingTab ___curTab, float ___viewRectHeight, ref  List<StyleItemDef> ___tmpStyleItems, bool ___devEditMode, Pawn ___pawn, float ___colorsHeight, Color ___desiredHairColor)
        {

            if (!SizedApparelSettings.drawPubicHair)
                return;


            //Widgets.DrawMenuSection(rect);
            //TabDrawer.DrawTabs<TabRecord>(rect, ___tabs, 200f);
            //rect = rect.ContractedBy(18f);
            switch (___curTab)
            {
                case (Dialog_StylingStation.StylingTab)24:

                    //Draw PubicHair Tab Code here!
                    //rect.yMax -= ___colorsHeight;
                    DrawStylingTypePubicHair(ref __instance, ref ___viewRectHeight, ref ___tmpStyleItems, ___devEditMode, ___pawn, ___colorsHeight, ___desiredHairColor, rect, ref ___hairScrollPosition, delegate (Rect r, PubicHairDef h)
                        {
                            GUI.color = ___desiredHairColor;
                            Widgets.DefIcon(r, h, null, 1.25f, null, false, null, null, null);
                            //Widgets.DrawTextureFitted(r, h.Icon, 1.25f, null);
                            GUI.color = Color.white;
                        }, delegate (PubicHairDef h)
                        {
                            ___pawn.GetComp<ApparelRecorderComp>().pubicHairDef = h;
                            //___pawn.story.hairDef = h;
                        }, (StyleItemDef h) => ___pawn.GetComp<ApparelRecorderComp>().pubicHairDef == h, (StyleItemDef h) => ___pawn.GetComp<ApparelRecorderComp>().initialPubicHairDef == h, null, false);

                    return;
                default:
                    return;
            }
        }

        //Some Copy Code from Rimnudeworld
        //maybe Some Var has to be ref
        static void DrawStylingTypePubicHair(ref Dialog_StylingStation dialog_StylingStation, ref float viewRectHeight, ref List<StyleItemDef> tmpStyleItems, bool devEditMode, Pawn pawn, float colorsHeight, Color desiredHairColor, Rect rect, ref Vector2 scrollPosition, Action<Rect, PubicHairDef> drawAction, Action<PubicHairDef> selectAction, Func<StyleItemDef, bool> hasStyleItem, Func<StyleItemDef, bool> hadStyleItem, Func<StyleItemDef, bool> extraValidator = null, bool doColors = false)
        {

            //Color desiredHairColor = AccessTools.FieldRefAccess<Dialog_StylingStation, Color>(dialog_StylingStation, "desiredHairColor");

            int total_pubes_count = 0;
            total_pubes_count = DefDatabase<PubicHairDef>.AllDefs.Count();

            //need child patch?
            if (total_pubes_count <= 0)
            {
                Widgets.NoneLabelCenteredVertically(rect, "(" + "NoneUsableForPawn".Translate(pawn.Named("PAWN")) + ")");
                return;
            }

            ApparelRecorderComp comp = pawn.GetComp<ApparelRecorderComp>();



            Rect viewRect = new Rect(rect.x, rect.y, rect.width - 16f, viewRectHeight);
            int num = Mathf.FloorToInt(viewRect.width / 60f) - 1;
            float num2 = (viewRect.width - (float)num * 60f - (float)(num - 1) * 10f) / 2f;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            tmpStyleItems.Clear();
            /*
            tmpStyleItems.AddRange(from x in DefDatabase<PubicHairDef>.AllDefs
                                                         where (devEditMode || PawnStyleItemChooser.WantsToUseStyle(pawn, x, null) || hadStyleItem(x)) && (extraValidator == null || extraValidator(x))
                                                         select x);*/
            tmpStyleItems.AddRange(DefDatabase<PubicHairDef>.AllDefs);// just add All Pubic Hair def.

            tmpStyleItems.SortBy((StyleItemDef x) => -PawnStyleItemChooser.StyleItemChoiceLikelihoodFor(x, pawn));
            if (tmpStyleItems.NullOrEmpty<StyleItemDef>())
            {
                Widgets.NoneLabelCenteredVertically(rect, "(" + "NoneUsableForPawn".Translate(pawn.Named("PAWN")) + ")");
            }
            else
            {
                Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
                foreach (StyleItemDef styleItemDef in tmpStyleItems)
                {
                    if (num5 >= num - 1)
                    {
                        num5 = 0;
                        num4++;
                    }
                    else if (num3 > 0)
                    {
                        num5++;
                    }
                    Rect rect2 = new Rect(rect.x + num2 + (float)num5 * 60f + (float)num5 * 10f, rect.y + (float)num4 * 60f + (float)num4 * 10f, 60f, 60f);
                    Widgets.DrawHighlight(rect2);
                    if (Mouse.IsOver(rect2))
                    {
                        Widgets.DrawHighlight(rect2);
                        TooltipHandler.TipRegion(rect2, styleItemDef.LabelCap);
                    }
                    if (drawAction != null)
                    {
                        drawAction(rect2, styleItemDef as PubicHairDef);
                    }
                    if (hasStyleItem(styleItemDef))
                    {
                        Widgets.DrawBox(rect2, 2, null);
                    }
                    if (Widgets.ButtonInvisible(rect2, true))
                    {
                        if (selectAction != null)
                        {
                            selectAction(styleItemDef as PubicHairDef);
                        }
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
                        PortraitsCache.SetDirty(pawn);
                    }
                    num3++;
                }
                if (Event.current.type == EventType.Layout)
                {
                    viewRectHeight = (float)(num4 + 1) * 60f + (float)num4 * 10f + 10f;
                }
                Widgets.EndScrollView();
            }
            if (doColors)
            {
                //dialog_StylingStation.DrawHairColors(new Rect(rect.x, rect.yMax + 10f, rect.width, dialog_StylingStation.colorsHeight));

                //Copy From RimnudeWorld
                Rect newrect = new Rect(rect.x, rect.yMax - 10f, rect.width, colorsHeight);

                Color _desiredHairColor = desiredHairColor;

                float numC = newrect.y;
                float height;
                Widgets.ColorSelector(new Rect(newrect.x, numC, newrect.width, 92f), ref _desiredHairColor, AllHairColors, out height, null, 22, 2);
                if (_desiredHairColor != desiredHairColor)
                {
                    var desiredHairColor_ = dialog_StylingStation.GetType().GetField("desiredHairColor", System.Reflection.BindingFlags.NonPublic
                         | System.Reflection.BindingFlags.Instance);
                    desiredHairColor_.SetValue(dialog_StylingStation, _desiredHairColor);
                }
                numC += 60f;
                if (desiredHairColor != pawn.story.HairColor && desiredHairColor != pawn.style.nextHairColor)
                {
                    Widgets.ThingIcon(new Rect(newrect.x, numC, Text.LineHeight, Text.LineHeight), ThingDefOf.Dye, null, null, 1.1f, null);
                    string text = "Required".Translate() + ": 1  " + ThingDefOf.Dye.label;
                    float x = Text.CalcSize(text).x;
                    Widgets.Label(new Rect(newrect.x + Text.LineHeight + 4f, numC, x, Text.LineHeight), text);
                    Rect rect2 = new Rect(newrect.x, numC, x + Text.LineHeight + 8f, Text.LineHeight);
                    if (Mouse.IsOver(rect2))
                    {
                        Widgets.DrawHighlight(rect2);
                        TooltipHandler.TipRegionByKey(rect2, "TooltipDyeExplanation");
                    }
                    numC += Text.LineHeight;

                    if (pawn.Map.resourceCounter.GetCount(ThingDefOf.Dye) < 1)
                    {
                        rect2 = new Rect(newrect.x, numC, newrect.width, Text.LineHeight);
                        Color color = GUI.color;
                        GUI.color = ColorLibrary.RedReadable;
                        Widgets.Label(rect2, "NotEnoughDye".Translate() + " " + "NotEnoughDyeWillRecolorHair".Translate());
                        GUI.color = color;
                        numC += rect2.height;
                    }
                }
            }

        }
        private static List<Color> allHairColors;
        private static List<Color> AllHairColors
        {
            get
            {
                if (allHairColors == null)
                {
                    allHairColors = (from ic in DefDatabase<ColorDef>.AllDefsListForReading
                                     select ic.color).ToList<Color>();
                    allHairColors.SortByColor((Color x) => x);
                }
                return allHairColors;
            }
        }


        static void AddPubicHairTab(Dialog_StylingStation stylingStation, List<TabRecord> tabs)
        {
            if (!SizedApparelSettings.drawPubicHair)
                return;

            var curTabField = AccessTools.Field(typeof(Dialog_StylingStation), "curTab");
            tabs.Add(new TabRecord("PubicHair".Translate().CapitalizeFirst(), delegate ()
            {

                curTabField.SetValue(stylingStation, (Dialog_StylingStation.StylingTab)24);
            }, (Dialog_StylingStation.StylingTab)curTabField.GetValue(stylingStation) == (Dialog_StylingStation.StylingTab)24));
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool isHair = false;
            MethodInfo tabAdd = AccessTools.DeclaredMethod(typeof(List<TabRecord>),"Add");
            foreach (var instruction in instructions)
            {
                if(instruction.opcode == OpCodes.Ldstr)
                {
                    if (instruction.OperandIs("Hair"))
                        isHair = true;
                    else
                        isHair = false;
                }


                if (isHair && instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(tabAdd))
                {
                    yield return instruction;//finish add hairTab

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    //Log.Message("this");
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(Dialog_StylingStation),"tabs"));
                    //Log.Message("tabs");
                    //yield return new CodeInstruction(OpCodes.Ldarg_0);
                    //Log.Message("this");
                    //yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(Dialog_StylingStation), "curTab"));
                    //Log.Message("curtab");
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SizedApparelStyleStationDrawTabsPatch), "AddPubicHairTab"));
                    //Log.Message("call");
                    //yield return new CodeInstruction(OpCodes.Ldarg_0);

                    //yield return new CodeInstruction(OpCodes.Ldarg_0);
                    //yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_StylingStation), "tabs"));
                    //yield return new CodeInstruction(OpCodes.Ldstr,"PubicHair".Translate().CapitalizeFirst());

                    //yield return new CodeInstruction(OpCodes.Ldarg_0);
                    //yield return new CodeInstruction(OpCodes.Ldarg_0);




                    isHair = false;
                }
                else
                    yield return instruction;

            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(Dialog_StylingStation), "Reset")]
    public class SizedApparelStyleStationResetPatch
    {
        public static void Prefix(Pawn ___pawn)
        {
            ApparelRecorderComp comp = ___pawn.GetComp<ApparelRecorderComp>();

            //this.pawn.story.hairDef = this.initialHairDef;
            comp.pubicHairDef = comp.initialPubicHairDef;
            comp.initialPubicHairDef = null;
        }
    }

    //Patching Constructors
    [HarmonyPatch(typeof(Dialog_StylingStation), MethodType.Constructor, new Type[] { typeof(Pawn), typeof(Thing) })]
    public class SizedApparelDialogStylingStationPatch
    {
        public static void Postfix(Pawn pawn)
        {
            var comp = pawn.GetComp<ApparelRecorderComp>();
            if (comp == null)
                return;
            comp.initialPubicHairDef = comp.pubicHairDef;
        }
    }

}

