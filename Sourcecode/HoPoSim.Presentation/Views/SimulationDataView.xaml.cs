using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HoPoSim.Presentation.Views
{
    [Export(typeof(SimulationDataView))]
    public partial class SimulationDataView : UserControl
    {
        public SimulationDataView()
        {
            InitializeComponent();
        }

		private void EntityCommandControl_CommandTriggered(object sender, System.Windows.RoutedEventArgs e)
		{
			entityContentControl.ScrollDetailsToHome();
			//nameTextBox.Focus();
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scv = (ScrollViewer)sender;
			scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
			e.Handled = true;
		}

		private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			e.Column = new DataGridTextColumn() { Header = e.PropertyName, Binding = new Binding("[" + e.PropertyName + "]") };
		}
	}
}
