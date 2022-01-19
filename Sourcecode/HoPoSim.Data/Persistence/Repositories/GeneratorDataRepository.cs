using HoPoSim.Data.Domain;
using HoPoSim.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Data.Persistence.Repositories
{
	public class GeneratorDataRepository : BaseRepository<GeneratorData>, IGeneratorDataRepository
	{
		public GeneratorDataRepository(Context context)
			: base(context)
		{ }

		public IEnumerable<GeneratorData> GetAllIncludingAll()
		{
			var distributions = base.Context.Set<Distribution>()
				.Include(d => d.Children)
				.ToList();

			return base.Context.Set<GeneratorData>()
				.Include(d => d.Distribution)
				.Include(d => d.Durchmesser).ThenInclude(d => d.Values)
				.Include(d => d.Abholzigkeit).ThenInclude(d => d.Values)
				.Include(d => d.Krümmung).ThenInclude(d => d.Values)
				.Include(d => d.Ovalität).ThenInclude(d => d.Values)
				.ToList();
		}

		public new Context Context
		{
			get { return Context as Context; }
		}
	}
}
