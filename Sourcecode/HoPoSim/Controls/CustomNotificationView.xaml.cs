using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;

namespace HoPoSim.Controls
{
    /// <summary>
    /// Interaction logic for CustomNotificationView.xaml
    /// </summary>
    public partial class CustomNotificationView : IInteractionRequestAware
    {
        public CustomNotificationView()
        {
            InitializeComponent();
        }

        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //var notification = this.Notification as INotification;
            //if (notification != null)
            //    notification. = true;
            if (this.FinishInteraction != null)
                this.FinishInteraction();
        }
    }
}
