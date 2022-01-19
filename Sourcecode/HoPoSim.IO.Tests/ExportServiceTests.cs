using NUnit.Framework;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace HoPoSim.IO.Tests
{
	[TestFixture]
	public class ExportServiceTests : TestBase
	{
		private string CopyTestDataFile(string src)
		{
			var fullpath = GetTestDataFileFullPath(src);
			var dest = GenerateTempDataFileFullPath($"{Guid.NewGuid()}.xlsx");
			File.Copy(fullpath, dest);
			return dest;
		}

		[TearDown]
		public void TearDown()
		{
			try
			{
				DirectoryInfo di = new DirectoryInfo(GetTestTmpDirectory());

				foreach (FileInfo file in di.GetFiles())
					file.Delete();
			}
			catch
			{ }
		}

		[Test]
		public void ExportExcel_WithValidInputFile_ExportsExpectedData()
		{
			var dest = CopyTestDataFile("TestExport.xlsx");
			var exportService = new ExportService();
			var dt = new DataTable("Test");
			dt.Columns.AddRange(new[] { new DataColumn("A", typeof(double)), new DataColumn("B"), new DataColumn("C") });
			dt.Rows.Add(1.0, "2", "3");
			dt.Rows.Add(2.0, "II", "III");
			dt.Rows.Add(3.0, "b", "c");
			dt.Rows.Add(4.0, "B", "C");

			exportService.ExportExcel(dest, new[] { new ExportTarget { DataTable = dt, SheetName="Test", RegionName = "Table" } });

			var import = new ImportService();
			var dt2 = import.ImportExcel(dest, "Test", "Table");
			AssertAreEquals(dt, dt2);
		}

		[Test]
		public void ExportExcel_WithInvalidPath_ThrowsExpectedException()
		{
			var fullpath = GetTestDataFileFullPath("invalid.xlsx");
			var exportService = new ExportService();

			var exception = Assert.Throws<ArgumentException>(() => exportService.ExportExcel(fullpath, new ExportTarget[] { }));
			StringAssert.Contains("invalid.xlsx", exception.InnerException.Message);
		}

		[Test]
		public void ExportExcel_WithInvalidArguments_ThrowsExpectedException()
		{
			var dest = CopyTestDataFile("TestExport.xlsx");
			var exportService = new ExportService();
			var dt = new DataTable("Test");
			dt.Columns.AddRange(new[] { new DataColumn("A"), new DataColumn("B"), new DataColumn("C") });
			dt.Rows.Add(new[] { 1, 2, 3 });
			dt.Rows.Add(new[] { "a", "b", "c" });
			dt.Rows.Add(new[] { "A", "B", "C" });

			var exception = Assert.Throws<ArgumentException>(() => exportService.ExportExcel(dest, new[] { new ExportTarget { DataTable = dt, SheetName = "Test", RegionName = "Invalid" } }));
			Assert.AreEqual("Cannot find Excel table 'Invalid' in sheet 'Test'.", exception.InnerException.Message);
		}
	}
}
