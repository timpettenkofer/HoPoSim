using Assets.Interfaces;
using Assets.Scripts.Framework;
using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeometryGenerator : MonoBehaviour, IGeometryGenerator
{
	public IEnumerable<GameObject> Generate(IModeler modeler, SimulationData data, GameObject parent)
	{
		var trunks = data?.Stämme;

		if (trunks == null)
			return null;

		var result = new List<GameObject>();

		ElectTrunksWithBranchStubs(data);

		var i = 0;
		foreach (var stamm in trunks)
		{
			GameObject trunk = modeler.CreateGeometry(stamm, data);
			if (++i % 10 == 0)
				ConfigurationHelper.Callback.Log($"Generating trunk {stamm.StammId} ...");
			trunk.name = stamm.StammId;
			trunk.transform.parent = parent.transform;
			AddComponents(trunk, stamm);
			result.Add(trunk);
		}
		return result;
	}

	private static void ElectTrunksWithBranchStubs(SimulationData data)
	{
		if (data.Baumart.IncludeBranches)
		{
			var trunks = data.Stämme;
			var shuffledTrunks = Helpers.ShuffleTrunks(trunks);

			var percentageOfTrunksWithBranchStubs = data.Baumart.BranchStubTrunkProportion * 0.01;
			var absoluteNumberOfTrunksWithBranches = (int)Math.Round(percentageOfTrunksWithBranchStubs * trunks.Count());

			foreach (var t in shuffledTrunks.Take(absoluteNumberOfTrunksWithBranches))
				t.HasBranchStubs = true;
		}
	}

	private static void AddComponents(GameObject trunk, Stamm stamm)
	{
		AddMeshComponents(trunk);
		AddInspector(trunk, stamm);
		AddTrunkPhysics(trunk);
		AddUIInfo(trunk);
		AddMoveTracker(trunk);
	}

	private static void AddMoveTracker(GameObject trunk)
	{
		trunk.AddComponent<TrunkMoveTracker>();
	}

	private static void AddUIInfo(GameObject trunk)
	{
		trunk.AddComponent<TrunkLabel>();
		trunk.AddComponent<TrunkOutline>();
	}

	private static void AddTrunkPhysics(GameObject trunk)
	{
		trunk.AddComponent<TrunkPhysics>();
	}

	private static void AddInspector(GameObject trunk, Stamm stamm)
	{
		var inspector = trunk.AddComponent<Inspector>();
		inspector.Parametrization = stamm;
	}

	private static void AddMeshComponents(GameObject trunk)
	{
		trunk.AddComponent<MeshVolume>();
	}
}
