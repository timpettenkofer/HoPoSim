using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class VisibilityAndConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return values.All(v => v is Visibility && (Visibility)v == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}