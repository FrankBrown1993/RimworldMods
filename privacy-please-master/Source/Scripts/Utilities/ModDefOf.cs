using Verse;
using RimWorld;
using AlienRace;

namespace Privacy_Please
{
    [DefOf]
    public static class ModBodyPartGroupDefOf
    {
        public static BodyPartGroupDef GenitalsBPG;
        public static BodyPartGroupDef AnusBPG;
        public static BodyPartGroupDef ChestBPG;
    }

    [DefOf]
    public static class ModPreceptDefOf
    {
        public static PreceptDef Exhibitionism_Acceptable;
        public static PreceptDef Exhibitionism_Approved;
    }

    [DefOf]
    public static class ModFleckDefOf
    {
        public static FleckDef EyeHeart;
    }

    [DefOf]
    public static class ModJobDefOf
    {
        public static JobDef WatchSex;
    }

    [DefOf]
    public static class ModInteractionDefOf
    {
        public static InteractionDef InviteToHaveSex;
        public static InteractionDef InviteToHaveGroupSex;
        public static InteractionDef InviteVoyeurism;
        public static InteractionDef InterruptedSex;
    }

    [DefOf]
    public static class ModSexActReactionDefOf
    {
        public static SexActReactionDef reactionToExhibitionism;
    }
}
