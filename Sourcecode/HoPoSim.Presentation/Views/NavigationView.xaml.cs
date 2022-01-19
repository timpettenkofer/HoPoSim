using HoPoSim.Framework;
using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Views
{
	public class NavigationItem
	{
		public string Title { get; set; }
		public Uri Uri { get; set; }
		public string Icon { get; set; } = "None";
		public string IconMaterial { get; set; } = "None";
		public IEnumerable<NavigationItem> Children { get; set; }
		public bool HasChildren() { return Children != null; }
		public Action Callback { get; set; }
	}

	[Export]
	public partial class NavigationView : UserControl
	{
		[ImportingConstructor]
		public NavigationView(IRegionManager regionManager)
		{
			InitializeTabs();
			InitializeComponent();
			this.Loaded += NavigationView_Loaded;
			_regionManager = regionManager;
			_regionManager.Regions[RegionNames.MainContentRegion].ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
		}

		IRegionManager _regionManager;

		private void NavigationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			//buttonList.SelectedItem = null;
		}

		public static Uri homeViewUri = new Uri("HoPoSim.Presentation.Views.HomeView", UriKind.Relative);
		public static Uri generatorViewUri = new Uri("HoPoSim.Presentation.Views.GeneratorView", UriKind.Relative);
		public static Uri simulationViewUri = new Uri("HoPoSim.Presentation.Views.SimulationView", UriKind.Relative);
		public static Uri statistikenViewUri = new Uri("HoPoSim.Presentation.Views.StatistikenView", UriKind.Relative);
		public static Uri ausgabeViewUri = new Uri("HoPoSim.Presentation.Views.AusgabeView", UriKind.Relative);
		public static Uri simulationDataViewUri = new Uri("HoPoSim.Presentation.Views.SimulationDataView", UriKind.Relative);
		public static Uri einstellungenViewUri = new Uri("HoPoSim.Views.SettingsView", UriKind.Relative);
		public static Uri einstellungenViewUri2 = new Uri(typeof(EinstellungenView).FullName, UriKind.Relative);



		private void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
				return;
			var activeUri = _regionManager.Regions[RegionNames.MainContentRegion].ActiveViews
				.Select(v => new Uri(v.GetType().FullName, UriKind.Relative))
				.FirstOrDefault();

			NavigationItem navItem = Tabs.FirstOrDefault(t => t.Uri == activeUri || (t.HasChildren() && t.Children.Any(c => c.Uri == activeUri)));
			buttonList.SelectedItem = navItem;
		}


		private void InitializeTabs()
		{

			var items = new NavigationItem[]
			{
				new NavigationItem() { Title = "Home", Uri = homeViewUri, Icon="HomeOutline" },
				new NavigationItem() { Title = "Generator", Uri = generatorViewUri, Icon="Beaker" },

				new NavigationItem() { Title = "Eingabe", Uri = simulationDataViewUri, Icon="InboxArrowDown" },
				new NavigationItem() { Title = "Simulation", Uri = simulationViewUri, Icon="Printer3d"},
				
				//new NavigationItem() { Title = "Ausgabe", Uri = ausgabeViewUri, Icon="InboxArrowUp" }, // , Children=dbItems },
				new NavigationItem() { Title = "Einstellungen", Uri = einstellungenViewUri, Icon="Settings",
				Callback = () => { _regionManager.RequestNavigate(RegionNames.SettingsRegion, einstellungenViewUri2);} },

			}.ToList();

			Tabs = items;


			//foreach (var item in items)
			//    buttonList.Items.Add(new TabItem() { Header = item.Title, Tag = item.Uri });
		}

		public IList<NavigationItem> Tabs { get; set; }

		private void RequestNavigate(NavigationItem navItem)
		{
			buttonList.SelectedItem = navItem;
			_regionManager.RequestNavigate(RegionNames.MainContentRegion, navItem.Uri);
			if (navItem.Callback != null)
				navItem.Callback();
		}

		private void Border_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var border = sender as Border;
			var item = border.Tag as NavigationItem;
			TriggerNavigationItem(item);
		}

		private void TriggerNavigationItem(NavigationItem item)
		{
			if (item.HasChildren())
			{
				var ddb = buttonList
					.FindChildren<DropDownButton>(true)
					.FirstOrDefault(b => b.DataContext == item);
				ddb.IsExpanded = true;
				return;
			}
			RequestNavigate(item);
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var button = sender as Button;
			var item = button.Tag as NavigationItem;
			RequestNavigate(item);
		}

		private void buttonList_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				var tabControl = sender as TabControl;
				var navItem = tabControl.SelectedItem as NavigationItem;
				TriggerNavigationItem(navItem);
			}

		}
	}
}
