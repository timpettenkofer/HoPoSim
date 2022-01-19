using HoPoSim.Data.Domain;
using HoPoSim.Framework;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System;
using HoPoSim.Data.Interfaces;
using HoPoSim.Framework.Interfaces;

namespace HoPoSim.IO.Statistics
{
    public class PersonStatisctics : IPersonStatistics
    {
        public PersonStatisctics(string category, int value, int gesamt)
        {
            Category = category;
            Value = value;
            Gesamt = gesamt;
        }

        public string Category { get; }

        public int Value { get; }

        public int Gesamt { get; }
    }

    public class EntityStatisctics : IEntityStatistics
    {
        public EntityStatisctics(string category, int value, int sum)
        {
            Category = category;
            Value = value;
            Sum = sum;
        }

        public string Category { get; }

        public int Value { get; }

        public int Sum { get; }
    }

    [Export(typeof(IStatistics))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class Statistics : IStatistics
    {
        [ImportingConstructor]
        public Statistics(IGlobalConfigService config, IUnitOfWorkFactory uowfactory)
        {
            Config = config;
            UOWFactory = uowfactory;
        }

        //public IEnumerable<IEntityStatistics> GetEntityStatisticsFor(IEntity entity)
        //{
        //    var uow = UOWFactory.Create();
        //    var result = new List<IEntityStatistics>();
        //    for (int i = 0; i < 3; ++i)
        //    {
        //        result.Add(new EntityStatisctics($"Id: {entity.Id}", i, i * 100));
        //    }
        //    return result;
        //}

        //public IEnumerable<IPersonStatistics> GetPersonStatisticsFor(Person person)
        //{
        //    var uow = UOWFactory.Create();
        //    var result = new List<IPersonStatistics>();
        //    for(int i = 0; i < 3; ++i)
        //    {
        //        result.Add(new PersonStatisctics(person.Nachname, i, i * 100));
        //    }
        //    return result;
        //}

        private IGlobalConfigService Config { get; }
        private IUnitOfWorkFactory UOWFactory { get; }
    }
}
