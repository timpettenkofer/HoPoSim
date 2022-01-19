using HoPoSim.Framework;
using System.Diagnostics;
using System.Windows.Navigation;

namespace HoPoSim.Presentation.Controls
{
	public partial class AboutWindow
	{
		public AboutWindow()
		{
			DataContext = this;
			InitializeComponent();
		}

		public string ApplicationPublishVersion
		{
			get
			{
				return $"{ApplicationHelper.ApplicationName} {ApplicationHelper.PublishVersion}";
			}
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
