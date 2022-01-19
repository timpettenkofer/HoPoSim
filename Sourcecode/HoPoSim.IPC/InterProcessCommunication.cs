using HoPoSim.IPC.WCF;
using System.ComponentModel.Composition;

namespace HoPoSim.IPC
{
	[Export(typeof(IInterProcessCommunication))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	internal class InterProcessCommunication : IInterProcessCommunication
	{
		public Server Server { get; set; }
		public Client Client { get; set; }

		public InterProcessCommunication()
		{
		}

		public void StartServer()
		{
			if (Server == null)
			{
				Server = new Server();
				Server.Start();
			}
		}

		public void StartClient()
		{
			if (Client == null)
			{
				Client = new Client();
				Client.Start();
				//Client.Callback.ServerRequest += Callback_ServerRequest;
			}
		}

		//private void Callback_ServerRequest(object sender, Request request)
		//{
		//	Console.WriteLine($"IPC Get response from client: {request.Command}{request.Response}");
		//}
	}
}
