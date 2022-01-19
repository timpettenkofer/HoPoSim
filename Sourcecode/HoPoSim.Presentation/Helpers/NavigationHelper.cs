using HoPoSim.Framework;
using HoPoSim.Presentation.Controls;
using HoPoSim.Presentation.Views;
using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace HoPoSim.Presentation.Helpers
{
    public interface INavigationHelper
    {
        void NavigateTo(Type view, int? id = null);
        void NavigateTo(Type view, int? id, KeyValuePair<string, object> additionalParameter);
		void OpenHelp();
		void ShowAbout();
		void NavigateToSettings();
	}

    [Export(typeof(INavigationHelper))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NavigationHelper : INavigationHelper
    {
        [ImportingConstructor]
        public NavigationHelper(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }
        IRegionManager RegionManager { get; }

        public void NavigateTo(Type view, int? id = null)
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", id);
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(view.FullName, UriKind.Relative), parameters);
        }

        public void NavigateTo(Type view, int? id, KeyValuePair<string, object> additionalParameter)
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", id);
            parameters.Add(additionalParameter.Key, additionalParameter.Value);
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(view.FullName, UriKind.Relative), parameters);
        }

		public void NavigateToSettings()
		{
			RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("HoPoSim.Views.SettingsView", UriKind.Relative));
			RegionManager.RequestNavigate(RegionNames.SettingsRegion, new Uri(typeof(EinstellungenView).FullName, UriKind.Relative));
		}

		private MetroWindow aboutWindow;

		public void ShowAbout()
		{
			if (aboutWindow != null)
			{
				aboutWindow.Activate();
				return;
			}

			aboutWindow = new AboutWindow();
			//aboutWindow.Owner = this;
			aboutWindow.Closed += (o, args) => aboutWindow = null;
			//aboutWindow.Left = this.Left + this.ActualWidth / 2.0;
			//aboutWindow.Top = this.Top + this.ActualHeight / 2.0;
			aboutWindow.Show();
		}


		public void OpenHelp()
		{
			try
			{
				var htmlHelpFile = "hoposim_help.html";
				var fullpath = Path.Combine(GetExecutingFolder(), "Help", htmlHelpFile);
				System.Diagnostics.Process.Start(fullpath);
			}
			catch
			{
				return;
			}
		}

		private static string GetExecutingFolder()
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}
	}
}
