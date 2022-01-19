using HoPoSim.Data.Interfaces;
using Prism.Commands;
using System;

namespace HoPoSim.Presentation.Filter
{
	public interface INumberFilterDescription : IFilterDescription
	{
		double? From { get; set; }
		double? To { get; set; }
	}

	public class NumberFilterDescription<T> : FilterDescription<T>, INumberFilterDescription where T : class, IHaveNameProperty
	{
		public NumberFilterDescription(string displayName, string propertyName)
			: base(displayName, propertyName, new Tuple<double?, double?>(null, null))
		{
			ResetToFilter = new DelegateCommand(() => To = null, () => To != null);
			ResetFromFilter = new DelegateCommand(() => From = null, () => From != null);
		}
		public DelegateCommand ResetToFilter { get; }
		public DelegateCommand ResetFromFilter { get; }

		public override void RaiseCanExecuteChanged()
		{
			base.RaiseCanExecuteChanged();
			ResetFromFilter.RaiseCanExecuteChanged();
			ResetToFilter.RaiseCanExecuteChanged();
		}

		public double? From
		{
			get { return SelectedValue != null ? (SelectedValue as Tuple<double?, double?>).Item1 : null; }

			set
			{
				SelectedValue = new Tuple<double?, double?>(value, (SelectedValue is Tuple<double?, double?> ? (SelectedValue as Tuple<double?, double?>).Item2 : null));
				RaisePropertyChanged(nameof(From));
			}
		}

		public double? To
		{
			get { return SelectedValue != null ? (SelectedValue as Tuple<double?, double?>).Item2 : null; }

			set
			{
				SelectedValue = new Tuple<double?, double?>((SelectedValue is Tuple<double?, double?> ? (SelectedValue as Tuple<double?, double?>).Item1 : null), value);
				RaisePropertyChanged(nameof(To));
			}
		}

		public override bool IsMatch(object candidate)
		{
			var tuple = SelectedValue as Tuple<double?, double?>;
			if (tuple != null)
			{
				var from = tuple.Item1;
				var to = tuple.Item2;
				var number = Convert.ToDouble(GetEntityValue(candidate));
				if (from != null && (number < from)) return false;
				if (to != null && (number > to)) return false;
				return true;
			}
			return true;
		}
	}
}
