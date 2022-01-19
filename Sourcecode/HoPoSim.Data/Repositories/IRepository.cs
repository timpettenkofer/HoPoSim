using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HoPoSim.Data.Repositories
{
	public interface IRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
	{
		IEnumerable<TEntity> GetAllActive(bool readOnly = false);
	}

	public interface IBaseRepository<TEntity> where TEntity : class
	{
		TEntity Get(int id);
		IEnumerable<TEntity> GetAll(bool readOnly = false);
		IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

		TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
		TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

		void Add(TEntity entity);
		void AddRange(IEnumerable<TEntity> entities);

		void Remove(TEntity entity);
		void RemoveRange(IEnumerable<TEntity> entities);
	}
}