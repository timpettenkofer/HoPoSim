using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Framework.Converters
{
	public class BooleanOrConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return values.Any(v => v != DependencyProperty.UnsetValue && (bool)v == true);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}