using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace RJWSexperience.Ideology.Precepts
{
	/// <summary>
	/// Special Def extension for the Incestuous issue precepts
	/// </summary>
	[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "Def loader")]
	public class DefExtension_Incest : DefModExtension
	{
		public List<BloodRelationDegree> allowManualRomanceOnlyFor;
		public List<BloodRelationDegreeFactor> bloodRelationDegreeRomanceFactors;

		private Dictionary<BloodRelationDegree, float> _relationDegreeFactorsDict;

		public bool TryGetRomanceChanceFactor(BloodRelationDegree relationDegree, out float romanceChanceFactor)
		{
			if (bloodRelationDegreeRomanceFactors.NullOrEmpty())
			{
				romanceChanceFactor = 1f;
				return false;
			}

			if (_relationDegreeFactorsDict == null)
			{
				_relationDegreeFactorsDict = new Dictionary<BloodRelationDegree, float>();
				foreach (BloodRelationDegreeFactor relationDegreeFactor in bloodRelationDegreeRomanceFactors)
				{
					_relationDegreeFactorsDict.Add((BloodRelationDegree)relationDegreeFactor.bloodRelationDegree, relationDegreeFactor.romanceChanceFactor);
				}
			}

			return _relationDegreeFactorsDict.TryGetValue(relationDegree, out romanceChanceFactor);
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (BloodRelationDegreeFactor factor in bloodRelationDegreeRomanceFactors)
			{
				foreach(string error in factor.ConfigErrors())
				{
					yield return error;
				}
			}
		}

		public class BloodRelationDegreeFactor
		{
			public BloodRelationDegree? bloodRelationDegree;
			public float romanceChanceFactor;

			public IEnumerable<string> ConfigErrors()
			{
				if (bloodRelationDegree == null)
				{
					yield return "<bloodRelationDegree> is empty";
				}

				if (romanceChanceFactor == 0f)
				{
					yield return "<romanceChanceFactor> should be > 0";
				}
			}
		}
	}
}
