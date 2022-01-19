using HoPoSim.Data.Domain;
using HoPoSim.Framework.Serializers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Tests.Domain
{
	[TestFixture]
	public class GeneratorDataTests : TestBase
	{
		[Test]
		public void GeneratorData_Constructor_Always_InitializesDistributions()
		{
			var data = new GeneratorData(3, 2, 1, 1);

			Assert.IsNotNull(data.Distribution);
			var dm = data.Distribution.Children;
			Assert.AreEqual(dm.Count(), 3);
			Assert.IsTrue(dm.All(d => d.Children.Count == 2));
			var a = dm.SelectMany(d => d.Children);
			Assert.AreEqual(a.Count(), 6);
			Assert.IsTrue(a.All(d => d.Children.Count == 1));
			var k = a.SelectMany(d => d.Children);
			Assert.AreEqual(k.Count(), 6);
			var o = k.SelectMany(d => d.Children);
			Assert.AreEqual(o.Count(), 6);
		}

		[Test]
		public void GeneratorData_Constructor_Always_AssignsValidRangeId()
		{
			var data = new GeneratorData(4, 3, 2, 1);

			var dm = data.Distribution.Children;
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, dm.Select(d => d.RangeId));

			foreach (var dk in dm)
				CollectionAssert.AreEqual(new[] { 1, 2, 3 }, dk.Children.Select(d => d.RangeId));

			foreach(var ak in dm.SelectMany(d => d.Children))
				CollectionAssert.AreEqual(new[] { 1, 2 }, ak.Children.Select(d => d.RangeId));

			foreach (var o in dm.SelectMany(d => d.Children).SelectMany(d => d.Children))
				CollectionAssert.AreEqual(new[] { 1 }, o.Children.Select(d => d.RangeId));
		}

		[Test]
		public void HasUninitializedQuotas_EmptyDistributionTree_ReturnsTrue()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);

			var result = data.HasUninitializedQuotas(data.Distribution);

			Assert.IsTrue(result);
		}


		[Test]
		public void HasUninitializedQuotas_WithDistributionValues_ReturnsFalse()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_invalid_durchmesser.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var result = data.HasUninitializedQuotas(data.Distribution);

			Assert.IsFalse(result);
		}

		[Test]
		public void HasValidQuotas_NotMatchingDurchmesserQuota_ReturnsFalse()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_invalid_durchmesser.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var result = data.HasValidQuotas();

			Assert.IsFalse(result);
		}

		[Test]
		public void HasValidQuotas_NotMatchingAbholzigkeitQuota_ReturnsFalse()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_invalid_abholzigkeit.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var result = data.HasValidQuotas();

			Assert.IsFalse(result);
		}

		[Test]
		public void HasValidQuotas_NotMatchingKrümmungQuota_ReturnsFalse()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_invalid_krümmung.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var result = data.HasValidQuotas();

			Assert.IsFalse(result);
		}

		[Test]
		public void HasValidQuotas_NotMatchingOvalitätQuota_ReturnsFalse()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 2);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_invalid_ovalität.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);

			var result = data.HasValidQuotas();

			Assert.IsFalse(result);
		}

		[Test]
		public void HasValidQuotas_AllMatchingQuotas_ReturnsTrue()
		{
			var data = GeneratorExtensions.CreateGeneratorData(2, 2, 1, 1);
			var json = LoadStringFromFile("RefFiles\\Generator\\distribution_valid.json");
			data.Distribution = Serializer<Distribution>.FromJSON(json);
			
			var result = data.HasValidQuotas();

			Assert.IsTrue(result);
		}
	}
}
