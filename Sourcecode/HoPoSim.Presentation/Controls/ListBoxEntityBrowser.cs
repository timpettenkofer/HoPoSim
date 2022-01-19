using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Controls
{
    public class ListBoxEntityBrowser : Control
    {
        static ListBoxEntityBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxEntityBrowser), new FrameworkPropertyMetadata(typeof(ListBoxEntityBrowser)));
        }


        public DataTemplate BrowserItemTemplate
        {
            get { return (DataTemplate)base.GetValue(BrowserItemTemplateProperty); }
            set { base.SetValue(BrowserItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty BrowserItemTemplateProperty =
          DependencyProperty.Register("BrowserItemTemplate", typeof(DataTemplate), typeof(ListBoxEntityBrowser),
              new UIPropertyMetadata(null));

    }
}
