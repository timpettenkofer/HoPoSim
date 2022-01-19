using Assets.Interfaces;
using Assets.IPC;

namespace Assets
{
	public interface IResultProcessor
	{
		void Process(IterationOutcomeArgs outcome, SimulationSettings settings, IpcCallback callback);
	}
}
