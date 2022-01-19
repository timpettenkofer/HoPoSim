using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HoPoSim.IO.Tests
{
	public class DataTableItemArrayEqualityComparer : IEqualityComparer<DataRow>
	{
		public bool Equals(DataRow x, DataRow y)
		{
			var x_items = x.ItemArray.Select(i => { return (i is int) ? Convert.ToDouble(i) : i; });
			var y_items = y.ItemArray.Select(i => { return (i is int) ? Convert.ToDouble(i) : i; });

			return x_items.SequenceEqual(y_items);
		}

		private int HashString(IEnumerable<char> chars)
		{
			int hash = 23;
			foreach(char c in chars)
			{
				hash = hash * 31 + c;
			}
			return hash;
		}

		public int GetHashCode(DataRow row)
		{
			return HashString(row.ItemArray.SelectMany(i => i.ToString()));
		}
	}
}
