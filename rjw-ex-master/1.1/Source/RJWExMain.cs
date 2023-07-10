using Verse;
using HarmonyLib;
using System.Reflection;
using System;

namespace rjwex
{
    [StaticConstructorOnStartup]
    public static class RJWExMain
    {
        static RJWExMain()
        {

            Logger.ListBegin("[RJWEx] Patching...");
            var harmony = new Harmony("rimworld.ekss.rjwex");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
			Logger.ListPrint("...done");
        }
	}

	public static class Logger
    {

        /*
         public static class Logger
	    {
		    private static readonly LogMessageQueue messageQueueRJW = new LogMessageQueue();
		    public static void Message(string text)
		    {
			    bool DevModeEnabled = RJWSettings.DevMode;
			    if (!DevModeEnabled) return;
			    Debug.Log(text);
			    messageQueueRJW.Enqueue(new LogMessage(LogMessageType.Message, text, StackTraceUtility.ExtractStackTrace()));
		    }
	    }*/
        private static string prefix = "[RJWEx] ";
        private static string listBuffer = "";

        public static void ListBegin(string line = "")
        {
            listBuffer = (line.NullOrEmpty() ? "" : prefix) + line;
        }

        public static void ListLine(string line)
        {
            listBuffer += (Environment.NewLine + prefix + line);
        }
        public static void ListPrint(string line = "")
        {
            if (!line.NullOrEmpty())
            {
                listBuffer += (Environment.NewLine + prefix + line);
            }
            Log.Message(listBuffer);
            listBuffer = "";
        }
        public static void Print(string line)
        {
            Log.Message(prefix + line);
        }
    }
}