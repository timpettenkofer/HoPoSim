using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
    public class FilePathToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && File.Exists(value as string)? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
