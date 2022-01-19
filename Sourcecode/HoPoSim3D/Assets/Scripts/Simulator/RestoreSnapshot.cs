using Assets.Interfaces;
using HoPoSim.IPC.DAO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestoreSnapshot : BaseStrategy
{
	public override void BuildInitialConfiguration(IEnumerable<GameObject> polterTrunks, IEnumerable<GameObject> polterunterlageTrunks)
	{
		var data = ConfigurationHelper.SimulationSettings?.SnapshotData;
		if (string.IsNullOrEmpty(data))
		{
			var msg = $"[ERROR] Cannot restore simulation 3d configuration. No snapshot data available.";
			throw new ConfigurationException(msg, null);
		}

		var snapshot = Serializer<Snapshot>.FromJSON(data);
		var allTrunks = polterTrunks.Concat(polterunterlageTrunks).ToList();

		foreach(var position in snapshot.Positions)
		{
			var trunk = allTrunks.FirstOrDefault(t => t.name == position.Id);
			if (trunk != null)
				RestoreState(trunk, position);
		}
	}

	public override void OnIterationStarting()
	{
		IterationStarted = true;
	}

	private void RestoreState(GameObject go, StammPosition state)
	{
		var rb = go.GetComponent<Rigidbody>();

		var pos = state.Pos;
		rb.transform.position = new Vector3(pos.X, pos.Y, pos.Z);

		var rot = state.Rot;
		rb.transform.rotation = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);
	}

	public override void OnFixedUpdate()
	{
		NotifyIterationSuccess();
	}
}
