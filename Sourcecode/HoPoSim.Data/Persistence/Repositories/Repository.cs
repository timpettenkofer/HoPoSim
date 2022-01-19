using HoPoSim.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HoPoSim.Data.Repositories
{
	public class Repository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity> where TEntity : class, IActivableEntity
	{
		public Repository(DbContext context) : base(context)
		{ }

		public IEnumerable<TEntity> GetAllActive(bool readOnly = false)
		{
			var query = Context.Set<TEntity>().Where(p => p.Active);
			return readOnly ?
				query.AsNoTracking().ToList() :
				query.ToList();
		}
	}

	public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity
	{
		protected readonly DbContext Context;

		public BaseRepository(DbContext context)
		{
			Context = context;
		}

		public TEntity Get(int id)
		{
			// Here we are working with a DbContext, not PlutoContext. So we don't have DbSets 
			// such as Courses or Authors, and we need to use the generic Set() method to access them.
			return Context.Set<TEntity>().FirstOrDefault(d => d.Id == id); //Find(id);
		}

		public IEnumerable<TEntity> GetAll(bool readOnly = false)
		{
			// Note that here I've repeated Context.Set<TEntity>() in every method and this is causing
			// too much noise. I could get a reference to the DbSet returned from this method in the 
			// constructor and store it in a private field like _entities. This way, the implementation
			// of our methods would be cleaner:
			// 
			// _entities.ToList();
			// _entities.Where();
			// _entities.SingleOrDefault();
			// 
			// I didn't change it because I wanted the code to look like the videos. But feel free to change
			// this on your own.
			return readOnly ? Context.Set<TEntity>().AsNoTracking().ToList() : Context.Set<TEntity>().ToList();
		}

		public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
		{
			return Context.Set<TEntity>().Where(predicate);
		}

		public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return Context.Set<TEntity>().FirstOrDefault(predicate);
		}

		public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return Context.Set<TEntity>().SingleOrDefault(predicate);
		}

		public void Add(TEntity entity)
		{
			Context.Set<TEntity>().Add(entity);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			Context.Set<TEntity>().AddRange(entities);
		}

		public void Remove(TEntity entity)
		{
			Context.Set<TEntity>().Remove(entity);
		}

		public void RemoveRange(IEnumerable<TEntity> entities)
		{
			Context.Set<TEntity>().RemoveRange(entities);
		}
	}
}
