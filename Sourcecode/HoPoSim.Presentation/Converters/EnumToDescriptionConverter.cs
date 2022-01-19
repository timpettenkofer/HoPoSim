using HoPoSim.Framework.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class EnumToDescriptionConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Enum myEnum = (Enum)value;
			string description = myEnum.GetEnumDescription();
			return description;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}
}
