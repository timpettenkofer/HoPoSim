using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Domain;
using HoPoSim.Data.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HoPoSim.Framework.Attributes;

namespace HoPoSim.Presentation.ViewModels
{
	public interface IHaveEntityProperty
	{
		object Entity
		{
			get;
		}
	}

	public class ValidableActivableEntityViewModel<T> : ValidableEntityViewModel<T>, IHaveActiveProperty where T : class, IActivableEntity, new()
	{

		public ValidableActivableEntityViewModel(T thing = null) : base(thing)
		{
		}

		public bool Active
		{
			get { return This.Active; }
			set { SetProperty(This.Active, value, () => This.Active = value); }
		}
	}


	public class ValidableEntityViewModel<T> : ValidableViewModel, IHaveIdProperty, IHaveEntityProperty, IValidable where T : class, IEntity, new()
	{
		protected T This;

		public static implicit operator T(ValidableEntityViewModel<T> thing) { return thing.This; }

		public ValidableEntityViewModel(T thing = null)
		{
			This = (thing == null) ? new T() : thing;
		}

		[ComputedProperty]
		public object Entity
		{
			get { return This; }
		}

		public int Id
		{
			get { return This.Id; }
			set { SetProperty(This.Id, value, () => This.Id = value); }
		}

		protected bool SetProperty<T1>(T1 currentValue, T1 newValue, Action DoSet,
		   [CallerMemberName] String property = null)
		{
			if (!ValidateProperty(property, newValue, This))
				return false;
			ValidateProperty(property, newValue, this, false);

			if (EqualityComparer<T1>.Default.Equals(currentValue, newValue))
				return false;
			DoSet.Invoke();
			OnPropertyChanged(property);
			return true;
		}

		public bool ValidateInstance()
		{
			return base.ValidateInstance(This, true) || this.ValidateInstance(this, false);
		}

		public string ValidationErrorsAsString()
		{
			return ErrorsContainer.FormattedValidationResults;
		}
	}
}
