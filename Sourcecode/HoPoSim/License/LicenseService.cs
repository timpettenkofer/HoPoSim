using DotNetLicense;
using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace HoPoSim.License
{
	public class LicenseService
	{
		public LicenseService(IInteractionService interaction)
		{
			_interaction = interaction;
		}
		IInteractionService _interaction;

		public bool CheckLicense()
		{
			try
			{ 
				var resources = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
				LicenseManager manager = new LicenseManager();
				var public_key_path = GetFullPath("hoposim_public.key");
				manager.LoadPublicKeyFromFile(public_key_path);

				var licensePath = GetLicenseFilePath();
				DotNetLicense.License baseLicense = manager.LoadLicenseFromDisk(licensePath);
				HoPoSimLicense myLicense = new HoPoSimLicense(baseLicense);
				CheckLicenseValidity(myLicense);
				return true;
			}
			catch (LicenseVerificationException licenseEx)
			{
				//Malformed xml, or someone change the file and the signature is failing.
				_interaction.RaiseNotification(licenseEx.Message, "Lizenzfehler");
				return false;
			}
			catch(Exception e)
			{
				_interaction.RaiseNotification($"A fatal error occured during license checking: {e.Message}", "Lizenzfehler");
				return false;
			}
		}

		private static string _licensedTo = Resources.ApplicationOwner;
		private static void CheckLicenseValidity(HoPoSimLicense license)
		{
			if (!_licensedTo.Equals(license.LicensedTo))
				throw new LicenseVerificationException(string.Format(Resources.LicenseToError, _licensedTo));

			if (license.ExpirationDate < DateTime.Now)
				throw new LicenseVerificationException(string.Format(Resources.ExpiredLicenseError, license.ExpirationDate.ToLongDateString()));
			return;
		}

		private static string GetLicenseFilePath()
		{
			var searchPaths = new List<string>();
			var folders = new[] {
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolder.MyDocuments,
				Environment.SpecialFolder.CommonApplicationData
			};
			foreach (var folder in folders)
			{
				foreach (var subfolder in new[] { "HoPoSim", string.Empty })
				{
					var path = Environment.GetFolderPath(folder);
					var filePath = Path.Combine(path, subfolder, "hoposim.lic");
					if (File.Exists(filePath))
						return filePath;
					searchPaths.Add(filePath);
				}
			}
			throw new LicenseVerificationException(string.Format(Resources.NoLicenseError, String.Join("\n", searchPaths)));
		}

		private static string GetFullPath(string dataFileName)
		{
			var file = Path.Combine(GetExecutingFolder(), dataFileName);
			return file;
		}

		private static string GetExecutingFolder()
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}
	}
}

