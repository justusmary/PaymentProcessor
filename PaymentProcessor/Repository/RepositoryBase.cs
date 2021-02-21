using Microsoft.EntityFrameworkCore;
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
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        #region Delete
        public Task DeleteAsync(T entity)
        {
            return Task.FromResult(_dbSet.Remove(entity));
        }

        public Task DeleteAsync(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
        #endregion

        #region Read
        public async Task<IList<T>> GetListAsync(Expression<Func<T, bool>> predicate = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                           int index = 0,
                                           int size = 20,
                                           bool enableTracking = true,
                                           CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (!enableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync(cancellationToken);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IList<TResult>> GetListAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                          Expression<Func<T, bool>> predicate = null,
                                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                                          int index = 0,
                                                          int size = 10,
                                                          bool ignoreQueryFilters = false,
                                                          bool enableTracking = true,
                                                          CancellationToken cancellationToken = default) where TResult : class
        {
            IQueryable<T> query = _dbSet;

            if (!enableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

            if (orderBy != null)
                return await orderBy(query).Select(selector).ToListAsync(cancellationToken);

            return await query.Select(selector).ToListAsync(cancellationToken);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null,
                                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                            bool enableTracking = true,
                                            bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = _dbSet;

            if (!enableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (orderBy != null)
                return await orderBy(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }
        #endregion

        #region Add
        public Task<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            return _dbSet.AddAsync(entity, cancellationToken).AsTask();
        }

        public Task InsertAsync(params T[] entities)
        {
            return _dbSet.AddRangeAsync(entities);
        }

        public Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        #endregion

        #region Update
        public Task UpdateAsync(T entity)
        {
            return Task.FromResult(_dbSet.Update(entity));
        }

        public Task UpdateAsync(params T[] entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        #endregion
    }
}