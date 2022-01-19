namespace HoPoSim.Framework.Interfaces
{
	public interface IApplicationUpdateService
	{
		bool CanCheckForUpdates();
		void CheckForUpdates();
	}
}
