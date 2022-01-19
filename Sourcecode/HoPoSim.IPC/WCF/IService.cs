using System;
using System.ServiceModel;

namespace HoPoSim.IPC.WCF
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallbackService))]
	public interface IService
	{
		event EventHandler<Message> ClientResponse;

		[OperationContract(IsOneWay = true)]
		void Connect();

		[OperationContract(IsOneWay = true)]
		void SendResponse(Message request);
	}
}
