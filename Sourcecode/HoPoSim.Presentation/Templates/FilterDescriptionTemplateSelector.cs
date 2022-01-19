using HoPoSim.Presentation.Filter;
using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Templates
{
	public class FilterDescriptionTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DefaultDataTemplate { get; set; }
		public DataTemplate EntityDataTemplate { get; set; }
		public DataTemplate DateDataTemplate { get; set; }
		public DataTemplate EnumDataTemplate { get; set; }
		public DataTemplate BooleanDataTemplate { get; set; }
		public DataTemplate NumberDataTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var fd = item as IFilterDescription;
			if (fd is IEntityFilterDescription)
			{
				return EntityDataTemplate;
			}
			if (fd is IDateFilterDescription)
			{
				return DateDataTemplate;
			}
			if (fd is IBooleanFilterDescription)
			{
				return BooleanDataTemplate;
			}
			if (fd is IEnumFilterDescription)
			{
				return EnumDataTemplate;
			}
			if (fd is INumberFilterDescription)
			{
				return NumberDataTemplate;
			}
			return DefaultDataTemplate;
		}
	}
}
