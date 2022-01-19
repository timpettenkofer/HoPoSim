using HoPoSim.IPC.DAO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Interfaces
{
	public interface IGeometryGenerator
	{
		IEnumerable<GameObject> Generate(IModeler modeler, SimulationData data, GameObject parent);
	}
}
