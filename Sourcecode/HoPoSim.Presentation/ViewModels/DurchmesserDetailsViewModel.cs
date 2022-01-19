using HoPoSim.Data.Domain;
using System.ComponentModel;

namespace HoPoSim.Presentation.ViewModels
{
	public class DurchmesserDetailsViewModel : ValidableEntityViewModel<Durchmesser>
	{
		public DurchmesserDetailsViewModel(Durchmesser d, PropertyChangedEventHandler onPropertyChanged) 
			: base(d)
		{
			IsSelected = RangeId == 1;
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

		public int Rindenstärke
		{
			get { return This.Rindenstärke; }
			set { SetProperty(This.Rindenstärke, value, () => This.Rindenstärke = value); }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		private bool _isSelected;
	}
}
