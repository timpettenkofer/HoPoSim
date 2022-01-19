using HoPoSim.Data.Domain;
using NUnit.Framework;

namespace HoPoSim.Data.Tests.Domain
{
	[TestFixture]
	public class DurchmesserTests
	{
		[Test]
		public void Durchmesser_Constructor_Always_InitializesClasses()
		{
			var durchmesser = new Parameter<Durchmesser, int>(3);

			Assert.AreEqual(3, durchmesser.Values.Count);
			Assert.IsNotNull(durchmesser[1]);
			Assert.IsNotNull(durchmesser[2]);
			Assert.IsNotNull(durchmesser[3]);
		}
	}
}
