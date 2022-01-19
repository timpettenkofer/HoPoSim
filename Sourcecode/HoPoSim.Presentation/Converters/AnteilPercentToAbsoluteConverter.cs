using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HoPoSim.Presentation.Converters
{
	public class AnteilPercentToAbsoluteConverter : DependencyObject, IMultiValueConverter
	{
		public int Stammanzahl
		{
			get { return (int)GetValue(StammanzahlProperty); }
			set { SetValue(StammanzahlProperty, value); }
		}

		public static readonly DependencyProperty StammanzahlProperty =
			DependencyProperty.Register("Stammanzahl",
										typeof(int),
										typeof(AnteilPercentToAbsoluteConverter),
										new PropertyMetadata(0));

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double? percent = values[0] as double?;

			if (percent == null)
				return DependencyProperty.UnsetValue;

			return Data.Extensions.Extensions.AnteilAbsolute(percent.Value, Stammanzahl);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			double? absolute = value as double?;

			if (absolute == null)
				return new[]{ DependencyProperty.UnsetValue, Stammanzahl};

			return new object[]{ Data.Extensions.Extensions.AnteilPercent(absolute.Value, Stammanzahl), Stammanzahl };
		}
	}
}
