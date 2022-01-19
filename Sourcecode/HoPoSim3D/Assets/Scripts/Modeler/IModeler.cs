using HoPoSim.IPC.DAO;
using UnityEngine;

namespace Assets.Interfaces
{
	public interface IModeler
	{
		GameObject CreateGeometry(Stamm stamm, SimulationData data);
	}
}
