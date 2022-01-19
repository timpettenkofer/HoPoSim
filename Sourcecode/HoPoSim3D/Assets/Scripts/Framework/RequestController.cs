using Assets;
using Assets.IPC;
using HoPoSim.IPC.WCF;
using System;
using UnityEngine;

public class RequestController : MonoBehaviour
{
	Client IPC { get; set; }
	MessageDispatcher MessageDispatcher { get; set; }

	void Awake()
	{
		try
		{
			IPC = new Client();
			IPC.Start();
			IPC.Callback.ServerRequest += Callback_ServerRequest;

			MessageDispatcher = new MessageDispatcher();

			DontDestroyOnLoad(this.gameObject);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Could not start IPC client: {e.Message}");
		}
	}

	private void Callback_ServerRequest(object sender, Message request)
	{
		Process(request);
	}


	private bool Process(Message request)
	{
		if (!RequestTargetsCurrentProcess(request))
			return false;

		var callback = new IpcCallback(IPC, request?.ProcessId ?? -1, request.Command);

		switch (request.Command)
		{
			case Message.CommandCode.SIMULATION:
				{
					ExecuteOnMainThread.RunOnMainThread.Enqueue(() => MessageDispatcher.StartSimulation(request, callback));
					return true;
				}
			case Message.CommandCode.VISUALIZATION:
				{
					ExecuteOnMainThread.RunOnMainThread.Enqueue(() => MessageDispatcher.StartVisualization(request, callback));
					return true;
				}
			case Message.CommandCode.EXPORT_3D:
				{
					ExecuteOnMainThread.RunOnMainThread.Enqueue(() => MessageDispatcher.StartExport3d(request, callback));
					return true;
				}
			case Message.CommandCode.EXPORT_IMG:
				{
					ExecuteOnMainThread.RunOnMainThread.Enqueue(() => MessageDispatcher.StartExportImages(request, callback));
					return true;
				}
			default:
				{
					var response = SetReponseData(request, Message.StatusCode.BAD_REQUEST, $"Unknow command {request.Command}");
					IPC.SendResponse(response);
					return false;
				}
		}
	}

	private bool RequestTargetsCurrentProcess(Message request)
	{
		var targetProcessId = request.ProcessId;
		int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
		return targetProcessId == -1 || targetProcessId == currentProcessId;
	}

	private Message SetReponseData(Message request, Message.StatusCode statusCode, string response)
	{
		request.Data = response;
		request.Code = statusCode;
		return request;
	}
}
