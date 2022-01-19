using HoPoSim.Data.Domain;
using System.Collections.Generic;

namespace HoPoSim.Data.Repositories
{
	public interface IDistributionRepository : IBaseRepository<Distribution>
	{
		IEnumerable<Distribution> GetAllIncludingAll();
	}
}
