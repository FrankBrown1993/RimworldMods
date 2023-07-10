using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace rjwwhoring
{
	/// <summary>
	/// Rimworld object for storing the world/save info
	/// </summary>
	public class DataStore : WorldComponent
	{
		public Dictionary<int, BedData> bedData = new Dictionary<int, BedData>();

		public DataStore(World world) : base(world)
		{
		}

		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				bedData.RemoveAll(item => item.Value == null || !item.Value.IsValid);
			}
				
			base.ExposeData();
			Scribe_Collections.Look(ref bedData, "BedData", LookMode.Value, LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if (bedData == null) bedData = new Dictionary<int, BedData>();
			}
		}

		public BedData GetBedData(Building_Bed bed)
		{
			BedData res;
			var filled = bedData.TryGetValue(bed.thingIDNumber, out res);
			if ((res == null) || (!res.IsValid))
			{
				if (filled)
				{
					bedData.Remove(bed.thingIDNumber);
				}
				res = new BedData(bed);
				bedData.Add(bed.thingIDNumber, res);
			}
			return res;
		}
	}
}