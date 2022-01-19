using System;
using System.ServiceModel;

namespace HoPoSim.IPC.WCF
{
	public class Client
	{
		public IService Service { get; set; }
		public Callback Callback { get; set; }

		public void Start()
		{
			Callback = new Callback();
			var context = new InstanceContext(Callback);

			var pipeFactory =
				 new DuplexChannelFactory<IService>(context,
				 Binding.Create(), 
				 new EndpointAddress("net.tcp://127.0.0.1/HoPoSim"));

			Service = pipeFactory.CreateChannel();
			Service.Connect();
		}

		

		public void SendResponse(Message message)
		{
			Service.SendResponse(message);
		}
	}
}
