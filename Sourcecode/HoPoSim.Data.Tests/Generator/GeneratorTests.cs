using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;
using HoPoSim.Framework.Serializers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HoPoSim.Data.Tests.Generator
{
	[TestFixture]
	public class GeneratorTests : TestBase
	{
		private GeneratorData CreateSimpleGeneratorData(int stammanzahl, float länge)
		{
			var data = GeneratorExtensions.CreateGeneratorData(1, 1, 1, 1, stammanzahl);
			data.Länge = länge;
			data.InitDurchmesser(1, 100, 200, 0);
			data.InitAbholzigkeit(1, 10, 20);
			data.InitKrümmung(1, 10, 20);
			data.InitOvalität(1, 0.8, 1.0);

			var json = "{\"RangeId\":-1,\"Total\":100,\"Percent\":100.0,\"Absolute\":100,\"Children\":[{\"RangeId\":1,\"Total\":100,\"Percent\":100.0,\"Absolute\":100,\"Children\":[{\"RangeId\":1,\"Total\":100,\"Percent\":100.0,\"Absolute\":100,\"Children\":[{\"RangeId\":1,\"Total\":100,\"Percent\":100.0,\"Absolute\":100,\"Children\":[{\"RangeId\":1,\"Total\":100,\"Percent\":100.0,\"Absolute\":100,\"Children\":[]}]}]}]}]}";
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			return data;
		}

		[Test]
		public void Generator_Generate_Always_OutputsTrunkId()
		{
			int stammanzahl = 100;
			var data = CreateSimpleGeneratorData(stammanzahl, 1.0f);
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();

			generator.Generate(data, stammdaten);

			var ids = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.STAMM_ID]);
			var expected = Enumerable.Range(1, stammanzahl).Select(i => i.ToString());
			CollectionAssert.AreEquivalent(expected, ids);
		}

		[Test]
		public void Generator_Generate_Always_OutputsRindenstärkeParameter()
		{
			int stammanzahl = 100;
			float länge = 6.0f;
			int expectedValue = 12;
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			var generator = new Data.Generator.Generator();
			data.InitDurchmesser(1, 100, 200, 12);
			var stammdaten = new Stammdaten();

			generator.Generate(data, stammdaten);

			var rindenstärke = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.RINDENSTÄRKE]).ToList();
			Assert.AreEqual(stammanzahl, rindenstärke.Count());
			Assert.IsTrue(rindenstärke.All(r => (int)r == expectedValue));
		}


		[Test]
		public void Generator_Generate_Always_OutputsExpectedKrümmungParameters()
		{
			int stammanzahl = 100;
			float länge = 6.0f;
			int minKrümmung = 10;
			int maxKrümmung = 20;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitKrümmung(1, minKrümmung, maxKrümmung);

			generator.Generate(data, stammdaten);

			var krümmungen = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.KRÜMMUNG]).ToList();
			Assert.AreEqual(stammanzahl, krümmungen.Count());
			Assert.IsTrue(krümmungen.All(k => 
				(int)k >= minKrümmung && (int)k <= maxKrümmung));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedOvalitätParameters()
		{
			int stammanzahl = 100;
			float länge = 6.0f;
			double minOvalität = 0.8;
			double maxOvalität = 1.0;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitOvalität(1, minOvalität, maxOvalität);

			generator.Generate(data, stammdaten);

			var ovalitäten = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.OVALITÄT]).ToList();
			Assert.AreEqual(stammanzahl, ovalitäten.Count());
			Assert.IsTrue(ovalitäten.All(k =>
				(double)k >= minOvalität && (double)k <= maxOvalität));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedAbholzigkeitParameters()
		{
			int stammanzahl = 100;
			float länge = 6.0f;
			int minAbholzigkeit = 10;
			int maxAbholzigkeit = 20;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitAbholzigkeit(1, minAbholzigkeit, maxAbholzigkeit);

			generator.Generate(data, stammdaten);

			var krümmungen = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.ABHOLZIGKEIT]).ToList();
			Assert.AreEqual(stammanzahl, krümmungen.Count());
			Assert.IsTrue(krümmungen.All(k =>
				(int)k >= minAbholzigkeit && (int)k <= maxAbholzigkeit));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedMittendurchmesserMitRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 6.0f;
			int minValue = 100;
			int maxValue = 200;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitDurchmesser(1, minValue, maxValue, 0);

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_MITTE_mR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k => 
				(int)k >= minValue && (int)k <= maxValue));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedMittendurchmesserOhneRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 6.0f;			
			int minValue = 100;
			int maxValue = 200;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitDurchmesser(1, minValue, maxValue, 15);

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_MITTE_oR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k => 
				(int)k >= minValue - 2 * 15 && (int)k <= maxValue - 2 * 15));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedZopfdurchmesserMitRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 4.0f;
			int minValue = 100;
			int maxValue = 200;
			int abholzigkeit = 12;
			var expectedAbholzigkeit = abholzigkeit * länge / 2; 
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitDurchmesser(1, minValue, maxValue, 10);
			data.InitAbholzigkeit(1, abholzigkeit, abholzigkeit);

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_ZOPF_mR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k => 
				(int)k >= minValue - expectedAbholzigkeit && (int)k <= maxValue - expectedAbholzigkeit));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedZopfdurchmesserOhneRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 4.0f;
			int minValue = 100;
			int maxValue = 200;
			int rindenstärke = 10;
			int abholzigkeit = 12;
			var expectedAbholzigkeit = abholzigkeit * länge / 2;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitDurchmesser(1, minValue, maxValue, rindenstärke);
			data.InitAbholzigkeit(1, abholzigkeit, abholzigkeit);

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_ZOPF_oR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k => 
				(int)k >= minValue - expectedAbholzigkeit - 2* rindenstärke && (int)k <= maxValue - expectedAbholzigkeit - 2* rindenstärke));

		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedStirndurchmesserMitRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 4.0f;
			int minValue = 100;
			int maxValue = 200;
			int abholzigkeit = 12;
			var expectedAbholzigkeit = abholzigkeit * länge / 2;
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();
			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitDurchmesser(1, minValue, maxValue, 10);
			data.InitAbholzigkeit(1, abholzigkeit, abholzigkeit);

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_STIRN_mR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k =>
				(int)k >= minValue + expectedAbholzigkeit && (int)k <= maxValue + expectedAbholzigkeit));
		}

		[Test]
		public void Generator_Generate_Always_OutputsExpectedStirndurchmesserOhneRindeParameters()
		{
			int stammanzahl = 100;
			float länge = 4.0f;
			int minValue = 100;
			int maxValue = 200;
			int rindenstärke = 10;
			int abholzigkeit = 12;
			var expectedAbholzigkeit = abholzigkeit * länge / 2;

			var data = CreateSimpleGeneratorData(stammanzahl, länge);
			data.InitAbholzigkeit(1, abholzigkeit, abholzigkeit);
			data.InitDurchmesser(1, minValue, maxValue, rindenstärke);
			var generator = new Data.Generator.Generator();
			var stammdaten = new Stammdaten();

			generator.Generate(data, stammdaten);

			var durchmesser = stammdaten.DataTable.AsEnumerable().Select(r => r[Stammdaten.D_STIRN_oR]).ToList();
			Assert.AreEqual(stammanzahl, durchmesser.Count());
			Assert.IsTrue(durchmesser.All(k =>
				(int)k >= minValue + expectedAbholzigkeit - 2 * rindenstärke && (int)k <= maxValue + expectedAbholzigkeit - 2 * rindenstärke));
		}

		[Test]
		public void Generator_Generate_WithMultidimensionalClasses_OutputsExpectedDataTable()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 2, 1);
			data.Länge = 6.0f;
			data.LängeVariation = 1.0f;
			data.Stammanzahl = 100;

			data.InitDurchmesser(1, 100, 200, 10);
			data.InitDurchmesser(2, 200, 300, 15);
			data.InitAbholzigkeit(1, 10, 20);
			data.InitAbholzigkeit(2, 20, 30);
			data.InitKrümmung(1, 10, 20);
			data.InitKrümmung(2, 20, 35);
			data.InitOvalität(1, 0.8, 1.0);

			var json = LoadStringFromFile("RefFiles\\Generator\\TestDistribution_2_2_2_1_100.json"); 
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var generator = new Data.Generator.Generator();
			generator.SetSeed(1000);
			var stammdaten = new Stammdaten();

			generator.Generate(data, stammdaten);

			AssertEqualReferenceFile(stammdaten.DataTable, "RefFiles\\Generator\\Stammdaten.ref");
		}

		private IList<Stamm> GenerateStämme(int start, int count)
		{
			return Enumerable.Range(start, count)
				.Select(i => new Stamm(i.ToString()))
				.ToList();
		}

		[Test]
		public void Generator_DrawCandidates_ValidParameters_ReturnsExpectedResults()
		{
			var input = GenerateStämme(1, 10);
			var generator = new Data.Generator.Generator();

			var results = generator.DrawCandidates(input, 3);

			Assert.AreEqual(3, results.Count());
			Assert.AreEqual(3, results.Select(s => s.StammId).Count());
		}

		[Test]
		public void Generator_DrawCandidates_CountGreaterThanCandidatesList_ReturnsExpectedResults()
		{
			var input = GenerateStämme(1, 5);
			var generator = new Data.Generator.Generator();

			var results = generator.DrawCandidates(input, 10);

			Assert.AreEqual(5, results.Count());
			Assert.AreEqual(5, results.Select(s => s.StammId).Count());
		}

		[Test]
		public void Generator_RemoveCandidatesFromList_ValidParameters_ReturnsExpectedResults()
		{
			var input = GenerateStämme(1, 5);
			var toBeRemoved = GenerateStämme(1, 3);
			var expected = GenerateStämme(4, 2);
			var generator = new Data.Generator.Generator();

			var results = generator.RemoveCandidatesFromList(toBeRemoved, input);

			Assert.AreEqual(2, results.Count());
			CollectionAssert.AreEqual(expected.Select(s => s.StammId), results.Select(s => s.StammId));
		}

		[Test]
		public void Generator_RemoveCandidatesFromList_UnexpectedCandidates_ReturnsExpectedResults()
		{
			var input = GenerateStämme(1, 5);
			var toBeRemoved = GenerateStämme(3, 5);
			var expected = GenerateStämme(1, 2);
			var generator = new Data.Generator.Generator();

			var results = generator.RemoveCandidatesFromList(toBeRemoved, input);

			Assert.AreEqual(2, results.Count());
			CollectionAssert.AreEqual(expected.Select(s => s.StammId), results.Select(s => s.StammId));
		}

		[Test]
		public void Generator_GenerateRootParameter_StammfußAnteilEqualsZero_SetsStammfußhöheToZero()
		{
			var input = new GeneratorData { StammfußAnteil = 0 };
			var stämme = GenerateStämme(1, 10);
			var generator = new Data.Generator.Generator();

			generator.GenerateRootParameter(input, stämme);

			Assert.IsTrue(stämme.All(s => s.Stammfußhöhe == 0));
		}

		[TestCase(0, 0)]
		[TestCase(50, 5)]
		[TestCase(80, 8)]
		[TestCase(100, 10)]
		public void Generator_GenerateRootParameter_WithStammfußAnteilEquals_SetsCorrectAmountOfTrunks(int amount, int expected)
		{
			var input = new GeneratorData { StammfußAnteil = amount, StammfußMinHeight = 10, StammfußMaxHeight = 30 };
			var stämme = GenerateStämme(1, 10);
			var generator = new Data.Generator.Generator();

			generator.GenerateRootParameter(input, stämme);

			Assert.AreEqual(expected, stämme.Count(s => s.Stammfußhöhe != 0));
		}

		[TestCase(0, 0)]
		[TestCase(10, 20)]
		[TestCase(50, 60)]
		[TestCase(70, 70)]
		public void Generator_GenerateRootParameter_WithStammfußAnteilAndValidHeightInterval_SetsCorrectValues(int min, int max)
		{
			var input = new GeneratorData { StammfußAnteil = 100, StammfußMinHeight = min, StammfußMaxHeight = max };
			var stämme = GenerateStämme(1, 10);
			var generator = new Data.Generator.Generator();

			generator.GenerateRootParameter(input, stämme);

			Assert.IsTrue(stämme.All(s => s.Stammfußhöhe >= min && s.Stammfußhöhe <= max));
		}
	}
}
