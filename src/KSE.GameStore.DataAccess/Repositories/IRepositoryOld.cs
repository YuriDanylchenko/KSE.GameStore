using KSE.GameStore.DataAccess.Entities;
using System.Linq.Expressions;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IRepositoryOld<T, TKey> where T : BaseEntity<TKey>
{
    Task<T?> GetByIdAsync(TKey id,
        Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task<IEnumerable<T>> ListAsync(
        int pageNumber = 1,
        int pageSize   = 10,
        Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task<IEnumerable<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber = 1,
        int pageSize   = 10,
        Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
    Task SaveChangesAsync();
}
