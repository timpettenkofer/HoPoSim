using HoPoSim.Data.Domain;
using HoPoSim.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Persistence.Repositories
{
	class DistributionRepository : BaseRepository<Distribution>, IDistributionRepository
	{
		public DistributionRepository(Context context)
			: base(context)
		{ }

		public IEnumerable<Distribution> GetAllIncludingAll()
		{
			return base.Context.Set<Distribution>()
				.Include(k => k.Children)
				.ToList();
		}

		public new Context Context
		{
			get { return Context as Context; }
		}
	}
}
