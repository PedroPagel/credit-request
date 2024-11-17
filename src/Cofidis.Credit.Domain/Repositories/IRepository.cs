using Cofidis.Credit.Domain.Entities;
using System.Linq.Expressions;

namespace Cofidis.Credit.Domain.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Add(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task<int> Delete(Expression<Func<TEntity, bool>> predicate);
    }
}
