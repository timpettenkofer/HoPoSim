using System.Collections.Generic;
using System.Data;

namespace HoPoSim.IO.Interfaces
{
	public interface IExportTarget
	{
		string SheetName { get; set; }
		string RegionName { get; set; }
		DataTable DataTable { get; set; }
		bool ExportHeaders { get; set; }
		bool ShowSheet { get; set; }
	}

	public interface IExportService
	{
		void ExportExcel(string file, IEnumerable<IExportTarget> exports);
		void Copy(string sourceFile, string targetFile);
	}
}
