using Assets;
using Assets.Interfaces;
using Assets.IPC;
using Assets.Scripts.Framework;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simulator : MonoBehaviour
{
	private IEnumerable<GameObject> Trunks { get; set; }

	public string defaultModeler;
	public string defaultStrategy;
	public string defaultProcessor;

	public SimulationData Data { get; set; }
	public SimulationSettings Settings { get; set; }
	public IpcCallback Callback { get; set; }
	public IGeometryGenerator Generator { get; set; }
	public IModeler Modeler { get; set; }
	public IStrategy Strategy { get; set; }
	public IResultProcessor Processor { get; set; }

	#region Configuration
	private void Configure()
	{
		Data = ConfigurationHelper.SimulationData;
		Settings = ConfigurationHelper.SimulationSettings;
		Callback = ConfigurationHelper.Callback;

		Generator = gameObject.GetComponent<IGeometryGenerator>();

		Modeler = ResolveModeler();
		Strategy = ResolveStrategy();
		Processor = ResolveProcessor();

		Settings.Modeler = ResolveTypeName(Modeler);
		Settings.Strategy = ResolveTypeName(Strategy);
		Settings.Processor = ResolveTypeName(Processor);
		ConfigurePhysicsMaterial();

		Callback.Log($"Setting time-out period to {Settings.TimeOutPeriod} sec.");
		Strategy.SetTimeOutPeriod(Settings.TimeOutPeriod);

		InitializePhysicsEngine();
	}

	public void InitializePhysicsEngine()
	{
		Time.timeScale = 1f;
		Physics.gravity = new Vector3(0, -1f, 0);
		Physics.sleepThreshold = 0.005f;
		Physics.defaultContactOffset = 0.005f;
	}

	private void ConfigurePhysicsMaterial()
	{
		TrunkPhysics.ConfigurePhysicWoodMaterial(Data.WoodFriction);
	}

	private IResultProcessor ResolveProcessor()
	{
		return ResolveComponent(Settings.Processor) is IResultProcessor processor ? 
			processor : 
			ResolveComponent(defaultProcessor) as IResultProcessor;
	}

	private IStrategy ResolveStrategy()
	{
		return ResolveComponent(Settings.Strategy) is IStrategy strategy ?
			strategy :
			ResolveComponent(defaultStrategy) as IStrategy;
	}

	private IModeler ResolveModeler()
	{
		return ResolveComponent(Settings.Modeler) is IModeler modeler ?
			modeler : 
			ResolveComponent(defaultModeler) as IModeler;
	}

	private Component ResolveComponent(string typeName)
	{
		return typeName != null ?
			gameObject.AddComponent(Type.GetType(typeName)) :
			null;
	}

	private string ResolveTypeName(object obj)
	{
		return obj.GetType().FullName;
	}
	#endregion

	public void TriggerSimulation()
	{
		if (Trunks == null)
			throw new ArgumentException("Empty trunks models");

		timer = new Timer();
		var start = Settings.IterationStart;

		if (IsValidIteration(start))
		{
			Strategy.Initialize(Data);
			Strategy.IterationFinished += OnIterationFinished;
			Strategy.BadConfigurationDetected += OnBadConfigurationDetected;
			Callback.Log("Starting simulation...");
			Physics.autoSyncTransforms = true;
			Simulate(start);
		}
	}

	private void OnBadConfigurationDetected(object sender, BadConfigurationArgs e)
	{
		if (e.FixConfiguration != null)
		{
			e.FixConfiguration(Data);
			Strategy.Initialize(Data);
			Simulate(e.Iteration);
			return;
		}
		StartNextIteration(e.Iteration);
	}

	private void OnIterationFinished(object sender, IterationOutcomeArgs e)
	{
		var elapsedTimeForIteration = timer.ElapsedSeconds();
		Callback.Log($"Simulation iteration {e.Iteration} finished with result {e.Status} ({elapsedTimeForIteration} seconds).");
		Processor.Process(e, Settings, Callback);
		StartNextIteration(e.Iteration);
	}

	private void StartNextIteration(int lastIteration)
	{
		var nextIteration = lastIteration + 1;
		if (IsValidIteration(nextIteration))
			Simulate(nextIteration);
		else
		{
			if (Settings.SendCommandFinishedMessage)
				Reporter.SendCommandStatus(Message.CommandCode.SIMULATION, Message.StatusCode.SUCCESS, $"Simulation finished ({Settings.IterationEnd - Settings.IterationStart} iterations).", Callback);
		}
	}

	private bool IsValidIteration(int iteration)
	{
		return iteration < Settings.IterationEnd;
	}

	public void Simulate(int iteration)
	{
		Callback.Log($"Configuring simulation iteration {iteration}...");

		timer.Reset();
		InitializeRandomSeedForIteration(iteration);
		RestoreDefaultPositions(Trunks);

		var randomized_trunks = Helpers.ShuffleTrunks(Trunks);

		if (Data.Poltermaße.Zufallsspiegelung)
			Data.Poltermaße.Seitenspiegelung = UnityEngine.Random.Range(0, 100);

		PerformSideFlipping(randomized_trunks, Data.Poltermaße.Seitenspiegelung);
		var polterTrunks = Strategy.Configure(iteration, randomized_trunks);
		PerformTransversalRotation(polterTrunks, Data.Poltermaße);

		Callback.Log($"\nStarting simulation iteration {iteration}...");
		Strategy.Perform();
	}

	private Timer timer = null;


	private void RestoreDefaultPositions(IEnumerable<GameObject> objects)
	{
		foreach (var go in objects)
		{
			go.transform.rotation = new Quaternion();
			go.transform.position = Vector3.zero;
			go.transform.localRotation = new Quaternion();
			go.transform.localPosition = Vector3.zero;

			var bounds = go.GetComponent<MeshFilter>().sharedMesh.bounds;

			go.transform.position = new Vector3(0f, -bounds.center.y, 0f);
			// Apply random rotation around tree axis
			go.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0, 360));
		}
	}

	private void InitializeRandomSeedForIteration(int iteration)
	{
		var seed = Settings.Seed + iteration;
		UnityEngine.Random.InitState(seed);
	}

	private void PerformTransversalRotation(IEnumerable<GameObject> trunks, PolterConfiguration configuration)
	{
		var proportion = configuration.CrossTrunksProportion;
		var minAngle = configuration.CrossTrunksMinimumAngle;
		var maxAngle = configuration.CrossTrunksMaximumAngle;

		int numTrunksToRotate = (int)Math.Round(trunks.Count() * (proportion / 100.0f), 0);
		var shuffledTrunks = Helpers.ShuffleTrunks(trunks);
		for (int i = 0; i < numTrunksToRotate; ++i)
		{
			int angle = UnityEngine.Random.Range(minAngle, maxAngle) * ((i % 2) > 0 ? +1 : -1);
			shuffledTrunks[i].transform.RotateAround(Vector3.zero, Vector3.up, angle);
		}

	}

	private void PerformSideFlipping(List<GameObject> trunks, float seitenspiegelung)
	{
		int numTrunksToFlip =  (int)Math.Round(trunks.Count * (seitenspiegelung / 100.0f), 0);
		var shuffledTrunks = Helpers.ShuffleTrunks(trunks);
		for (int i = 0; i < shuffledTrunks.Count; ++i)
		{
			int flipped = i < numTrunksToFlip ? -1 : 1;
			shuffledTrunks[i].transform.RotateAround(Vector3.zero, Vector3.right, flipped * 90.0f);
		}
	}

	private void CreateTrunkGeometry()
	{
		DeleteAllExistingTrunkModels();
		try
		{
			Callback.Log($"Creating trunks...");
			var parent = GameObject.Find("Trunks");
			Trunks = Generator.Generate(Modeler, Data, parent);
			Callback.Log("Trunks created.");
		}
		catch (Exception e)
		{
			throw new Exception("Error while modelling trunks", e);
		}
	}


	private void DeleteAllExistingTrunkModels()
	{
		Trunks?
			.ToList()
			.ForEach(t => DestroyImmediate(t));
	}

	void Start()
	{
		try
		{
			Configure();
			if (!Settings.IsComplete)
			{
				Callback.Log($"Setting random seed to {Settings.Seed}");
				InitializeRandomSeedForIteration(0);
				CreateTrunkGeometry();
				TriggerSimulation();
			}
		}
		catch(Exception e)
		{
			var msg = $"Some errors occured during simulation. {e.ToString()}";
			Reporter.SendCommandStatus(Message.CommandCode.SIMULATION, Message.StatusCode.ERROR, msg, Callback);
			//throw new Exception(msg, e);
		}

	}
}

