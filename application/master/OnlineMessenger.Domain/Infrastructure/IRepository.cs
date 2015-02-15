using System;
using System.Linq;
using System.Linq.Expressions;

namespace OnlineMessenger.Domain.Infrastructure
{
    public interface IRepository<T> where T : new()
    {
        IQueryable<T> Query(params Expression<Func<T, object>>[] includes);
        void Add(T entity);
        void Delete(params T[] entity);
        void Update(T entity);
    }
}
