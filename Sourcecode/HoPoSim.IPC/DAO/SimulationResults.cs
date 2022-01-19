using Newtonsoft.Json;

namespace HoPoSim.IPC.DAO
{
	public enum IterationResult
	{
		Success,
		Error,
		BadConfiguration,
		Timeout
	};

	public class SimulationResults
	{
		[JsonProperty(Required = Required.Always)]
		public int SimulationConfigurationId { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int SimulationSeed { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int SimulationQuality { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int FotooptikQuality { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string SimulationSnapshot { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int IterationId { get; set; }

		[JsonProperty(Required = Required.Always)]
		public IterationResult IterationStatus { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double StirnflächeV { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double StirnflächeH { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double FotooptikV { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double FotooptikH { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Fotooptik { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double FotooptikStützpunkteV { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double FotooptikStützpunkteH { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PolygonzugV { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PolygonzugH { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Polygonzug { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double SektionV { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double SektionH { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Sektion { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PoltervolumeMR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PoltervolumeOR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PolterunterlagevolumeMR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double PolterunterlagevolumeOR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Rindenanteil { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFSektionMR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFSektionOR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFPolygonzugMR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFPolygonzugOR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFFotooptikMR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double UFFotooptikOR { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Höhe { get; set; }

		[JsonProperty(Required = Required.Always)]
		public double Breite { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Modeler { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Strategy { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Processor { get; set; }
	}
}
