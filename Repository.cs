using CabaVS.Shared.Abstractions.Repository;
using CabaVS.Shared.Extensions;
using CabaVS.Shared.Extensions.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CabaVS.Shared.EFCore.UnitOfWork
{
    public class Repository<TContext, TEntity> : IRepository<TEntity> where TContext : DbContext where TEntity : class
    {
        protected readonly TContext DbContext;

        protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();
        protected IQueryable<TEntity> Table => DbSet.AsQueryable();

        public Repository(TContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null,
            string[] includes = null, int skip = 0, int take = 0)
        {
            return DbSet.ApplyPredicate(predicate).ApplyInclude(includes).SkipAndTake(skip, take).ToListAsync();
        }

        public virtual Task<List<TResult>> GetListAsync<TResult>(Expression<Func<TEntity, TResult>> selectExpression,
            Expression<Func<TEntity, bool>> predicate = null, string[] includes = null, int skip = 0, int take = 0)
        {
            if (selectExpression == null) throw new ArgumentNullException(nameof(selectExpression));

            return DbSet.ApplyPredicate(predicate).ApplyInclude(includes).SkipAndTake(skip, take)
                .Select(selectExpression).ToListAsync();
        }

        public virtual Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate = null, string[] includes = null)
        {
            return DbSet.ApplyPredicate(predicate).ApplyInclude(includes).FirstOrDefaultAsync();
        }

        public virtual Task<TResult> GetFirstAsync<TResult>(Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, bool>> predicate = null, string[] includes = null)
        {
            if (selectExpression == null) throw new ArgumentNullException(nameof(selectExpression));

            return DbSet.ApplyPredicate(predicate).ApplyInclude(includes).Select(selectExpression).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var createdEntity = await DbSet.AddAsync(entity);
            return createdEntity.Entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            DbContext.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await GetFirstAsync(predicate: predicate);
            if (entity == null)
            {
                return;
            }

            DbSet.Remove(entity);
        }

        public virtual Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return DbSet.ApplyPredicate(predicate).AnyAsync();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return DbSet.ApplyPredicate(predicate).CountAsync();
        }
    }
}