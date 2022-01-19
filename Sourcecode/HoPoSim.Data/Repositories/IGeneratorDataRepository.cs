using HoPoSim.Data.Domain;
using System.Collections.Generic;

namespace HoPoSim.Data.Repositories
{
    public interface IGeneratorDataRepository : IBaseRepository<GeneratorData>
    {
		IEnumerable<GeneratorData> GetAllIncludingAll();
	}
}
