using KSE.GameStore.DataAccess.Entities;
using System.Linq.Expressions;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IRepository<T, TKey> where T : BaseEntity<TKey>
{
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> ListAsync(int pageNumber = 1, int pageSize = 10);
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int pageNumber = 1, int pageSize = 10);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
