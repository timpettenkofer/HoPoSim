using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HoPoSim.Data.Domain
{
	public class SimulationConfiguration : BaseEntity
	{
		public SimulationConfiguration()
		{
			Results = new List<SimulationResults>();
		}

		[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Entity_RequiredFieldFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string Name { get; set; }

		[DataType(DataType.MultilineText)]
		public string Bemerkungen { get; set; }

		public int Iterationanzahl { get; set; }

		[NotMapped]
		public int IterationStart
		{
			get { return Results.Count(); }
		}

		public int TimeOutPeriod { get; set; }

		public int Seed { get; set; }

		public string Data { get; set; }

		public int Quality { get; set; }

		public int FotooptikQuality { get; set; }

		public virtual ICollection<SimulationResults> Results { get; set; }

		[ForeignKey("SimulationDataId")]
		public virtual SimulationData SimulationData { get; set; }
		public int SimulationDataId { get; set; }
	}
}
