using HoPoSim.Data.Domain;
using NUnit.Framework;

namespace HoPoSim.Data.Tests.Domain
{
	[TestFixture]
	public class DistributionTests
	{
		[Test]
		public void Distribution_ToPercent_Always_ReturnsExpectedResults()
		{
			var distribution = new Distribution { Total = 50, Absolute = 10 };

			var percent = distribution.ToPercent();

			Assert.AreEqual(20, percent);
		}

		[Test]
		public void Distribution_ToAbsolute_ExactRounding_ReturnsExpectedResults()
		{
			var distribution = new Distribution { Total = 50, Percent = 10 };

			var absolute = distribution.ToAbsolute();

			Assert.AreEqual(5, absolute);
		}

		[Test]
		public void Distribution_ToAbsolute_NotExactRounding_ReturnsExpectedResults()
		{
			var distribution = new Distribution { Total = 100, Percent = 33.3 };

			var absolute = distribution.ToAbsolute();

			Assert.AreEqual(33, absolute);
		}
	}
}
