using HoPoSim.Data.Domain;
using HoPoSim.Data.Generator;
using HoPoSim.Data.Model;
using HoPoSim.Framework;
using HoPoSim.Framework.Serializers;
using HoPoSim.IO.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HoPoSim.IO.Tests
{
	public class StammlisteWriterTests : TestBase
	{
		private static Stammdaten ReadStammdatenFrom(string infile)
		{
			var reader = new StammlisteReader(new ImportService());
			var data = reader.ReadStammdaten(infile);
			return data;
		}

		private string GenerateExcelTmpFileName()
		{ 
			var dest = GenerateTempDataFileFullPath($"{Guid.NewGuid()}.xlsx");
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
		public void WriteStammdaten_Always_ExportsDurchmesserInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Durchmesser>
			{
				new Durchmesser() { RangeId = 1, MinValue = 100, MaxValue = 200, Rindenstärke = 5 },
				new Durchmesser() { RangeId = 2, MinValue = 200, MaxValue = 300, Rindenstärke = 7 },
			};
			var input = new GeneratorData(2,1,1,1)
			{
				Durchmesser = new Parameter<Durchmesser,int>(values),
				Abholzigkeit = new Parameter<Abholzigkeit,int>(),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität,double>(Enumerable.Empty<Ovalität>())
			};
			input.Stammanzahl = 100;
			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.DurchmesserRegion);
			AssertAreEquals(Converter.AsDataTable(input.Durchmesser), dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsAbholzigkeitInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Abholzigkeit>
			{
				new Abholzigkeit { RangeId = 1, MinValue = 100, MaxValue = 200 },
				new Abholzigkeit { RangeId = 2, MinValue = 200, MaxValue = 300 },
			};
			var input = new GeneratorData(1,2,1,1)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(values),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität, double>(Enumerable.Empty<Ovalität>())
			};
			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.AbholzigkeitRegion);
			AssertAreEquals(Converter.AsDataTable(input.Abholzigkeit), dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsKrümmungInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Krümmung>
			{
				new Krümmung { RangeId = 1, MinValue = 100, MaxValue = 200 },
				new Krümmung { RangeId = 2, MinValue = 200, MaxValue = 300 },
			};
			var input = new GeneratorData(1,1,2,1)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(Enumerable.Empty<Abholzigkeit>()),
				Krümmung = new Parameter<Krümmung, int>(values),
				Ovalität = new Parameter<Ovalität, double>(Enumerable.Empty<Ovalität>())
			};
			var data = new Stammdaten();
			new Generator().Generate(input, data);
			//	var str3 = DumpDataTableToString(input.Krümmung.DataTable);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.KrümmungRegion);
			AssertAreEquals(Converter.AsDataTable(input.Krümmung), dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsOvalitätInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Ovalität>
			{
				new Ovalität { RangeId = 1, MinValue = 0.75, MaxValue = 0.85 },
				new Ovalität { RangeId = 2, MinValue = 0.85, MaxValue = 1.0 },
			};
			var input = new GeneratorData(1, 1, 1, 2)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(Enumerable.Empty<Abholzigkeit>()),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität, double>(values)
			};
			var data = new Stammdaten();
			new Generator().Generate(input, data);
			//	var str3 = DumpDataTableToString(input.Krümmung.DataTable);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.OvalitätRegion);
			AssertAreEquals(Converter.AsDataTable(input.Ovalität), dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsDurchmesserDistributionInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Durchmesser>
			{
				new Durchmesser() { RangeId = 1, MinValue = 100, MaxValue = 200, Rindenstärke = 5 },
				new Durchmesser() { RangeId = 2, MinValue = 200, MaxValue = 300, Rindenstärke = 7 },
			};
			var input = new GeneratorData(2,1,1,1)
			{
				Durchmesser = new Parameter<Durchmesser, int>(values),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(Enumerable.Empty<Abholzigkeit>()),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität, double>(Enumerable.Empty<Ovalität>()),
				Stammanzahl = 100
			};

			var json = LoadStringFromFile("RefFiles\\distribution_durchmesser_2_1_1_1_100.json");
			input.Distribution = Serializer<Distribution>.FromJSON(json);

			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.DurchmesserverteilungenRegion);
			var expectedTable = Converter.DurchmesserDistributionsAsDataTable(input.Distribution);
			AssertAreEquals(expectedTable, dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsAbholzigkeitDistributionInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Abholzigkeit>
			{
				new Abholzigkeit { RangeId = 1, MinValue = 100, MaxValue = 200 },
				new Abholzigkeit { RangeId = 2, MinValue = 200, MaxValue = 300 },
			};
			var input = new GeneratorData(1,3,1,1)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(values),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität, double>(Enumerable.Empty<Ovalität>()),
				Stammanzahl = 100
			};
		
			var json = LoadStringFromFile("RefFiles\\distribution_abholzigkeit_1_3_1_1_100.json");
			input.Distribution = Serializer<Distribution>.FromJSON(json);

			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.AbholzigkeitsverteilungenRegion);
			var expectedTable = Converter.AbholzigkeitDistributionsAsDataTable(input.Distribution);
			AssertAreEquals(expectedTable, dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsKrümmungDistributionInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Krümmung>
			{
				new Krümmung { RangeId = 1, MinValue = 100, MaxValue = 200 },
				new Krümmung { RangeId = 2, MinValue = 200, MaxValue = 300 },
			};
			var input = new GeneratorData(1,1,3,1)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(Enumerable.Empty<Abholzigkeit>()),
				Krümmung = new Parameter<Krümmung, int>(values),
				Ovalität = new Parameter<Ovalität, double>(Enumerable.Empty<Ovalität>()),
				Stammanzahl = 100
			};
			var json = LoadStringFromFile("RefFiles\\distribution_krümmung_1_1_3_1_100.json");
			input.Distribution = Serializer<Distribution>.FromJSON(json);

			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.KrümmungsverteilungenRegion);
			var expectedTable = Converter.KrümmungDistributionsAsDataTable(input.Distribution);
			AssertAreEquals(expectedTable, dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsOvalitätDistributionInputParameters()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var values = new List<Ovalität>
			{
				new Ovalität { RangeId = 1, MinValue = 0.75, MaxValue = 0.85 },
				new Ovalität { RangeId = 2, MinValue = 0.85, MaxValue = 1.0 },
			};
			var input = new GeneratorData(1, 1, 1, 2)
			{
				Durchmesser = new Parameter<Durchmesser, int>(Enumerable.Empty<Durchmesser>()),
				Abholzigkeit = new Parameter<Abholzigkeit, int>(Enumerable.Empty<Abholzigkeit>()),
				Krümmung = new Parameter<Krümmung, int>(Enumerable.Empty<Krümmung>()),
				Ovalität = new Parameter<Ovalität, double>(values),
				Stammanzahl = 100
			};
			var json = LoadStringFromFile("RefFiles\\distribution_ovalität_1_1_1_2_100.json");
			input.Distribution = Serializer<Distribution>.FromJSON(json);

			var data = new Stammdaten();
			new Generator().Generate(input, data);

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.EingabeparameterSheet, ApplicationTemplates.Stammdaten.OvalitätsverteilungenRegion);
			var expectedTable = Converter.OvalitätDistributionsAsDataTable(input.Distribution);
			AssertAreEquals(expectedTable, dt);
		}

		[Test]
		public void WriteStammdaten_Always_ExportsStammdaten()
		{
			var reader = new StammlisteReader(new ImportService());
			var writer = new StammlisteWriter(reader, new ExportService());
			var file = GenerateExcelTmpFileName();
			ApplicationFolders.CopyTemplate("Templates//Stammdaten.xlsx", file);
			var input = new GeneratorData(2, 2, 1, 1) { Stammanzahl = 10 };
			var data = new Stammdaten(LoadFromFile("Stammdaten.txt"));

			Assert.DoesNotThrow(() => writer.WriteStammdaten(file, input, data));

			var import = new ImportService();
			var dt = import.ImportExcel(file, ApplicationTemplates.Stammdaten.StammdatenSheet, ApplicationTemplates.Stammdaten.StammdatenRegion);

			//var s = DumpDataTableToString(dt);
			AssertAreEquals(data.DataTable, dt);
		}
	}
}
