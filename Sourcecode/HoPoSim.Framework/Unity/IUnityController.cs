using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HoPoSim.IPC.WCF;

namespace HoPoSim.Framework.Unity
{
	public interface IUnityController
	{
		void SendMessage(Message request);

		event EventHandler<Message> SimulationResultsReceived;

		event EventHandler<Message> SimulationProcessFinished;

		event EventHandler<Message> SimulationLogReceived;

		event EventHandler ProcessClosedByUser;

		bool HasCurrentlyActiveSharedProcess { get; }

		Task<Process> StartSharedProcess(ProcessWindowStyle windowStyle);

		void CloseSharedProcess();
		//Task<Process> StartProcess(ProcessWindowStyle windowStyle);

		//void CloseProcess(int processId);
	}
}
