using HoPoSim.Presentation.Views;
using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Templates
{
    public class TabItemsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;
            NavigationItem navItem = item as NavigationItem;
            if (navItem.HasChildren())
            {
                return elemnt.FindResource("MultiTabHeaderTemplate") as DataTemplate;
            }
            else
            {
                return elemnt.FindResource("TabHeaderTemplate") as DataTemplate;
            }
        }
    }
}
