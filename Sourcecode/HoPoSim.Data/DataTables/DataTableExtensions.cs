using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace HoPoSim.Data.Extensions
{
	public static class DataTableExtensions
	{
		public static double GetColumnSum(this DataTable dt, string columnName)
		{
			var results = (from DataRow row in dt.Rows select (double)row[columnName]).Sum();
			return results;
		}

		public static double GetColumnAverage(this DataTable dt, string columnName)
		{
			if (dt.Rows.Count == 0) return 0;
			var results = (from DataRow row in dt.Rows select (double)row[columnName]).Average();
			return results;
		}

		public static int GetRowCount(this DataTable dt)
		{
			var results = (from DataRow row in dt.Rows select 1).Count();
			return results;
		}

		public static string AsString(this DataTable dt)
		{
			using (var writer = new StringWriter())
			{
				dt.WriteXml(writer, XmlWriteMode.WriteSchema);
				return writer.ToString();
			}
		}

		public static DataTable LoadFromString(string value)
		{
			var dt = new DataTable();

			using (var reader = XmlReader.Create(new StringReader(value)))
			{
				dt.ReadXml(reader);
				dt.AcceptChanges();
				return dt;
			}
		}
	}
}
