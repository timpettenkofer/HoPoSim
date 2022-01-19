using System;

/*
 * https://github.com/sargentstudley/dotnetlicensing
 */
namespace HoPoSim.License
{
	public class HoPoSimLicense : DotNetLicense.License
	{
		/// <summary>
		/// Creates a new Northwind license, unsigned. 
		/// </summary>
		public HoPoSimLicense() : base()
		{
			//We make sure to intialize our values in the license so that we have base values. 
			this.ExpirationDate = DateTime.Now;
			this.LicensedTo = "";
			this.NumberOfUsers = 0;
		}

		/// <summary>
		/// Constructs a NorthwindLicense based on the super-class license returned from DotNetLicense.LicenseManager
		/// </summary>
		/// <param name="baseLicense"></param>
		public HoPoSimLicense(DotNetLicense.License baseLicense) : base(baseLicense.ToXml())
		{

		}

		/// <summary>
		/// The date the license expires. 
		/// </summary>
		public DateTime ExpirationDate
		{
			get
			{
				return DateTime.Parse(base.GetAttribute("ExpirationDate"));
			}
			set
			{
				base.AddOrChangeAttribute("ExpirationDate", value.ToShortDateString());
			}
		}

		/// <summary>
		/// The person or organization the license was issued to.
		/// </summary>
		public string LicensedTo
		{
			get
			{
				return base.GetAttribute("LicensedTo");
			}
			set
			{
				base.AddOrChangeAttribute("LicensedTo", value);
			}
		}

		/// <summary>
		/// The number of users this license supports. 
		/// </summary>
		public int NumberOfUsers
		{
			get
			{
				return int.Parse(base.GetAttribute("NumberOfUsers"));
			}
			set
			{
				base.AddOrChangeAttribute("NumberOfUsers", value.ToString());
			}
		}

		//internal void CreateKeyPair()
		//{
		//	var lm = new DotNetLicense.LicenseManager();
		//	lm.CreateKeyPairs("C:\\dev\\HoPoSim", "hoposim");
		//}

		//internal void CreateLicense()
		//{
		//	HoPoSimLicense license = new HoPoSimLicense();
		//	license.LicensedTo = Resources.ApplicationOwner;
		//	license.ExpirationDate = DateTime.Now.AddYears(100);
		//	license.NumberOfUsers = 100;

		//	LicenseManager lm = new LicenseManager();
		//	lm.LoadPrivateKeyFromFile("C:\\dev\\HoPosim\\hoposim_private.key");

		//	lm.SignAndSaveNewLicense(license, "C:\\dev\\HoPoSim\\hoposim.lic");
		//}
	}
}



