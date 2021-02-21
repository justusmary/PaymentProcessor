using PaymentProcessor.Repository;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentProcessor.Uow
{
    public interface IUnitOfWorkBase<TContext> : IDisposable where TContext : DbContext
    {
        TContext Context { get; }

        IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class;

        int Commit();

        Task<int> CommitAsync();
    }
}