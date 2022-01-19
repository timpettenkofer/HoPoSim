using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class CombineVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return values.All(v => v != DependencyProperty.UnsetValue && (Visibility)v == Visibility.Visible) ?
				Visibility.Visible :
				Visibility.Hidden;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
