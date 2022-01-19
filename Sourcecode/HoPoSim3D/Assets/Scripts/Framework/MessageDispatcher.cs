using Assets.IPC;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using System;
using UnityEngine.SceneManagement;

namespace Assets
{
	public class MessageDispatcher
	{
		private void SetConfiguration(SimulationData data, SimulationSettings settings, IpcCallback callback)
		{
			ConfigurationHelper.SimulationData = data;
			ConfigurationHelper.SimulationSettings = settings;
			ConfigurationHelper.Callback = callback;
		}

		private void StartSimulator(Message request, IpcCallback callback, Func<Message, SimulationSettings> createSettings)
		{
			var data = Serializer<SimulationData>.FromJSON(request.Data);
			var settings = createSettings(request);
			SetConfiguration(data, settings, callback);
			SceneManager.LoadScene("Simulator");
		}

		public void StartSimulation(Message request, IpcCallback callback)
		{
			try
			{
				callback.Log("Starting simulation...");
				StartSimulator(request, callback, SimulationSettings.CreateSimulationSettings);
			}
			catch (Exception e)
			{
				callback.Log("Error during simulation", e);
			}
		}

		public void StartVisualization(Message request, IpcCallback callback)
		{
			try
			{
				callback.Log("Starting visualization...");
				StartSimulator(request, callback, SimulationSettings.CreateVisualizationSettings);
			}
			catch (Exception e)
			{
				callback.Log("Error during visualization", e);
			}
		}

		public void StartExport3d(Message request, IpcCallback callback)
		{
			try
			{
				callback.Log("Starting export 3d...");
				StartSimulator(request, callback, SimulationSettings.CreateExport3dSettings);
			}
			catch (Exception e)
			{
				callback.Log("Error during visualization", e);
			}
		}

		public void StartExportImages(Message request, IpcCallback callback)
		{
			try
			{
				callback.Log("Starting export images...");
				StartSimulator(request, callback, SimulationSettings.CreateExportImagesSettings);
			}
			catch (Exception e)
			{
				callback.Log("Error during visualization", e);
			}
		}
	}
}
