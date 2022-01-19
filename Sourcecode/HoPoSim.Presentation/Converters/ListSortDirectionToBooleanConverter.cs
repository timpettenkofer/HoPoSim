using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
    class ListSortDirectionToBooleanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is ListSortDirection && ListSortDirection.Ascending == (ListSortDirection)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !(bool)value ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }
    }
}
