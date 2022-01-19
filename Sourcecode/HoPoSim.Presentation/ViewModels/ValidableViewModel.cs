using HoPoSim.Data;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Presentation.Validation;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HoPoSim.Presentation.ViewModels
{
	public abstract class ValidableViewModel : InteractionViewModel, INotifyPropertyChanged, INotifyDataErrorInfo
	{
		protected ValidableViewModel() { }

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// sets the storage to the value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="storage"></param>
		/// <param name="value"></param>
		/// <param name="propertyName"></param>
		/// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			//ValidateProperty(propertyName, value);

			if (object.Equals(storage, value)) return false;

			storage = value;
			OnPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var eventHandler = this.PropertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <typeparam name="T">The type of the property that has a new value</typeparam>
		/// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
		protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			var propertyName = Microsoft.Practices.Prism.Mvvm.PropertySupport.ExtractPropertyName(propertyExpression);
			this.OnPropertyChanged(propertyName);
		}

		#endregion

		#region INotifyDataErrorInfo Members

		private ErrorsContainer errorsContainer;

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

		public ErrorsContainer ErrorsContainer
		{
			get
			{
				if (errorsContainer == null)
				{
					errorsContainer =
						new ErrorsContainer(pn => OnErrorsChanged(pn));
				}

				return this.errorsContainer;
			}
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return ErrorsContainer.GetErrors(propertyName);
		}

		public bool HasErrors
		{
			get { return ErrorsContainer.HasErrors; }
		}

		protected virtual void OnErrorsChanged(string propertyName)
		{
			//var eventHandler = this.PropertyChanged;
			//if (eventHandler != null)
			//{
			//    eventHandler(this, new PropertyChangedEventArgs(propertyName));
			//}
		}

		protected void OnErrorsChanged<T>(Expression<Func<T>> propertyExpression)
		{
			var propertyName = Microsoft.Practices.Prism.Mvvm.PropertySupport.ExtractPropertyName(propertyExpression);
			OnErrorsChanged(propertyName);
		}

		#endregion

		#region property validation

		protected bool ValidateProperty(object value, [CallerMemberName] string propertyName = null, object instance = null)
		{
			return ValidateProperty(propertyName, value, instance ?? this);
		}

		public bool ValidateInstance(object instance, bool clearPreviousErrors = true)
		{
			var results = new List<ValidationResult>();
			var context = new ValidationContext(instance, ServiceLocator.Current, null);

			var properties = instance.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(ValidationAttribute), false));
			foreach (var propertyInfo in properties)
			{
				var propertyName = propertyInfo.Name;
				var value = propertyInfo.GetValue(instance);
				ValidateProperty(propertyName, value, instance, clearPreviousErrors);
			}
			return !HasErrors;
		}

		protected bool ValidateProperty(string propertyName, object value, object instance, bool clearPreviousErrors = true)
		{
			if (clearPreviousErrors)
				ErrorsContainer.ClearErrors(propertyName);

			PropertyInfo propertyInfo = (instance ?? this).GetType().GetProperty(propertyName);
			var results = new List<ValidationResult>();
			var context = new ValidationContext(this, ServiceLocator.Current, null);
			IEnumerable<ValidationAttribute> attributes = GetValidationAttributes(propertyInfo);

			bool isValid = Validator.TryValidateValue(value, context, results, attributes);
			if (results.Any())
				ErrorsContainer.SetErrors(propertyName, results.Select(r => r.ErrorMessage));
			OnErrorsChanged(propertyName);
			return isValid;
		}

		private static IEnumerable<ValidationAttribute> GetValidationAttributes(PropertyInfo propertyInfo)
		{
			var attributes = propertyInfo?.GetCustomAttributes(true).OfType<ValidationAttribute>();
			return attributes != null ? attributes : Enumerable.Empty<ValidationAttribute>();
		}

		public void SetValidationError(string propertyName, Exception e)
		{
			var errorMessage = ExceptionMessageService.Translate(e);
			ErrorsContainer.ClearErrors(propertyName);
			ErrorsContainer.SetErrors(propertyName, new[] { errorMessage });
			OnErrorsChanged(propertyName);
		}

		IExceptionMessageService ExceptionMessageService
		{
			get
			{
				if (_exceptionMessageService == null)
					_exceptionMessageService = new ExceptionMessageService();
				return _exceptionMessageService;
			}
		}
		IExceptionMessageService _exceptionMessageService;


		#endregion

	}
}
