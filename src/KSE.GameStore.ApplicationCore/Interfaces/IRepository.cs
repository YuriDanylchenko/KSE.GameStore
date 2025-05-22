using System.Linq.Expressions;

namespace KSE.GameStore.ApplicationCore.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> ListAsync(int? pageSize, int? pageNumber);
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int? pageSize, int? pageNumber);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
