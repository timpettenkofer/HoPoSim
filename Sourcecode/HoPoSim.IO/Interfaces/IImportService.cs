using System.Data;

namespace HoPoSim.IO.Interfaces
{
	public interface IImportService
	{
		DataTable ImportExcel(string file, string sheetName, string tableName);
		DataTable ImportCsv(string file, string delimiter = ";");
	}
}
