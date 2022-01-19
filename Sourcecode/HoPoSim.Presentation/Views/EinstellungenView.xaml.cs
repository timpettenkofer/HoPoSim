using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.IO;
using HoPoSim.IO.Interfaces;
using HoPoSim.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace HoPoSim.Presentation.Views
{
	[Export(typeof(EinstellungenView))]
	public partial class EinstellungenView : System.Windows.Controls.UserControl
	{
		[ImportingConstructor]
		public EinstellungenView(IImportService importService, IInteractionService interactionService)
		{
			InitializeComponent();
			ImportService = importService;
			InteractionService = interactionService;
			//this.Loaded += EinstellungenView_Loaded;
		}

		private IImportService ImportService { get; set; }
		private IInteractionService InteractionService { get; set; }

		//private void EinstellungenView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		//{
		//    var dc = this.DataContext;
		//    DataContext = null;
		//    DataContext = dc;
		//}

		private string _tabName = "Einstellungen";
		public string TabName
		{
			get { return _tabName; }
		}

		private void BrowseDatabaseFolderButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = DataContext as EinstellungenViewModel;
			if (vm != null)
				BrowseFolder(vm.DatabaseDirectory, path => vm.DatabaseDirectory = path);
		}

		private void BrowseDocumentsFolderButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = DataContext as EinstellungenViewModel;
			if (vm != null)
				BrowseFolder(vm.DocumentsDirectory, path => vm.DocumentsDirectory = path);
		}

		private void BrowseSimulator3DFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = DataContext as EinstellungenViewModel;
			if (vm != null)
				BrowseFile(vm.Simulator3DFilePath, path => vm.Simulator3DFilePath = path, "exe file (*.exe)|*.exe");
		}

		private void BrowseExport3dDirectoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = DataContext as EinstellungenViewModel;
			if (vm != null)
				BrowseFolder(vm.Export3dDirectoryPath, path => vm.Export3dDirectoryPath = path);
		}

		private void BrowseExportImgDirectoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var vm = DataContext as EinstellungenViewModel;
			if (vm != null)
				BrowseFolder(vm.ExportImgDirectoryPath, path => vm.ExportImgDirectoryPath = path);
		}

		private void BrowseFolder(string startDir, Action<string> func)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() { SelectedPath = startDir };
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result == DialogResult.OK)
				func(folderBrowserDialog.SelectedPath);
		}

		private void BrowseFile(string startDir, Action<string> func, string filter = null)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog() { InitialDirectory = startDir };
			if (filter != null)
				openFileDialog.Filter = filter;
			DialogResult result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
				func(openFileDialog.FileName);
		}

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		private void NavigateButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			foreach (Expander expander in FindVisualChildren<Expander>(this))
			{
				expander.IsExpanded = false;
			}

			var button = sender as System.Windows.Controls.Button;
			if (button == null) return;
			var element = button.Tag as Expander;
			if (element != null)
			{
				element.BringIntoView();
				element.IsExpanded = true;
			}

		}

		//private void ImportFileButton_Click(object sender, RoutedEventArgs eventArgs)
		//{
		//	try
		//	{
		//		var vm = DataContext as EinstellungenViewModel;
		//		if (vm != null)
		//			ImportService.ImportExcel(vm.ImportFilePath);
		//	}
		//	catch (Exception e)
		//	{
		//		InteractionService.RaiseNotification(e.ToString(), "Import fehlgeschlagen");
		//	}
		//}
	}
}
