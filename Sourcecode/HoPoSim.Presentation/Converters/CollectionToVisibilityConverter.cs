using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection;
            return collection != null && collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
