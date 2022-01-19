using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Domain;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using HoPoSim.Framework.Attributes;

namespace HoPoSim.Presentation.ViewModels
{
	public class GeneratorDataDetailsViewModel : ValidableEntityViewModel<GeneratorData>, IHaveNameProperty
	{
		public GeneratorDataDetailsViewModel(IUnitOfWork unitOfWork, GeneratorData data, PropertyChangedEventHandler commit) : base(data)
		{
			UnitOfWork = unitOfWork;
			Commit = commit;
			InitializeDataViews();
		}

		public GeneratorDataDetailsViewModel(IUnitOfWork unitOfWork, GeneratorDataDetailsViewModel source, PropertyChangedEventHandler commit) 
			: base(source.This.Clone())
		{
			UnitOfWork = unitOfWork;
			Commit = commit;
			InitializeDataViews();
		}

		private IUnitOfWork UnitOfWork;
		PropertyChangedEventHandler Commit { get; }


		private void InitializeDataViews()
		{
			Distribution = new DistributionDetailsViewModel(This.Distribution, SubPropertyChanged);

			var durchmesser = This.Durchmesser.Values.Select(k => new DurchmesserDetailsViewModel(k, Commit)).OrderBy(v => v.RangeId).ToList();
			DurchmesserView = new ObservableCollection<DurchmesserDetailsViewModel>(durchmesser);

			var abholzigkeit = This.Abholzigkeit.Values.Select(k => new AbholzigkeitDetailsViewModel(k, Commit)).OrderBy(v => v.RangeId).ToList();
			AbholzigkeitView = new ObservableCollection<AbholzigkeitDetailsViewModel>(abholzigkeit);

			var krümmung = This.Krümmung.Values.Select(k => new KrümmungDetailsViewModel(k, Commit)).OrderBy(v => v.RangeId).ToList();
			KrümmungView = new ObservableCollection<KrümmungDetailsViewModel>(krümmung);

			var ovalität = This.Ovalität.Values.Select(k => new OvalitätDetailsViewModel(k, Commit)).OrderBy(v => v.RangeId).ToList();
			OvalitätView = new ObservableCollection<OvalitätDetailsViewModel>(ovalität);
		}

		[ComputedProperty]
		public DistributionDetailsViewModel Distribution { get; private set; }

		#region Durchmesser
		[ComputedProperty]
		public IEnumerable<DistributionDetailsViewModel> DurchmesserDistributions
		{
			get
			{
				if (_durchmesserDistributions == null)
					_durchmesserDistributions = Distribution.Children.OrderBy(c => c.RangeId).ToList();
				return _durchmesserDistributions;
			}
		}
		private IEnumerable<DistributionDetailsViewModel> _durchmesserDistributions;

		[ComputedProperty]
		public double DurchmesserPercentSum
		{
			get { return DurchmesserDistributions.Sum(d => d.Percent); }
		}

		[ComputedProperty]
		public double DurchmesserAbsoluteSum
		{
			get { return DurchmesserDistributions.Sum(d => d.Absolute); }
		}

		[ComputedProperty]
		public DistributionDetailsViewModel SelectedDurchmesser
		{
			get
			{
				return DurchmesserDistributions.First(d => d.IsSelected);
			}
		}
		#endregion

		#region Abholzigkeit
		[ComputedProperty]
		public IEnumerable<DistributionDetailsViewModel> AbholzigkeitDistributions
		{
			get { return SelectedDurchmesser.Children; }
		}

		[ComputedProperty]
		public DistributionDetailsViewModel SelectedAbholzigkeit
		{
			get
			{
				return SelectedDurchmesser.Children.First(c => c.IsSelected);
			}
		}

		[ComputedProperty]
		public double AbholzigkeitPercentSum
		{
			get { return SelectedDurchmesser.Children.Sum(d => d.Percent); }
		}

		[ComputedProperty]
		public double AbholzigkeitAbsoluteSum
		{
			get { return SelectedDurchmesser.Children.Sum(d => d.Absolute); }
		}
		#endregion

		#region Krümmung
		[ComputedProperty]
		public IEnumerable<DistributionDetailsViewModel> KrümmungDistributions
		{
			get { return SelectedAbholzigkeit.Children; }
		}

		[ComputedProperty]
		public DistributionDetailsViewModel SelectedKrümmung
		{
			get
			{
				return SelectedAbholzigkeit.Children.First(c => c.IsSelected);
			}
		}

		[ComputedProperty]
		public double KrümungPercentSum
		{
			get { return SelectedAbholzigkeit.Children.Sum(d => d.Percent); }
		}

		[ComputedProperty]
		public double KrümmungAbsoluteSum
		{
			get { return SelectedAbholzigkeit.Children.Sum(d => d.Absolute); }
		}
		#endregion

		#region Ovalität
		[ComputedProperty]
		public IEnumerable<DistributionDetailsViewModel> OvalitätDistributions
		{
			get { return SelectedKrümmung.Children; }
		}

		[ComputedProperty]
		public double OvalitätPercentSum
		{
			get { return SelectedKrümmung.Children.Sum(d => d.Percent); }
		}

		[ComputedProperty]
		public double OvalitätAbsoluteSum
		{
			get { return SelectedKrümmung.Children.Sum(d => d.Absolute); }
		}
		#endregion

		#region Data Views
		[ComputedProperty]
		public ObservableCollection<DurchmesserDetailsViewModel> DurchmesserView
		{
			get; set;
		}

		[ComputedProperty]
		public ObservableCollection<AbholzigkeitDetailsViewModel> AbholzigkeitView
		{
			get; set;
		}

		[ComputedProperty]
		public ObservableCollection<KrümmungDetailsViewModel> KrümmungView
		{
			get; set;
		}

		[ComputedProperty]
		public ObservableCollection<OvalitätDetailsViewModel> OvalitätView
		{
			get; set;
		}
		#endregion

		public bool HasValidQuotas()
		{
			return This.HasValidQuotas();
		}

		private void SubPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is DistributionDetailsViewModel)
			{
				NotifyAll();
			}
			// refreshes klasse button border brush
			OnPropertyChanged(nameof(Entity));
			Commit(sender, e);
		}
		

		public string Name
		{
			get { return This.Name; }
			set { SetProperty(This.Name, value, () => This.Name = value); }
		}

		public int Stammanzahl
		{
			get { return This.Stammanzahl; }
			set
			{
				if (SetProperty(This.Stammanzahl, value, () => This.Stammanzahl = value))
				{
					Distribution.Total = value;
					// refreshes all distributions
					OnPropertyChanged(nameof(Entity));
					NotifyAll();
				}
			}
		}

		public int StammfußAnteil
		{
			get { return This.StammfußAnteil; }
			set { SetProperty(This.StammfußAnteil, value, () => This.StammfußAnteil = value); }
		}

		public int StammfußMinHeight
		{
			get { return This.StammfußMinHeight; }
			set { SetProperty(This.StammfußMinHeight, value, () => This.StammfußMinHeight = value); }
		}

		public int StammfußMaxHeight
		{
			get { return This.StammfußMaxHeight; }
			set { SetProperty(This.StammfußMaxHeight, value, () => This.StammfußMaxHeight = value); }
		}


		private void NotifyAll()
		{
			OnPropertyChanged(nameof(SelectedDurchmesser));
			OnPropertyChanged(nameof(SelectedAbholzigkeit));
			OnPropertyChanged(nameof(SelectedKrümmung));
			OnPropertyChanged(nameof(AbholzigkeitDistributions));
			OnPropertyChanged(nameof(KrümmungDistributions));
			OnPropertyChanged(nameof(OvalitätDistributions));

			OnPropertyChanged(nameof(DurchmesserPercentSum));
			OnPropertyChanged(nameof(DurchmesserAbsoluteSum));
			OnPropertyChanged(nameof(AbholzigkeitPercentSum));
			OnPropertyChanged(nameof(AbholzigkeitAbsoluteSum));
			OnPropertyChanged(nameof(KrümungPercentSum));
			OnPropertyChanged(nameof(KrümmungAbsoluteSum));
			OnPropertyChanged(nameof(OvalitätPercentSum));
			OnPropertyChanged(nameof(OvalitätAbsoluteSum));
		}

		public float Länge
		{
			get { return This.Länge; }
			set { SetProperty(This.Länge, value, () => This.Länge = value); }
		}

		public float LängeVariation
		{
			get { return This.LängeVariation; }
			set { SetProperty(This.LängeVariation, value, () => This.LängeVariation = value); }
		}

		public string Bemerkungen
		{
			get { return This.Bemerkungen; }
			set { SetProperty(This.Bemerkungen, value, () => This.Bemerkungen = value); }
		}
	}
}

