using KSE.GameStore.DataAccess.Entities;
using System.Linq.Expressions;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IRepository<T, TKey> where T : BaseEntity<TKey>
{
    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task contains the entity if found, or null if not.</returns>
    Task<T?> GetByIdAsync(TKey id);

    /// <summary>
    /// Lists entities with optional pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>A task that represents the asynchronous operation. The task contains a collection of entities.</returns>
    Task<IEnumerable<T>> ListAsync(int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Lists entities that match a given predicate with optional pagination.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <returns>A task that represents the asynchronous operation. The task contains a collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Lists all entities that match a given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A task that represents the asynchronous operation. The task contains a collection of entities that match the predicate.</returns>
    Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);

    /// <summary>
    /// Saves changes made to the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveChangesAsync();
}