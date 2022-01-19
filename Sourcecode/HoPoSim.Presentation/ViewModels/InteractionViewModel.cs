using System;
using Prism.Regions;

namespace HoPoSim.Presentation.ViewModels
{
	public class InteractionViewModel
	{
		#region IConfirmNavigationRequest
		public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
		{
			continuationCallback(true);
		}
		#endregion
	}
}
