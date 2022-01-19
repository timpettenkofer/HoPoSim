using HoPoSim.Data.Domain;
using System.Collections.Generic;

namespace HoPoSim.Data.Tests
{
	public static class GeneratorExtensions
	{
		public static GeneratorData CreateGeneratorData(int dm, int a, int k, int o, int stammanzahl = 100)
		{
			var data = new GeneratorData(dm, a, k, o);
			data.Stammanzahl = stammanzahl;
			return data;
		}

		public static void InitDurchmesser(this GeneratorData data, int klasseId, int minValue, int maxValue, int rindenstärke)
		{
			var k = data.Durchmesser[klasseId];
			k.MinValue = minValue;
			k.MaxValue = maxValue;
			k.Rindenstärke = rindenstärke;
		}

		public static void InitAbholzigkeit(this GeneratorData data, int klasseId, int minValue, int maxValue)
		{
			var k = data.Abholzigkeit[klasseId];
			k.MinValue = minValue;
			k.MaxValue = maxValue;
		}

		public static void InitKrümmung(this GeneratorData data, int klasseId, int minValue, int maxValue)
		{
			var k = data.Krümmung[klasseId];
			k.MinValue = minValue;
			k.MaxValue = maxValue;
		}

		public static void InitOvalität(this GeneratorData data, int klasseId, double minValue, double maxValue)
		{
			var o = data.Ovalität[klasseId];
			o.MinValue = minValue;
			o.MaxValue = maxValue;
		}
	}
}
