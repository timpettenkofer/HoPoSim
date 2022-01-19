namespace HoPoSim.Framework.Reports
{
    public interface IReportDocument
    {
        void SaveAs(string filename);
        void Close();
        void AddContent(IReportDocument part);
    }
}
