using Verse;

namespace LewdBiotech.Helpers
{
    public static class LBTLogger
    {
        public static void Message(string message)
        {
            Log.Message("[INFO][LewdBT] - " + message);
        }

        public static void Warning(string message)
        {
            Log.Message("[WARN][LewdBT] - " + message);
        }

        public static void Error(string message)
        {
            Log.Message("[ ERR][LewdBT] - " + message);
        }

        public static void MessageGroupHead(string message)
        {
            Log.Message("[INFO][LewdBT]╦═ " + message);
        }
        public static void MessageGroupBody(string message)
        {
            Log.Message("[INFO][LewdBT]╠═══ " + message);
        }
        public static void MessageGroupFoot(string message)
        {
            Log.Message("[INFO][LewdBT]╚═══ " + message);
        }
    }
}
