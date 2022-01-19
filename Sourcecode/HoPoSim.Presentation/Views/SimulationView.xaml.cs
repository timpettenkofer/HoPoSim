using HoPoSim.Presentation.ViewModels;
using MahApps.Metro.Controls;
using Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HoPoSim.Presentation.Views
{
	[Export(typeof(SimulationView))]
	public partial class SimulationView : UserControl
	{
		public SimulationView()
		{
			InitializeComponent();
		}

		//private void EntityCommandControl_CommandTriggered(object sender, System.Windows.RoutedEventArgs e)
		//{
		//	entityContentControl.ScrollDetailsToHome();
		//	//nameTextBox.Focus();
		//}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
			e.Handled = true;
		}

		private void MenuItem_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var isVisible = (bool)e.NewValue;
			if (isVisible)
			{
				MenuItem menuItem = sender as MenuItem;
				if (menuItem != null)
				{
					DelegateCommand cmd = menuItem.Command as DelegateCommand;
					if (cmd != null)
					{
						menuItem.Command = null;
						menuItem.Command = cmd;
					}
							//menuItem.IsEnabled = cmd.CanExecute();
				}
			}
		}

		private MetroWindow dataWindow;

		private MetroWindow GetDataWindow()
		{
			if (dataWindow != null)
			{
				dataWindow.Close();
			}
			dataWindow = new MetroWindow() { Owner = (MetroWindow)(Application.Current.MainWindow), WindowStartupLocation = WindowStartupLocation.CenterOwner, Title = "Polterungsdaten", Width = 500, Height = 300 };
			dataWindow.Closed += (o, args) => dataWindow = null;
			return dataWindow;
		}

		private void ShowData_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var dc = DataContext as SimulationViewModel;

			var w = this.GetDataWindow();
			var binding = new Binding();
			binding.Mode = BindingMode.TwoWay;
			binding.Source = dc;
			binding.Path = new PropertyPath("SelectedItem.Data");
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			var content = new TextBox() {
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				
				TextWrapping = TextWrapping.NoWrap,
				AcceptsReturn = true,
			};
			BindingOperations.SetBinding(content, TextBox.TextProperty, binding);

			w.Content = content;
			w.BorderThickness = new Thickness(1);
			w.GlowBrush = null;
			w.SetResourceReference(MetroWindow.BorderBrushProperty, "AccentColorBrush");
			w.Show();
		}

	}
}
