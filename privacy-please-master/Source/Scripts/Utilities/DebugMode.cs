using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Privacy_Please
{
    public static class DebugMode
    {
        public static void Message(string text)
        {
            if (BasicSettings.debugMode)
            { Log.Message("[Privacy, Please!] " + text); }
        }
    }
}
