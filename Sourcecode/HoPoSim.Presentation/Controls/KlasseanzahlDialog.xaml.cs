using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;

namespace HoPoSim.Presentation.Controls
{
	public partial class KlasseanzahlDialog : INotifyPropertyChanged
	{
		public KlasseanzahlDialog(int durchmesserklassen = 5, int abholzhigkeitklassen = 5, int krümmungklassen = 5, int ovalitätklassen = 2)
		{
			InitializeComponent();

			Caption = "Bitte wählen Sie die Anzahlen von Klassen aus,\nfür die die Daten erstellt werden sollen.";

			DurchmesserklasseAnzahl = durchmesserklassen;
			AbholzigkeitsklasseAnzahl = abholzhigkeitklassen;
			KrümmungsklasseAnzahl = krümmungklassen;
			OvalitätsklasseAnzahl = ovalitätklassen;

			DataContext = this;

			_customDialog = new CustomDialog();
			_customDialog.Content = this;
		}

		public event EventHandler OnClose;

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


		public int DurchmesserklasseAnzahl
		{
			get { return _durchmesserklasseAnzahl; }
			set
			{
				_durchmesserklasseAnzahl = value;
				NotifyPropertyChanged(nameof(DurchmesserklasseAnzahl));
			}
		}
		private int _durchmesserklasseAnzahl;

		public int KrümmungsklasseAnzahl
		{
			get { return _krümmungsklasseAnzahl; }
			set
			{
				_krümmungsklasseAnzahl = value;
				NotifyPropertyChanged(nameof(KrümmungsklasseAnzahl));
			}
		}
		private int _krümmungsklasseAnzahl;

		public int OvalitätsklasseAnzahl
		{
			get { return _ovalitätsklasseAnzahl; }
			set
			{
				_ovalitätsklasseAnzahl = value;
				NotifyPropertyChanged(nameof(OvalitätsklasseAnzahl));
			}
		}
		private int _ovalitätsklasseAnzahl;

		public int AbholzigkeitsklasseAnzahl
		{
			get { return _abholzigkeitsklasseAnzahl; }
			set
			{
				_abholzigkeitsklasseAnzahl = value;
				NotifyPropertyChanged(nameof(AbholzigkeitsklasseAnzahl));
			}
		}
		private int _abholzigkeitsklasseAnzahl;

		public MessageDialogResult Result { get; set; }

		public async void ShowCustomDialog()
		{
			await ((MetroWindow)(Application.Current.MainWindow)).ShowMetroDialogAsync(_customDialog);
		}

		CustomDialog _customDialog;

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageDialogResult.Canceled;
			((MetroWindow)(Application.Current.MainWindow)).HideMetroDialogAsync(_customDialog);
			Close();
		}

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
