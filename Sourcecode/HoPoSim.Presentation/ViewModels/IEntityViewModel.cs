using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Validation;

namespace HoPoSim.Presentation.ViewModels
{
	public interface IHaveConverter<T>
	{
		T Convert();
	}

	public interface IViewStateAwareViewModel
	{
		bool IsExpanded { get; set; }
		bool IsSelected { get; set; }
	}

	public interface IEntityViewModel : IHaveNameProperty, IHaveIdProperty, IHaveActiveProperty, IHaveEntityProperty, IValidable
	{
	}
}
