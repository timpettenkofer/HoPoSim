using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HoPoSim.Presentation.Extensions
{
	public class EnumBindingSourceExtension : MarkupExtension
	{
		public BindingBase EnumType { get; set; }

		public EnumBindingSourceExtension() { }


		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			DependencyObject targetObject;
			DependencyProperty targetProperty;

			if (target != null && target.TargetObject is DependencyObject && target.TargetProperty is DependencyProperty)
			{
				targetObject = (DependencyObject)target.TargetObject;
				targetProperty = (DependencyProperty)target.TargetProperty;
			}
			else
			{
				return this; // magic
			}

			BindingOperations.SetBinding(targetObject, EnumBindingSourceExtension.EnumTypeProperty, EnumType);
			Type enumType = targetObject.GetValue(EnumTypeProperty) as Type;


			if (null == enumType)
				throw new InvalidOperationException("The EnumType must be specified.");


			Type actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
			Array enumValues = Enum.GetValues(actualEnumType);

			if (actualEnumType == enumType)
			{
				return PrependEmptyToArray(enumValues);
			}

			Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
			enumValues.CopyTo(tempArray, 1);
			return PrependEmptyToArray(tempArray);
		}

		private Array PrependEmptyToArray(Array array)
		{
			var values = new object[array.Length + 1];
			values[0] = string.Empty;
			Array.Copy(array, 0, values, 1, array.Length);
			return values;
		}

		private static DependencyProperty EnumTypeProperty = DependencyProperty.RegisterAttached("EnumType", typeof(Type)
					   , typeof(EnumBindingSourceExtension), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
	}
}
