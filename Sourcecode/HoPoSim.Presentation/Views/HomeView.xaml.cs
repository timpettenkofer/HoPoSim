using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.IO;
using HoPoSim.Presentation.Helpers;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Views
{
	[Export(typeof(HomeView))]
	public partial class HomeView : UserControl
	{
		[ImportingConstructor]
		public HomeView(INavigationHelper navHelper, IInteractionService interactionService)
		{
			_navHelper = navHelper;
			_interactionService = interactionService;
			InitializeComponent();
		}
		INavigationHelper _navHelper;
		IInteractionService _interactionService;

		private void Generator_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.NavigateTo(typeof(GeneratorView));
		}

		private void Eingabe_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.NavigateTo(typeof(SimulationDataView));
		}

		private void Simulation_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.NavigateTo(typeof(SimulationView));
		}

		private void Ausgabe_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//_navHelper.NavigateTo(typeof(AusgabeView));
		}

		private void Einstellungen_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.NavigateToSettings();
		}

		private void Hilfe_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.OpenHelp();
		}

		private void Vorlage_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var file = DocumentHandler.GetSavedAsFileFromUserSelection(HoPoSim.IO.Serialization.StammlisteReader.FileExtensionFilter, "Stammliste.xlsx");
			if (file != null)
			{
				try
				{
					ApplicationFolders.CopyTemplate(ApplicationTemplates.Stammdaten.FilePath, file);
				}
				catch(Exception exception)
				{
					_interactionService.RaiseNotificationAsync(exception.Message, "Fehler beim Kopieren der Datei");
				}
			}
		}

		private void About_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			_navHelper.ShowAbout();
		}
	}
}
