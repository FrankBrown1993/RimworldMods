﻿using RimWorld;
using System;

namespace RJWSexperience
{
	public static class Utility
	{
		private static readonly Random random = new Random(Environment.TickCount);

		public static float RandGaussianLike(float min, float max, int iterations = 3)
		{
			double res = 0;
			for (int i = 0; i < iterations; i++)
			{
				res += random.NextDouble();
			}
			res /= iterations;

			return ((float)res).Denormalization(min, max);
		}

		public static void SetTo(this Pawn_RecordsTracker records, RecordDef record, float value)
		{
			float recordval = records.GetValue(record);
			records.AddTo(record, value - recordval);
		}

		public static float Normalization(this float num, float min, float max)
		{
			return (num - min) / (max - min);
		}

		public static float Denormalization(this float num, float min, float max)
		{
			return (num * (max - min)) + min;
		}
	}
}
