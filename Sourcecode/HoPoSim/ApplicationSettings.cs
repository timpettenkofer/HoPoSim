using HoPoSim.Framework.Interfaces;
using System.ComponentModel.Composition;

namespace HoPoSim
{
	[Export(typeof(ISettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ApplicationSettings : ISettings
    {
        public object this[string propertyName]
        {
            get
            {
                return Properties.Settings.Default[propertyName];
            }
            set
            {
                Properties.Settings.Default[propertyName] = value;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
