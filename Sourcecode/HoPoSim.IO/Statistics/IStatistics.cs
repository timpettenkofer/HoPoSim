using HoPoSim.Data.Domain;
using System.Collections.Generic;

namespace HoPoSim.IO.Statistics
{
    public interface IPersonStatistics
    {
        string Category { get; }
        int Value { get; }
        int Gesamt { get; }
    }

    public interface IEntityStatistics
    {
        string Category { get; }
        int Value { get; }
        int Sum { get; }
    }

    public interface IStatistics
    {
        //IEnumerable<IEntityStatistics> GetEntityStatisticsFor(IEntity entity);
        //IEnumerable<IPersonStatistics> GetPersonStatisticsFor(Person person);
    }
}
