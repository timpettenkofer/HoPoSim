using HoPoSim.Data.Generator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Tests.Generator
{
	[TestFixture]
	public class RandomExtensionsTests 
	{
		[Test]
		public void NextDouble_MinValueGreaterThanMaxValue_ThrowsArgumentException()
		{
			var rnd = new Random();

			var exception = Assert.Throws<ArgumentException>(() => rnd.NextInt(100, 50));

			Assert.AreEqual("minValue must be less than maxValue", exception.Message);
		}

		[TestCase(50, 100)]
		[TestCase(50, 50)]
		[TestCase(1, 2)]
		[TestCase(0, 1)]
		public void NextDouble_WithValidBounds_ReturnsValueInRange(int min, int max)
		{
			var rnd = new Random();

			var result = rnd.NextInt(min, max);

			Assert.GreaterOrEqual(result, min);
			Assert.LessOrEqual(result, max);
		}

		[Test]
		public void NextDouble_WithValidBounds_ReturnsDifferentValuesForEachCall()
		{
			var rnd = new Random();
			var min = 50;
			var max = 100;

			List<int> results = new List<int>
			{
				rnd.NextInt(min, max),
				rnd.NextInt(min, max),
				rnd.NextInt(min, max)
			};

			Assert.AreNotEqual(1, results.Distinct().Count());
		}

		[Test]
		public void TakeRandom_WithValidArguments_ReturnsExpectedNumberOfElements()
		{
			var rnd = new Random();
			var input = new List<int> { 0, 1, 2, 3, 4, 5 };

			var result = input.TakeRandom(rnd, 3);

			Assert.AreEqual(3, result.Count());
		}

		[Test]
		public void TakeRandom_WithValidArguments_ReturnsDifferentValuesForDifferentCalls()
		{
			var rnd = new Random();
			var input = Enumerable.Range(0, 200);

			var result1 = input.TakeRandom(rnd, 3);
			var result2 = input.TakeRandom(rnd, 3);

			CollectionAssert.AreNotEquivalent(result1, result2);
		}

		[Test]
		public void TakeRandom_WithNegativeCount_ThrowsArgumentException()
		{
			var rnd = new Random();
			var input = new List<int> { 0, 1, 2, 3, 4, 5 };

			var exception = Assert.Throws<ArgumentException>(() => input.TakeRandom(rnd, -1));

			Assert.AreEqual("count must be positive", exception.Message);
		}

		[Test]
		public void Shuffle_WithValidArguments_ReturnsExpectedResults()
		{
			var rnd = new Random();
			var input = Enumerable.Range(0, 10);

			var shuffle1 = input.Shuffle(rnd);
			var shuffle2 = input.Shuffle(rnd);

			CollectionAssert.AreEquivalent(shuffle1, shuffle2);
			CollectionAssert.AreNotEqual(shuffle1, shuffle2);
		}

		[Test]
		public void Shuffle_WithEmptyArgumentList_ReturnsEmptyList()
		{
			var rnd = new Random();
			var input = new List<int>();

			var shuffle = input.Shuffle(rnd);

			CollectionAssert.IsEmpty(shuffle);
		}
	}
}
