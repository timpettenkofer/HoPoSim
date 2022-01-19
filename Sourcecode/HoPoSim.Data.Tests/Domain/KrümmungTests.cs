using HoPoSim.Data.Domain;
using NUnit.Framework;

namespace HoPoSim.Data.Tests.Domain
{

	[TestFixture]
	public class KrümmungTests
	{
		[Test]
		public void Krümmung_Constructor_Always_InitializesClasses()
		{
			var krümmung = new Parameter<Krümmung,int>(3);

			Assert.AreEqual(3, krümmung.Values.Count);
			Assert.IsNotNull(krümmung[1]);
			Assert.IsNotNull(krümmung[2]);
			Assert.IsNotNull(krümmung[3]);
		}
	}
}
