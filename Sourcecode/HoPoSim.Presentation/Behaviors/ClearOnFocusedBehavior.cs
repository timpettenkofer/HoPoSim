using System.Windows;

namespace HoPoSim.Presentation.Behaviors
{
    class ClearOnFocusedBehavior : System.Windows.Interactivity.Behavior<System.Windows.Controls.TextBox>
    {
        public static readonly DependencyProperty DefaultTextProperty =
        DependencyProperty.Register("DefaultText", typeof(string), typeof(ClearOnFocusedBehavior));

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        private void _GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (System.Windows.Controls.TextBox)sender;
            textBox.Text = textBox.Text == DefaultText ? string.Empty : textBox.Text;
        }

        private void _LostFocus(object sender, RoutedEventArgs e)
        {
            //var textBox = (System.Windows.Controls.TextBox)sender;
            //textBox.Text = textBox.Text == string.Empty ? defaultText : textBox.Text;
        }

        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += _GotFocus;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= _GotFocus;
        }
    }
}
