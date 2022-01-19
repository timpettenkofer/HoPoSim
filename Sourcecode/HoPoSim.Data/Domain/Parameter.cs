using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HoPoSim.Data.Domain
{
	public class Parameter<T, B> : BaseEntity where T : Range<B>, new()
	{
		public Parameter()
		{
			Values = new List<T>();
		}

		public Parameter(IEnumerable<T> values)
		{
			Values = new List<T>(values);
		}

		public Parameter(Parameter<T,B> copyThis)
		{
			Values = copyThis.Values
				.Select(k => k.Clone())
				.Cast<T>()
				.ToList();
		}

		public Parameter(int numValues)
		{
			Values = Enumerable.Range(1, numValues)
				.Select(i => new T
				{
					RangeId = i,
					MinValue = default(B),
					MaxValue = default(B),
				})
				.ToList();
		}

		public T this[int klasseId]
		{
			get
			{
				return Values.FirstOrDefault(k => k.RangeId == klasseId);
			}
		}

		public virtual ICollection<T> Values { get; set; }
	}
}
