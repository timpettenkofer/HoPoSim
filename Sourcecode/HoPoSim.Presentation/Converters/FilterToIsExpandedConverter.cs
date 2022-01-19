using HoPoSim.Presentation.Filter;
using System;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
    public class FilterToIsExpandedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            var value1 = values[0];
            var value2 = values[1];

            var filter = values[2] as IFilterDescription;
            if (filter != null && filter.IsDefaultValue)
                return false;

            if (value1 == null)
                return value2 != null;
            return !value1.Equals(value2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[0];
        }
    }
}
