using Newtonsoft.Json;

namespace HoPoSim.IPC.DAO
{
	public class SimulationConfiguration
	{
		[JsonProperty(Required = Required.Always)]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Comments { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int Iterations { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int IterationStart { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int TimeOutPeriod { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int Seed { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int Quality { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int FotooptikQuality { get; set; }
	}
}
