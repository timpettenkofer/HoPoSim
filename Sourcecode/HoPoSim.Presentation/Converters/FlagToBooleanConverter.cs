using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
    public class FlagToBooleanConverter : IValueConverter
    {
        // Convert flag [value] to boolean, true if matches [param]
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return false;
            if (param == DependencyProperty.UnsetValue || param == null)
                return false;
            return ((int)value & (int)param);
        }

        // Convert boolean to enum, returning [param] if true
        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
            //return (bool)value ? param : Binding.DoNothing;
        }
    }
}
