using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoPoSim.Data.Domain
{
	public enum IterationResult
	{
		[Description("OK")]
		Success,
		[Description("Fehler")]
		Error,
		[Description("Fehler (Konfiguration)")]
		BadConfiguration,
		[Description("Timeout")]
		Timeout
	};

	public class SimulationResults : BaseEntity
	{
		public int IterationId { get; set; }

		public int Quality { get; set; }

		public int FotooptikQuality { get; set; }

		public IterationResult IterationStatus { get; set; }

		public double StirnflächeV { get; set; }

		public double StirnflächeH { get; set; }

		public double FotooptikV { get; set; }

		public double FotooptikH { get; set; }

		public double Fotooptik { get; set; }

		public double FotooptikStützpunkteV { get; set; }

		public double FotooptikStützpunkteH { get; set; }

		public double PolygonzugV { get; set; }

		public double PolygonzugH { get; set; }

		public double Polygonzug { get; set; }

		public double SektionV { get; set; }

		public double SektionH { get; set; }

		public double Sektion { get; set; }

		public double PoltervolumeMR { get; set; }

		public double PoltervolumeOR { get; set; }

		public double PolterunterlagevolumeMR { get; set; }

		public double PolterunterlagevolumeOR { get; set; }

		public double Rindenanteil { get; set; }

		public double UFSektionOR { get; set; }

		public double UFSektionMR { get; set; }

		public double UFPolygonzugOR { get; set; }

		public double UFPolygonzugMR { get; set; }

		public double UFFotooptikOR { get; set; }

		public double UFFotooptikMR { get; set; }

		public double Höhe { get; set; }

		public double Breite { get; set; }


		public string Modeler { get; set; }

		public string Strategy { get; set; }

		public string Processor { get; set; }


		[ForeignKey("SimulationConfigurationId")]
		public virtual SimulationConfiguration SimulationConfiguration { get; set; }
		public int SimulationConfigurationId { get; set; }

		public string SimulationSnapshot { get; set; }
	}
}
