using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Domain;
using HoPoSim.Presentation.Validation;
using System.Collections.ObjectModel;
using System.Linq;
using HoPoSim.Presentation.Extensions;

namespace HoPoSim.Presentation.ViewModels
{
	public class SimulationDetailsViewModel : ValidableEntityViewModel<SimulationConfiguration>, IHaveNameProperty
	{
		public SimulationDetailsViewModel(SimulationConfiguration data = null) : base(data)
		{
			var results = This.Results.Select(r => new SimulationResultsViewModel(r)).ToList();
			Results = new ObservableCollection<SimulationResultsViewModel>(results);
		}

		public bool IsValid()
		{
			return Iterationanzahl > 0 && !string.IsNullOrEmpty(Data);
		}

		public string ToJsonConfiguration(bool indented)
		{
			var dao = This.ToDAO();
			return IPC.DAO.Serializer<IPC.DAO.SimulationConfiguration>.ToJSON(dao, indented);
		}

		public string ToJsonData()
		{
			var dao = IPC.DAO.Serializer<IPC.DAO.SimulationData>.FromJSON(This.Data);
			return IPC.DAO.Serializer<IPC.DAO.SimulationData>.ToJSON(dao, false);
		}

		public string Name
		{
			get { return This.Name; }
			set { SetProperty(This.Name, value, () => This.Name = value); }
		}

		public string Bemerkungen
		{
			get { return This.Bemerkungen; }
			set { SetProperty(This.Bemerkungen, value, () => This.Bemerkungen = value); }
		}

		public int Iterationanzahl
		{
			get { return This.Iterationanzahl; }
			set { SetProperty(This.Iterationanzahl, value, () => This.Iterationanzahl = value); }
		}

		public int TimeOutPeriod
		{
			get { return This.TimeOutPeriod; }
			set { SetProperty(This.TimeOutPeriod, value, () => This.TimeOutPeriod = value); }
		}

		public int Seed
		{
			get { return This.Seed; }
			set { SetProperty(This.Seed, value, () => This.Seed = value); }
		}

		public int Quality
		{
			get { return This.Quality; }
			set { SetProperty(This.Quality, value, () => This.Quality = value); }
		}

		public int FotooptikQuality
		{
			get { return This.FotooptikQuality; }
			set { SetProperty(This.FotooptikQuality, value, () => This.FotooptikQuality = value); }
		}

		public int SimulationDataId
		{
			get { return This.SimulationDataId; }
			set { SetProperty(This.SimulationDataId, value, () => This.SimulationDataId = value); }
		}

		public SimulationData SimulationData
		{
			get { return This.SimulationData; }
		}

		[IsValidSerializedSimulationData]
		public string Data
		{
			get { return This.Data; }
			set
			{
				if (SetProperty(This.Data, value, () => This.Data = value))
					OnPropertyChanged(nameof(Stammanzahl));
			}
		}

		public int Stammanzahl
		{
			get
			{
				return Data != null? GetDataCount(Data) : 0;
			}
		}

		public static int GetDataCount(string input)
		{
			try
			{
				var data = IPC.DAO.Serializer<HoPoSim.IPC.DAO.SimulationData>.FromJSON(input);
				return data.Stämme.Count();
			}
			catch
			{
				return 0;
			}
		}

		public SimulationResultsViewModel SelectedResults { get; set; }

		public bool HasResults
		{
			get { return Results.Any(); }
		}

		public void ClearResults()
		{
			This.Results.Clear();
			Results.Clear();
			OnPropertyChanged(nameof(Results));
		}

		public void AddResult(SimulationResults results)
		{
			This.Results.Add(results);
			Results.Add(new SimulationResultsViewModel(results));
			OnPropertyChanged(nameof(Results));
		}

		public ObservableCollection<SimulationResultsViewModel> Results
		{
			get; set;
		}
	}
}
