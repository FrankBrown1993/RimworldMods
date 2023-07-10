using Verse;
using Verse.AI;
using RimWorld;
using rjw;

namespace rjwex
{
	public class ThinkNode_ConditionalCanUseFM : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			if (p.Map == null ||
				p.HostileTo(Faction.OfPlayer) ||
				xxx.is_animal(p) ||
				p.Drafted)
				return false;

			return true;
		}
	}
}