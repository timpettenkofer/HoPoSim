using Newtonsoft.Json;

namespace HoPoSim.IPC.DAO
{
	public enum ExportFormat
	{
		FBX,
		OBJ,
		PNG
	}

	public class ExportSettings
	{
		[JsonProperty(Required = Required.Always)]
		public string Path { get; set; }

		[JsonProperty(Required = Required.Always)]
		public ExportFormat Format { get; set; }
	}

	public class ImageExportSettings : ExportSettings
	{
		[JsonProperty(Required = Required.Always)]
		public int Width { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int Height { get; set; }
	}
}
