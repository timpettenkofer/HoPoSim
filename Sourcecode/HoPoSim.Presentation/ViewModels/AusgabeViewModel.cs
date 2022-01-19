using HoPoSim.Framework.Unity;
using HoPoSim.IPC;
using HoPoSim.IPC.WCF;
using Prism.Mvvm;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace HoPoSim.Presentation.ViewModels
{
	// TODO remove
	[Export(typeof(AusgabeViewModel))]
	public class AusgabeViewModel : BindableBase
	{
		[ImportingConstructor]
		public AusgabeViewModel(IInterProcessCommunication ipc)
		{
			//IPC = ipc;
			//IPC.StartServer();
		}
		private IInterProcessCommunication IPC { get; }

		public Process StartUnityProcess(IntPtr windowHandle)
		{
			//var process = UnityIntegration.StartUnityProcess(windowHandle);
			//Service.Instance.ClientResponse += Service_ClientResponse;
			//return process;
			return null;
		}

		public void SendMessage(string msg)
		{
			if (Service.Callback != null)
				Service.Callback.SendCallbackRequest(new Message { Data = msg, Command = Message.CommandCode.EXPORT_3D });
		}
	}
}

