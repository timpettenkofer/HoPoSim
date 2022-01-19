using HoPoSim.Data.Domain;
using System.ComponentModel;

namespace HoPoSim.Presentation.ViewModels
{
	public class OvalitätDetailsViewModel : ValidableEntityViewModel<Ovalität>
	{
		public OvalitätDetailsViewModel(Ovalität o, PropertyChangedEventHandler onPropertyChanged)
			: base(o)
		{
			PropertyChanged += onPropertyChanged;
		}

		public int RangeId
		{
			get { return This.RangeId; }
		}

		public double MinValue
		{
			get { return This.MinValue; }
			set { SetProperty(This.MinValue, value, () => This.MinValue = value); }
		}

		public double MaxValue
		{
			get { return This.MaxValue; }
			set { SetProperty(This.MaxValue, value, () => This.MaxValue = value); }
		}
	}
}

