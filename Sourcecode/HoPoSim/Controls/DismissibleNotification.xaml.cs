using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Controls
{
	/// <summary>
	/// Interaction logic for DismissibleNotification.xaml
	/// </summary>
	public partial class DismissibleNotification : UserControl
	{
		public DismissibleNotification(string title, string message)
		{
			InitializeComponent();

			Title = title;
			Caption = message;

			DataContext = this;

			_customDialog = new CustomDialog();
			_customDialog.Content = this;
		}

		public event EventHandler OnClose;

		public string Title { get; }

		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				NotifyPropertyChanged(nameof(Caption));
			}
		}
		private string _caption;

		public bool DoNotShowAgain { get; set; }

		public MessageDialogResult Result { get; set; }

		public async void ShowCustomDialog()
		{
			await ((MetroWindow)(Application.Current.MainWindow)).ShowMetroDialogAsync(_customDialog);
		}

		CustomDialog _customDialog;

		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageDialogResult.Affirmative;
			((MetroWindow)(Application.Current.MainWindow)).HideMetroDialogAsync(_customDialog);
			Close();
		}


		private void Close()
		{
			if (OnClose == null) return;

			EventArgs args = new EventArgs();
			OnClose(this, args);
		}

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
