using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Presentation.Helpers;
using HoPoSim.Presentation.Views;
using Prism.Commands;
using Prism.Regions;
using System.ComponentModel.Composition;
using System.Windows;

namespace HoPoSim
{
	[Export]
	public partial class Shell
	{
		[ImportingConstructor]
		public Shell(IGlobalConfigService config, IInteractionService interactionService, IRegionManager regionManager, INavigationHelper navHelper)
		{
			//navigationService.Navigated += NavigationService_Navigated;
			GoBackCommand = new DelegateCommand(this.GoBack, this.CanGoBack);
			GoForwardCommand = new DelegateCommand(this.GoForward, this.CanGoForward);
			InteractionService = interactionService;
			_config = config;
			DataContext = this;
			InitializeComponent();
			_regionManager = regionManager;
			_navHelper = navHelper;
		}

		INavigationHelper _navHelper;

		//private void NavigationService_Navigated(object sender, RegionNavigationEventArgs e)
		//{
		//    GoBackCommand.RaiseCanExecuteChanged();
		//    GoForwardCommand.RaiseCanExecuteChanged();
		//}

		private IGlobalConfigService _config;

		#region Navigation
		private IRegionManager _regionManager;

		public DelegateCommand GoBackCommand { get; private set; }
		public DelegateCommand GoForwardCommand { get; private set; }

		private void GoBack()
		{
			var navigationService = _regionManager.Regions[RegionNames.MainContentRegion].NavigationService;
			if (navigationService.Journal.CanGoBack)
			{
				navigationService.Journal.GoBack();
			}
		}

		private bool CanGoBack()
		{
			return true; // navigationService.Journal.CanGoBack;
		}

		private void GoForward()
		{
			var navigationService = _regionManager.Regions[RegionNames.MainContentRegion].NavigationService;
			if (navigationService.Journal.CanGoForward)
			{
				navigationService.Journal.GoForward();
			}
		}

		private bool CanGoForward()
		{
			return true; // navigationService.Journal.CanGoForward;
		}
		#endregion

		public IInteractionService InteractionService { get; }

		public string ApplicationTitle
		{
			get
			{
				return $"HoPoSim"; // {Properties.Resources.ApplicationOwner}";
			}
		}

		public string PublishVersion
		{
			get
			{
				return ApplicationHelper.PublishVersion;
			}
		}


		private void AboutButtonClick(object sender, RoutedEventArgs e)
		{
			_navHelper.ShowAbout();
		}

		private void HelpButtonClick(object sender, RoutedEventArgs e)
		{
			_navHelper.OpenHelp();
		}

		private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_navHelper.NavigateTo(typeof(HomeView), null);
		}
	}

}

