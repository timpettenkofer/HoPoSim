using HoPoSim.Data.Domain;
using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class DistributionToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var data = values[0] as GeneratorData;
			if (data == null)
				return DependencyProperty.UnsetValue;

			var rootDistribution = values[1] as Distribution;

			if (rootDistribution == null)
				return DependencyProperty.UnsetValue;

			bool checkDirectChildrenOnly = parameter is bool && (bool)parameter;
			var isUninitialized = data.HasUninitializedQuotas(rootDistribution, checkDirectChildrenOnly);
			if (isUninitialized)
				return Brushes.Orange;
			var isValid = data.HasValidQuotas(rootDistribution, checkDirectChildrenOnly);
			return isValid ? Brushes.Green : Brushes.Red;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
