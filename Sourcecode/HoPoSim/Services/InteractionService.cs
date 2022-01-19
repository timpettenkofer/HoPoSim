using HoPoSim.Controls;
using HoPoSim.Framework.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Prism.Interactivity.InteractionRequest;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace HoPoSim.Services
{
	[Export(typeof(IInteractionService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class InteractionService : IInteractionService
	{
		private IDialogCoordinator DialogCoordinator
		{
			get
			{
				if (_dialogCoordinator == null)
					_dialogCoordinator = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance;
				return _dialogCoordinator;
			}
		}
		private IDialogCoordinator _dialogCoordinator;

		#region Confirmation Request
		public void ExecuteIfUserArchivingConfirmed(Action continuationCallback)
		{
			var title = "Eintrag archivieren";
			var message = "Möchten Sie den ausgewählten Eintrag wirklich archivieren?";
			ExecuteIfUserConfirmed(title, message, continuationCallback);
		}

		public void ExecuteIfUserDeletionConfirmed(Action continuationCallback)
		{
			var title = "Eintrag löschen";
			var message = "Möchten Sie den ausgewählten Eintrag wirklich löschen?";
			ExecuteIfUserConfirmed(title, message, continuationCallback);
		}

		public void ExecuteIfUserConfirmed(string title, string message, Action continuationCallback, Action fallbackCallback = null, string affirmativeButtonText = "OK", string negativeButtonText = "Abbrechen")
		{
			var settings = new MetroDialogSettings() { AffirmativeButtonText = affirmativeButtonText, NegativeButtonText = negativeButtonText, DialogResultOnCancel = MessageDialogResult.Negative };
			DialogCoordinator.ShowMessageAsync(Application.Current.MainWindow, title, message, MessageDialogStyle.AffirmativeAndNegative, settings)
			.ContinueWith(t =>
			{
				Application.Current.Dispatcher.Invoke(
					() =>
					{
						if (t.Result == MessageDialogResult.Affirmative)
							continuationCallback();
						else fallbackCallback?.Invoke();
					});
			});
		}
		#endregion

		#region Notification Request
		public void RaiseNotificationAsync(string message, string title = "Warnung")
		{
			try
			{
				var settings = new MetroDialogSettings() { AffirmativeButtonText = "OK", DialogResultOnCancel = MessageDialogResult.Affirmative };
				Application.Current.Dispatcher.Invoke(
					() =>
					DialogCoordinator.ShowMessageAsync(Application.Current.MainWindow, title, message, MessageDialogStyle.Affirmative, settings));
			}
			catch (InvalidOperationException)
			{
				// weird "This Visual is not connected to a PresentationSource"...
			}
		}

		public void RaiseDismissibleNotificationAsync(string message, Func<bool> doShowDialog, Action<bool> setDoShowDialog, string title = "Information")
		{
			try
			{
				if (!doShowDialog())
					return;

				var notification = new DismissibleNotification(title, message);
				notification.OnClose += (o, args) =>
				{
					setDoShowDialog(!notification.DoNotShowAgain);
				};
				notification.ShowCustomDialog();
			}
			catch (Exception e)
			{
			}
		}

		private void ShowDismissibleDialog()
		{
			var dialog = new CustomDialog();
		}

		public InteractionRequest<INotification> NotificationRequest
		{
			get { return _notificationRequest; }
		}
		private InteractionRequest<INotification> _notificationRequest = new InteractionRequest<INotification>();

		public void RaiseNotification(string message, string title = "Warnung")
		{
			try
			{
				this.NotificationRequest.Raise(
				   new Notification { Content = message, Title = title });
			}
			catch (InvalidOperationException)
			{
				// weird "This Visual is not connected to a PresentationSource"...
			}
		}
		#endregion
	}
}
