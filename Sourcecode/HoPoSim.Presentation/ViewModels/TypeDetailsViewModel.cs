using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HoPoSim.Presentation.ViewModels
{
	public interface ITypeDetailsViewModel<T> : INotifyPropertyChanged
	{
		T Source
		{
			get;
		}
	}

	public class TypeDetailsViewModel<T> : ValidableViewModel, ITypeDetailsViewModel<T>
	{
		public TypeDetailsViewModel(T item)
		{
			Source = item;
		}

		public T Source
		{
			get; private set;
		}

		public object this[string propertyName]
		{
			get
			{
				var value = typeof(T).GetProperty(propertyName).GetValue(Source);
				return value;
			}
			set
			{
				var currentValue = typeof(T).GetProperty(propertyName).GetValue(Source);
				SetProperty<object>(currentValue, value, () => typeof(T).GetProperty(propertyName).SetValue(Source, value), propertyName);
			}
		}

		protected bool SetProperty<T1>(T1 currentValue, T1 newValue, Action DoSet,
		   [CallerMemberName] String property = null)
		{
			ValidateProperty(property, newValue, Source);

			if (EqualityComparer<T1>.Default.Equals(currentValue, newValue))
				return false;
			DoSet.Invoke();
			OnPropertyChanged(property);
			return true;
		}
	}
}
