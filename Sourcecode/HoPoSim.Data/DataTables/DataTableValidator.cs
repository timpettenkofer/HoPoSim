using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;

namespace HoPoSim.Data.DataTables
{
	[Export(typeof(DataTableValidator))]
	public class DataTableValidator
	{
		public bool Validate(DataTable dt, IEnumerable<string> skipColumns)
		{
			return true;
		}
	}
}
