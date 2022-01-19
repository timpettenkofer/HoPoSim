using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Domain
{
	public class Distribution : BaseEntity
	{
		public Distribution()
		{ }

		internal Distribution(Distribution copyThis)
		{
			RangeId = copyThis.RangeId;
			Total = copyThis.Total;
			Percent = copyThis.Percent;
			Absolute = copyThis.Absolute;
			Children = copyThis.Children.Select(c => new Distribution(c)).ToList();
		}

		public int RangeId { get; set; }

		public int Total { get; set; }

		public double Percent { get; set; }

		public int Absolute { get; set; }

		public virtual IList<Distribution> Children { get; set; } = Enumerable.Empty<Distribution>().ToList();


		public double ToPercent()
		{
			return Total != 0 ? (Absolute * 100.0) / (double)Total : 0.0;
		}

		public int ToAbsolute()
		{
			return Convert.ToInt32( Math.Round((Percent * Total) / 100.0, MidpointRounding.ToEven));
		}
	}
}
