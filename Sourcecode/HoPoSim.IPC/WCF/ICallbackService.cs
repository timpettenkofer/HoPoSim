using System.ServiceModel;

namespace HoPoSim.IPC.WCF
{
	public interface ICallbackService
	{
		[OperationContract(IsOneWay = true)]
		void SendCallbackRequest(Message request);
	}
}
