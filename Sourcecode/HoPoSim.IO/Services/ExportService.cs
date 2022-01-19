using HoPoSim.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Excel = NetOffice.ExcelApi;

namespace HoPoSim.IO
{
	public class ExportTarget : IExportTarget
	{
		public string SheetName { get; set; }
		public string RegionName { get; set; }
		public DataTable DataTable { get; set; }
		public bool ExportHeaders { get; set; }
		public bool ShowSheet { get; set; }
	}

	[Export(typeof(IExportService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ExportService : IExportService
	{
		public void ExportExcel(string file, IEnumerable<IExportTarget> exports)
		{
			ExportExcelData(file, exports);
		}

		private void ExportExcelData(string file, IEnumerable<IExportTarget> exports)
		{
			Excel.Application application = null;
			Excel.Workbook workbook = null;

			try
			{
				application = new Excel.Application
				{
					DisplayAlerts = false
				};

				var workbooks = application.Workbooks;
				object readOnly = false;
				workbook = workbooks.Open(file, null, readOnly);

				WriteDataTablesToExcelExcel(workbook, exports);
			}
			catch (Exception e)
			{
				throw new ArgumentException("Cannot write to output data.", e.GetBaseException());
			}
			finally
			{
				// close word and dispose reference 
				if (workbook != null)
				{
					workbook.Close(true);
					workbook.Dispose();
				}
				if (application != null)
				{
					application.Quit();
					application.Dispose();
				}
			}
		}

		private void WriteDataTablesToExcelExcel(Excel.Workbook workbook, IEnumerable<IExportTarget> exports)
		{ 
			foreach (var export in exports)
			{
				Excel.Worksheet sheet = GetSheet(workbook, export.SheetName);

				var isSheetProtected = sheet.ProtectScenarios;
				if (isSheetProtected)
					UnprotectSheet(sheet);

				var dataTable = export.DataTable;
				var table = GetTable(sheet, export.RegionName);

				//Get all data into an array
				var tempArray = new object[dataTable.Rows.Count, dataTable.Columns.Count];
				for (var r = 0; r < dataTable.Rows.Count; r++)
				{
					for (var c = 0; c < dataTable.Columns.Count; c++)
						tempArray[r, c] = dataTable.Rows[r][c];
				}

				if (export.ExportHeaders)
				{
					//Get column names into an array
					var tempHeadingArray = new object[dataTable.Columns.Count];
					for (var i = 0; i < dataTable.Columns.Count; i++)
					{
						tempHeadingArray[i] = dataTable.Columns[i].ColumnName;
					}
					////Create style used for displaying column names
					//var style = application.ActiveWorkbook.Styles.Add("NewStyle");
					//style.Font.Name = "Verdana";
					//style.Font.Size = 10;
					////style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
					//style.Font.Bold = true;

					AddColumnNames(table, tempHeadingArray);
					//AddExcelHeadingText(sheet);
				}

				AddDataRows(table, dataTable, tempArray);

				if (export.ShowSheet && IsHidden(sheet))
					ShowSheet(sheet);

				if (isSheetProtected)
					ProtectSheet(sheet);
			}
		}

		private static bool IsHidden(Excel.Worksheet sheet)
		{
			return sheet.Visible != Excel.Enums.XlSheetVisibility.xlSheetVisible;
		}

		private static void ShowSheet(Excel.Worksheet sheet)
		{
			sheet.Visible = Excel.Enums.XlSheetVisibility.xlSheetVisible;
		}

		private static Excel.ListObject GetTable(Excel.Worksheet sheet, string name)
		{
			try
			{
				var tables = sheet.ListObjects;
				return tables.First(t => t.Name == name);
			}
			catch
			{
				throw new ArgumentException($"Cannot find Excel table '{name}' in sheet '{sheet.Name}'.");
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

		private static void AddDataRows(Excel.ListObject table, DataTable datatable, object[,] tempArray)
		{
			var rowCount = datatable.Rows.Count;
			var columnCount = datatable.Columns.Count; 
			if (rowCount > 0)
			{
				var range = table.Range.Resize(rowCount, columnCount);
				var dest = range.Offset(1); // offset headers row
				dest.Value = tempArray;
			}
		}

		private static void AddColumnNames(Excel.ListObject table, object[] tempHeadingArray)
		{
			var columnNameRange = table.Range.Resize(1, tempHeadingArray.Length);
			//columnNameRange.Style = "NewStyle";
			columnNameRange.Value = tempHeadingArray;
			//columnNameRange.UseStandardWidth = true;
		}

		public static void ProtectSheet(Excel.Worksheet worksheet)
		{
			if (worksheet != null)
			{
				// hide formual and allow filtering
				worksheet.UsedRange.FormulaHidden = true;
				worksheet.Protect(null, false, true, true, false, false, false, false, false, false, false, false, false, false, true, false);
			}
		}

		public static void UnprotectSheet(Excel.Worksheet worksheet)
		{
			if (worksheet != null)
			{
				worksheet.Unprotect();
				worksheet.UsedRange.FormulaHidden = false;
			}
		}

		public void Copy(string sourceFile, string targetFile)
		{
			File.Copy(sourceFile, targetFile, true);
		}

		//private static void AddExcelHeadingText(Excel.Worksheet sheet)
		//{
		//	//Apply styling to heading text
		//	sheet.Cells[1, 1].Value = "Excel heading text";
		//	sheet.Cells[1, 1].Font.Name = "Verdana";
		//	sheet.Cells[1, 1].Font.Italic = true;
		//	sheet.Cells[1, 1].Font.Underline = true;
		//	sheet.Cells[1, 1].Font.Size = 14;
		//	//sheet.Cells[1, 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkRed);
		//}
	}
}
