using HoPoSim.Data.Domain;
using HoPoSim.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HoPoSim.Presentation.ViewModels
{
	public class DistributionDetailsViewModel : ValidableEntityViewModel<Distribution>
	{
		public DistributionDetailsViewModel(Distribution d, PropertyChangedEventHandler commit)
			: base(d)
		{
			_isSelected = RangeId == 1;
			Children = d.Children
				.OrderBy(c => c.RangeId)
				.Select(c => new DistributionDetailsViewModel(c, commit))
				.ToList();
			Commit = commit;
			PropertyChanged += DistributionDetailsViewModel_PropertyChanged;
			
		}
		PropertyChangedEventHandler Commit { get; }

		private void DistributionDetailsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Commit(sender, e);
		}

		public int RangeId
		{
			get { return This.RangeId; }
		}

		public IList<DistributionDetailsViewModel> Children { get; }

		public double Percent
		{
			get { return This.Percent; }
			set
			{
				if (SetProperty(This.Percent, value, () => This.Percent = value))
				{
					This.Absolute = This.ToAbsolute();
					OnPropertyChanged(nameof(Absolute));
					UpdateChildrenTotal();
				}
			}
		}

		public int Absolute
		{
			get { return This.Absolute; }
			set
			{
				if (SetProperty(This.Absolute, value, () => This.Absolute = value))
				{
					This.Percent = This.ToPercent();
					UpdateChildrenTotal();
					OnPropertyChanged(nameof(Percent));
				}
			}
		}

		public int Total
		{
			get { return This.Total; }
			set
			{
				if (SetProperty(This.Total, value, () => This.Total = value))
				{
					This.Absolute = This.ToAbsolute();
					OnPropertyChanged(nameof(Absolute));
					UpdateChildrenTotal();
				}
			}
		}

		private void UpdateChildrenTotal()
		{
			foreach (var child in Children)
				child.Total = Absolute;
		}

		[ComputedProperty]
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		private bool _isSelected;

	}
}
