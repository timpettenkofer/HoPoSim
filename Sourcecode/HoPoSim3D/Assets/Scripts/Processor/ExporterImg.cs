using Assets;
using Assets.Interfaces;
using Assets.IPC;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExporterImg : MonoBehaviour, IResultProcessor
{
	private FocusCamera focusCamera;

	public void Process(IterationOutcomeArgs outcome, SimulationSettings settings, IpcCallback callback)
	{
		var info = Serializer<ImageExportSettings>.FromJSON(settings.Settings);
		var width = info.Width;
		var height = info.Height;
		var path = info.Path;

		ConfigurationHelper.Callback.Log($"Preparing scene snapshots export ({width}x{height} pixels) to {path} ...");
		focusCamera = Camera.main.GetComponent<FocusCamera>();

		try
		{
			ExportSnapshots(outcome, width, height, path);

			var msg = $"Successfully exported images in {path}";
			Reporter.SendCommandStatus(Message.CommandCode.EXPORT_IMG, Message.StatusCode.SUCCESS, msg, callback);
		}
		catch(Exception e)
		{
			var msg = $"Some errors occured during image export. {e.Message}";
			Reporter.SendCommandStatus(Message.CommandCode.EXPORT_IMG, Message.StatusCode.ERROR, msg, callback);
		}
	}

	private void ExportSnapshots(IterationOutcomeArgs outcome, int width, int height, string path)
	{
		var sides = new[] { Side.FRONT, Side.BACK };
		foreach (var side in sides)
		{
			PrepareScene(side, width, height);
			var simulationName = ConfigurationHelper.SimulationData.Name;
			var filename = GenerateUniqueFileName(path, $"HoPoSim_{simulationName}_Iteration_{outcome.Iteration}_{side}.png", true);
			TakeSnapshot(filename, width, height);
			RestoreScene();
		}
	}

	public static string GenerateUniqueFileName(string dir, string startFilename, bool appendDirectoryToResult)
	{
		var name = Path.GetFileNameWithoutExtension(startFilename);
		var ext = Path.GetExtension(startFilename);

		string newFile = Path.Combine(dir, $"{name}{ext}");
		if (!File.Exists(newFile))
			return appendDirectoryToResult ? newFile : $"{name}{ext}";

		int i = 0;
		do
		{
			i++;
			newFile = Path.Combine(dir, $"{name}_{i}{ext}");
		} while (File.Exists(newFile));
		return appendDirectoryToResult ? newFile : $"{name}_{i}{ext}";
	}

	private void TakeSnapshot(string outfile, int width, int height)
	{
		ScreenshotHandler.TakeScreenshot(outfile, width, height);
	}

	private void PrepareScene(Side side, int width, int height)
	{
		PositionCamera(side, width, height);
		SetLabelsVisibility(true);
		SetOutlinesVisibility(true);
		ShowConcaveHull(side);
		//ShowConvexHull(side);
		ShowSektions(side);
	}

	private void RestoreScene()
	{
		SetLabelsVisibility(false);
		SetOutlinesVisibility(false);
		HideConcaveHull();
		//HideConvexHull();
		HideSektions();
	}

	private void PositionCamera(Side side,int width, int height)
	{
		if (focusCamera != null)
			focusCamera.Focus(side, (float)width / (float)height);
	}

	private void SetLabelsVisibility(bool visible)
	{
		var trunks = Resources.FindObjectsOfTypeAll<TrunkLabel>().ToList();
		trunks.ForEach(t => t.SetVisible(visible));
	}

	private void SetOutlinesVisibility(bool visible)
	{
		var outlines = Resources.FindObjectsOfTypeAll<TrunkOutline>().ToList();
		outlines.ForEach(t => t.SetVisible(visible));
	}

	private void ShowConcaveHull(Side side)
	{
		var hull = GameObject.FindObjectOfType<ConcaveHullOutline>();
		hull.ForceBuild(side, 0.03f);
	}

	private void HideConcaveHull()
	{
		var hull = GameObject.FindObjectOfType<ConcaveHullOutline>();
		hull.Destroy(Side.BOTH);
	}

	private void ShowConvexHull(Side side)
	{
		var hull = GameObject.FindObjectOfType<Fotooptik>();
		hull.ForceBuild(side, 0.03f);
	}

	private void HideConvexHull()
	{
		var hull = GameObject.FindObjectOfType<Fotooptik>();
		hull.Destroy(Side.BOTH);
	}

	private void ShowSektions(Side side)
	{
		var sektion = GameObject.FindObjectOfType<Sektionraummaß>();
		sektion.ForceBuild(side, 0.02f);
	}

	private void HideSektions()
	{
		var sektion = GameObject.FindObjectOfType<Sektionraummaß>();
		sektion.Destroy(Side.BOTH);
	}
}
