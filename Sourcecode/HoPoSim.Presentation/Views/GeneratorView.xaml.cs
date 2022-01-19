using HoPoSim.Presentation.Helpers;
using MahApps.Metro.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HoPoSim.Presentation.Views
{
	[Export(typeof(GeneratorView))]
	public partial class GeneratorView
	{
		[ImportingConstructor]
		public GeneratorView(INavigationHelper navHelper)
		{
			_navHelper = navHelper;
			InitializeComponent();
		}
		INavigationHelper _navHelper;

		private void EntityCommandControl_CommandTriggered(object sender, System.Windows.RoutedEventArgs e)
		{
			entityContentControl.ScrollDetailsToHome();
			nameTextBox.Focus();
		}

		private void NumericUpDown_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var numeric = (NumericUpDown)sender;
				DependencyProperty prop = NumericUpDown.ValueProperty;

				BindingExpression binding = BindingOperations.GetBindingExpression(numeric, prop);
				if (binding != null) { binding.UpdateSource(); }
			}
		}

		//private void Lieferschein_Button_Click(object sender, RoutedEventArgs e)
		//{
		//	var button = sender as Button;
		//	_navHelper.NavigateTo(typeof(LieferscheinView), (int)button.Tag);
		//}
	}
}
