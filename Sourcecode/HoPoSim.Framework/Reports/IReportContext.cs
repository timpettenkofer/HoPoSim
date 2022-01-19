using System.Collections.Generic;

namespace HoPoSim.Framework.Reports
{
    public interface IReportContext
    {
        string ApplicationOwner { get; set; }

        IDictionary<string, object> AdditionalData { get; }
    }
}
