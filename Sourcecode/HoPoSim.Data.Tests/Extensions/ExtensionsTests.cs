using HoPoSim.Data.Extensions;
using HoPoSim.Data.Interfaces;
using NUnit.Framework;
using Ext = HoPoSim.Data.Extensions.Extensions;

namespace HoPoSim.Data.Tests.Extensions
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		public void AnteilAbsolute_Always_ReturnsExpectedResult()
		{
			Assert.AreEqual(50.0, Ext.AnteilAbsolute(50, 100));
			Assert.AreEqual(33.0, Ext.AnteilAbsolute(33.3, 100));
			Assert.AreEqual(26, Ext.AnteilAbsolute(25.8, 100));
			Assert.AreEqual(0.0, Ext.AnteilAbsolute(0, 99));
			Assert.AreEqual(99, Ext.AnteilAbsolute(100, 99));
		}

		[Test]
		public void AnteilPercent_Always_ReturnsExpectedResult()
		{
			Assert.AreEqual(50.0, Ext.AnteilPercent(50, 100));
			Assert.AreEqual(33.0, Ext.AnteilPercent(33, 100));
			Assert.AreEqual(2.5, Ext.AnteilPercent(2.5, 100));
			Assert.AreEqual(0.0, Ext.AnteilPercent(0, 99));
			Assert.AreEqual(100, Ext.AnteilPercent(100, 100));
			Assert.AreEqual(0, Ext.AnteilPercent(10, 0));
		}

		private class HaveAnteil : IHaveAnteilPercentProperty
		{
			public HaveAnteil(double anteil)
			{
				AnteilPercent = anteil;
			}
			public double AnteilPercent { get; set; }
		}

		[Test]
		public void GetStammAbsoluteAnteil_Always_ReturnsExpectedResult()
		{
			Assert.AreEqual(50, new HaveAnteil(50.0).GetStammAbsoluteAnteil(100));
			Assert.AreEqual(33, new HaveAnteil(33.333).GetStammAbsoluteAnteil(100));
			Assert.AreEqual(0, new HaveAnteil(0.0).GetStammAbsoluteAnteil(10));
			Assert.AreEqual(0, new HaveAnteil(10.0).GetStammAbsoluteAnteil(0));
		}
	}
}
