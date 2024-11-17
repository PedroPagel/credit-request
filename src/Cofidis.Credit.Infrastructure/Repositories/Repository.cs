using Cofidis.Credit.Domain.Entities;
using Cofidis.Credit.Domain.Repositories;
using Cofidis.Credit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace Cofidis.Credit.Infrastructure.Repositories
{
    public abstract class Repository<TEntity>(CreditDbContext db) : IRepository<TEntity> where TEntity : Entity, new()
    {
        public readonly DbSet<TEntity> DbSet = db.Set<TEntity>();
        public CreditDbContext Db = db;

        public void Dispose() => Db?.Dispose();

        public virtual async Task<IEnumerable<TEntity>> GetAll() =>
            await DbSet.AsNoTracking().ToListAsync();

        public virtual async Task<TEntity> GetById(Guid id) => await DbSet.FindAsync(id);

        public virtual async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> Add(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            return await Db.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Update(TEntity entity)
        {
            DbSet.Update(entity);
            return await Db.SaveChangesAsync() > 0;
        }

        public virtual async Task<int> Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var range = DbSet.Where(predicate);
            DbSet.RemoveRange(range);
            return await Db.SaveChangesAsync();
        }
    }
}
