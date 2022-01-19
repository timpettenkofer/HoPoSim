using System;
using System.ServiceModel;

namespace HoPoSim.IPC.WCF
{
	public class Server
	{
		public void Start()
		{
			var host = new ServiceHost(Service.Instance, new Uri("net.tcp://127.0.0.1"));
			
			host.AddServiceEndpoint(typeof(IService),
				Binding.Create(),
				"HoPoSim");
			host.Open();
		}

		public void SendRequest(Message request)
		{
			if (Service.Callback != null)
				Service.Callback.SendCallbackRequest(request);
		}

		// https://stackoverflow.com/questions/1895732/where-to-store-data-for-current-wcf-call-is-threadstatic-safe
		//OperationContext operationContext = OperationContext.Current;
		//operationContext.IncomingMessageProperties.Add("SessionKey", "ABCDEFG");

		//to get the value
		//var ccc = aaa.IncomingMessageProperties["SessionKey"];
	}
}
