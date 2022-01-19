using Prism.Interactivity.InteractionRequest;
using System;

namespace HoPoSim.Framework.Interfaces
{
	public interface IInteractionService
	{
		void RaiseNotificationAsync(string message, string title = "Warnung");
		void RaiseDismissibleNotificationAsync(string message, Func<bool> doShowDialog, Action<bool> setDoShowDialog, string title = "Information");
		void ExecuteIfUserConfirmed(string title, string message, Action continuationCallback, Action fallbackCallback = null, string affirmativeButtonText = "OK", string negativeButtonText = "Abbrechen");
		void ExecuteIfUserArchivingConfirmed(Action continuationCallback);
		void ExecuteIfUserDeletionConfirmed(Action continuationCallback);

		InteractionRequest<INotification> NotificationRequest { get; }
		void RaiseNotification(string message, string title = "Warnung");

	}
}
