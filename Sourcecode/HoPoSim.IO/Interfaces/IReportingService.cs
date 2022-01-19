using System.Collections.Generic;

namespace HoPoSim.IO.Interfaces
{
    public interface IReportingService
    {
        void ExportToExcel<T>(IEnumerable<T> query, string file);
    }
}
