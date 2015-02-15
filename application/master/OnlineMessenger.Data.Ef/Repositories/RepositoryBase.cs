using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace OnlineMessenger.Data.Ef.Repositories
{
    public class RepositoryBase<T> where T : class, new()
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<T> DbSet;

        public RepositoryBase(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }

        public IQueryable<T> Query(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return query;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(params T[] entities)
        {
            DbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
