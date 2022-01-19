using System;

namespace HoPoSim.IPC.WCF
{
	[Serializable]
	public class Message
	{
		public enum CommandCode
		{
			SIMULATION = 0,
			VISUALIZATION,
			EXPORT_3D,
			EXPORT_IMG
		}

		public enum StatusCode
		{
			ERROR = -1,
			SUCCESS = 0,
			BAD_REQUEST = 1,
			ITERATION_RESULTS = 2,
			LOG = 3
		}

		public CommandCode Command { get; set; }
		public string Configuration { get; set; }
		public string Data { get; set; }
		public string Settings { get; set; }
		public StatusCode Code { get; set; }
		public int ProcessId { get; set; }
	}
}
