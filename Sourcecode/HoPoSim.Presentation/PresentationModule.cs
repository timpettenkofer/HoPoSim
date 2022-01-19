using HoPoSim.Presentation.Views;
using HoPoSim.Framework;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel.Composition;

namespace HoPoSim.Presentation
{
	[ModuleExport(typeof(PresentationModule))]
	public class PresentationModule : IModule
	{
		IRegionManager _regionManager;

		[ImportingConstructor]
		public PresentationModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(HomeView));
			_regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(NavigationView));
			_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(GeneratorView));
			_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(StatistikenView));
			_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(SimulationView));
			_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(SimulationDataView));
			//_regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(AusgabeView));
			_regionManager.RegisterViewWithRegion(RegionNames.SettingsRegion, typeof(EinstellungenView));

		}
	}
}
