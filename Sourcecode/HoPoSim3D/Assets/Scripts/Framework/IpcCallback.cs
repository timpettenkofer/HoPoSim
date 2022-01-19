using HoPoSim.IPC.WCF;
using System;
using static HoPoSim.IPC.WCF.Message;

namespace Assets.IPC
{
	public class IpcCallback
	{
		public IpcCallback(Client ipc, int processId, CommandCode command)
		{
			IPC = ipc;
			ProcessId = processId;
			Command = command;
		}

		private Client IPC { get; }
		private int ProcessId { get; }
		private CommandCode Command { get; }


		public void Log(string message)
		{
			InternalLog(message);
		}

		public void Log(string message, Exception e)
		{
			InternalLog($"{message}: {e.GetBaseException()}");
		}

		private void InternalLog(string message)
		{
			var msg = new Message()
			{
				Command = Command,
				Code = StatusCode.LOG,
				Data = message
			};
			Send(msg);
		}

		public void Send(Message msg)
		{
			msg.ProcessId = ProcessId;
			IPC.SendResponse(msg);
		}
	}
}
