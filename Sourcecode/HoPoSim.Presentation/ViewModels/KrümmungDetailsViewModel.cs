using HoPoSim.Data.Domain;
using System.ComponentModel;

namespace HoPoSim.Presentation.ViewModels
{
	public class KrümmungDetailsViewModel : ValidableEntityViewModel<Krümmung>
	{
		public KrümmungDetailsViewModel(Krümmung k, PropertyChangedEventHandler onPropertyChanged)
			: base(k)
		{
			PropertyChanged += onPropertyChanged;
		}

		public int RangeId
		{
			get { return This.RangeId; }
		}

		public int MinValue
		{
			get { return This.MinValue; }
			set { SetProperty(This.MinValue, value, () => This.MinValue = value); }
		}

		public int MaxValue
		{
			get { return This.MaxValue; }
			set { SetProperty(This.MaxValue, value, () => This.MaxValue = value); }
		}
	}
}

