using System;

namespace HoPoSim.IPC.WCF
{
	public class Callback : ICallbackService
	{
		public event EventHandler<Message> ServerRequest;

		public void SendCallbackRequest(Message request)
		{
			if (ServerRequest != null)
				ServerRequest(this, request);
		}
	}
}
