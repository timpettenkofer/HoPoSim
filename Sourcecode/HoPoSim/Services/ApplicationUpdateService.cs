using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Deployment.Application;
using System.Windows;

namespace HoPoSim.Services
{
	// https://msdn.microsoft.com/en-en/library/ff699336.aspx

	[Export(typeof(IApplicationUpdateService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ApplicationUpdateService : IApplicationUpdateService
	{
		[ImportingConstructor]
		public ApplicationUpdateService(IInteractionService interaction)
		{
			_interaction = interaction;
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				// Hook up an event handler for update check completed events.
				ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += OnCheckForUpdatesCompleted;
				ApplicationDeployment.CurrentDeployment.UpdateCompleted += new AsyncCompletedEventHandler(OnUpdateCompleted);
			}
		}

		IInteractionService _interaction;
		bool m_RequiredUpdateDetected = false;

		void OnCheckForUpdatesCompleted(object sender, CheckForUpdateCompletedEventArgs e)
		{
			if (e.UpdateAvailable)
			{
				if (e.IsUpdateRequired)
				{
					m_RequiredUpdateDetected = true;
				}
				ApplicationDeployment.CurrentDeployment.UpdateAsync();
			}
			else
			{
				_interaction.RaiseNotificationAsync("Kein Update verfügbar", "Ihre Software ist auf dem neuesten Stand ");
			}
		}

		void OnUpdateCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (m_RequiredUpdateDetected)
			{
				_interaction.ExecuteIfUserConfirmed("Erforderliches Update",
					"Erforderliches Update heruntergeladen. Möchten Sie die Anwendung jetzt neu starten?",
					() => Restart());
			}
			else
			{
				_interaction.ExecuteIfUserConfirmed("Update gefunden",
					"Software-Update heruntergeladen. Möchten Sie die Anwendung jetzt neu starten?",
					() => Restart());
			}
		}

		private void Restart()
		{
			System.Windows.Forms.Application.Restart();
			Application.Current.Shutdown();
		}

		public bool CanCheckForUpdates()
		{
			return ApplicationDeployment.IsNetworkDeployed;
		}

		public void CheckForUpdates()
		{
			// Check to ensure the application is running through ClickOnce.
			if (ApplicationDeployment.IsNetworkDeployed)
			{
				// Check for updates asynchronization.
				ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
			}
		}
	}
}
