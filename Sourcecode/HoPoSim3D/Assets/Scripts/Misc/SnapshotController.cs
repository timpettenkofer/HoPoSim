using HoPoSim.IPC.DAO;
using System.Linq;
using UnityEngine;

public static class SnapshotController
{

	public static string CreateSnapshot()
	{
		var activeTrunks =
			PolterManager.GetPolterTrunks(PolterManager.PolterunterlageTag)
			.Concat(PolterManager.GetPolterTrunks(PolterManager.PolterTag))
			.Where(t => t.activeInHierarchy);

		var snapshot = new Snapshot()
		{
			Positions = activeTrunks.Select(t => ExtractPosition(t))
		};

		return Serializer<Snapshot>.ToJSON(snapshot, false);
	}

	private static StammPosition ExtractPosition(GameObject go)
	{
		var rb = go.GetComponent<Rigidbody>();
		var pos = rb.transform.position;
		var rot = rb.transform.rotation;

		var position = new StammPosition()
		{
			Id = go.name,
			Pos = new Position() { X = pos.x, Y = pos.y, Z = pos.z },
			Rot = new Rotation() { X = rot.x, Y = rot.y, Z = rot.z, W = rot.w }
		};
		return position;
	}
}
