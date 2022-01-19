using MahApps.Metro;
using MahApps.Metro.Controls;
using MahAppsMetroThemesSample;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HoPoSim.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    [Export(typeof(SettingsView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SettingsView : UserControl
    {
        public static readonly DependencyProperty ColorsProperty
            = DependencyProperty.Register("Colors",
                                          typeof(List<KeyValuePair<string, Color>>),
                                          typeof(SettingsView),
                                          new PropertyMetadata(default(List<KeyValuePair<string, Color>>)));

        public List<KeyValuePair<string, Color>> Colors
        {
            get { return (List<KeyValuePair<string, Color>>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        public SettingsView()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Colors = typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)))
                .ToList();

            //var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            //ThemeManager.ChangeAppStyle(System.Windows.Application.Current, theme.Item2, theme.Item1);
        }

        //private void ChangeWindowThemeButtonClick(object sender, RoutedEventArgs e)
        //{
        //    var theme = ThemeManager.DetectAppStyle(this);
        //    ThemeManager.ChangeAppStyle(this, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
        //}

        //private void ChangeWindowAccentButtonClick(object sender, RoutedEventArgs e)
        //{
        //    var theme = ThemeManager.DetectAppStyle(this);
        //    ThemeManager.ChangeAppStyle(this, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
        //}

        private void ChangeAppThemeButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
            ThemeManagerHelper.SaveTheme("Base" + ((Button)sender).Content);
        }

        private void ChangeAppAccentButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
        }

        private void CustomThemeAppButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, theme.Item2, ThemeManager.GetAppTheme("CustomTheme"));
        }

        private void CustomAccent1AppButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent("CustomAccent1"), theme.Item1);
        }

        private void CustomAccent2AppButtonClick(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent("CustomAccent2"), theme.Item1);
        }

        //private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var selectedAccent = AccentSelector.SelectedItem as Accent;
        //    if (selectedAccent != null)
        //    {
        //        var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
        //        ThemeManager.ChangeAppStyle(System.Windows.Application.Current, selectedAccent, theme.Item1);
        //        System.Windows.Application.Current.MainWindow.Activate();
        //    }
        //}

        private void ColorsSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedColor = this.ColorsSelector.SelectedItem as KeyValuePair<string, Color>?;
            if (selectedColor.HasValue)
            {
                var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
                ThemeManagerHelper.CreateAppStyleBy(selectedColor.Value.Value, true);
                System.Windows.Application.Current.MainWindow.Activate();

                ThemeManagerHelper.SaveAccent(selectedColor.Value.Key);
            }
        }
    }
}
