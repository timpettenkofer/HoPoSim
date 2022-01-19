using HoPoSim.Data.Model;
using NUnit.Framework;

namespace HoPoSim.Data.Tests.Model
{
	[TestFixture]
	public class StammdatenTests : TestBase
	{
		[Test]
		public void Add_Always_AddsNewItemToDataTable()
		{
			var stammdaten = GetTestStammdatenTable;
			var stamm = new Stamm("test");
			stamm.Rindenstärke = 1;
			stamm.Länge = 6.0f;
			var count = stammdaten.Stammanzahl;

			stammdaten.Add(stamm);

			Assert.AreEqual(count + 1, stammdaten.Stammanzahl);
		}
	}
}
