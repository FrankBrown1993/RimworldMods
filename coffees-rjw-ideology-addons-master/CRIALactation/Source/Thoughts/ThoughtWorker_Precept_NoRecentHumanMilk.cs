using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace CRIALactation
{
    public class ThoughtWorker_Precept_NoRecentHumanMilk : ThoughtWorker_Precept, IPreceptCompDescriptionArgs
    {
        public IEnumerable<NamedArgument> GetDescriptionArgs()
        {
            yield return MinDaysSinceLastHumanMeatForThought.Named("HUMANMILKREQUIREDINTERVAL");
        }

        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            int num = Mathf.Max(0, p.TryGetComp<CompLactation>().lastHumanLactationIngestedTick);
            return Find.TickManager.TicksGame - num > 480000 && !LactationUtility.IsHucow(p);
        }

        public const int MinDaysSinceLastHumanMeatForThought = 8;
    }
}
