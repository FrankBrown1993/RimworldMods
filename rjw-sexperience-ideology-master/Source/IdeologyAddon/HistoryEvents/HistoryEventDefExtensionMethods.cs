using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace RJWSexperience.Ideology.HistoryEvents
{
	public static class HistoryEventDefExtensionMethods
	{
		public static void RecordEventWithPartner(this HistoryEventDef def, Pawn pawn, Pawn partner)
		{
			//Log.Message($"[RSI] Recording event {def.ToStringWithPartner(pawn, partner)}");
			List<TwoPawnEventRule> secondaryEventRules = def.GetModExtension<DefExtension_SecondaryEvents>()?.generationRules;

			if (!secondaryEventRules.NullOrEmpty())
			{
				//Log.Message($"[RSI] Event has {secondaryEventRules?.Count} secondary events");
				foreach (var rule in secondaryEventRules.Where(rule => rule.Applies(pawn, partner)))
				{
					//Log.Message($"[RSI] Recording secondary event {def.defName}");
					rule.historyEventDef.RecordEventWithPartner(pawn, partner);
				}
			}

			HistoryEvent historyEvent = def.CreateEventWithPartner(pawn, partner);
			Find.HistoryEventsManager.RecordEvent(historyEvent);
			//Log.Message($"[RSI] Recorded event {historyEvent.def.ToStringWithPartner(pawn, partner)}");
		}

		public static HistoryEvent CreateEvent(this HistoryEventDef def, Pawn pawn)
		{
			return new HistoryEvent(def, pawn.Named(HistoryEventArgsNames.Doer));
		}

		public static HistoryEvent CreateEventWithPartner(this HistoryEventDef def, Pawn pawn, Pawn partner)
		{
			//Log.Message($"[RSI] Creating event {def.ToStringWithPartner(pawn, partner)}");
			HistoryEventDef overrideEvent = def.GetModExtension<DefExtension_EventOverrides>()?.overrideRules.FirstOrFallback(rule => rule.Applies(pawn, partner))?.historyEventDef;

			if (overrideEvent != null)
			{
				//Log.Message($"[RSI] Event overridden by {overrideEvent.ToStringWithPartner(pawn, partner)}");
				return overrideEvent.CreateEventWithPartner(pawn, partner);
			}

			return new HistoryEvent(def, pawn.Named(HistoryEventArgsNames.Doer), partner.Named(ArgsNamesCustom.Partner));
		}

		private static string ToStringWithPartner(this HistoryEventDef def, Pawn pawn, Pawn partner)
		{
			return $"{def.defName}, doer {pawn.NameShortColored}, partner {partner.NameShortColored}";
		}
	}
}
