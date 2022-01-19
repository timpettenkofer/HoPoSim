using HoPoSim.Data.Model;
using HoPoSim.IO.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HoPoSim.IO.Tests
{
	[TestFixture]
	public class StammlisteReaderTests : TestBase
	{
		[Test]
		public void ReadStammdaten_WithDataTableHavingMissingColumn_ThrowsArgumentException()
		{
			var reader = new StammlisteReader(new ImportService());
			var dt = new DataTable("Daten");
			dt.Columns.Add(new DataColumn("Länge", typeof(double)));
			dt.Columns.Add(new DataColumn("D Stirn mit Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("D Mitte mit Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("D Zopf mit Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("D Stirn ohne Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("D Mitte ohne Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("D Zopf ohne Rinde", typeof(int)));
			dt.Columns.Add(new DataColumn("Abholzigkeit", typeof(int)));
			dt.Columns.Add(new DataColumn("Krümmung", typeof(int)));
			dt.Columns.Add(new DataColumn("Rindenstärke", typeof(int)));
			dt.Columns.Add(new DataColumn("Ovalität", typeof(double)));


			var exception = Assert.Throws<ArgumentException>(() => reader.ReadStammdaten(dt));
			Assert.AreEqual("Input Table does not have any 'Einzelstamm ID' column!", exception.Message);
		}


		[Test]
		public void ReadStammdaten_WithValidInputDataTable_RenamesColumns()
		{
			var reader = new StammlisteReader(new ImportService());
			IEnumerable<string> expectedColumnNames = GetExpectedColumnNames();

			var dt = new DataTable("Daten");
			dt.Columns.Add(new DataColumn("Einzelstamm ID", typeof(double)));
			dt.Columns.Add(new DataColumn("Länge (m)", typeof(double)));
			dt.Columns.Add(new DataColumn("D Stirn mit Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("D Mitte mit Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("D Zopf mit Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("D Stirn ohne Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("D Mitte ohne Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("D Zopf ohne Rinde (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("Abholzigkeit (mm/lfm)", typeof(int)));
			dt.Columns.Add(new DataColumn("Krümmung (mm/lfm)", typeof(int)));
			dt.Columns.Add(new DataColumn("Rindenstärke (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("Ovalität (]0,1])", typeof(double)));
			dt.Columns.Add(new DataColumn("Stammfußhöhe (cm)", typeof(int)));

			var result = reader.ReadStammdaten(dt);

			Assert.IsTrue(expectedColumnNames.All(n => result.DataTable.Columns.Contains(n)));
		}

		private static IEnumerable<string> GetExpectedColumnNames()
		{
			var expectedColumnNames = new[]
			{
				Stammdaten.STAMM_ID,
				Stammdaten.LÄNGE,
				Stammdaten.D_STIRN_mR,
				Stammdaten.D_MITTE_mR,
				Stammdaten.D_ZOPF_mR,
				Stammdaten.D_STIRN_oR,
				Stammdaten.D_MITTE_oR,
				Stammdaten.D_ZOPF_oR,
				Stammdaten.ABHOLZIGKEIT,
				Stammdaten.KRÜMMUNG,
				Stammdaten.RINDENSTÄRKE,
				Stammdaten.OVALITÄT,
				Stammdaten.STAMMFUßHÖHE
			};
			return expectedColumnNames;
		}

		[Test]
		public void ReadStammdaten_WithInvalidFilename_ThrowsException()
		{
			var reader = new StammlisteReader(new ImportService());

			var exception = Assert.Throws<ArgumentException>(() => reader.ReadStammdaten("invalid"));
		}

		[Test]
		public void ReadStammdaten_WithValidFilename_ReturnsExpectedResults()
		{
			var reader = new StammlisteReader(new ImportService());
			IEnumerable<string> expectedColumnNames = GetExpectedColumnNames();
			var fullpath = GetTestDataFileFullPath("Stammdaten.xlsx");

			var result = reader.ReadStammdaten(fullpath);

			Assert.AreEqual(expectedColumnNames.Count(), result.DataTable.Columns.Count);
			var str1 = DumpDataTableToString(result.DataTable);

			Assert.AreEqual(2, result.DataTable.Rows.Count);
		}
	}
}
