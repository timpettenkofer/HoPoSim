using HoPoSim.Data.Domain;
using NUnit.Framework;

namespace HoPoSim.Data.Tests.Domain
{

	[TestFixture]
	public class AbholzigkeitTests
	{
		[Test]
		public void Abholzigkeit_Constructor_Always_InitializesClasses()
		{
			var abholzigkeit = new Parameter<Abholzigkeit, int>(3);

			Assert.AreEqual(3, abholzigkeit.Values.Count);
			Assert.IsNotNull(abholzigkeit[1]);
			Assert.IsNotNull(abholzigkeit[2]);
			Assert.IsNotNull(abholzigkeit[3]);
		}
	}
}
