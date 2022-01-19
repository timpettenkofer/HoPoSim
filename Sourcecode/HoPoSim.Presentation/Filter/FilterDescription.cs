using HoPoSim.Data.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.ComponentModel;
using System.Reflection;

namespace HoPoSim.Presentation.Filter
{
    #region Interface
    public interface IFilterDescription
    {
        bool IsDefaultValue { get; }
        bool IsMatch(object candidate);

        string DisplayName { get; }
        object DefaultValue { get; }
        object SelectedValue { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }
    #endregion

    public class FilterDescription<T> : BindableBase, IFilterDescription where T : class, IHaveNameProperty
    {
        public FilterDescription(string displayName, string propertyName, object defaultValue)
        {
            DisplayName = displayName;
            DefaultValue = defaultValue;
            PropertyName = propertyName;
            PropertyInfo = typeof(T).GetProperty(PropertyName);
            ResetFilter = new DelegateCommand(() => SelectedValue = DefaultValue, () => !IsDefaultValue);
        }

        public string PropertyName { get; }
        public string DisplayName { get; }
        public object DefaultValue { get; }
        private PropertyInfo PropertyInfo { get; }
        public DelegateCommand ResetFilter { get; }

        public virtual bool IsDefaultValue
        {
            get
            {
                return DefaultValue == null ? SelectedValue == null : DefaultValue.Equals(SelectedValue);
            }
        }

        public virtual bool IsMatch(object candidate)
        {
            return SelectedValue.Equals(GetEntityValue(candidate));
        }

        protected virtual object GetEntityValue(object candidate)
        {
            return PropertyInfo.GetValue(candidate);
        }

        public override string ToString()
        {
            return $"{DisplayName} = {SelectedValue}";
        }

        public object SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                _selectedValue = value;
                RaiseCanExecuteChanged();
				RaisePropertyChanged(nameof(SelectedValue));
            }
        }
        private object _selectedValue;

        public virtual void RaiseCanExecuteChanged()
        {
            ResetFilter.RaiseCanExecuteChanged();
			RaisePropertyChanged(nameof(IsDefaultValue));
        }
    }
}
