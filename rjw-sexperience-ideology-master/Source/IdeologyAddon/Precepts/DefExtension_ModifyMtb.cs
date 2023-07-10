using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Precepts
{
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Field values are loaded from XML")]
	public abstract class DefExtension_ModifyMtb : DefModExtension, IPreceptTipPostfix
	{
		protected abstract string TipTemplateKey { get; }

		public float multiplier = 1f;

		public string GetTip() => TipTemplateKey.Translate(multiplier.ToString());

		public override IEnumerable<string> ConfigErrors()
		{
			if (multiplier == 1f)
			{
				yield return "There is no point if <multiplier> is 1";
			}
			else if (multiplier <= 0f)
			{
				yield return "<multiplier> must be > 0";
			}
		}
	}
}
