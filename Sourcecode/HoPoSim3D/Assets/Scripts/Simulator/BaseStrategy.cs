using Assets.Interfaces;
using Assets.Scripts.Framework;
using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseStrategy : MonoBehaviour, IStrategy
{
	private Timer timeOutTimer;

	protected SimulationData CurrentSimulationData { get; private set; }

	protected float SteigungswinkelInRadians { get; private set; }

	public float TimeoutInSeconds { get; private set; }

	public float EpsilonPolter { get { return 0.05f; } }

	protected bool IterationStarted = false;

	public virtual void OnIterationStarting() { }
	public virtual void OnIterationTimedOut() { }
	public abstract void BuildInitialConfiguration(IEnumerable<GameObject> polterTrunks, IEnumerable<GameObject> polterunterlageTrunks);
	public abstract void OnFixedUpdate();

	public void Initialize(SimulationData data)
	{
		CurrentSimulationData = data;
		SteigungswinkelInRadians = Mathf.Deg2Rad * data.Poltermaße.Steigungswinkel;
		Physics.autoSimulation = false;
		timeOutTimer = new Timer();
		SetSleepThreshold();
	}

	public void SetSleepThreshold(float value = 0.005f)
	{
		Physics.sleepThreshold = value;
	}

	public void FixedUpdate()
	{
		try
		{
			if (!IterationStarted && !IsPhysicsEngineRunning())
			{
				IterationStarted = true;
				MakePolterunterlageKinematic();
			}

			if (IsRunning() && HasTimedOut())
			{
				NotifyIterationTimeOut();
				return;
			}

			if (IterationStarted && Physics.autoSimulation)
				OnFixedUpdate();
		}
		catch (ConfigurationException e)
		{
			ConfigurationHelper.Callback.Log(e.Message);
			BadConfigurationDetected?.Invoke(this, new BadConfigurationArgs(CurrentIteration, e.FixConfiguration));
		}
	}

	private IEnumerable<GameObject> BuildPolterunterlage(IEnumerable<GameObject> trunks)
	{
		var polterunterlageTrunks = PolterunterlageBuilder.Build(trunks, CurrentSimulationData).ToList();
		return polterunterlageTrunks;
	}

	public void SetTimeOutPeriod(int seconds)
	{
		TimeoutInSeconds = seconds;
	}

	protected bool HasTimedOut()
	{
		return timeOutTimer.ElapsedSeconds() > TimeoutInSeconds;
	}

	public void LogSimulationInfo()
	{
		var poltermaße = CurrentSimulationData.Poltermaße;
		ConfigurationHelper.Callback.Log("Simulation data");
		ConfigurationHelper.Callback.Log($"- Baumart: {CurrentSimulationData.Baumart.Name}");
		ConfigurationHelper.Callback.Log($"- Holz: Rindenbeschädigungen {CurrentSimulationData.Holz.Rindenbeschädigungen}% / Krümmungsvarianten {CurrentSimulationData.Holz.Krümmungsvarianten}%");
		ConfigurationHelper.Callback.Log($"- Physics: Holzdichte {CurrentSimulationData.WoodDensity} kg/m3 / Holzfriktion {CurrentSimulationData.WoodFriction}");
		ConfigurationHelper.Callback.Log($"- Polterlänge: (mit Polterunterlage) {poltermaße.ActualPolterlänge:0.##}m / (Polter) {poltermaße.MinimumPolterlänge:0.##}m");
		ConfigurationHelper.Callback.Log($"- Zufallsspiegelung: {poltermaße.Zufallsspiegelung}");
		ConfigurationHelper.Callback.Log($"- Seitenspiegelung: {poltermaße.Seitenspiegelung}%");
		ConfigurationHelper.Callback.Log($"- Stapelqualität: {poltermaße.CrossTrunksProportion}% zwischen {poltermaße.CrossTrunksMinimumAngle}° und {poltermaße.CrossTrunksMaximumAngle}°");
		ConfigurationHelper.Callback.Log($"- Steigungswinkel: {poltermaße.Steigungswinkel}°");
		ConfigurationHelper.Callback.Log($"- Polterunterlage Stammanzahl: {poltermaße.Polterunterlagestammanzahl}");
	}

	private List<TrunkMoveTracker> Trackers { get; set; }

	protected bool IsPhysicsEngineRunning()
	{
		return Trackers.Any(t => t.IsAwaken());
	}

	protected void ForcePhysicsEngineRun()
	{
		foreach (var t in Trackers)
			t.WakeUp();
	}

	public int CurrentIteration { get; private set; }

	public event EventHandler<IterationOutcomeArgs> IterationFinished;
	public event EventHandler<BadConfigurationArgs> BadConfigurationDetected;

	public float CurrentHorizontalPolterOffset { get; private set; }

	public IEnumerable<GameObject> Configure(int iteration, IEnumerable<GameObject> trunks)
	{
		try
		{
			CurrentIteration = iteration;

			var polterunterlageTrunks = BuildPolterunterlage(trunks);
			UpdateConfigurationData(polterunterlageTrunks);
			var polterTrunks = trunks.Except(polterunterlageTrunks).ToList();

			ConfigurePolterManager(polterunterlageTrunks, polterTrunks);
			CurrentHorizontalPolterOffset = ComputePolterunterlageHorizontalOffset(polterunterlageTrunks);

			Trackers = trunks
				.Select(t => t.GetComponent<TrunkMoveTracker>())
				.ToList();

			return polterTrunks;
		}
		catch (Exception e)
		{
			ConfigurationHelper.Callback.Log(e.Message);
			BadConfigurationDetected?.Invoke(this, new BadConfigurationArgs(CurrentIteration, null));
		}
		return null;
	}

	public void Perform()
	{
		try
		{
			var polterTrunks = PolterManager.GetPolterTrunks(PolterManager.PolterTag);
			var polterunterlageTrunks = PolterManager.GetPolterTrunks(PolterManager.PolterunterlageTag);
			polterTrunks = Helpers.ShuffleTrunks(polterTrunks);

			BuildInitialConfiguration(polterTrunks, polterunterlageTrunks);
			SetInitialCameraPosition();

			LogSimulationInfo();
			ConfigurationHelper.Callback.Log("Performing physics simulation...");

			
			timeOutTimer.Reset();

			OnIterationStarting();
			Physics.autoSimulation = true;
		}
		catch (ConfigurationException e)
		{
			ConfigurationHelper.Callback.Log(e.Message);
			BadConfigurationDetected?.Invoke(this, new BadConfigurationArgs(CurrentIteration, e.FixConfiguration));
		}
		catch(Exception e)
		{
			ConfigurationHelper.Callback.Log(e.Message);
			BadConfigurationDetected?.Invoke(this, new BadConfigurationArgs(CurrentIteration, null));
		}

	}

	protected void MakePolterunterlageKinematic()
	{
		foreach (var tracker in Trackers)
			tracker.MakeKinematicIfPolterunterlage();
	}

	private static void ConfigurePolterManager(IEnumerable<GameObject> polterunterlageTrunks, List<GameObject> polterTrunks)
	{
		PolterManager.SetPolterunterlageTrunks(polterunterlageTrunks);
		PolterManager.SetPolterTrunks(polterTrunks);
		PolterManager.IgnoreCollision(PolterManager.PolterunterlageLayer, PolterManager.PolterToolsLayer);
	}

	private bool IsRunning()
	{
		return Physics.autoSimulation;
	}

	protected void NotifyIterationSuccess()
	{
		Physics.autoSimulation = false;
		ConfigurationHelper.Callback.Log("Physics simulation finished.");
		IterationFinished?.Invoke(this, new IterationOutcomeArgs(CurrentIteration, IterationResult.Success));
	}

	protected void NotifyIterationTimeOut()
	{
		Physics.autoSimulation = false;
		OnIterationTimedOut();
		ConfigurationHelper.Callback.Log("Physics simulation timed out.");
		IterationFinished?.Invoke(this, new IterationOutcomeArgs(CurrentIteration, IterationResult.Timeout));
	}

	protected void SetInitialCameraPosition()
	{
		var focusCamera = Camera.main.GetComponent<FocusCamera>();
		if (focusCamera != null)
			focusCamera.Focus(Side.FRONT);
	}

	protected float ComputePolterunterlageHorizontalOffset(IEnumerable<GameObject> polterunterlageTrunks)
	{
		if (!polterunterlageTrunks.Any())
			return 0f;

		var polterunterlageBoundsFirstRow = BoundingBox.GetAxisAlignedBounds(polterunterlageTrunks.Where(t => t.transform.position.z < 0));
		var polterunterlageBoundsSecondRow = BoundingBox.GetAxisAlignedBounds(polterunterlageTrunks.Where(t => t.transform.position.z > 0));

		var polterExpectedWidth = CurrentSimulationData.Poltermaße.MinimumPolterlänge;
		var polterUnterlageWidth = Mathf.Min(polterunterlageBoundsFirstRow.size.x, polterunterlageBoundsSecondRow.size.x);
		return (polterUnterlageWidth - polterExpectedWidth) * 0.5f;
	}

	protected float GetPolterunterlageVerticalOffset()
	{
		if (!CurrentSimulationData.Poltermaße.Polterunterlage)
			return EpsilonPolter;
		return CurrentSimulationData.Poltermaße.Polterunterlagehöhe + EpsilonPolter;
	}

	protected void UpdateConfigurationData(IEnumerable<GameObject> polterunterlageTrunks)
	{
		var unterlageBounds = BoundingBox.GetAxisAlignedBounds(polterunterlageTrunks);
		var unterlageLength = unterlageBounds.size.x;

		CurrentSimulationData.Poltermaße.ActualPolterlänge = Mathf.Max(CurrentSimulationData.Poltermaße.MinimumPolterlänge, unterlageLength);
		CurrentSimulationData.Poltermaße.Polterunterlagehöhe = unterlageBounds.size.y;
		CurrentSimulationData.Poltermaße.Polterunterlagestammanzahl = polterunterlageTrunks.Count();
	}

	#region Helpers
	protected void MoveTrunkAt(GameObject obj, Bounds bounds, float x, float y)
	{
		var shift_x = x - bounds.min.x;
		var shift_y = y - bounds.min.y;
		obj.transform.Translate(shift_x, shift_y, 0, Space.World);
	}
	#endregion


}
