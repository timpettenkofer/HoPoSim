using HoPoSim.Data.Domain;
using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Persistence.Repositories;
using HoPoSim.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HoPoSim.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;

        public UnitOfWork(Context context)
        {
            _context = context;
            GeneratorData = new GeneratorDataRepository(_context);
			Distributions = new DistributionRepository(_context);
			SimulationData = new BaseRepository<SimulationData>(_context);
			SimulationConfigurations = new BaseRepository<SimulationConfiguration>(_context);
			BaumartParametrizations = new BaseRepository<BaumartParametrization>(_context);
			Stapelqualitäten = new BaseRepository<Stapelqualität>(_context);
		}

		public IGeneratorDataRepository GeneratorData { get; private set; }
		public IDistributionRepository Distributions { get; }
		public IBaseRepository<SimulationData> SimulationData { get; private set; }
		public IBaseRepository<SimulationConfiguration> SimulationConfigurations { get; private set; }
		public IBaseRepository<BaumartParametrization> BaumartParametrizations { get; private set; }
		public IBaseRepository<Stapelqualität> Stapelqualitäten { get; private set; }

		public int DatabaseVersion
		{
			get { return _context.ActualDatabaseVersion; }
		}

		public void DiscardChanges(object entity)
		{
			_context.Entry(entity).State = EntityState.Unchanged;
		}

		public void DeleteEntity(object entity)
		{
			_context.Entry(entity).State = EntityState.Deleted;
		}

		public void Complete()
		{
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		public void RefreshAll()
		{
			foreach (var entity in _context.ChangeTracker.Entries())
			{
				entity.Reload();
			}
		}
	}
}
