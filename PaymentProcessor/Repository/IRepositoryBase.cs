using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentProcessor.Repository
{
    public interface IRepositoryBase<T> : IDisposable where T : class
    {
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool enableTracking = true,
            bool ignoreQueryFilters = false
        );

        Task<IList<T>> GetListAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int index = 0,
            int size = 10,
            bool enableTracking = true,
            CancellationToken cancellationToken = default);


        Task<IList<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int index = 0,
            int size = 10,
            bool ignoreQueryFilters = false,
            bool enableTracking = true,
            CancellationToken cancellationToken = default)
            where TResult : class;

        Task<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default);
        Task InsertAsync(params T[] entities);
        Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity);
        Task UpdateAsync(params T[] entities);
        Task UpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);
        Task DeleteAsync(params T[] entities);
        Task DeleteAsync(IEnumerable<T> entities);
    }
}