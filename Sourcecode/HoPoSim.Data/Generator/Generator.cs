using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using HoPoSim.Data.Extensions;

namespace HoPoSim.Data.Generator
{
	public interface IGenerator
	{
		void Generate(GeneratorData input, Stammdaten target);
	}

	[Export(typeof(IGenerator))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class Generator : IGenerator
	{
		public Generator()
		{
			Random = new Random();
		}

		internal void SetSeed(int seed)
		{
			Random = new Random(seed);
		}

		public void Generate(GeneratorData input, Stammdaten data)
		{
			if (input.HasValidQuotas() && input.Stammanzahl > 0)
				InternalGenerate(input, data);
		}

		private void InternalGenerate(GeneratorData input, Stammdaten data)
		{
			IList<Stamm> stämme = new List<Stamm>();
			CreateTrunksWithId(input, stämme);

			GenerateTrunkLengths(input, stämme);

			GenerateRootParameter(input, stämme);

			IList<Stamm> pool = stämme.ToList();
			GenerateCategoryParameters(input, pool);

			AddToResults(stämme, data);
		}

		internal static void CreateTrunksWithId(GeneratorData input, IList<Stamm> stämme)
		{
			for (int i = 0; i < input.Stammanzahl; ++i)
			{
				var stamm = new Stamm((i + 1).ToString());
				stämme.Add(stamm);
			}
		}

		internal void GenerateTrunkLengths(GeneratorData input, IEnumerable<Stamm> trunks)
		{
			var maxVariation = Convert.ToInt32(input.LängeVariation * 100);
			foreach (var stamm in trunks)
			{
				var variationInPercent = Random.Next(-maxVariation, +maxVariation) / 10000.0f;
				stamm.Länge = Math.Round(input.Länge * ( 1 + variationInPercent), 2, MidpointRounding.AwayFromZero);
			}
		}

		internal void GenerateRootParameter(GeneratorData input, IList<Stamm> stämme)
		{
			if (input.StammfußAnteil > 0)
			{
				var numTrunksWithRoots = Convert.ToInt32(Extensions.Extensions.AnteilAbsolute(input.StammfußAnteil, stämme.Count()));
				var trunksWithRoots = stämme.Shuffle(Random).Take(numTrunksWithRoots);
				var minHeight = input.StammfußMinHeight;
				var maxHeight = input.StammfußMaxHeight;
				foreach(var trunk in trunksWithRoots)
				{
					var rootHeight = Random.NextInt(minHeight, maxHeight);
					trunk.Stammfußhöhe = rootHeight;
				}
			}
		}

		private static void AddToResults(IList<Stamm> stämme, Stammdaten data)
		{
			data.Clear();
			foreach (var stamm in stämme)
				data.Add(stamm);
		}

		private IList<Stamm> GenerateCategoryParameters(GeneratorData input, IList<Stamm> pool)
		{
			foreach (Durchmesser dm in input.Durchmesser.Values.Shuffle(Random))
			{
				int stammAnteil = GetAbsoluteRatio(input.Distribution, dm);
				var candidates = DrawCandidates(pool, stammAnteil);
				HandleAbholzigkeit(input, candidates, dm);
				pool = RemoveCandidatesFromList(candidates, pool);
			}
			return pool;
		}

		
		private void HandleAbholzigkeit(GeneratorData input, IList<Stamm> pool, Durchmesser dm)
		{
			foreach (Abholzigkeit a in input.Abholzigkeit.Values.Shuffle(Random))
			{
				int stammAnteil = GetAbsoluteRatio(input.Distribution, dm, a);
				var candidates = DrawCandidates(pool, stammAnteil);
				HandleKrümmung(input, candidates, dm, a);
				pool = RemoveCandidatesFromList(candidates, pool);
			}
		}

		private IList<Stamm> HandleKrümmung(GeneratorData input, IList<Stamm> pool, Durchmesser dm, Abholzigkeit a)
		{
			foreach (Krümmung k in input.Krümmung.Values.Shuffle(Random))
			{
				int stammAnteil = GetAbsoluteRatio(input.Distribution, dm, a, k);
				var candidates = DrawCandidates(pool, stammAnteil);
				HandleOvalität(input, candidates, dm, a, k);
				pool = RemoveCandidatesFromList(candidates, pool);
			}
			return pool;
		}

		private IList<Stamm> HandleOvalität(GeneratorData input, IList<Stamm> pool, Durchmesser dm, Abholzigkeit a, Krümmung k)
		{
			foreach (Ovalität o in input.Ovalität.Values.Shuffle(Random))
			{
				int stammAnteil = GetAbsoluteRatio(input.Distribution, dm, a, k, o);
				var candidates = DrawCandidates(pool, stammAnteil);
				GenerateParameters(candidates, dm, a, k, o);
				pool = RemoveCandidatesFromList(candidates, pool);
			}
			return pool;
		}

		private int GetAbsoluteRatio(Distribution root, Durchmesser dm)
		{
			return root
				.Children[dm.RangeId - 1]
				.Absolute;
		}

		private int GetAbsoluteRatio(Distribution root, Durchmesser dm, Abholzigkeit a)
		{
			return root
				.Children[dm.RangeId - 1]
				.Children[a.RangeId - 1]
				.Absolute;
		}

		private int GetAbsoluteRatio(Distribution root, Durchmesser dm, Abholzigkeit a, Krümmung k)
		{
			return root
				.Children[dm.RangeId - 1]
				.Children[a.RangeId - 1]
				.Children[k.RangeId - 1]
				.Absolute;
		}

		private int GetAbsoluteRatio(Distribution root, Durchmesser dm, Abholzigkeit a, Krümmung k, Ovalität o)
		{
			return root
				.Children[dm.RangeId - 1]
				.Children[a.RangeId - 1]
				.Children[k.RangeId - 1]
				.Children[o.RangeId - 1]
				.Absolute;
		}


		internal void GenerateParameters(IEnumerable<Stamm> stämme, Durchmesser dm, Abholzigkeit a, Krümmung k, Ovalität o)
		{
			foreach (var stamm in stämme)
			{
				stamm.D_Mitte_mR = stamm.D_Mitte_oR = Random.NextInt(dm.MinValue, dm.MaxValue);
				stamm.Abholzigkeit = Random.NextInt(a.MinValue, a.MaxValue);
				var abholzigkeit = stamm.Länge * stamm.Abholzigkeit * 0.5;
				stamm.D_Stirn_mR = stamm.D_Stirn_oR = Convert.ToInt32(Math.Round((double)stamm.D_Mitte_mR + abholzigkeit));
				stamm.D_Zopf_mR = stamm.D_Zopf_oR = Convert.ToInt32(Math.Round((double)stamm.D_Mitte_mR - abholzigkeit));

				var rindenstärke = dm.Rindenstärke;
				stamm.Rindenstärke = rindenstärke;
				stamm.D_Mitte_oR -= 2 * rindenstärke;
				stamm.D_Stirn_oR -= 2 * rindenstärke;
				stamm.D_Zopf_oR -= 2 * rindenstärke;

				stamm.Krümmung = Random.NextInt(k.MinValue, k.MaxValue);
				stamm.Ovalität = Random.NextDouble(o.MinValue, o.MaxValue);
			}
		}

		internal IList<Stamm> DrawCandidates(IList<Stamm> pool, int count)
		{
			return pool.TakeRandom(Random, count).ToList();
		}

		internal IList<Stamm> RemoveCandidatesFromList(IList<Stamm> candidates, IList<Stamm> pool)
		{
			return pool.Except(candidates, new StammIdComparer()).ToList();
		}

		private Random Random
		{
			get;
			set;
		}
	}
}
