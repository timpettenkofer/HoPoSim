using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Generator
{
	public static class RandomExtensions
	{
		public static int NextInt(this Random random, int minValue, int maxValue)
		{
			if (minValue > maxValue)
				throw new ArgumentException("minValue must be less than maxValue");

			return Convert.ToInt32(Math.Round(minValue + random.NextDouble() * (maxValue - minValue), 0));
		}

		public static double NextDouble(this Random random, double minValue, double maxValue, int decimals = 2)
		{
			if (minValue > maxValue)
				throw new ArgumentException("minValue must be less than maxValue");

			return Math.Round(minValue + random.NextDouble() * (maxValue - minValue), decimals);
		}

		public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> input, Random random, int count)
		{
			if (count < 0)
				throw new ArgumentException("count must be positive");

			return input.OrderBy(x => random.Next()).Take(count);
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> input, Random random)
		{
			return input.OrderBy(x => random.Next());
		}
	}
}
