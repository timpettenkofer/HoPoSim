using HoPoSim.Data.Interfaces;
using Prism.Commands;
using System;

namespace HoPoSim.Presentation.Filter
{
    public interface IDateFilterDescription : IFilterDescription
    {
        DateTime? From { get; set; }
        DateTime? To { get; set; }
    }

    public class DateFilterDescription<T> : FilterDescription<T>, IDateFilterDescription where T : class, IHaveNameProperty
    {
        public DateFilterDescription(string displayName, string propertyName)
            : base(displayName, propertyName, new Tuple<DateTime?, DateTime?>(null, null))
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

        public DateTime? From
        {
            get { return SelectedValue != null ? (SelectedValue as Tuple<DateTime?, DateTime?>).Item1 : null; }

            set
            {
                SelectedValue = new Tuple<DateTime?, DateTime?>(value, (SelectedValue is Tuple<DateTime?, DateTime?> ? (SelectedValue as Tuple<DateTime?, DateTime?>).Item2 : null));
				RaisePropertyChanged(nameof(From));
            }
        }

        public DateTime? To
        {
            get { return SelectedValue != null ? (SelectedValue as Tuple<DateTime?, DateTime?>).Item2 : null; }

            set
            {
                SelectedValue = new Tuple<DateTime?, DateTime?>((SelectedValue is Tuple<DateTime?, DateTime?> ? (SelectedValue as Tuple<DateTime?, DateTime?>).Item1 : null), value);
				RaisePropertyChanged(nameof(To));
            }
        }

        public override bool IsMatch(object candidate)
        {
            var tuple = SelectedValue as Tuple<DateTime?, DateTime?>;
            if (tuple != null)
            {
                var from = tuple.Item1;
                var to = tuple.Item2;
                var date = GetEntityValue(candidate) as DateTime?;
                if (from != null && (!date.HasValue || date < from)) return false;
                if (to != null && (!date.HasValue || date > to)) return false;
                return true;
            }
            return true;
        }
    }
}
