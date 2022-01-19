using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Views
{
    [Export(typeof(StatistikenView))]
    public partial class StatistikenView : UserControl
    {
        public StatistikenView()
        {
            InitializeComponent();
        }

        private string _tabName = "Statistiken";
        public string TabName
        {
            get { return _tabName; }
        }
        }
}
