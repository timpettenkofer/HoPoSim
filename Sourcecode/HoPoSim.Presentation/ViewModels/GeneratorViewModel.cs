using System.ComponentModel.Composition;
using HoPoSim.Framework;
using HoPoSim.Presentation.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using HoPoSim.Presentation.Filter;
using Prism.Commands;
using HoPoSim.IO;
using HoPoSim.Presentation.Helpers;
using System;
using HoPoSim.Data.Domain;
using System.Diagnostics;
using HoPoSim.Presentation.Controls;
using MahApps.Metro.Controls.Dialogs;
using HoPoSim.Data.Interfaces;
using HoPoSim.Framework.Interfaces;
using HoPoSim.IO.Interfaces;
using HoPoSim.IO.Serialization;
using HoPoSim.Data.Generator;
using HoPoSim.Data.Model;

namespace HoPoSim.Presentation.ViewModels
{
	[Export(typeof(GeneratorViewModel))]
    public class GeneratorViewModel : EntityViewModel<GeneratorDataDetailsViewModel>
    {
        [ImportingConstructor]
        public GeneratorViewModel(IUnitOfWorkFactory uowFactory, IGlobalConfigService config, IGenerator generator, IStammlisteWriter writer) :
            base(uowFactory)
        {
            Config = config;
			Generator = generator;
			StammlisteWriter = writer;
			ExportCommand = new DelegateCommand(this.Export, this.CanExport);
			CopyCommand = new DelegateCommand(this.Copy, this.CanCopy);
		}

		public IGlobalConfigService Config { get; }
		public IGenerator Generator { get; }
		private IStammlisteWriter StammlisteWriter { get; }
		public DelegateCommand ExportCommand { get; set; }
		public DelegateCommand CopyCommand { get; set; }

		public bool CanExport()
		{
			return SelectedItem != null;
		}

		public void Export()
		{
			if (SelectedItem.HasValidQuotas())
				ExportToFile();
			else
				InteractionService.RaiseNotificationAsync("Das Zusammenzählen von einigen Klassenanteile stimmt nicht mit der Gesamtstammanzahl überein.",
					"Bitte prüfen Sie Ihre Eingabedaten");
				
		}

		private void ExportToFile()
		{
			var file = DocumentHandler.GetSavedAsFileFromUserSelection(StammlisteReader.FileExtensionFilter, GetDefaultExportFileName());
			if (file != null)
			{
				try
				{
					var input = (GeneratorData)SelectedItem.Entity;
					var data = new Stammdaten();
					Generator.Generate(input, data);
					ApplicationFolders.CopyTemplate(ApplicationTemplates.Stammdaten.FilePath, file);
					StammlisteWriter.WriteStammdaten(file, input, data);
					Process.Start(file);
				}
				catch (Exception exception)
				{
					InteractionService.RaiseNotificationAsync(exception.GetBaseException().Message, 
						"Fehler beim Erstellen der Datei");
				}
			}
		}

		private string GetDefaultExportFileName()
		{
			return $"Stammliste_{SelectedItem.Stammanzahl}_Stck_{DateTime.Now:dd-MM-yyy}.xlsx";
		}

		public bool CanCopy()
		{
			return SelectedItem != null;
		}

		public void Copy()
		{
			var data = new GeneratorDataDetailsViewModel(UnitOfWork, SelectedItem, OnNotifyPropertyChanged)
			{
				Name = Entities.GenerateUniqueName($"{SelectedItem.Name}_Kopie", "D2")
			};
			InternalAdd(data);
		}

		public override void OnDeletingEntity(GeneratorDataDetailsViewModel entity)
		{
			// EF Core delete cascade does not operate here force delete all dependent entities
			var data = entity.Entity as GeneratorData;
			UnitOfWork.DeleteEntity(data.Distribution);
			foreach (var v in data.Abholzigkeit.Values)
				UnitOfWork.DeleteEntity(v);
			UnitOfWork.DeleteEntity(data.Abholzigkeit);
			foreach (var v in data.Krümmung.Values)
				UnitOfWork.DeleteEntity(v);
			UnitOfWork.DeleteEntity(data.Krümmung);
			foreach (var v in data.Ovalität.Values)
				UnitOfWork.DeleteEntity(v);
			UnitOfWork.DeleteEntity(data.Ovalität);
			foreach (var v in data.Durchmesser.Values)
				UnitOfWork.DeleteEntity(v);
			UnitOfWork.DeleteEntity(data.Durchmesser);
		}

		public override void RaiseCanExecuteChanged()
		{
			base.RaiseCanExecuteChanged();
			ExportCommand.RaiseCanExecuteChanged();
			CopyCommand.RaiseCanExecuteChanged();
		}

		public override IList<GeneratorDataDetailsViewModel> LoadEntities()
		{
			var data = UnitOfWork.GeneratorData.GetAllIncludingAll()
				.Select(d => new GeneratorDataDetailsViewModel(UnitOfWork, d, OnNotifyPropertyChanged))
				.ToList();
			data.ForEach(a => a.PropertyChanged += OnNotifyPropertyChanged);
			return data;
		}

		protected override EntityFilter<GeneratorDataDetailsViewModel> CreateFilter()
		{
			var filter = base.CreateFilter();
			filter.GetSearchableFields = e => $"{e.Name} {e.Bemerkungen}";
			return filter;
		}

		protected override IEnumerable<IFilterDescription> EntitiesFilterDescriptions
		{
			get
			{
				return new IFilterDescription[] {
					new NumberFilterDescription<GeneratorDataDetailsViewModel>("Länge", nameof(GeneratorDataDetailsViewModel.Länge)),
					new NumberFilterDescription<GeneratorDataDetailsViewModel>("Stammanzahl", nameof(GeneratorDataDetailsViewModel.Stammanzahl)),
				};
			}
		}

		#region Sorting
		public override IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> CustomSortDescriptions
		{
			get
			{
				return new Dictionary<string, IEnumerable<SortDescription>>
				{
					{ "Name", new[] {new SortDescription(nameof(GeneratorDataDetailsViewModel.Name), ListSortDirection.Ascending)}},
					{ "Länge", new[] {new SortDescription(nameof(GeneratorDataDetailsViewModel.Länge), ListSortDirection.Descending)}},
					{ "Stammanzahl", new[] {new SortDescription(nameof(GeneratorDataDetailsViewModel.Stammanzahl), ListSortDirection.Descending)}}
				};
			}
		}
		#endregion


		#region Status and Infos
		public override string StatusBarInfo
		{
			get
			{
				return FilterToString();
			}
		}

		public override string DetailsHeader(GeneratorDataDetailsViewModel e)
        {
            return e != null ? $"Konfigurationsdaten '{e.Name}'" : "Eigenschaften";
        }
        #endregion

        #region Commands
        public override void Add()
		{
			var dialog = new KlasseanzahlDialog();
			dialog.OnClose += (o, args) => 
			{
				if (dialog.Result != MessageDialogResult.Canceled)
					CreateGeneratorData(dialog.DurchmesserklasseAnzahl, dialog.AbholzigkeitsklasseAnzahl, dialog.KrümmungsklasseAnzahl, dialog.OvalitätsklasseAnzahl);
			};
			dialog.ShowCustomDialog();
		}

		private void CreateGeneratorData(int durchmesser, int abholzigkeit, int krümmung, int ovalität)
		{
			var generatorData = new GeneratorData(durchmesser, abholzigkeit, krümmung, ovalität);
			var data = new GeneratorDataDetailsViewModel(UnitOfWork, generatorData, OnNotifyPropertyChanged)
			{
				Name = NewEntityName,
				Stammanzahl = 100,
				Länge = 3.0f,
				StammfußAnteil = 0,
				StammfußMinHeight = 20,
				StammfußMaxHeight = 30
			};
			InternalAdd(data);
		}

		private void InternalAdd(GeneratorDataDetailsViewModel data)
		{
			data.PropertyChanged += OnNotifyPropertyChanged;
			Entities.Add(data);
			UnitOfWork.GeneratorData.Add(data);
			CommitToDatabase();
			SelectedItem = data;
		}

		public override string NewEntityName => Entities.GenerateUniqueName("Daten", "D1");
		#endregion
	}
}
