using HoPoSim.Data.Interfaces;
using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Framework.Unity;
using HoPoSim.IO;
using HoPoSim.IO.Interfaces;
using HoPoSim.IO.Serialization;
using HoPoSim.IPC.DAO;
using HoPoSim.IPC.WCF;
using HoPoSim.Presentation.Extensions;
using HoPoSim.Presentation.Filter;
using HoPoSim.Presentation.Helpers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static HoPoSim.IPC.WCF.Message;

namespace HoPoSim.Presentation.ViewModels
{
	[Export(typeof(SimulationViewModel))]
	public class SimulationViewModel : EntityViewModel<SimulationDetailsViewModel>
	{
		[ImportingConstructor]
		public SimulationViewModel(IUnitOfWorkFactory uowFactory, INavigationHelper navigationHelper, IUnityController unity, IExportService exportService, IGlobalConfigService config) :
			base(uowFactory)
		{
			ExportService = exportService;
			NavigationHelper = navigationHelper;
			Config = config;
			UnityController = unity;
			UnityController.SimulationResultsReceived += UnityController_SimulationResultsReceived;
			UnityController.SimulationLogReceived += UnityController_SimulationLogReceived;
			UnityController.SimulationProcessFinished += UnityController_SimulationProcessFinished;
			UnityController.ProcessClosedByUser += UnityController_ProcessClosedByUser;

			StartCommand = new DelegateCommand(this.Start, this.CanStart);
			StopCommand = new DelegateCommand(this.Stop, this.CanStop);
			ExportCommand = new DelegateCommand(this.Export, this.CanExport);
			VisualizeCommand = new DelegateCommand(this.Visualize, this.CanVisualize);
			ExportImagesCommand = new DelegateCommand<string>(this.ExportImages, this.CanExportImages);
			Export3dCommand = new DelegateCommand<string>(this.Export3d, this.CanExport3d);

			ClearOutput = new DelegateCommand(this.ClearMessages);
		}


		private IUnityController UnityController { get; }
		private Process SimulationProcess { get; set; }
		public INavigationHelper NavigationHelper { get; }
		private IExportService ExportService { get; }
		private IGlobalConfigService Config { get; }

		public DelegateCommand StartCommand { get; set; }
		public DelegateCommand StopCommand { get; set; }
		public DelegateCommand ExportCommand { get; set; }
		public DelegateCommand VisualizeCommand { get; set; }
		public DelegateCommand<string> ExportImagesCommand { get; set; }
		public DelegateCommand<string> Export3dCommand { get; set; }

		public DelegateCommand ClearOutput { get; set; }

		public override string NewEntityName => Entities.GenerateUniqueName("Simulation", "D4");

		public override void Add()
		{
			throw new NotImplementedException();
		}

		public override bool CanDelete()
		{
			return base.CanDelete() && !UnityController.HasCurrentlyActiveSharedProcess;
		}

		public override void RaiseCanExecuteChanged()
		{
			base.RaiseCanExecuteChanged();
			DeleteCommand.RaiseCanExecuteChanged();
			StartCommand.RaiseCanExecuteChanged();
			StopCommand.RaiseCanExecuteChanged();
			ExportCommand.RaiseCanExecuteChanged();
			VisualizeCommand.RaiseCanExecuteChanged();
			Export3dCommand.RaiseCanExecuteChanged();
			ExportImagesCommand.RaiseCanExecuteChanged();
		}

		public bool CanStop()
		{
			return UnityController.HasCurrentlyActiveSharedProcess;
		}

		public void Stop()
		{
			InteractionService.ExecuteIfUserConfirmed("Simulation beenden",
					"Möchten Sie die laufende Simulation wirklich beenden?",
					() => { StopProcess(); },
					() => { },
					"Beenden", "Abbrechen");
		}

		public bool CanExport()
		{
			return SelectedItem != null && SelectedItem.HasResults;
		}

		public void Export()
		{
			var file = DocumentHandler.GetSavedAsFileFromUserSelection(StammlisteReader.FileExtensionFilter, GetDefaultExportFileName("xlsx"));
			if (file != null)
			{
				try
				{
					ExportService.Copy(SelectedItem.SimulationData?.SourceFile, file);
					var exports = new[]
					{
						new ExportTarget
						{
							DataTable = Converter.AsDataTable(SelectedItem.SimulationData),
							SheetName = ApplicationTemplates.Stammdaten.SimulationDataSheet,
							RegionName = ApplicationTemplates.Stammdaten.SimulationDataRegion,
							ExportHeaders = true,
							ShowSheet = true
						},
						new ExportTarget
						{
							DataTable = Converter.AsResultsDataTable(SelectedItem),
							SheetName = ApplicationTemplates.Stammdaten.AusgabenSheet,
							RegionName = ApplicationTemplates.Stammdaten.AusgabenRegion,
							ExportHeaders = true,
							ShowSheet = true
						}
					};
					ExportService.ExportExcel(file, exports);
					Process.Start(file);
				}
				catch (Exception exception)
				{
					InteractionService.RaiseNotificationAsync(exception.GetBaseException().Message,
						"Fehler beim Erstellen der Datei");
				}
			}
		}

		private string GetDefaultExportFileName(string extension)
		{
			return ReplaceInvalidChars($"HoPoSim_{SelectedItem.Name}_{DateTime.Now:dd-MM-yyy}.{extension}");
		}

		private static string ReplaceInvalidChars(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}

		#region Visualization
		public bool CanVisualize()
		{
			return !UnityController.HasCurrentlyActiveSharedProcess && SelectedItem?.SelectedResults != null;
		}

		public void Visualize()
		{
			var results = SelectedItem?.SelectedResults;
			if (results == null) return;
			var config = results.ToJsonResults();
			ExecuteProcessOnUserConfirmation(() => StartProcess(CommandCode.VISUALIZATION, config, null, ProcessWindowStyle.Normal));
		}

#if DEBUG
		private void StartProcess(CommandCode code, string config, string settings, ProcessWindowStyle windowStyle)
		{
			ClearMessages();
			Process process = null;
			var data = GenerateSimulationData();
			SendMessage(process, code, data, config, settings);
			RaiseCanExecuteChanged();
		}
#else
		private async void StartProcess(CommandCode code, string config, string settings, ProcessWindowStyle windowStyle)
		{
			ClearMessages();
			var process = await StartUnityProcess(windowStyle);
			var data = GenerateSimulationData();
			SendMessage(process, code, data, config, settings);
			RaiseCanExecuteChanged();
		}
#endif

		private void StopProcess()
		{
			UnityController.CloseSharedProcess();
			RaiseCanExecuteChanged();
		}

		// UNITY EDITOR
		//private async Task<Process> StartUnityProcess_UnityEditor(ProcessWindowStyle windowStyle)
		//{
		//	return null;
		//}

		private async Task<Process> StartUnityProcess(ProcessWindowStyle windowStyle)
		{
			//var process = await UnityController.StartProcess(windowStyle);
			var process = await UnityController.StartSharedProcess(windowStyle);
			if (process == null)
			{
				InteractionService.RaiseNotificationAsync("Die Verbindung mit der HoPoSim 3D Simulator kann nicht erstellt werden.\nBitte prüfen Sie, dass der eingestellte 3D Simulator Pfad korrekt ist,\nschließen Sie das HoPoSim Fenster und versuchen Sie erneut, die Simulation zu starten.");
				return null;
			}
			return process;
		}
#endregion

#region Exports
		public bool CanExportImages(string format)
		{
			return !UnityController.HasCurrentlyActiveSharedProcess && SelectedItem?.SelectedResults != null;
		}
		public void ExportImages(string format)
		{
			var results = SelectedItem?.SelectedResults;
			if (results == null) return;
			var settings = BuildImageExportSettings();
			var config = results.ToJsonResults();
			ExecuteProcessOnUserConfirmation(() => StartProcess(CommandCode.EXPORT_IMG, config, settings, ProcessWindowStyle.Hidden));
		}

		private string BuildImageExportSettings()
		{
			var dir = DocumentHandler.GetTargetFolderFromUserSelection(Config.ExportImgDirectoryPath);
			var settings = new ImageExportSettings() { Path = dir, Format = ExportFormat.PNG, Width = Config.ExportImgWidth, Height = Config.ExportImgHeight };
			return Serializer<ImageExportSettings>.ToJSON(settings, false);
		}


		public bool CanExport3d(string format)
		{
			return !UnityController.HasCurrentlyActiveSharedProcess && SelectedItem?.SelectedResults != null;
		}
		public void Export3d(string format)
		{
			var results = SelectedItem?.SelectedResults;
			if (results == null) return;
			var settings = Build3dExportSettings(format);
			if (settings == null) return;
			var config = results.ToJsonResults();
			ExecuteProcessOnUserConfirmation(() => StartProcess(CommandCode.EXPORT_3D, config, settings, ProcessWindowStyle.Hidden));
		}

		private string Build3dExportSettings(string format)
		{
			string extension = null;
			string filter = null;
			ExportFormat exportFormat = ExportFormat.OBJ;

			switch (format)
			{
				case "fbx":
					{
						extension = "fbx";
						filter = "FBX Files|*.fbx;";
						exportFormat = ExportFormat.FBX;
						break;
					}
				case "obj":
					{
						extension = "obj";
						filter = "Wavefront Files|*.obj;";
						exportFormat = ExportFormat.OBJ;
						break;
					}
				default:
					InteractionService.RaiseNotificationAsync($"Unsupported file format {format}");
					return null;
			}

			var defaultFileName = Path.Combine(Config.Export3dDirectoryPath, GetDefaultExportFileName(extension));
			var file = DocumentHandler.GetSavedAsFileFromUserSelection(filter, defaultFileName);
			if (file == null) return null;
			var settings = new ExportSettings() { Path = file, Format = exportFormat };
			return Serializer<ExportSettings>.ToJSON(settings, false);
		}
#endregion


#region Simulation
		public bool CanStart()
		{
			return !UnityController.HasCurrentlyActiveSharedProcess && SelectedItem != null && SelectedItem.IsValid();
		}

		public void Start()
		{
			ExecuteProcessOnUserConfirmation(() => StartSimulation());
		}

		// "Einige Ergebnisse liegen für diese Simulation bereits vor. Möchten Sie die Simulation fortsetzen oder erneut starten?"

		//private void StartSimulation()
		//{
		//	SelectedItem.ClearResults();
		//	var data = GenerateSimulationData();
		//	var config = GenerateSimulationConfig();
		//	SendMessage(CommandCode.SIMULATION, data, config);
		//}

		

		// UNITY EDITOR
		//private void StartSimulationProcess()
		//{
		//	Process process = null;
		//	ClearMessages();
		//	var data = GenerateSimulationData();
		//	var config = GenerateSimulationConfig();
		//	SendMessage(process, CommandCode.SIMULATION, data, config, null);
		//	RaiseCanExecuteChanged();
		//}

		private void StartSimulation()
		{
			if (SelectedItem.HasResults)
				InteractionService.ExecuteIfUserConfirmed("Simulation starten",
					"Einige Ergebnisse liegen für diese Simulation bereits vor. Möchten Sie die Simulation fortsetzen oder erneut starten?",
					() => { SelectedItem.ClearResults(); StartSimulationProcess(); },
					() => { StartSimulationProcess(); },
					"Neu starten", "Fortsetzen");
			else
				StartSimulationProcess();
		}

		private void StartSimulationProcess()
		{
			SelectedItem.Seed = new Random().Next(1, 10000);
			StartProcess(CommandCode.SIMULATION, GenerateSimulationConfig(), null, ProcessWindowStyle.Minimized);
		}

		private string GenerateSimulationData()
		{
			return SelectedItem?.ToJsonData();
		}

		private string GenerateSimulationConfig()
		{
			return SelectedItem?.ToJsonConfiguration(false);
		}

		private void ExecuteProcessOnUserConfirmation(Action action)
		{
			if (UnityController.HasCurrentlyActiveSharedProcess)
				InteractionService.ExecuteIfUserConfirmed("Neue Simulation starten",
					"Eine Simulation läuft bereits. Möchten Sie die benden?",
					() => action());
			else
				action();
		}

		public void SendMessage(Process process, CommandCode cmdCode, string data, string config, string settings)
		{
			var processId = process != null ? process.Id : -1;
			var request = new Message { ProcessId = processId, Command = cmdCode, Data = data, Configuration = config, Settings = settings };
			UnityController.SendMessage(request);
		}

		private void UnityController_SimulationLogReceived(object sender, Message msg)
		{
			Log(msg, "LOG");
		}

		private void UnityController_SimulationResultsReceived(object sender, Message msg)
		{
			ProcessResults(msg);
			RaiseCanExecuteChanged();
		}

		private void UnityController_SimulationProcessFinished(object sender, Message e)
		{
			StopProcess();
		}

		private void UnityController_ProcessClosedByUser(object sender, EventArgs e)
		{
			RaiseCanExecuteChanged();
		}

		private void ProcessResults(Message msg)
		{
			try
			{
				var response = Serializer<SimulationResults>.FromJSON(msg.Data);
				CollectResult(response.FromDAO(), UnitOfWork?? UnitOfWorkFactory.Create());
			}
			catch (Exception e)
			{
				Log(msg, $"ERROR while processing results. {e.GetBaseException().Message}");
			}
		}

		private void CollectResult(Data.Domain.SimulationResults response, IUnitOfWork uow)
		{
			var simulation = Entities?.FirstOrDefault(s => s.Id == response.SimulationConfigurationId);
			if (simulation != null)
			{
				simulation.AddResult(response);
			}
			else
			{
				var simulationConfiguration = uow.SimulationConfigurations.Get(response.SimulationConfigurationId);
				simulationConfiguration.Results.Add(response);
			}
			uow.Complete();
		}

		private void Log(Message response, string msg = null)
		{
			Messages += $"{response.Data}{Environment.NewLine}";
			OnPropertyChanged(nameof(Messages));
			Console.WriteLine($"[{response.ProcessId}]: {msg} - {response.Data}");
		}

		private void ClearMessages()
		{
			Messages = string.Empty;
			OnPropertyChanged(nameof(Messages));
		}

		public string Messages
		{
			get;
			set;
		}
#endregion

#region Sorting
		public override IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> CustomSortDescriptions
		{
			get
			{
				return new Dictionary<string, IEnumerable<SortDescription>>
				{
					{ "Name", new[] {new SortDescription(nameof(SimulationDetailsViewModel.Name), ListSortDirection.Ascending)}},
					{ "Iterationanzahl", new[] {new SortDescription(nameof(SimulationDetailsViewModel.Iterationanzahl), ListSortDirection.Descending)}},
				};
			}
		}
#endregion

#region Filters
		public override string StatusBarInfo
		{
			get
			{
				return FilterToString();
			}
		}
		

		protected override IEnumerable<IFilterDescription> EntitiesFilterDescriptions
		{
			get
			{
				return new IFilterDescription[] {
					new NumberFilterDescription<SimulationDetailsViewModel>("Iterationanzahl", nameof(SimulationDetailsViewModel.Iterationanzahl)),
				};
			}
		}

		protected override EntityFilter<SimulationDetailsViewModel> CreateFilter()
		{
			var filter = base.CreateFilter();
			filter.GetSearchableFields = e => $"{e.Name} {e.Bemerkungen}";
			return filter;
		}
#endregion

		public override IList<SimulationDetailsViewModel> LoadEntities()
		{
			var simulations = UnitOfWork.SimulationConfigurations.GetAll()
				.Select(s => new SimulationDetailsViewModel(s))
				.ToList();
			simulations.ForEach(a => a.PropertyChanged += OnNotifyPropertyChanged);
			return simulations;
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				OnPropertyChanged(nameof(IsBusy));
			}
		}
		private bool _isBusy = false;

		public override string DetailsHeader(SimulationDetailsViewModel e)
		{
			return e != null ? $"Simulation '{e.Name}'" : "Eigenschaften";
		}
	}
}
