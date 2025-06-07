using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KSE.GameStore.DataAccess.Repositories;

public class Repository<T, TKey> : IRepository<T, TKey> where T : BaseEntity<TKey>
{
    protected readonly GameStoreDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(GameStoreDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(TKey id) => await _dbSet.FindAsync(id);
    
    public async Task<IEnumerable<T>> ListAsync(int pageNumber = 1, int pageSize = 10)
    {
        IQueryable<T> query = _dbSet;

        var skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int pageNumber = 1, int pageSize = 10)
    {
        var query = _dbSet.Where(predicate);

        var skip = (pageNumber - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
    }
    
    public async Task<IEnumerable<T>> ListAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}