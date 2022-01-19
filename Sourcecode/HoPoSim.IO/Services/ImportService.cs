using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using Excel = NetOffice.ExcelApi;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using HoPoSim.IO.Interfaces;
using System.Threading;

namespace HoPoSim.IO
{
	[Export(typeof(IImportService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ImportService : IImportService
	{
		public DataTable ImportExcel(string file, string sheetName, string tableName)
		{
			return ImportExcelData(file, sheetName, tableName);
		}

		private DataTable ImportExcelData(string file, string sheetName, string tableName)
		{

			Excel.Application application = null;
			Excel.Workbook workbook = null;

			try
			{
				application = new Excel.Application();
				application.DisplayAlerts = false;

				object readOnly = true;
				var workbooks = application.Workbooks;
				workbook = workbooks.Open(file, null, readOnly);

				var dt = ReadDataTableFromExcelTable(workbook, sheetName, tableName);
				return dt;
			}
			catch (Exception e)
			{
				throw new ArgumentException("Cannot read input data.", e);
			}
			finally
			{
				// close word and dispose reference 
				if (workbook != null)
				{
					workbook.Close(false);
					workbook.Dispose();
				}
				if (application != null)
				{
					application.Quit();
					application.Dispose();
				}
			}
		}

		private DataTable ReadDataTableFromExcelTable(Excel.Workbook workbook, string sheetName, string tableName)
		{
			Excel.Worksheet worksheet = null;
			try
			{
				object misValue = System.Reflection.Missing.Value;
				worksheet = GetSheet(workbook, sheetName);
				if (worksheet == null)
					return null;
				Excel.ListObject table = worksheet.ListObjects.FirstOrDefault(t => t.Name.Equals(tableName));
				if (table == null)
					return null;

				DataTable tbl = new DataTable() { TableName = tableName };
				var range = table.Range;
				object[,] srcRange = (object[,])table.Range.Value2;

				int nbRows = srcRange.GetLength(0);
				int nbColumns = srcRange.GetLength(1);

				// get column names from header line
				for (int i = 1; i <= nbColumns; i++)
				{
					tbl.Columns.Add(srcRange[1, i].ToString(), typeof(object));
				}

				// read values row by row
				for (int i = 1; i < nbRows; i++)
				{
					tbl.Rows.Add();

					for (int j = 1; j <= nbColumns; j++)
					{
						tbl.Rows[i - 1][j - 1] = srcRange[i + 1, j];
					}
				}
				return tbl;
			}
			catch
			{
				return null;
			}
			finally
			{
				try
				{
					//   releaseObject(worksheet);
				}
				catch { }
			}
		}

		private static Excel.Worksheet GetSheet(Excel.Workbook doc, string sheetname)
		{
			try
			{
				return doc.Sheets.Cast<Excel.Worksheet>().ToList().First(s => s.Name == sheetname);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// workaround for "call was rejected by callee" exception
				Thread.Sleep(1000);
				return GetSheet(doc, sheetname);
			}
		}

		public DataTable ImportCsv(string file, string delimiter = ";")
		{
			return ReadDataTableFromCSVFile(file, delimiter);
		}

		private DataTable ReadDataTableFromCSVFile(string file, string delimiter)
		{
			DataTable csvData = new DataTable() { TableName = "Data" };
			try
			{
				using (TextFieldParser csvReader = new TextFieldParser(file, Encoding.UTF8))
				{
					csvReader.SetDelimiters(new string[] { delimiter });
					csvReader.HasFieldsEnclosedInQuotes = true;
					//read column names
					string[] colFields = csvReader.ReadFields();
					foreach (string column in colFields)
					{
						DataColumn dataColumn = new DataColumn(column);
						dataColumn.AllowDBNull = true;
						csvData.Columns.Add(dataColumn);
					}
					while (!csvReader.EndOfData)
					{
						string[] fieldData = csvReader.ReadFields();
						//Making empty value as null
						for (int i = 0; i < fieldData.Length; i++)
						{
							if (fieldData[i] == "")
							{
								fieldData[i] = null;
							}
						}
						csvData.Rows.Add(fieldData);
					}
				}
			}
			catch (Exception e)
			{
				throw new ArgumentException("Cannot not read input data.", e);
			}
			return csvData;
		}
	}
}
