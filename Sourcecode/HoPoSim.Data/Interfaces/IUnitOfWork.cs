using HoPoSim.Data.Domain;
using HoPoSim.Data.Repositories;
using System;

namespace HoPoSim.Data.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		IGeneratorDataRepository GeneratorData { get; }
		IDistributionRepository Distributions { get; }
		IBaseRepository<SimulationData> SimulationData { get; }
		IBaseRepository<SimulationConfiguration> SimulationConfigurations { get; }
		IBaseRepository<BaumartParametrization> BaumartParametrizations { get; }
		IBaseRepository<Stapelqualität> Stapelqualitäten { get; }

		int DatabaseVersion { get; }

		void DeleteEntity(object entity);
		void DiscardChanges(object entity);
		void Complete();
		void RefreshAll();
	}
}
