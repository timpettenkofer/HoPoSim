using System;
using System.Globalization;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class SingleToNullableDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is float)
			{
				return System.Convert.ToDouble(value);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is double)
			{
				return System.Convert.ToSingle(value);
			}

			return null;
		}
	}
}
