using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.License;
using HoPoSim.Views;
using Prism.Mef;
using Prism.Regions;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace HoPoSim
{
	// Create a Bootstrapper Class using MEF Bootstrapper
	public class Bootstrapper : MefBootstrapper
	{
		// Override the CreateShell returning an instance of our shell.
		protected override DependencyObject CreateShell()
		{
			return Container.GetExportedValue<Shell>();
		}

		// Override the InitializeShell setting the MainWindow on the application and showing the shell.
		protected override void InitializeShell()
		{
			Application.Current.MainWindow.Show();
		}

		protected override void InitializeModules()
		{
			var licenseService = new LicenseService(Container.GetExportedValue<IInteractionService>());
			if (!licenseService.CheckLicense())
				Application.Current.Shutdown();

			base.InitializeModules();
			Container.GetExportedValue<IRegionManager>().RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(SettingsView));
		}

		// Override the ConfigureAggregateCatalog.
		// Add the new assembly catalogs which will register all classes with [Export] attributes
		protected override void ConfigureAggregateCatalog()
		{
			base.ConfigureAggregateCatalog();
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IGlobalConfigService).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(HoPoSim.Presentation.PresentationModule).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(HoPoSim.Data.UnitOfWork).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(HoPoSim.IO.ReportingService).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(HoPoSim.IPC.IInterProcessCommunication).Assembly));
		}
	}
}
