using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace HoPoSim.IPC.WCF
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class Service : IService
	{
		public event EventHandler<Message> ClientResponse;

		public void Connect()
		{
			Callback = OperationContext.Current.GetCallbackChannel<ICallbackService>();
		}

		public static void Disconnect()
		{
			Callback = null;
		}

		public static ICallbackService Callback { get; set; }

		public static IService Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Service();
				return _instance;
			}
		}
		private static IService _instance = null;

		public static void SendMessage(Message request)
		{
			if (Callback != null)
				Callback.SendCallbackRequest(request);
		}

		public static async Task<bool> WaitForCallback(double timeoutInSeconds = 15.0)
		{
			if (Callback != null)
				return true;
			return await Task<bool>.Run(() =>
			System.Threading.SpinWait.SpinUntil(() => Callback != null, TimeSpan.FromSeconds(timeoutInSeconds)));

		}

		public void SendResponse(Message request)
		{
			ClientResponse?.Invoke(this, request);
		}
	}
}
