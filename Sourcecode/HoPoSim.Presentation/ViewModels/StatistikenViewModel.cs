using Prism.Mvvm;
using System.ComponentModel.Composition;

namespace HoPoSim.Presentation.ViewModels
{
    [Export(typeof(StatistikenViewModel))]
    public class StatistikenViewModel : BindableBase
    {
        private string _title = "Statistiken Form";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        } 
    }
}
