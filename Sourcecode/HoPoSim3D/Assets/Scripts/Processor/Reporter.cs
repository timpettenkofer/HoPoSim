using Assets.IPC;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using static HoPoSim.IPC.WCF.Message;

namespace Assets
{
	public static class Reporter
	{
		public static void SendSimulationResults(SimulationResults results, IpcCallback callback)
		{
			var msg = Serializer<SimulationResults>.ToJSON(results, false);
			var message = new Message { Data = msg, Code = StatusCode.ITERATION_RESULTS, Command = Message.CommandCode.SIMULATION };
			callback.Send(message);
		}

		public static void SendCommandStatus(CommandCode cmd, StatusCode code, string msg, IpcCallback callback)
		{
			callback.Log(msg);
			var message = new Message { Command = cmd, Code = code, Data = msg };
			callback.Send(message);
		}
	}
}
