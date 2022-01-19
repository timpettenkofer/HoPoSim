using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using HoPoSim.Presentation.Extensions;
using HoPoSim.IO;
using System.Data;
using System;
using HoPoSim.Presentation.Helpers;
using Prism.Commands;
using System.Diagnostics;
using System.Threading;
using HoPoSim.Presentation.Filter;
using System.ComponentModel;
using HoPoSim.Framework;
using HoPoSim.Data.Model;
using HoPoSim.Presentation.Views;
using HoPoSim.Data.Interfaces;
using HoPoSim.Framework.Interfaces;
using HoPoSim.IO.Interfaces;
using HoPoSim.Data.Domain;
using HoPoSim.Framework.Attributes;

namespace HoPoSim.Presentation.ViewModels
{
	[Export(typeof(SimulationDataViewModel))]
    public class SimulationDataViewModel : EntityViewModel<SimulationDataDetailsViewModel>
    {
		[ImportingConstructor]
		public SimulationDataViewModel(IStammlisteReader reader, IUnitOfWorkFactory uowFactory, IGlobalConfigService config, INavigationHelper navigationHelper) :
			base(uowFactory)
		{
			StammlisteReader = reader;
			LoadCommand = new DelegateCommand(this.Load, this.CanLoad);
			OpenCommand = new DelegateCommand(this.Open, this.CanOpen);
			UpdateCommand = new DelegateCommand(this.Update, this.CanUpdate);
			SimulationCommand = new DelegateCommand(this.Simulate, this.CanSimulate);

			Config = config;
			DocumentHandler = new DocumentHandler(() => ApplicationFolders.CombinePath(config, "SimulationData"), config, InteractionService);
			NavigationHelper = navigationHelper;
		}

		public IGlobalConfigService Config { get; }
		public DocumentHandler DocumentHandler { get; }
		public INavigationHelper NavigationHelper { get; }

		public DelegateCommand LoadCommand { get; set; }
		public DelegateCommand OpenCommand { get; set; }
		public DelegateCommand UpdateCommand { get; set; }
		public DelegateCommand SimulationCommand { get; set; }

		private IStammlisteReader StammlisteReader { get; set; }

		public override string NewEntityName => Entities.GenerateUniqueName("Daten", "D1");

		public override void Add()
		{
			SimulationDataDetailsViewModel entity = CreateNewSimulationData();
			entity.PropertyChanged += OnNotifyPropertyChanged;
			UnitOfWork.SimulationData.Add(entity);
			CommitToDatabase();
			Entities.Add(entity);
			SelectedItem = entity;
		}

		private SimulationDataDetailsViewModel CreateNewSimulationData()
		{
			return new SimulationDataDetailsViewModel(Stapequalitäten) {
				Name = NewEntityName,
				Polterlänge =20.0f, Steigungswinkel=45, Seitenspiegelung = 50.0f, Zufallsspiegelung = false, Polterunterlage = true,
				WoodDensity =520.0f, WoodFriction=0.55f, Rindenbeschädigungen=0,
				Krümmungsvarianten=50, Baumart = Baumarten.First(), Stapelqualität = Stapequalitäten.First()
			};
		}

		public void StartThread(ThreadStart method)
		{
			Thread t = new Thread(method);
			t.Start();
		}

		public bool CanSimulate()
		{
			return SelectedItem != null && SelectedItem.IsValid();
		}

		public void Simulate()
		{
			var configuration = GenerateActualSimulation();
			if (configuration != null)
			{
				GenerateIdealSimulation(configuration.Name);
				NavigationHelper.NavigateTo(typeof(SimulationView), configuration.Id);
			}
		}

		private SimulationConfiguration GenerateIdealSimulation(string name)
		{
			var json = SelectedItem.ToJSON(true, true);
			var comment = "Datensatz für die Ermittlung des Abweichungsfaktors vom 'idealen' Polter-Raumvolumen.\n" +
				"In diesem Datumssatz sind alle Stämme zu perfekten Rundzylindern umgerechnet (Ovalität=1, Krümmung=0, Abholzigkeit=0)\n" + 
				"und alle Durchmesser sind vom originalen Mittendurchmesser abgeleitet.";
			return GenerateSimulation($"{name} (ideal)", json, comment);
		}

		private SimulationConfiguration GenerateActualSimulation()
		{
			var json = SelectedItem.ToJSON(true, false);
			return GenerateSimulation(GenerateName(SelectedItem.Name), json);
		}

		private SimulationConfiguration GenerateSimulation(string name, string json_data, string comment = null)
		{
			try
			{
				SimulationDetailsViewModel simulation = new SimulationDetailsViewModel()
				{
					Name = name,
					Iterationanzahl = 1,
					TimeOutPeriod = 300,
					Quality = 6,
					FotooptikQuality = 3,
					Seed = new Random().Next(1, 10000),
					Data = json_data,
					Bemerkungen = comment,
					SimulationDataId = SelectedItem.EntityId

				};
				// ensure no duplicate
				var entity = UnitOfWork.SimulationConfigurations.FirstOrDefault(s => s.Name == simulation.Name);
				if (entity != null)
					UnitOfWork.SimulationConfigurations.Remove(entity);
				// add new simulation
				UnitOfWork.SimulationConfigurations.Add(simulation);
				CommitToDatabase();

				entity = UnitOfWork.SimulationConfigurations.FirstOrDefault(s => s.Name == simulation.Name);
				return entity;
			}
			catch(Exception e)
			{
				InteractionService.RaiseNotificationAsync($"Die Simulation konnte nicht generiert werden:\r\n{e.Message}", "Fehler");
				return null;
			}
		}

		private string GenerateName(string name)
		{
			var entities = UnitOfWork.SimulationConfigurations.GetAll(true);
			return entities.GenerateUniqueName($"{name}", e => e.Name, "D1");
		}

		public bool CanLoad()
		{
			return SelectedItem != null;
		}

		public void Load()
		{
			if (SelectedItem.Stammdaten != null)
				InteractionService.ExecuteIfUserConfirmed("Neue Stammdaten laden",
					"Möchten Sie den ausgewählten Stammdaten unwiderruflich überschreiben?",
					() => StartThread(InternalLoad));
			else
				StartThread(InternalLoad);

		}

		private void InternalLoad()
		{
			var file = GetTargetFileFromUser();
			if (file == null)
				return;
			InternalLoad(file);
		}

		private void InternalLoad(string file)
		{
			try
			{
				IsBusy = true;
				var sourcefile = DocumentHandler.CopyToDirectory(file);
				var dt = ImportDataTableFromFile(sourcefile);
				SelectedItem.Stammdaten = dt ?? throw new ArgumentException("Stammdaten nich vorhanden.");
				SelectedItem.SourceFile = sourcefile;
			}
			catch (Exception e)
			{
				var message = $"Die Daten konnten nicht geladen werden.\n" +
					"Bitte überprüfen Sie, ob die Datei existiert und ob sie gültige Stammdaten enthält.\n" +
					e.Message;
				InteractionService.RaiseNotificationAsync(message, "Fehler beim Laden der Datei");
			}
			finally
			{
				IsBusy = false;
				RaiseCanExecuteChanged();
			}
		}

		public bool CanUpdate()
		{
			return SelectedItem?.SourceFile != null;
		}

		public void Update()
		{
			StartThread(() => InternalLoad(SelectedItem.SourceFile));
		}

		private string GetTargetFileFromUser()
		{
			return DocumentHandler.GetFileToBeAddedFromUserSelection(HoPoSim.IO.Serialization. StammlisteReader.FileExtensionFilter);
		}

		private Stammdaten ImportDataTableFromFile(string file)
		{
			var dt = StammlisteReader.ReadStammdaten(file);
			return dt;
		}

		public bool CanOpen()
		{
			return SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.SourceFile);
		}

		public void Open()
		{
			StartThread(InternalOpenReadOnly);
		}

		private void InternalOpenReadOnly()
		{
			try
			{
				IsBusy = true;
				var startInfo = new ProcessStartInfo()
				{
					FileName = "excel.exe",
					Arguments = $"/r {SelectedItem.SourceFile}"
				};
				Process.Start(startInfo);
			}
			catch(Exception e)
			{
				var message = $"Die Datei '{SelectedItem.SourceFile}' konnte nicht geöffnet werden.\n" +
					"Bitte überprüfen Sie, ob die Datei existiert und ob sie zugreifbar ist.\n" +
					e.Message;
				InteractionService.RaiseNotificationAsync(message, "Fehler beim Öffnen der Datei");
			}
			finally
			{
				IsBusy = false;
			}
		}

		
		public override void RaiseCanExecuteChanged()
		{
			base.RaiseCanExecuteChanged();
			LoadCommand.RaiseCanExecuteChanged();
			OpenCommand.RaiseCanExecuteChanged();
			UpdateCommand.RaiseCanExecuteChanged();
			SimulationCommand.RaiseCanExecuteChanged();
		}



		public override string DetailsHeader(SimulationDataDetailsViewModel e)
		{
			return e != null ? $"Simulation Data '{e.Name}'" : "Eigenschaften";
		}

		#region Sorting
		public override IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> CustomSortDescriptions
		{
			get
			{
				return new Dictionary<string, IEnumerable<SortDescription>>
				{
					{ "Name", new[] {new SortDescription(nameof(SimulationDataDetailsViewModel.Name), ListSortDirection.Ascending)}},
					{ "Stammanzahl", new[] {new SortDescription(nameof(SimulationDataDetailsViewModel.Stammanzahl), ListSortDirection.Descending)}},
					{ "Poltertiefe", new[] {new SortDescription(nameof(SimulationDataDetailsViewModel.Tiefe), ListSortDirection.Descending)}},
				};
			}
		}
		#endregion

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
					new NumberFilterDescription<SimulationDataDetailsViewModel>("Stammanzahl", nameof(SimulationDataDetailsViewModel.Stammanzahl)),
					new NumberFilterDescription<SimulationDataDetailsViewModel>("Poltertiefe", nameof(SimulationDataDetailsViewModel.Tiefe)),
				};
			}
		}

		protected override EntityFilter<SimulationDataDetailsViewModel> CreateFilter()
		{
			var filter = base.CreateFilter();
			filter.GetSearchableFields = e => $"{e.Name} {e.Bemerkungen}";
			return filter;
		}

		public override IList<SimulationDataDetailsViewModel> LoadEntities()
		{
			InitializeEntitiesRelations();

			var data = UnitOfWork.SimulationData.GetAll()
				.Select(e => new SimulationDataDetailsViewModel(Stapequalitäten, e))
				.ToList();
			data.ForEach(a => a.PropertyChanged += OnNotifyPropertyChanged);
			return data;
		}

		public void InitializeEntitiesRelations()
		{
			Baumarten = UnitOfWork.BaumartParametrizations.GetAll(false).OrderBy(b => b.Id);
			OnPropertyChanged(nameof(Baumarten));
			Stapequalitäten = UnitOfWork.Stapelqualitäten.GetAll(false).OrderBy(b => b.Level);
			OnPropertyChanged(nameof(Stapequalitäten));
		}

		new void OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			base.OnNotifyPropertyChanged(sender, e);
			if (e.PropertyName == nameof(SimulationDataDetailsViewModel.Baumart))
			{
				var message = $"Die Baumartparametrisierung wurde auf {SelectedItem.Baumart.Name} gesetzt.\n\n" +
							"Die folgenden Parameter werden für diese Baumart verwendet:\n\n" +
							$"- Wurzelanlauf/Stammfuß:\n" +
							$"Wurzelanlauf Berücksichtung = {SelectedItem.Baumart.IncludeRoots}\n" +
							$"Anzahl  Wurzelknoten = {SelectedItem.Baumart.MinRootFlareNumber} bis {SelectedItem.Baumart.MaxRootFlareNumber}\n" +
							$"Wurzelradius Multiplikator = {SelectedItem.Baumart.MinRootRadiusMultiplier} bis {SelectedItem.Baumart.MaxRootRadiusMultiplier}\n\n" +
							$"- Astigkeit:\n" +
							$"Aststummel Berücksichtung = {SelectedItem.Baumart.IncludeBranches}\n" +
							$"Anteil Stämme mit Aststummel = {SelectedItem.Baumart.BranchStubTrunkProportion}%\n" +
							$"Lange der Aststummel = {SelectedItem.Baumart.BranchStubMinLength}mm bis {SelectedItem.Baumart.BranchStubMaxLength}mm\n" +
							$"Höhe der Aststummel (bezogen auf die Länge des Stammes) = {SelectedItem.Baumart.BranchStubMinHeight} bis {SelectedItem.Baumart.BranchStubMaxHeight}\n" +
							$"Neigung der Aststummel [0,1] = {SelectedItem.Baumart.BranchStubAverageAngle}\n" +
							$"Aststummelradius Multiplikator = {SelectedItem.Baumart.BranchStubRadiusMultiplier}\n" +
							$"Anzahl von Aststummel per Laufmeter = {SelectedItem.Baumart.BranchStubNumberPerMeter}\n";
				//$"Modellierungsrauschen Einfluss = {SelectedItem.Baumart.MinNoiseStrength} bis {SelectedItem.Baumart.MaxNoiseStrength}\n";
				InteractionService.RaiseDismissibleNotificationAsync(message, () => Config.ShowBaumartInformationMessages, b => Config.ShowBaumartInformationMessages = b);
			}
		}

		[ComputedProperty]
		public IEnumerable<BaumartParametrization> Baumarten
		{
			get; private set;
		}

		[ComputedProperty]
		public IEnumerable<Stapelqualität> Stapequalitäten
		{
			get; private set;
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
	}
}
