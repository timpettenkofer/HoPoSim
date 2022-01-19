using HoPoSim.Framework.Interfaces;
using HoPoSim.IPC;
using HoPoSim.IPC.WCF;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HoPoSim.Framework.Unity
{
	[Export(typeof(IUnityController))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class UnityController : IUnityController
	{
		[ImportingConstructor]
		public UnityController(IInterProcessCommunication ipc, IGlobalConfigService config)
		{
			Config = config;
			IPC = ipc;
			IPC.StartServer();
			Service.Instance.ClientResponse += Service_ClientResponse;
		}
		private IInterProcessCommunication IPC { get; }
		private IGlobalConfigService Config { get; }
		private Process Process { get; set; }

		public event EventHandler<Message> SimulationResultsReceived;

		public event EventHandler<Message> SimulationLogReceived;

		public event EventHandler<Message> SimulationProcessFinished;

		public event EventHandler ProcessClosedByUser;


		public bool HasCurrentlyActiveSharedProcess
		{
			get
			{
				return Process != null && !Process.HasExited;
			}
		}

		public async Task<Process> StartSharedProcess(ProcessWindowStyle windowStyle)
		{
			try
			{
				if (Process != null)
				{
					CloseSharedProcess();
				}
				Process = StartUnityProcess(windowStyle);
				Process.Exited += Process_Exited;

				bool result = await Service.WaitForCallback();
				return result? Process : null;
			}
			catch
			{
				return null;
			}
		}

#if DEBUG
		public void CloseSharedProcess()
		{
			TearDownSharedProcess();
		}
#else
		public void CloseSharedProcess()
		{
			Process.Kill();
			TearDownSharedProcess();
		}
#endif

		private void TearDownSharedProcess()
		{
			if (Process != null)
			{
				Process.Exited -= Process_Exited;
				Process = null;
				Service.Disconnect();
			}
		}


		private void Process_Exited(object sender, EventArgs e)
		{
			ProcessClosedByUser?.Invoke(this, e);
			TearDownSharedProcess();
		}

		private Process StartUnityProcess(ProcessWindowStyle windowStyle)
		{
			var process = new Process();
			process.StartInfo.FileName = Config.Simulator3dExeFile; // @".\Unity\HoPoSim3D.exe";
																	//process.StartInfo.Arguments = "-parentHWND " + windowHandle.ToInt32() + " " + Environment.CommandLine;
			process.StartInfo.WindowStyle = windowStyle;
			process.StartInfo.Arguments = Environment.CommandLine;
			process.EnableRaisingEvents = true;
			//process.StartInfo.RedirectStandardOutput = true;
			//process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.CreateNoWindow = true;

			process.Start();
			process.WaitForInputIdle();
			return process;
		}

		public void SendMessage(Message request)
		{
			Service.SendMessage(request);
		}

		public void Service_ClientResponse(object sender, Message response)
		{
			switch (response?.Code)
			{
				case Message.StatusCode.ITERATION_RESULTS:
					SimulationResultsReceived?.Invoke(this, response);
					return;
				case Message.StatusCode.LOG:
					SimulationLogReceived?.Invoke(this, response);
					return;
				case Message.StatusCode.SUCCESS:
				case Message.StatusCode.ERROR:
				case Message.StatusCode.BAD_REQUEST:
					SimulationProcessFinished?.Invoke(this, response);
					break;
				default:
					throw new ArgumentException($"Unsupported message's status code {response?.Code}");
			}
		}
	}
}
