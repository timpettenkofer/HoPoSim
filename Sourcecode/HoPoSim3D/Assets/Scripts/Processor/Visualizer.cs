using Assets;
using Assets.Interfaces;
using Assets.IPC;
using UnityEngine;

public class Visualizer : MonoBehaviour, IResultProcessor
{
	public void Process(IterationOutcomeArgs outcome, SimulationSettings settings, IpcCallback callback)
	{
		ShowHelpButton();

		var focusCamera = Camera.main.GetComponent<FocusCamera>();
		if (focusCamera != null)
			focusCamera.Focus(Side.FRONT);
	}

	private void ShowHelpButton()
	{
		var simulator = GameObject.FindGameObjectWithTag("GameController");
		var dc = simulator.GetComponent<DecorationController>();
		dc.ShowHelpButton();
	}
}
