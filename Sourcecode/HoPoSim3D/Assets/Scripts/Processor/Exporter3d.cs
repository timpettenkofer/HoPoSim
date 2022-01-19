using Assets;
using Assets.Interfaces;
using Assets.IPC;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using System;
using UnityEngine;

public class Exporter3d : MonoBehaviour, IResultProcessor
{
	public void Process(IterationOutcomeArgs outcome, SimulationSettings settings, IpcCallback callback)
	{
		var info = Serializer<ExportSettings>.FromJSON(settings.Settings);

		ConfigurationHelper.Callback.Log ($"Exporting 3d models to {info.Path}");
		var trunks = GameObject.Find("Trunks");
		
		bool success = Export(trunks, info.Path, info.Format);

		var msg = success ? "Successfully exported 3d models." : "Some errors occured during 3d export.";
		var code = success ? Message.StatusCode.SUCCESS : Message.StatusCode.ERROR;
		Reporter.SendCommandStatus(Message.CommandCode.EXPORT_3D, code, msg, callback);
	}

	private bool Export(GameObject gameObject, string filepath, ExportFormat format)
	{
		try
		{
			switch (format)
			{
				case ExportFormat.FBX:
					return UnityFBXExporter.FBXExporter.ExportGameObjToFBX(gameObject, filepath);
				case ExportFormat.OBJ:
					{
						var objExporter = new OBJExporter();
						objExporter.Export(gameObject, filepath);
						return true;
					}
				default:
					ConfigurationHelper.Callback.Log($"Unrecognized file format {format}");
					return false;
			}
		}
		catch(Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while exporting: {e.Message}");
			return false;
		}
	}
}