using UnityEngine;
using Verse;
using Verse.AI;

namespace rjwex
{
	/// <summary>
	/// Class to inherit to use Draw Offset and Angle Offset
	/// </summary>
	public abstract class JobDriverWithOffsets : JobDriver
	{
		protected Vector3 drawOffset = new Vector3(0, 0, 0);
		public Vector3 GetDrawOffset() { return drawOffset; }

		protected float angleOffset = 0.0f;
		public float GetAngleOffset() { return angleOffset; }
	}

	public static class DrawUtils
	{
		/// <summary>
		/// Get draw point offset for pawn if current JobDriver inherits JobDriverWithOffsets class, else offset set to Vector3(0, 0, 0)
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static Vector3 GetDrawOffset(this Pawn pawn)
		{
			if (pawn.jobs?.curDriver is JobDriverWithOffsets job)
			{
				return job.GetDrawOffset();
			}
			return new Vector3(0, 0, 0);
		}

		/// <summary>
		/// Get angle offset for pawn if current JobDriver inherits JobDriverWithOffsets class, else offset set to 0.0f
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static float GetAngleOffset(this Pawn pawn)
		{
			if (pawn.jobs?.curDriver is JobDriverWithOffsets job)
			{
				return job.GetAngleOffset();
			}
			return 0.0f;
		}
	}
}
