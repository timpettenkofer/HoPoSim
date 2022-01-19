using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class ObjectsEqualToVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null)
				return DependencyProperty.UnsetValue;
			return values.Distinct().Count() == 1? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
