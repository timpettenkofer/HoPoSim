using HoPoSim.Data.Domain;
using HoPoSim.Data.Interfaces;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Presentation.Converters;
using HoPoSim.Presentation.Properties;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;

namespace HoPoSim.Presentation.ViewModels
{
	public class DataGridIntColumn : DataGridNumericUpDownColumn
	{
		public DataGridIntColumn(string header, string propertyName, string tooltip, double minimum = 0, double maximum = 100, double interval = 1, string stringFormat= " {0} %")
		{
			Header = header;
			Binding = Binding = new Binding(propertyName) { Converter = new Int32ToNullableDoubleConverter(), UpdateSourceTrigger = UpdateSourceTrigger.LostFocus };
			Minimum = minimum;
			Maximum = maximum;
			Interval = interval;
			StringFormat = stringFormat;
		}
	}
	[Export(typeof(EinstellungenViewModel))]
	public class EinstellungenViewModel : ValidableViewModel, IConfirmNavigationRequest, IRegionMemberLifetime, INavigationAware
	{
		[ImportingConstructor]
		public EinstellungenViewModel(IGlobalConfigService service, IUnitOfWorkFactory uowFactory,
			IInteractionService interaction, IApplicationUpdateService updateService)
		{
			UnitOfWorkFactory = uowFactory;
			UnitOfWork = UnitOfWorkFactory.Create();

			_updateService = updateService;
			_configService = service;
			_interaction = interaction;
			this.confirmExitInteractionRequest = new InteractionRequest<Confirmation>();

			ItemsViewModels = new Dictionary<string, IEinstellungenDetailsViewModel>
			{
				{
					"Stapelqualitäten",
					new EinstellungenDetailsViewModel<TypeDetailsViewModel<Stapelqualität>, Stapelqualität>(_interaction, OnNotifyPropertyChanged, uow => uow.Stapelqualitäten, CommitToDatabase, GetStapelqualitäten)
				},
				{
					"Baumarten",
					new EinstellungenDetailsViewModel<TypeDetailsViewModel<BaumartParametrization>, BaumartParametrization>(_interaction, OnNotifyPropertyChanged, uow => uow.BaumartParametrizations, CommitToDatabase, GetBaumarten)
				}
			};


			CheckForUpdatesCommand = new DelegateCommand(this.CheckForUpdates, this.CanCheckForUpdates);
		}

		public IEinstellungenDetailsViewModel this[string name]
		{
			get
			{
				return ItemsViewModels.ContainsKey(name) ? ItemsViewModels[name] : null;
			}
		}

		public IDictionary<string, IEinstellungenDetailsViewModel> ItemsViewModels
		{
			get;
			private set;
		}

		IInteractionService _interaction;
		IApplicationUpdateService _updateService;


		public DelegateCommand CheckForUpdatesCommand { get; private set; }

		public IEnumerable<Stapelqualität> GetStapelqualitäten(IUnitOfWork uow)
		{
			try
			{
				return uow.Stapelqualitäten
					.GetAll(false)
					.OrderBy(b => b.Level);
			}
			catch
			{
				return Enumerable.Empty<Stapelqualität>();
			}
		}

		public IEnumerable<BaumartParametrization> GetBaumarten(IUnitOfWork uow)
		{
			try
			{
				return uow.BaumartParametrizations
					.GetAll(false)
					.OrderBy(b => b.Id);
			}
			catch
			{
				return Enumerable.Empty<BaumartParametrization>();
			}
		}

		public void TryLoadEntities()
		{
			try
			{
				foreach (var vm in ItemsViewModels.Values)
					vm.Load(UnitOfWork);
			}
			catch
			{ }
		}

		public bool CanCheckForUpdates()
		{
			return _updateService.CanCheckForUpdates();
		}

		public void CheckForUpdates()
		{
			_updateService.CheckForUpdates();
		}


		[Import]
		IExceptionMessageService ExceptionMessageService
		{ get; set; }

		protected bool CommitToDatabase()
		{
			try
			{
				UnitOfWork.Complete();
				InitializeViewModel();
				return true;
			}
			catch (Exception e)
			{
				var message = ExceptionMessageService.Translate(e.InnerException ?? e);
				_interaction.RaiseNotificationAsync(message, "Fehler");
				InitializeViewModel();
				return false;
			}
		}

		protected void OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			CommitToDatabase();
		}

		IGlobalConfigService _configService = null;
		private readonly InteractionRequest<Confirmation> confirmExitInteractionRequest;

		public IInteractionRequest ConfirmExitInteractionRequest
		{
			get { return this.confirmExitInteractionRequest; }
		}

		[Required(ErrorMessageResourceName = "Einstellungen_DatabaseDirectory_RequiredFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string DatabaseDirectory
		{
			get { return _configService.DatabaseDirectory; }
			set
			{
				ValidateProperty(value);
				_configService.DatabaseDirectory = value;
				OnPropertyChanged(nameof(DatabaseDirectory));
			}
		}

		[Required(ErrorMessageResourceName = "Einstellungen_DocumentsDirectory_RequiredFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string DocumentsDirectory
		{
			get { return _configService.DocumentsDirectory; }
			set
			{
				ValidateProperty(value);
				_configService.DocumentsDirectory = value;
				OnPropertyChanged(nameof(DocumentsDirectory));
			}
		}


		public string Simulator3DFilePath
		{
			get { return _configService.Simulator3dExeFile; }
			set
			{
				ValidateProperty(value);
				_configService.Simulator3dExeFile = value;
				OnPropertyChanged(nameof(Simulator3DFilePath));
			}
		}

		public string Export3dDirectoryPath
		{
			get { return _configService.Export3dDirectoryPath; }
			set
			{
				ValidateProperty(value);
				_configService.Export3dDirectoryPath = value;
				OnPropertyChanged(nameof(Export3dDirectoryPath));
			}
		}

		public string ExportImgDirectoryPath
		{
			get { return _configService.ExportImgDirectoryPath; }
			set
			{
				ValidateProperty(value);
				_configService.ExportImgDirectoryPath = value;
				OnPropertyChanged(nameof(ExportImgDirectoryPath));
			}
		}

		public int ExportImgWidth
		{
			get { return _configService.ExportImgWidth; }
			set
			{
				ValidateProperty(value);
				_configService.ExportImgWidth = value;
				OnPropertyChanged(nameof(ExportImgWidth));
			}
		}

		public int ExportImgHeight
		{
			get { return _configService.ExportImgHeight; }
			set
			{
				ValidateProperty(value);
				_configService.ExportImgHeight = value;
				OnPropertyChanged(nameof(ExportImgHeight));
			}
		}

		public bool ShowBaumartInformationMessages
		{
			get { return _configService.ShowBaumartInformationMessages; }
			set
			{
				ValidateProperty(value);
				_configService.ShowBaumartInformationMessages = value;
				OnPropertyChanged(nameof(ShowBaumartInformationMessages));
			}
		}

		public bool KeepAlive
		{
			get
			{
				return true;
			}
		}

		void IConfirmNavigationRequest.ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
		{
			if (HasErrors)
			{
				this.confirmExitInteractionRequest.Raise(
					new Confirmation { Content = Resources.ConfirmNavigateAwayFromSettingsMessage, Title = Resources.ConfirmNavigateAwayFromSettingsTitle },
					c => { continuationCallback(c.Confirmed); });
			}
			else
			{
				continuationCallback(true);
			}
		}

		private void InitializeViewModel()
		{
			UnitOfWork = UnitOfWorkFactory.Create();
			TryLoadEntities();
			OnPropertyChanged(null);
		}
		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			InitializeViewModel();
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			// We always want a new instance of settings, so we should return false to indicate
			// this doesn't handle the navigation request.
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			UnitOfWork.Dispose();
			UnitOfWork = null;

			// Intentionally not implemented
			return;
		}

		protected IUnitOfWorkFactory UnitOfWorkFactory;
		protected IUnitOfWork UnitOfWork;
	}
}
