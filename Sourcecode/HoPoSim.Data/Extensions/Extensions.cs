using HoPoSim.Data.Interfaces;
using System;

namespace HoPoSim.Data.Extensions
{
	public static class Extensions
	{
		public static double AnteilAbsolute(double percent, int total)
		{
			return Math.Round((percent * total) / 100.0, MidpointRounding.ToEven);
		}

		public static double AnteilPercent(double absolute, int total)
		{
			return total != 0 ? (absolute * 100.0) / (double)total : 0.0;
		}

		public static int GetStammAbsoluteAnteil(this IHaveAnteilPercentProperty item, int stammanzahl)
		{
			return Convert.ToInt32(AnteilAbsolute(item.AnteilPercent, stammanzahl));
		}
	}
}
