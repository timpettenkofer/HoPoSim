using System.Collections.Generic;

namespace HoPoSim.Framework.Reports
{
    public interface IReportingService
    {
        IEnumerable<IReportTemplate> AvailableTemplates { get; }
        IReportContext CreateContext();
        void Generate(string outfile, IEnumerable<IReportTemplate> composingParts, IReportContext context);
        void TryOpen(string file);
    }
}
