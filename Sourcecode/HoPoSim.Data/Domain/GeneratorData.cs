using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace HoPoSim.Data.Domain
{
	public class GeneratorData : BaseEntity
	{
		public GeneratorData()
		{ }

		public GeneratorData(int numDurchmesser, int numAbholzigkeit, int numKrümmung, int numOvalität)
		{
			Durchmesser = new Parameter<Durchmesser,int>(numDurchmesser);
			Abholzigkeit = new Parameter<Abholzigkeit,int>(numAbholzigkeit);
			Krümmung = new Parameter<Krümmung,int>(numKrümmung);
			Ovalität = new Parameter<Ovalität,double>(numOvalität);

			var root = new Distribution() { RangeId = -1,  Percent = 100.0 };
			InitializeDistributionTree(root, new []{ numDurchmesser, numAbholzigkeit, numKrümmung, numOvalität});
			Distribution = root;
		}

		internal static void InitializeDistributionTree(Distribution node, IEnumerable<int> numberOfNodesPerSubLevel)
		{
			var numberOfChildren = numberOfNodesPerSubLevel.FirstOrDefault();
			if (numberOfChildren > 0)
			{
				var children = Enumerable.Range(1, numberOfChildren)
					.Select(i => new Distribution() { RangeId = i })
					.ToList();

				node.Children = children;
				children.ForEach(c => InitializeDistributionTree(c, numberOfNodesPerSubLevel.Skip(1)));
			}
			
		}

		[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Entity_RequiredFieldFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string Name { get; set; }

		public int Stammanzahl { get; set; }

		public float Länge { get; set; }

		public float LängeVariation { get; set; }

		public int StammfußAnteil { get; set; }

		public int StammfußMinHeight { get; set; }

		public int StammfußMaxHeight { get; set; }

		[DataType(DataType.MultilineText)]
		public string Bemerkungen { get; set; }

		public virtual Parameter<Durchmesser,int> Durchmesser { get; set; }
		public virtual Parameter<Abholzigkeit,int> Abholzigkeit { get; set; }
		public virtual Parameter<Krümmung,int> Krümmung { get; set; }
		public virtual Parameter<Ovalität,double> Ovalität { get; set; }

		public virtual Distribution Distribution { get; set; }

		public bool HasValidQuotas()
		{
			return HasValidQuotas(Distribution);
		}

		public bool HasValidQuotas(Distribution root, bool directChildrenOnly = false)
		{
			return root == null || HasValidQuotas(root.Children, root.Absolute, directChildrenOnly);
		}

		private bool HasValidQuotas(ICollection<Distribution> nodes, int total, bool directChildrenOnly = false)
		{
			if (nodes.Count == 0)
				return true;
			var sum = nodes.Sum(d => d.Absolute);
			if (sum != total)
				return false;
			return  directChildrenOnly?
				true :
				nodes.All(n => HasValidQuotas(n.Children, n.Absolute));
		}

		public bool HasUninitializedQuotas(Distribution root, bool directChildrenOnly = false)
		{
			return root != null && HasUninitializedQuotas(root.Children, root.Absolute, directChildrenOnly);
		}

		private bool HasUninitializedQuotas(ICollection<Distribution> nodes, int total, bool directChildrenOnly = false)
		{
			if (nodes.Count == 0)
				return true;
			var allUntouched = nodes.All(d => d.Absolute == 0);
			if (!allUntouched)
				return false;
			return directChildrenOnly ?
				true :
				nodes.All(n => HasUninitializedQuotas(n.Children, n.Absolute));
		}


		public GeneratorData Clone()
		{
			var data = new GeneratorData()
			{
				Name = Name,
				Länge = Länge,
				LängeVariation = LängeVariation,
				Bemerkungen = Bemerkungen,
				Durchmesser = new Parameter<Durchmesser,int>(Durchmesser),
				Abholzigkeit = new Parameter<Abholzigkeit,int>(Abholzigkeit),
				Krümmung = new Parameter<Krümmung,int>(Krümmung),
				Ovalität = new Parameter<Ovalität,double>(Ovalität),
				Distribution = new Distribution(Distribution)
			};
			data.Stammanzahl = Stammanzahl;
			return data;
		}
	}
}
