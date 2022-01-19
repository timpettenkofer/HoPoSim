using NUnit.Framework;
using System;


namespace HoPoSim.IO.Tests
{
	[TestFixture]
	public class ImportServiceTests : TestBase
	{
		[TestCase("Stammdaten.xlsx", "RefFiles\\Stammdaten_excel.ref")]
		public void ImportExcel_WithValidInputFile_ImportsExpectedData(string infile, string reffile)
		{
			var fullpath = GetTestDataFileFullPath(infile);
			var importService = new ImportService();

			var result = importService.ImportExcel(fullpath, ApplicationTemplates.Stammdaten.StammdatenSheet, ApplicationTemplates.Stammdaten.StammdatenRegion);

			AssertEqualReferenceFile(result, reffile);
		}

		[TestCase("invalid.xlsx", ApplicationTemplates.Stammdaten.StammdatenSheet, ApplicationTemplates.Stammdaten.StammdatenRegion)]
		public void ImportExcel_WithInvalidPath_ThrowsExpectedException(string infile, string sheetName, string tableName)
		{
			var fullpath = GetTestDataFileFullPath(infile);
			var importService = new ImportService();

			Assert.Throws<ArgumentException>(() => importService.ImportExcel(fullpath, sheetName, tableName));
		}

		[TestCase("Stammdaten.xlsx", "InvalidSheetName", ApplicationTemplates.Stammdaten.StammdatenRegion)]
		[TestCase("Stammdaten.xlsx", ApplicationTemplates.Stammdaten.StammdatenSheet, "InvalidTableName")]
		public void ImportExcel_WithInvalidArguments_ReturnsNull(string infile, string sheetName, string tableName)
		{
			var fullpath = GetTestDataFileFullPath(infile);
			var importService = new ImportService();

			var result = importService.ImportExcel(fullpath, sheetName, tableName);
			Assert.IsNull(result);
		}


		[TestCase("Stammdaten.csv", "RefFiles\\Stammdaten_csv.ref")]
		public void ImportCsv_WithValidInputFile_ImportsExpectedData(string infile, string reffile)
		{
			var fullpath = GetTestDataFileFullPath(infile);
			var importService = new ImportService();

			var result = importService.ImportCsv(fullpath, ";");

			AssertEqualReferenceFile(result, reffile);
		}


		[TestCase("invalid.csv", ";")]
		public void ImportCsv_WithInvalidPath_ThrowsExpectedException(string infile, string delimiter)
		{
			var fullpath = GetTestDataFileFullPath(infile);
			var importService = new ImportService();

			Assert.Throws<ArgumentException>(() => importService.ImportCsv(fullpath, delimiter));
		}
	}
}
