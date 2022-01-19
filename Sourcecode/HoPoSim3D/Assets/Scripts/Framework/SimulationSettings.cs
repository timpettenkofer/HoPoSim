using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;

namespace Assets
{
	public class SimulationSettings
	{
		public static SimulationSettings CreateSimulationSettings(Message msg)
		{
			var configuration = Serializer<SimulationConfiguration>.FromJSON(msg.Configuration);

			var settings = new SimulationSettings
			{
				SimulationConfigurationId = configuration.Id,
				Seed = configuration.Seed,
				TimeOutPeriod = configuration.TimeOutPeriod,
				Quality = configuration.Quality,
				FotooptikQuality = configuration.FotooptikQuality,
				IterationStart = configuration.IterationStart + 1,
				IterationEnd = configuration.Iterations + 1,
				SendCommandFinishedMessage = true
			};
			return settings;
		}

		public static SimulationSettings CreateVisualizationSettings(Message msg)
		{
			var results = Serializer<SimulationResults>.FromJSON(msg.Configuration);
			var settings = new SimulationSettings
			{
				Seed = results.SimulationSeed,
				TimeOutPeriod = 3600,
				Quality = results.SimulationQuality,
				FotooptikQuality = results.FotooptikQuality,
				IterationStart = results.IterationId,
				IterationEnd = results.IterationId + 1,
				SnapshotData = results.SimulationSnapshot,
				Modeler = results.Modeler,
				Strategy = typeof(RestoreSnapshot).FullName,
				Processor = typeof(Visualizer).FullName
			};
			return settings;
		}

		public static SimulationSettings CreateExport3dSettings(Message msg)
		{
			var settings = CreateVisualizationSettings(msg);
			settings.Settings = msg.Settings;
			settings.Processor = typeof(Exporter3d).FullName;
			return settings;
		}

		public static SimulationSettings CreateExportImagesSettings(Message msg)
		{
			var settings = CreateVisualizationSettings(msg);
			settings.Settings = msg.Settings;
			settings.Processor = typeof(ExporterImg).FullName;
			return settings;
		}

		public bool IsComplete {  get { return !(IterationStart < IterationEnd); } }

		public int SimulationConfigurationId { get; set; }
		public int Seed { get; set; }
		public int TimeOutPeriod { get; set; }
		public int Quality { get; set; }
		public int FotooptikQuality { get; set; }
		public int IterationStart { get; set; }
		public int IterationEnd { get; set; }
		public bool SendCommandFinishedMessage { get; set; }
		public string Settings { get; set; }
		public string SnapshotData { get; set; }
		public string Modeler { get; set; }
		public string Strategy { get; set; }
		public string Processor { get; set; }
	}
}
