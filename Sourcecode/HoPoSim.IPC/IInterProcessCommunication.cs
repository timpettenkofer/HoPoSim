using HoPoSim.IPC.WCF;

namespace HoPoSim.IPC
{
	public interface IInterProcessCommunication
	{
		void StartServer();
		void StartClient();

		Server Server { get; set; }
		Client Client { get; set; }
	}
}
