using HoPoSim.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public abstract class MultiSelectionFilterDescription<T> : FilterDescription<T>
        where T : class, IHaveNameProperty
    {
        public MultiSelectionFilterDescription(string displayName, string propertyName, object defaultValue)
            :base(displayName, propertyName, defaultValue)
        {
            base.PropertyChanged += EntityFilterDescription_PropertyChanged;
        }

        private void EntityFilterDescription_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(SelectedValue).Equals(e.PropertyName))
				RaisePropertyChanged(nameof(SelectedItems));
        }

        public override abstract bool IsDefaultValue { get; }

        public override bool IsMatch(object candidate)
        {
            var candidateValue = GetEntityValue(candidate);
            return candidateValue != null && SelectedItems.Keys.Contains(candidateValue);
        }

        public Dictionary<object, string> SelectedItems
        {
            get { return (Dictionary<object, string>)SelectedValue; }
            set
            {
                SelectedValue = value;
				RaisePropertyChanged(nameof(SelectedItems));
				RaisePropertyChanged(nameof(IsDefaultValue));
            }
        }

        public override string ToString()
        {
            return SelectedItems != null ? 
                $"{DisplayName} = {string.Join(",", SelectedItems.Keys)}" :
                $"{DisplayName} = null";
        }
    }
}
