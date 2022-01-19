namespace HoPoSim.Framework.Reports
{
    public interface IReportTemplate
    {
        string Name { get; }
        string Description { get; }
        string TemplatePath { get; }
        int Priority { get; }
        bool Private { get; }

        void Evaluate(IReportDocument root, IReportContext context);
    }
}
