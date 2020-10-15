using CabaVS.Shared.Abstractions.Repository;
using CabaVS.Shared.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CabaVS.Shared.EFCore.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly DbContext DbContext;

        public UnitOfWork(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<DbContext, TEntity>(DbContext);
        }

        public virtual async Task<int> CommitAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}