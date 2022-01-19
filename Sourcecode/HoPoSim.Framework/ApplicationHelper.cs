using System.Deployment.Application;

namespace HoPoSim.Framework
{
	public static class ApplicationHelper
	{

		public static string PublishVersion
		{
			get
			{
				var v = "Not installed";
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					v = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
				}
				return string.Format("Version: {0}", v);
			}
		}

		public static string ApplicationName
		{
			get
			{
				return "HoPoSim";
			}
		}
	}
}
