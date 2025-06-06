using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KSE.GameStore.DataAccess.Repositories;

public class Repository<T, TKey> : IRepositoryOld<T, TKey> where T : BaseEntity<TKey>
{
    protected readonly GameStoreDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(GameStoreDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(
        TKey id,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;
        if (include != null)
            query = include(query);
        
        return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id)!);
    }
    
    public async Task<IEnumerable<T>> ListAsync(
        int pageNumber = 1,
        int pageSize   = 10,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;
        if (include != null)
            query = include(query);

        var skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber = 1,
        int pageSize   = 10,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet.Where(predicate);
        if (include != null)
            query = include(query);

        var skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => _dbSet.AnyAsync(predicate);
    public IQueryable<T> Query() => _dbSet.AsNoTracking();
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}